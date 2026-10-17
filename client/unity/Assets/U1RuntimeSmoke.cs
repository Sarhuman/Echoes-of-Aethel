using UnityEngine;

public sealed class U1RuntimeSmoke : MonoBehaviour
{
    private void Start()
    {
        if (!Application.isBatchMode) return;
        U1Player player = FindFirstObjectByType<U1Player>();
        U1Enemy enemy = FindFirstObjectByType<U1Enemy>();
        if (player == null || enemy == null || player.Enemy != enemy || enemy.Player != player)
        {
            Debug.LogError("AETHEL_U1_RUNTIME_SMOKE_FAILED actor links");
            Application.Quit(1);
            return;
        }
        Debug.Log("AETHEL_U1_RUNTIME_SMOKE_OK scene actors linked");
        Application.Quit(0);
    }
}
