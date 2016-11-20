using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacter: MonoBehaviour, ICharacter
{
    public float moveForce = 10.0f;
    public ForceMode moveForceMode = ForceMode.VelocityChange;
    public float moveDrag = 16.3f;

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public float groundSpeed;
#endif

    public float jumpForce = 50.0f;
    public ForceMode jumpForceMode = ForceMode.VelocityChange;

    public float gravity = 200f;

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540f;

    public float groundCheckRadius = 0.51f;
    public string groundLayer = "Ground";

    private Rigidbody rb;

    private Vector3 desiredDirection = Vector3.zero;
    private bool desiredJump = false;

    private int groundLayerMask;

    #region ICharacter 

    public bool isGrounded { get; private set; }
    public bool isJumping { get; private set; }
    public bool isLanding { get; private set; }

    public void Stop()
    {
        this.desiredDirection = Vector3.zero;
    }

    public void Move(Vector3 direction)
    {
        this.desiredDirection = direction;
    }

    public void Turn(Quaternion rotation)
    {
        this.desiredDirection = (rotation * this.desiredDirection).normalized;
    }

    public void Jump()
    {
        this.desiredJump = true;
    }

    #endregion

    #region Unity Events 

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        groundLayerMask = LayerMask.GetMask(groundLayer);
    }

    protected virtual void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayerMask);
        isLanding = false;

        if (isGrounded)
        {
            rb.AddForce(desiredDirection * moveForce, moveForceMode);
            rb.velocity *= Mathf.Clamp01(1f - (moveDrag * Time.deltaTime));  // Apply ground drag before jump

            if (isJumping)
            {
                isJumping = false;
                isLanding = true;
            }
            else if (desiredJump)
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

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, ratio));
            }
        }
    }

    #endregion

}
