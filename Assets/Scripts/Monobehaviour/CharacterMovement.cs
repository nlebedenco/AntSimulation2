using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement: MonoBehaviour, ICharacterMovement
{
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540;

    public bool isGrounded { get { return controller.isGrounded; } }

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 desiredDirection = Vector3.zero;
    private bool desiredJump = false;

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
        controller = GetComponent<CharacterController>();
    }

    void Update ()
    {
        if (controller.isGrounded)
        {
            moveDirection = desiredDirection; 
            moveDirection *= speed;
            if (desiredJump)
                moveDirection.y = jumpSpeed;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (turnToDirectionOfMovement)
        {
            var lookAt = controller.velocity;
            if (!(Mathf.Approximately(lookAt.x, 0) && Mathf.Approximately(lookAt.z, 0)))
            {
                lookAt.y = 0;
                var targetRotation = Quaternion.LookRotation(lookAt);

                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                float timeToComplete = angle / turnSpeed;
                float ratio = Mathf.Min(1, Time.deltaTime / timeToComplete);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, ratio);
            }
        }

        desiredJump = false;
    }

    #endregion
}
