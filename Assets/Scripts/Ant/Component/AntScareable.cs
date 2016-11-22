using UnityEngine;

public class AntScareable : Ant
{
    public float chanceToTurn = 0.25f;
    public float maxTurningAngle = 90f;
    public float maxFear = 10f;
    public float fearDecayRate = 0.5f;

    [ReadOnly(RunMode.Any)]
    public float fear;

    protected AntAgentScareable agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentScareable(character, chanceToTurn, maxTurningAngle, maxFear, fearDecayRate);
        character.agent = agent;
    }

    void Update()
    {
        agent.chanceToTurn = chanceToTurn;
        agent.maxTurningAngle = maxTurningAngle;
        agent.maxFear = maxFear;

        fear = agent.fear;
    }

    #endregion
}
