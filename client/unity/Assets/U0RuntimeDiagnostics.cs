using UnityEngine;

public sealed class U0RuntimeDiagnostics : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"AETHEL_U0 editor={Application.unityVersion} " +
                  $"resolution={Screen.width}x{Screen.height} " +
                  $"cpu={SystemInfo.processorType} " +
                  $"gpu={SystemInfo.graphicsDeviceName} " +
                  $"ram_mb={SystemInfo.systemMemorySize} " +
                  $"vram_mb={SystemInfo.graphicsMemorySize}");

        if (Application.isBatchMode)
        {
            Application.Quit();
        }
    }
}
