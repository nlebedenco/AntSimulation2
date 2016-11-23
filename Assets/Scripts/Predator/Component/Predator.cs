using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IPredatorCharacter))]
[RequireComponent(typeof(ViewportShake))]
public class Predator: MonoBehaviour
{
    [Header("Materials")]

    [ReadOnly]
    public Material Normal;

    [ReadOnly]
    public Material Stealth;

    [Header("Presence")]

    [ReadOnly]
    public Collider Presence;

    [Header("Camera Effects")]

    public bool enableCameraShakeOnLanding = false;
    public float cameraShakeIntensity = 0.0025f;
    public float cameraShakeDuration = 0.1f;

    private IPredatorCharacter character;
    private ViewportShake cameraShake;
    private new Renderer renderer;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<IPredatorCharacter>();
        cameraShake = GetComponent<ViewportShake>();
        renderer = GetComponent<Renderer>();
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
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0;
        character.Move(direction.normalized);

        if (Input.GetButton("Jump"))
            character.Jump();

        if (Input.GetButton("Stealth"))
            character.Stealth();

        if (character.isStealthed)
        {
            renderer.material = Stealth;
            Presence.enabled = false;
        }
        else
        {
            renderer.material = Normal;
            Presence.enabled = true;
        }
    }

    #endregion
}
