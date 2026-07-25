using UnityEngine;

public class SmoothSpotLight : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera's transform
    public Transform spotlightTransform;  // Reference to the spotlight's transform
    public float smoothSpeed = 0.125f;  // Adjust the speed of the smoothness
    public Vector3 offset;  // Offset between camera and spotlight

    private void LateUpdate()
    {
        // Desired position for the spotlight
        Vector3 desiredPosition = cameraTransform.position + offset;
        
        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(spotlightTransform.position, desiredPosition, smoothSpeed);
        
        // Update the spotlight's position
        spotlightTransform.position = smoothedPosition;
        
        // Make the spotlight look in the same direction as the camera
        spotlightTransform.rotation = Quaternion.Lerp(spotlightTransform.rotation, cameraTransform.rotation, smoothSpeed);
    }
}
