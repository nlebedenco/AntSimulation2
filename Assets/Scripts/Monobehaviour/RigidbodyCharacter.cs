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

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540f;

    protected Rigidbody rb;

    private Vector3 desiredDirection = Vector3.zero;

    #region ICharacter 

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

    #endregion

    #region Unity Events 

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    protected virtual void FixedUpdate()
    {
        rb.AddForce(desiredDirection * moveForce, moveForceMode);
        rb.velocity *= Mathf.Clamp01(1f - (moveDrag * Time.deltaTime));

#if UNITY_EDITOR
        Vector3 grounVelocity = rb.velocity;
        grounVelocity.y = 0;
        groundSpeed = grounVelocity.magnitude;
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
