using UnityEngine;

public sealed class U1Camera : MonoBehaviour
{
    public Transform Target;
    public Vector3 Offset = new Vector3(9f, 13f, -11f);

    private void LateUpdate()
    {
        if (Target == null) return;
        Vector3 desired = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-6f * Time.deltaTime));
        transform.LookAt(Target.position + Vector3.up * 0.7f);
    }
}
