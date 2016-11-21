using UnityEngine;

public class AntScareable : Ant
{
    [ReadOnly]
    public float chanceToTurn = 0.25f;

    [ReadOnly]
    public float maxTurningAngle = 90f;

    protected AntAgentScareable agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentScareable(character, chanceToTurn, maxTurningAngle);
    }

    void Update()
    {
        agent.chanceToTurn = chanceToTurn;
        agent.maxTurningAngle = maxTurningAngle;
    }

    #endregion
}
