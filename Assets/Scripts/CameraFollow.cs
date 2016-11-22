using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public bool enableCameraLag = false;
    public float lagSpeed = 10;

    // Rotation

    public float distance = 100.0f;
    public float yawSpeed = 0.02f;
    public float pitchSpeed = 0.02f;
    public float zoomSpeed = 200f;

    public float minPitchAngle = 10f;
    public float maxPitchAngle = 90f;

    public float minDistance = 10f;
    public float maxDistance = 500f;

    #region Unity Events

    float x = 0.0f;
    float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        
        if (target != null)
        {
            bool movingCamera = Input.GetMouseButton(1);
            if (movingCamera)
            {
                x += Input.GetAxis("Mouse X") * yawSpeed;
                y -= Input.GetAxis("Mouse Y") * pitchSpeed;

                y = ClampAngle(y, minPitchAngle, maxPitchAngle);
            }

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, minDistance, maxDistance);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            
            if (!movingCamera && enableCameraLag)
            {
                float step = lagSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, position, step);
            }
            else
            {
                transform.position = position;
            }
        }
    }

    #endregion

    private static float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360F)
            angle += 360F;
        while (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}