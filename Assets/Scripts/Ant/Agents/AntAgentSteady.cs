using UnityEngine;

public class AntAgentSteady : AntAgentStraight<IAntCharacter>
{
    public AntAgentSteady(IAntCharacter character) : base(character) { }
}

public class AntAgentStraight<TCharacter>: Agent<TCharacter, AntAgentStraight<TCharacter>.IAgentState>
 where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter>
    {
        IAgentState OnBlocked(Collider obstacle);
    }

    #region States 

    private abstract class AgentState : IAgentState
    {
        protected AntAgentStraight<TCharacter> agent { get; private set; }

        public AgentState(AntAgentStraight<TCharacter> agent)
        {
            this.agent = agent;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract IAgentState OnBlocked(Collider obstacle);
    }

    private class Moving : AgentState
    {
        public Moving(AntAgentStraight<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.MoveForward();
        }

        public override void OnExit() { }

        public override IAgentState OnBlocked(Collider obstacle)
        {
            agent.TurnAround();
            return this;
        }
    }

    #endregion

    public AntAgentStraight(TCharacter character) : base(character)
    {
        state = new Moving(this);

        character.Blocked += character_Blocked;
    }

    public void MoveForward()
    {
        character.Move(character.transform.forward);
    }

    public void TurnAround()
    {
        character.Turn(Quaternion.AngleAxis(180, character.transform.up));
    }

    #region IAntCharacter Event Handlers

    private void character_Blocked(Collider obstacle)
    {
        state = state.OnBlocked(obstacle);
    }

    #endregion
}
