using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMovement: MonoBehaviour, ICharacterMovement
{
    public float maximumMoveSpeed = 20;

    public float moveForce = 6.0F;
    public ForceMode moveForceMode = ForceMode.VelocityChange;

    public float jumpForce = 8.0F;
    public ForceMode jumpForceMode = ForceMode.Impulse;

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540;

    public float groundCheckRadius = 0.5f;
    public string groundLayer = "Ground";

    public bool isGrounded { get; private set; }

    private Rigidbody rb;

    private Vector3 desiredDirection = Vector3.zero;
    private bool desiredJump = false;

    private int groundLayerMask;

    private float horizontalDrag = 0;

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
        groundLayerMask = LayerMask.GetMask(groundLayer);
    }

    public float speed;
    void Update()
    {
        if (turnToDirectionOfMovement)
        {
            //var lookAt = rb.velocity;
            //if (!(Mathf.Approximately(lookAt.x, 0) && Mathf.Approximately(lookAt.z, 0)))
            //{
            //    lookAt.y = 0;
            //    var targetRotation = Quaternion.LookRotation(lookAt);
            //
            //    float angle = Quaternion.Angle(transform.rotation, targetRotation);
            //    float timeToComplete = angle / turnSpeed;
            //    float ratio = Mathf.Min(1, Time.deltaTime / timeToComplete);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ratio);
            //}
        }
    }

    void FixedUpdate()
    {
        horizontalDrag = (2 * rb.mass * moveForce) / (maximumMoveSpeed * maximumMoveSpeed);

        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayerMask);

        if (isGrounded)
        {
            rb.AddForce(desiredDirection * moveForce * Time.deltaTime, moveForceMode);
            if (desiredJump)
                rb.AddForce(transform.up * jumpForce, jumpForceMode);
        }

        desiredJump = false;

        // Apply horizontal drag for maxSpeed
        //rb.velocity -= new Vector3(rb.velocity.x, 0, rb.velocity.z) * horizontalDrag * Time.deltaTime;

        speed = rb.velocity.magnitude;
    }

    #endregion

}
