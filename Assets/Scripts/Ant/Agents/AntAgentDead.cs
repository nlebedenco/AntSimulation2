using UnityEngine;

public class AntAgentDead : AntAgentDead<IAntCharacter>
{
    public AntAgentDead(IAntCharacter character) : base(character) { }
}

public class AntAgentDead<TCharacter> : Agent<TCharacter, AntAgentDead<TCharacter>.IAgentState>
    where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter> { }

    #region States 

    private class Dead : IAgentState
    {
        public void OnEnter() { }

        public void OnExit() { }
    }

    #endregion

    public AntAgentDead(TCharacter character) : base(character)
    {
        state = new Dead();
    }
}
