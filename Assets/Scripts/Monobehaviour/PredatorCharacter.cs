using UnityEngine;
using System.Collections;

public class PredatorCharacter : RigidbodyCharacter, IPredatorCharacter
{
    public float jumpForce = 50.0f;
    public ForceMode jumpForceMode = ForceMode.VelocityChange;
    public float groundCheckRadius = 0.51f;
    public string groundLayer = "Ground";

    private int groundLayerMask;
    private bool desiredJump = false;

    #region IPredatorCharacter

    public bool isGrounded { get; private set; }
    public bool isJumping { get; private set; }
    public bool isLanding { get; private set; }

    public void Jump()
    {
        this.desiredJump = true;
    }

    #endregion

    #region Unity Events 

    protected override void Awake()
    {
        base.Awake();

        groundLayerMask = LayerMask.GetMask(groundLayer);
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
            }
            else if (desiredJump)
            {
                rb.AddForce(transform.up * jumpForce, jumpForceMode);
                isJumping = true;
            }
        }

        desiredJump = false;
    }

    #endregion
}
