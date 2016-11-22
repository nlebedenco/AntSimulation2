using UnityEngine;

public class AntStatic: Ant
{
    protected AntAgentStatic agent;

    #region Unity Events

    protected override void Awake()
    {
        base.Awake();

        agent = new AntAgentStatic(character);
        character.agent = agent;
    }

    #endregion
}
