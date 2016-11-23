using UnityEngine;
using System.Collections;

public class PredatorCharacter : RigidbodyCharacter, IPredatorCharacter
{
    [Header("Jump")]

    public float jumpForce = 50.0f;
    public ForceMode jumpForceMode = ForceMode.VelocityChange;

    public bool enableMoveWhileJumping = true;
    public float moveForceModifierWhileJumping = 0.5f;
    public float moveDragModifierWhileJumping = 0.5f;

    [Header("Stealth")]

    public float stealthCooldown = 3f;

    [Header("Ant Detection")]

    [ReadOnly]
    public string antLayerName = "Ant";

    [Header("Ground Check")]

    [ReadOnly]
    public string groundLayerName = "Ground";
    public float groundCheckRadius = 0.51f;

    private int groundLayerMask;
    private int antLayer;

    private bool desiredJump = false;
    private bool desiredStealth = false;
    private float lastBrokenStealthTime;

    #region IPredatorCharacter

    public Agent agent { get; set; }

    private bool _stealthed;
    public bool isStealthed
    {
        get { return _stealthed; }
        set
        {
            if (_stealthed != value)
            {
                if (!value)
                    lastBrokenStealthTime = Time.time;
                _stealthed = value;
            }
        }
    }

    public bool isGrounded { get; private set; }
    public bool isJumping { get; private set; }
    public bool isLanding { get; private set; }

    public void Jump()
    {
        this.desiredJump = true;
    }

    public void Stealth()
    {
        this.desiredStealth = true;
    }

    #endregion

    #region Unity Events 

    protected override void Awake()
    {
        base.Awake();

        groundLayerMask = LayerMask.GetMask(groundLayerName);
        antLayer = LayerMask.NameToLayer(antLayerName);
    }

    protected override void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayerMask);
        isLanding = false;

        if (isGrounded)
        {
            base.FixedUpdate();

            if (isJumping)
            {
                isJumping = false;
                isLanding = true;
                isStealthed = false;
            }
            else if (desiredJump)
            {
                rigidbody.AddForce(transform.up * jumpForce, jumpForceMode);
                isJumping = true;
            }
            else if (desiredStealth)
            {
                if (lastBrokenStealthTime + stealthCooldown < Time.time)
                    isStealthed = true;
            }
        }
        else
        {
            if (isJumping)
            {
                var oldForce = moveForce;
                var oldDrag = moveDrag;

                moveForce *= moveForceModifierWhileJumping;
                moveDrag *= moveDragModifierWhileJumping;

                base.FixedUpdate();

                moveForce = oldForce;
                moveDrag = oldDrag;
            }
        }

        desiredJump = false;
        desiredStealth = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == antLayer)
            isStealthed = false;
    }

    #endregion
}
