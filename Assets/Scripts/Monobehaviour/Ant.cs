using UnityEngine;

using System;
using System.Collections;

[RequireComponent(typeof(IAntCharacter))]
public class Ant: MonoBehaviour
{
    [ReadOnly]
    public float chanceToTurn = 0.25f;

    [ReadOnly]
    public float maxTurningAngle = 90f;

    [ReadOnly]
    public float chanceToJump = 0.001f;

    IAntCharacter character;
    // AntAgentErratic agent;
    Agent agent;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<IAntCharacter>();
        // agent = new AntAgentErratic(character, chanceToJump, chanceToTurn, maxTurningAngle);
        agent = new AntAgentStatic(character);
    }

    void Update()
    {
        // agent.chanceToJump = chanceToJump;
        // agent.chanceToTurn = chanceToTurn;
        // agent.maxTurningAngle = maxTurningAngle;
    }

    #endregion
}
