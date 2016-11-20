using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IPredatorCharacter))]
[RequireComponent(typeof(ViewportShake))]
public class Predator: MonoBehaviour
{
    public bool enableCameraShakeOnLanding = false;
    public float cameraShakeIntensity = 0.0025f;
    public float cameraShakeDuration = 0.1f;

    private IPredatorCharacter character;
    private ViewportShake cameraShake;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<IPredatorCharacter>();
        cameraShake = GetComponent<ViewportShake>();
    }

    void FixedUpdate()
    {
        if (character.isLanding && enableCameraShakeOnLanding)
        {
            cameraShake.Shake(cameraShakeIntensity, cameraShakeDuration);
        }
    }

	void Update()
    {
        if (character.isGrounded)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            direction = Camera.main.transform.TransformDirection(direction);
            direction.y = 0;
            character.Move(direction.normalized);

            if (Input.GetButton("Jump"))
            {
                character.Jump();
            }
        }
    }

    #endregion
}
