using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacter: MonoBehaviour, ICharacter
{
    [Header("Movement")]

    public float moveForce = 10.0f;
    public ForceMode moveForceMode = ForceMode.VelocityChange;
    public float moveDrag = 16.3f;

    float ICharacter.moveForce { get { return moveForce; } set { moveForce = value; } }
    ForceMode ICharacter.moveForceMode { get { return moveForceMode; } set { moveForceMode = value; } }
    float ICharacter.moveDrag { get { return moveDrag; } set { moveDrag = value; } }

#if UNITY_EDITOR
    [ReadOnly(RunMode.Any)]
    public Vector3 groundVelocity;
    [ReadOnly(RunMode.Any)]
    public float groundSpeed;
#endif

    [Header("Turn")]

    public bool turnToDirectionOfMovement = true;
    public float turnSpeed = 540f;

    protected new Rigidbody rigidbody { get; private set;}

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
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
    }

    protected virtual void FixedUpdate()
    {
        rigidbody.AddForce(desiredDirection * moveForce, moveForceMode);
        float dragFactor = Mathf.Clamp01(1f - (moveDrag * Time.deltaTime));
        rigidbody.velocity = new Vector3(rigidbody.velocity.x * dragFactor, rigidbody.velocity.y, rigidbody.velocity.z * dragFactor);

#if UNITY_EDITOR
        groundVelocity = rigidbody.velocity;
        groundVelocity.y = 0;
        groundSpeed = groundVelocity.magnitude;
#endif

        if (turnToDirectionOfMovement)
        {
            if (desiredDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                float angle = Quaternion.Angle(rigidbody.rotation, targetRotation);
                float timeToComplete = angle / turnSpeed;
                float ratio = Mathf.Min(1, Time.deltaTime / timeToComplete);

                rigidbody.MoveRotation(Quaternion.Slerp(rigidbody.rotation, targetRotation, ratio));
            }
        }
    }

    #endregion

}
