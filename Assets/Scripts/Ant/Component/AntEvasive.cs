using UnityEngine;

public class AntEvasive : Ant
{
    protected AntAgentEvasive agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentEvasive(character);
        character.agent = agent;
    }

    #endregion
}
