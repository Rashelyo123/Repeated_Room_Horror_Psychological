using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    private void Reset()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Quaternion rotation = mainCamera.transform.rotation;
        transform.LookAt(worldPosition: transform.position + rotation * Vector3.forward,
                          worldUp: rotation * Vector3.up);
    }
}