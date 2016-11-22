using UnityEngine;

public class AntErratic : Ant
{
    public float chanceToTurn = 0.25f;
    public float maxTurningAngle = 90f;

    protected AntAgentErratic agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentErratic(character, chanceToTurn, maxTurningAngle);
        character.agent = agent;
    }

    void Update()
    {
        agent.chanceToTurn = chanceToTurn;
        agent.maxTurningAngle = maxTurningAngle;
    }

    #endregion
}
