using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CameraShake))]
public class RigidbodyMovement: MonoBehaviour, ICharacterMovement
{
    public bool enableCameraShakeOnLanding = false;
    public float cameraShakeIntensity = 0.0025f;
    public float cameraShakeDuration = 0.1f;

    public float moveForce = 20.0f;
    public ForceMode moveForceMode = ForceMode.VelocityChange;
    public float moveDrag = 16.65f;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public float groundSpeed;
#endif

    public float jumpForce = 70.0f;
    public ForceMode jumpForceMode = ForceMode.Impulse;

    public float gravity = 100f;

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540f;

    public float groundCheckRadius = 0.5f;
    public string groundLayer = "Ground";

    public bool isGrounded { get; private set; }
    private bool isJumping = false;

    private Rigidbody rb;
    private CameraShake cameraShake;

    private Vector3 desiredDirection = Vector3.zero;
    private bool desiredJump = false;

    private int groundLayerMask;

    public void Stop()
    {
        this.desiredDirection = Vector3.zero;
    }

    public void Move(Vector3 direction)
    {
        this.desiredDirection = direction;
    }

    public void Jump()
    {
        this.desiredJump = true;
    }

    #region Unity Events 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        cameraShake = GetComponent<CameraShake>();

        groundLayerMask = LayerMask.GetMask(groundLayer);
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayerMask);

        if (isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
                if (enableCameraShakeOnLanding)
                {
                    cameraShake.Shake(cameraShakeIntensity, cameraShakeDuration);
                }
            }

            rb.AddForce(desiredDirection * moveForce, moveForceMode);
            rb.velocity *= Mathf.Clamp01(1f - (moveDrag * Time.deltaTime));  // Apply ground move drag before jump

            if (desiredJump)
            {
                rb.AddForce(transform.up * jumpForce, jumpForceMode);
                isJumping = true;
            }
        }
        else
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        desiredJump = false;

#if UNITY_EDITOR
        groundSpeed = rb.velocity.magnitude;
#endif

        if (turnToDirectionOfMovement)
        {
            if (desiredDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                float angle = Quaternion.Angle(rb.rotation, targetRotation);
                float timeToComplete = angle / turnSpeed;
                float ratio = Mathf.Min(1, Time.deltaTime / timeToComplete);
                Quaternion deltaRotation = Quaternion.AngleAxis(angle * turnSpeed * Time.deltaTime, transform.up);

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, ratio));
            }
        }
    }

    #endregion

}
