using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ICharacterMovement))]
public class Ant : MonoBehaviour
{
    [ReadOnly]
    public float walkSpeedFactor = 1f;

    [ReadOnly]
    public float runSpeedFactor = 2f;

    [ReadOnly]
    public float chanceToTurn = 0.25f;

    [ReadOnly]
    public float minTurnAngle = -180f;

    [ReadOnly]
    public float maxTurnAngle = 180f;

    [ReadOnly]
    public float chanceToJump = 0.001f;

    [ReadOnly]
    public float fearStrength = 1f;

    [ReadOnly]
    public float fearDecay = 0.05f;

    public bool isFearContagious = true;

    [ReadOnly(RunMode.Any)]
    public float fear = 0;

    ICharacterMovement character;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<ICharacterMovement>();
    }

    Vector3 moveDirection;

    void Start()
    {
        moveDirection = transform.forward;
    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (Random.value < chanceToTurn)
        {
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(minTurnAngle, maxTurnAngle), Vector3.up);
            moveDirection = rotation * moveDirection;
        }

        if (Random.value < chanceToJump)
        {
            character.Jump();
        }

        character.Move(moveDirection);
    }

    #endregion
}
