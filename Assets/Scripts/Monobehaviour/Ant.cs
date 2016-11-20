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

    IAntCharacter character;
    AntAgentErratic agent;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<IAntCharacter>();
        agent = new AntAgentErratic(character, chanceToTurn, maxTurningAngle);
    }

    void Update()
    {
        agent.chanceToTurn = chanceToTurn;
        agent.maxTurningAngle = maxTurningAngle;
    }

    #endregion
}
