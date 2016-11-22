using UnityEngine;

public class AntAgentStraight : AntAgentStraight<IAntCharacter>
{
    public AntAgentStraight(IAntCharacter character) : base(character) { }
}

public class AntAgentStraight<TCharacter>: Agent<TCharacter, AntAgentStraight<TCharacter>.IAgentState>
 where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter> { }

    #region States 

    private class Straight : IAgentState
    {
        AntAgentStraight<TCharacter> agent;

        public Straight(AntAgentStraight<TCharacter> agent)
        {
            this.agent = agent;
        }

        public void OnEnter()
        {
            agent.MoveForward();
        }

        public void OnExit() { }
    }

    #endregion

    public AntAgentStraight(TCharacter character) : base(character)
    {
        state = new Straight(this);
    }

    public void MoveForward()
    {
        character.Move(character.transform.forward);
    }
}
