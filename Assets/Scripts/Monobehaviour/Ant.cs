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

    void Start()
    {
        
    }

    void Update()
    {
       
    }

    void FixedUpdate()
    {
       
    }

    #endregion
}
