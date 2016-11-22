using UnityEngine;

public class AntStraight: Ant
{
    protected AntAgentStraight agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentStraight(character);
        character.agent = agent;
    }

    #endregion
}
