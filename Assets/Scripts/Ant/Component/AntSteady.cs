using UnityEngine;

public class AntSteady: Ant
{
    protected AntAgentSteady agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentSteady(character);
        character.agent = agent;
    }

    #endregion
}
