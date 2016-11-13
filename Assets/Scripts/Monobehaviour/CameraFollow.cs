using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public bool enableCameraLag = false;
    public float lagSpeed = 10;
    public bool freezeX = false;
    public bool freezeY = false;
    public bool freezeZ = false;

    public Vector3 offset = Vector3.zero;

    #region Unity Events

    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the target, but offset by the calculated offset distance.
        if (target != null)
        {
            var newPosition = target.position + offset;
            if (freezeX) newPosition.x = transform.position.x;
            if (freezeY) newPosition.y = transform.position.y;
            if (freezeZ) newPosition.z = transform.position.z;

            if (enableCameraLag)
            {
                float step = lagSpeed * Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, newPosition, step);
            }
            else
            {
                transform.position = newPosition;
            }
        }
    }

    #endregion
}