using UnityEngine;
using System.Collections.Generic;

public class AntAgentEvasive : AntAgentEvasive<IAntCharacter>
{
    public AntAgentEvasive(IAntCharacter character) : base(character) { }
}

public class AntAgentEvasive<TCharacter> : Agent<TCharacter, AntAgentEvasive<TCharacter>.IAgentState>
    where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter>
    {
        IAgentState OnUpdate();
        IAgentState OnPredatorFound(IPredatorCharacter predator);
        IAgentState OnPredatorLost(IPredatorCharacter predator);
    }

    #region States

    private abstract class AgentState : IAgentState
    {
        protected AntAgentEvasive<TCharacter> agent { get; private set; }

        public AgentState(AntAgentEvasive<TCharacter> agent)
        {
            this.agent = agent;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract IAgentState OnPredatorFound(IPredatorCharacter predator);
        public abstract IAgentState OnPredatorLost(IPredatorCharacter predator);
        public abstract IAgentState OnUpdate();
    }

    private class Safe : AgentState
    {
        public Safe(AntAgentEvasive<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.Stop();
        }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            return this;
        }

        public override IAgentState OnPredatorFound(IPredatorCharacter predator)
        {
            agent.threats.Add(predator.transform);
            return new Evasive(agent);
        }

        public override IAgentState OnPredatorLost(IPredatorCharacter predator)
        {
            agent.threats.Remove(predator.transform);
            return this;

        }
    }

    private class Evasive : AgentState
    {
        public Evasive(AntAgentEvasive<TCharacter> agent) : base(agent) { }

        public override void OnEnter() { }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            agent.Escape();

            return this;
        }

        public override IAgentState OnPredatorFound(IPredatorCharacter predator)
        {
            agent.threats.Add(predator.transform);
            return this;
        }

        public override IAgentState OnPredatorLost(IPredatorCharacter predator)
        {
            agent.threats.Remove(predator.transform);
            if (agent.threats.Count == 0)
                return new Safe(agent);
            return this;
        }
    }

    #endregion

    public List<Transform> threats { get; private set; }

    public AntAgentEvasive(TCharacter character) : base(character)
    {
        character.Updated += character_Updated;
        character.PredatorFound += character_PredatorFound;
        character.PredatorLost += character_PredatorLost;

        state = new Safe(this);

        this.threats = new List<Transform>();
    }

    public void Stop()
    {
        character.Stop();
    }

    public void Move(Vector3 direction)
    {
        character.Move(direction);
    }

    public void MoveForward()
    {
        character.Move(character.transform.forward);
    }

    public void Escape()
    {
        Vector3 escapeDirection = Vector3.zero;
        Vector3 position = character.transform.position;
        for (int i = 0; i < threats.Count; ++i)
            escapeDirection += (position - threats[i].transform.position);

        escapeDirection.y = 0;
        Move(escapeDirection.normalized);
    }

    #region IAntCharacter Event Handlers

    private void character_Updated()
    {
        state = state.OnUpdate();
    }

    private void character_PredatorFound(IPredatorCharacter predator)
    {
        state = state.OnPredatorFound(predator);
    }

    private void character_PredatorLost(IPredatorCharacter predator)
    {
        state = state.OnPredatorLost(predator);
    }

    #endregion
}
