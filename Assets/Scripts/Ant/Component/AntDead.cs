using UnityEngine;

public class AntDead: Ant
{
    protected AntAgentDead agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentDead(character);
        character.agent = agent;
    }

    #endregion
}
