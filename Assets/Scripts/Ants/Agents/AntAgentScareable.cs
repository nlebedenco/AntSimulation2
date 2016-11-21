using UnityEngine;
using System.Collections.Generic;

public class AntAgentScareable : AntAgentScareable<IAntCharacter>
{
    public AntAgentScareable(IAntCharacter character, float chanceToTurn = 0, float maxTurningAngle = 90) : base(character, chanceToTurn, maxTurningAngle) { }
}

public class AntAgentScareable<TCharacter> : Agent<TCharacter, AntAgentScareable<TCharacter>.IAgentState>
    where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter>
    {
        IAgentState OnUpdate();
        IAgentState OnPredatorFound(Predator predator);
        IAgentState OnPredatorLost(Predator predator);
    }

    #region States

    private abstract class AgentState : IAgentState
    {
        protected AntAgentScareable<TCharacter> agent { get; private set; }

        public AgentState(AntAgentScareable<TCharacter> agent)
        {
            this.agent = agent;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract IAgentState OnPredatorFound(Predator predator);
        public abstract IAgentState OnPredatorLost(Predator predator);
        public abstract IAgentState OnUpdate();
    }

    private class Safe : AgentState
    {
        public Safe(AntAgentScareable<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.MoveForward();
            // agent.Stop();
        }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            agent.TurnAtRandom();
            return this;
        }

        public override IAgentState OnPredatorFound(Predator predator)
        {
            agent.threats.Add(predator.transform);
            return new Scared(agent);
        }

        public override IAgentState OnPredatorLost(Predator predator)
        {
            agent.threats.Remove(predator.transform);
            return this;

        }
    }

    private class Scared : AgentState
    {
        public Scared(AntAgentScareable<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            // agent.Stop();
        }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            Vector3 escapeDirection = Vector3.zero;
            Vector3 position = agent.character.transform.position;
            var threats = agent.threats;
            for (int i = 0; i < threats.Count; ++i)
                escapeDirection += (position - threats[i].transform.position);

            escapeDirection.y = 0;
            agent.Move(escapeDirection.normalized);

            return this;
        }

        public override IAgentState OnPredatorFound(Predator predator)
        {
            agent.threats.Add(predator.transform);
            return this;
        }

        public override IAgentState OnPredatorLost(Predator predator)
        {
            agent.threats.Remove(predator.transform);
            if (agent.threats.Count == 0)
                return new Safe(agent);
            return this;
        }
    }

    #endregion

    public float chanceToTurn;
    public float maxTurningAngle;

    public List<Transform> threats { get; private set; }

    public AntAgentScareable(TCharacter character, float chanceToTurn = 0, float maxTurningAngle = 90) : base(character)
    {
        character.Updated += character_Updated;
        character.PredatorFound += character_PredatorFound;
        character.PredatorLost += character_PredatorLost;

        state = new Safe(this);

        this.chanceToTurn = chanceToTurn;
        this.maxTurningAngle = maxTurningAngle;

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

    public void TurnAtRandom()
    {
        if (Random.value < chanceToTurn)
        {
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(-maxTurningAngle, maxTurningAngle), character.transform.up);
            character.Turn(rotation);
        }
    }

    #region IAntCharacter Event Handlers

    private void character_Updated()
    {
        state = state.OnUpdate();
    }

    private void character_PredatorFound(Predator predator)
    {
        state = state.OnPredatorFound(predator);
    }

    private void character_PredatorLost(Predator predator)
    {
        state = state.OnPredatorLost(predator);
    }

    #endregion
}
