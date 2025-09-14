using UnityEngine;

// Keeps the world-space canvas facing the main camera.
[DisallowMultipleComponent]
public class BillboardToCamera : MonoBehaviour
{
    public bool lockYAxisOnly = true;

    void LateUpdate()
    {
        var cam = Camera.main;
        if (cam == null) return;

        if (lockYAxisOnly)
        {
            Vector3 forward = transform.position - cam.transform.position;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, Vector3.up);
        }
    }
}