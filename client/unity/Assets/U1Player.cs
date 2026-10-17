using System;
using System.IO;
using System.Runtime.Serialization;
using Aethel.Campaign;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class U1Player : MonoBehaviour
{
    public U1Enemy Enemy;
    public Camera ViewCamera;
    public Transform Visual;

    public bool IsDead { get { return hp <= 0; } }
    public bool IsPaused { get { return paused; } }

    private CharacterController controller;
    private Vector3 facing = Vector3.forward;
    private int maxHp;
    private int maxMp;
    private int hp;
    private float mp;
    private float stamina = 100f;
    private float basicReadyAt;
    private float impactReadyAt;
    private float guardReadyAt;
    private float riftReadyAt;
    private float dashReadyAt;
    private float dashEndsAt;
    private float invulnerableUntil;
    private float guardEndsAt;
    private float lastMpSpentAt;
    private float messageEndsAt;
    private string message = "Atravesse o portão e enfrente o Lobo Etéreo.";
    private Color messageColor = Color.white;
    private bool victory;
    private bool routeComplete;
    private bool paused;
    private U1RewardSession rewardSession;
    private int savedGold;
    private int savedXp;
    private int savedLevel = 1;
    private string rewardNotice = "";

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        var startingState = CampaignState.Create("unity_playtest", "bastion", "twilight_bastion");
        maxHp = CharacterRules.MaxHp(startingState);
        maxMp = CharacterRules.MaxMp(startingState);
        hp = maxHp;
        mp = maxMp;
        try
        {
            rewardSession = new U1RewardSession(Path.Combine(Application.persistentDataPath, "aethel-u1-campaign.json"));
            CampaignState saved = rewardSession.Read();
            savedGold = saved.Gold;
            savedXp = saved.TotalXp;
            savedLevel = saved.Level;
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is SerializationException)
        {
            rewardNotice = "Save U1 indisponível; nenhuma recompensa será gravada";
            Debug.LogError("U1 reward save unavailable: " + ex.Message);
        }
        messageEndsAt = Time.time + 7f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) paused = !paused;
        if (Input.GetKeyDown(KeyCode.F6)) ResetAttempt();
        if (paused || IsDead) return;

        float dt = Time.deltaTime;
        stamina = Mathf.Min(100f, stamina + 12f * dt);
        if (Time.time - lastMpSpentAt >= 3f) mp = Mathf.Min(maxMp, mp + dt);

        Vector3 move = InputDirection();
        if (Input.GetKeyDown(KeyCode.Space) && stamina >= 25f && Time.time >= dashReadyAt)
        {
            stamina -= 25f;
            dashReadyAt = Time.time + 1.2f;
            dashEndsAt = Time.time + 0.2f;
            invulnerableUntil = Time.time + 0.12f;
            if (move.sqrMagnitude > 0.01f) facing = move;
        }
        if (Time.time < dashEndsAt) move = facing * 10f;
        else move *= 4f;
        controller.Move((move + Vector3.down * 0.2f) * dt);
        if (victory && !routeComplete && transform.position.z < -7f)
        {
            routeComplete = true;
            Show("Percurso U1 concluído — F6 para repetir", new Color(0.94f, 0.78f, 0.39f));
        }
        if (move.sqrMagnitude > 0.01f)
        {
            facing = move.normalized;
            Visual.rotation = Quaternion.Slerp(Visual.rotation, Quaternion.LookRotation(facing), 12f * dt);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetMouseButtonDown(0))
            TryAttack(42, 2.1f, 0.9f, 0, false, ref basicReadyAt);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
            TryAttack(50, 2.4f, 7f, 12, true, ref impactReadyAt);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time >= guardReadyAt && SpendMp(8))
            {
                guardEndsAt = Time.time + 2f;
                guardReadyAt = Time.time + 6f;
                Show("GUARDA", new Color(0.35f, 0.78f, 1f));
            }
            else Show("Guarda indisponível", Color.white);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.R))
            TryAttack(62, 3f, 9f, 20, false, ref riftReadyAt);
    }

    private Vector3 InputDirection()
    {
        float x = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) -
                  (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
        float z = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) -
                  (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
        Vector3 forward = ViewCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = ViewCamera.transform.right;
        right.y = 0f;
        right.Normalize();
        return Vector3.ClampMagnitude(right * x + forward * z, 1f);
    }

    private bool SpendMp(int amount)
    {
        if (amount == 0) return true;
        if (mp < amount) return false;
        mp -= amount;
        lastMpSpentAt = Time.time;
        return true;
    }

    private void TryAttack(int raw, float range, float cooldown, int cost, bool interrupt, ref float readyAt)
    {
        if (Time.time < readyAt || !SpendMp(cost))
        {
            Show("Habilidade indisponível", Color.white);
            return;
        }
        readyAt = Time.time + cooldown;
        if (Enemy == null || !Enemy.IsAlive)
        {
            Show("MISS", new Color(0.35f, 0.7f, 1f));
            return;
        }
        Vector3 toEnemy = Enemy.transform.position - transform.position;
        toEnemy.y = 0f;
        if (toEnemy.magnitude > range || Vector3.Dot(facing, toEnemy.normalized) < 0.25f)
        {
            Show("MISS", new Color(0.35f, 0.7f, 1f));
            return;
        }
        int damage = Enemy.ReceiveAttack(raw, interrupt);
        Show(damage.ToString(), new Color(1f, 0.3f, 0.25f));
        if (!Enemy.IsAlive)
        {
            victory = true;
            ConfirmReward();
        }
    }

    private void ConfirmReward()
    {
        if (rewardSession == null) return;
        try
        {
            int oldGold = savedGold;
            int oldXp = savedXp;
            ProgressionResult result = rewardSession.ClaimDefeatedWolf();
            savedGold = result.State.Gold;
            savedXp = result.State.TotalXp;
            savedLevel = result.State.Level;
            if (result.Status == ProgressionStatus.Accepted)
                rewardNotice = "+" + (savedXp - oldXp) + " XP  ·  +" + (savedGold - oldGold) + " ouro (salvo)";
            else if (result.Status == ProgressionStatus.Replayed)
                rewardNotice = "Recompensa deste lobo já recebida";
            else
                rewardNotice = "Recompensa pendente: " + result.Status + " — F6 para tentar novamente";
        }
        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is SerializationException)
        {
            rewardNotice = "Falha ao salvar recompensa — F6 para tentar novamente";
            Debug.LogError("U1 reward claim failed: " + ex.Message);
        }
    }

    public void ReceiveHit(int raw)
    {
        if (Time.time < invulnerableUntil || IsDead) return;
        int reduction = Time.time < guardEndsAt ? 4000 : 0;
        int damage = CombatMath.DirectDamage(raw, 0, false, 15000, 9, 0, reduction);
        hp = Mathf.Max(0, hp - damage);
        Show("-" + damage, new Color(1f, 0.3f, 0.25f));
        if (IsDead)
        {
            Show("Derrotado — F6 reinicia a tentativa", Color.white);
        }
    }

    private void ResetAttempt()
    {
        controller.enabled = false;
        transform.position = new Vector3(0f, 1f, -15f);
        controller.enabled = true;
        hp = maxHp;
        mp = maxMp;
        stamina = 100f;
        basicReadyAt = impactReadyAt = guardReadyAt = riftReadyAt = dashReadyAt = 0f;
        dashEndsAt = invulnerableUntil = guardEndsAt = 0f;
        victory = routeComplete = paused = false;
        if (rewardSession != null) rewardNotice = "";
        Enemy.ResetEncounter();
        Show("Nova tentativa", Color.white);
    }

    private void Show(string text, Color color)
    {
        message = text;
        messageColor = color;
        messageEndsAt = Time.time + 1.3f;
    }

    private void OnGUI()
    {
        GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.9f);
        GUI.Box(new Rect(16, 16, 350, 126), "");
        GUI.color = Color.white;
        GUI.Label(new Rect(29, 22, 320, 22), "BASTIÃO DO CREPÚSCULO  ·  PROVA UNITY");
        DrawBar(29, 50, "VIDA", hp, maxHp, new Color(0.65f, 0.17f, 0.14f));
        DrawBar(29, 78, "ÉTER", mp, maxMp, new Color(0.15f, 0.42f, 0.68f));
        DrawBar(29, 106, "FÔLEGO", stamina, 100f, new Color(0.65f, 0.49f, 0.12f));
        GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.9f);
        GUI.Box(new Rect(16, 148, 350, 28), "NÍVEL " + savedLevel + "  ·  " + savedXp + " XP  ·  " + savedGold + " OURO");

        GUI.color = new Color(0.04f, 0.06f, 0.1f, 0.86f);
        GUI.Box(new Rect(16, Screen.height - 82, 620, 66), "");
        GUI.color = Color.white;
        GUI.Label(new Rect(28, Screen.height - 75, 600, 25),
            "WASD mover  ·  1 atacar  ·  2/Q impacto  ·  3/E guarda  ·  4/R fenda");
        GUI.Label(new Rect(28, Screen.height - 49, 600, 25), "Espaço esquiva  ·  F6 reiniciar  ·  Esc pausa");
        if (rewardNotice != "")
            GUI.Box(new Rect(16, Screen.height - 122, 620, 34), rewardNotice);

        GUI.color = Color.white;
        string objective = IsDead ? "Tentativa encerrada" : routeComplete ? "Percurso U1 concluído — F6 para repetir" :
            victory ? "Lobo derrotado — retorne à Cidadela" :
            transform.position.z < 0f ? "Atravesse o portão para o Bosque" : "Derrote o Lobo Etéreo";
        GUI.Box(new Rect(Screen.width - 320, 16, 304, 55), objective);
        if (Enemy != null && Enemy.IsAlive && transform.position.z >= 0f)
            GUI.Box(new Rect(Screen.width / 2 - 160, 16, 320, 36), "LOBO ETÉREO  " + Enemy.Hp + "/" + U1Enemy.MaxHp);
        if (Time.time < messageEndsAt)
        {
            GUI.color = messageColor;
            GUI.Label(new Rect(Screen.width / 2 - 110, Screen.height / 2 - 80, 260, 35), message);
        }
        if (paused) GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 30, 200, 60), "PAUSADO");
        GUI.color = Color.white;
    }

    private static void DrawBar(float x, float y, string label, float value, float maximum, Color fill)
    {
        GUI.Label(new Rect(x, y, 72, 22), label);
        GUI.color = new Color(0.06f, 0.06f, 0.07f);
        GUI.DrawTexture(new Rect(x + 75, y + 2, 230, 17), Texture2D.whiteTexture);
        GUI.color = fill;
        GUI.DrawTexture(new Rect(x + 76, y + 3, 228 * Mathf.Clamp01(value / maximum), 15), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(x + 311, y, 70, 22), Mathf.CeilToInt(value) + "/" + Mathf.CeilToInt(maximum));
    }
}
