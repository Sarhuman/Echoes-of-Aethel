using Aethel.Campaign;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public sealed class U1Enemy : MonoBehaviour
{
    public U1Player Player;
    public Renderer Visual;
    public Color IdleColor = new Color(0.22f, 0.74f, 0.95f);
    public Color TelegraphColor = new Color(1f, 0.24f, 0.16f);

    public bool IsAlive { get { return hp > 0; } }
    public int Hp { get { return hp; } }
    public const int MaxHp = 60;
    public const int Defense = 5;

    private CharacterController controller;
    private Vector3 spawn;
    private int hp = MaxHp;
    private float attackReadyAt;
    private float windupEndsAt = -1f;
    private float stunnedUntil;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        spawn = transform.position;
    }

    private void Update()
    {
        if (!IsAlive || Player == null || Player.IsDead || Player.IsPaused) return;
        if (Player.transform.position.z < 0f)
        {
            windupEndsAt = -1f;
            Visual.material.color = IdleColor;
            ReturnToSpawn();
            return;
        }

        if (Time.time < stunnedUntil) return;
        Vector3 toPlayer = Player.transform.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;
        if (windupEndsAt >= 0f)
        {
            if (Time.time >= windupEndsAt)
            {
                windupEndsAt = -1f;
                Visual.material.color = IdleColor;
                attackReadyAt = Time.time + 1.5f;
                if (distance <= 2.25f && Player.transform.position.z >= 0f)
                    Player.ReceiveHit(18);
            }
            return;
        }

        if (distance > 1.7f && distance < 10f)
        {
            Vector3 direction = toPlayer.normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
            controller.Move((direction * 3f + Vector3.down * 0.2f) * Time.deltaTime);
        }
        else if (distance <= 1.9f && Time.time >= attackReadyAt)
        {
            windupEndsAt = Time.time + 0.7f;
            Visual.material.color = TelegraphColor;
        }
    }

    public int ReceiveAttack(int raw, bool interrupt)
    {
        if (!IsAlive) return 0;
        int damage = CombatMath.DirectDamage(raw, 0, false, 15000, Defense, 0, 0);
        hp = Mathf.Max(0, hp - damage);
        if (interrupt && hp > 0)
        {
            windupEndsAt = -1f;
            stunnedUntil = Time.time + 0.5f;
            Visual.material.color = IdleColor;
        }
        if (hp == 0)
        {
            windupEndsAt = -1f;
            Visual.material.color = new Color(0.2f, 0.2f, 0.26f);
            controller.enabled = false;
        }
        return damage;
    }

    public void ResetEncounter()
    {
        controller.enabled = false;
        transform.position = spawn;
        controller.enabled = true;
        hp = MaxHp;
        attackReadyAt = Time.time + 1f;
        windupEndsAt = -1f;
        stunnedUntil = 0f;
        Visual.material.color = IdleColor;
    }

    private void ReturnToSpawn()
    {
        Vector3 direction = spawn - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.05f)
            controller.Move((direction.normalized * 3f + Vector3.down * 0.2f) * Time.deltaTime);
    }
}
