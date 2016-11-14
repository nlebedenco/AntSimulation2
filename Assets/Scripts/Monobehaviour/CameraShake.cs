using UnityEngine;
using System.Collections;
public class CameraShake : MonoBehaviour
{
    private float intensity;
    private float duration;

    private bool isShaking = false;

    void OffsetCameraObliqueness(float xOffset, float yOffset)
    {
        float frustrumHeight = 2 * Camera.main.nearClipPlane * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustrumWidth = frustrumHeight * Camera.main.aspect;
        Matrix4x4 mat = Camera.main.projectionMatrix;
        mat[0, 2] = 2 * xOffset / frustrumWidth;
        mat[1, 2] = 2 * yOffset / frustrumHeight;
        Camera.main.projectionMatrix = mat;
    }

    void Update()
    {
        if (isShaking)
        {
            if (intensity > 1e-6f)
            {
                OffsetCameraObliqueness(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity));
                intensity -= intensity / duration * Time.deltaTime;
            }
            else
            {
                intensity = 0;
                OffsetCameraObliqueness(0, 0);
                isShaking = false;
            }
        }
    }

    public void Shake(float intensity, float duration)
    {
        this.intensity = intensity;
        this.duration = duration;
        this.isShaking = true;
    }
}