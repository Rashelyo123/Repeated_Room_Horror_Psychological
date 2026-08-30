using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool lockYAxisOnly = false; // opsional: kalau true, cuma muter di sumbu Y (gak ikut miring atas-bawah)

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        if (lockYAxisOnly)
        {
            Vector3 direction = targetCamera.transform.position - transform.position;
            direction.y = 0; // abaikan perbedaan tinggi, biar cuma muter horizontal
            if (direction.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(-direction);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
        }
    }

    public void SetCamera(Camera cam) => targetCamera = cam;
}