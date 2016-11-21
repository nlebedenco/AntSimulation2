using UnityEngine;
using System.Collections;

public class PredatorCharacter : RigidbodyCharacter, IPredatorCharacter
{
    public float jumpForce = 50.0f;
    public ForceMode jumpForceMode = ForceMode.VelocityChange;

    public bool enableMoveWhileJumping = true;
    public float moveForceModifierWhileJumping = 0.5f;
    public float moveDragModifierWhileJumping = 0.5f;

    public float groundCheckRadius = 0.51f;

    [ReadOnly]
    public string groundLayerName = "Ground";

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

        groundLayerMask = LayerMask.GetMask(groundLayerName);
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
    }

    #endregion
}
