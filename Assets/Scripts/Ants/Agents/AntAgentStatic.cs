using UnityEngine;

public class AntAgentStatic : AntAgentStatic<IAntCharacter>
{
    public AntAgentStatic(IAntCharacter character) : base(character) { }
}

public class AntAgentStatic<TCharacter> : Agent<TCharacter, AntAgentStatic<TCharacter>.IAgentState>
    where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter> { }

    #region States 

    private class Static : IAgentState
    {
        public void OnEnter() { }

        public void OnExit() { }
    }

    #endregion

    public AntAgentStatic(TCharacter character) : base(character)
    {
        state = new Static();
    }
}
