using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement: MonoBehaviour, ICharacterMovement
{
    
    public float moveForce = 20.0f;
    public ForceMode moveForceMode = ForceMode.VelocityChange;
    public float moveDrag = 16.65f;

    [ReadOnly(RunMode.Any)]
    public float groundSpeed;

    public float gravity = 100f;

    public float jumpForce = 8.0f;
    public ForceMode jumpForceMode = ForceMode.Impulse;

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540f;

    public float groundCheckRadius = 0.5f;
    public string groundLayer = "Ground";

    public bool isGrounded { get; private set; }

    private Rigidbody rb;

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

        groundLayerMask = LayerMask.GetMask(groundLayer);
    }

    void Update()
    {
        //if (turnToDirectionOfMovement)
        //{
        //    var lookAt = rb.velocity;
        //    if (!(Mathf.Approximately(lookAt.x, 0) && Mathf.Approximately(lookAt.z, 0)))
        //    {
        //        lookAt.y = 0;
        //        var targetRotation = Quaternion.LookRotation(lookAt);
        //    
        //        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        //        float timeToComplete = angle / turnSpeed;
        //        float ratio = Mathf.Min(1, Time.deltaTime / timeToComplete);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ratio);
        //    }
        //}
    }

    void FixedUpdate()
    {
        // moveDrag = (2 * moveForce) / (moveSpeed * moveSpeed);

        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayerMask);

        if (isGrounded)
        {
            rb.AddForce(desiredDirection * moveForce, moveForceMode);

            //moveDrag = 200 * moveForce / moveSpeed / moveSpeed;
            // Apply horizontal drag for maxSpeed
            rb.velocity *= Mathf.Clamp01(1f - (moveDrag * Time.deltaTime));

            if (desiredJump)
                rb.AddForce(transform.up * jumpForce, jumpForceMode);
        }
        else
        {
            rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        }

        desiredJump = false;

        groundSpeed = rb.velocity.magnitude;
    }

    #endregion

}
