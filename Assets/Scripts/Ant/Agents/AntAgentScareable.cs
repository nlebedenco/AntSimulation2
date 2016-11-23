using UnityEngine;
using System.Collections.Generic;

public class AntAgentScareable : AntAgentScareable<IAntCharacter>
{
    public AntAgentScareable(IAntCharacter character, float chanceToTurn = 0, float maxTurningAngle = 90, float maxFear = 10, float fearDecayRate = 0.5f) 
        : base(character, chanceToTurn, maxTurningAngle, maxFear, fearDecayRate) { }
}

public class AntAgentScareable<TCharacter> : Agent<TCharacter, AntAgentScareable<TCharacter>.IAgentState>
    where TCharacter : IAntCharacter
{
    public interface IAgentState : IState<TCharacter>
    {
        IAgentState OnUpdate();
        IAgentState OnPredatorFound(IPredatorCharacter predator);
        IAgentState OnPredatorLost(IPredatorCharacter predator);
        IAgentState OnAntFound(IAntCharacter ant);
        IAgentState OnAntLost(IAntCharacter ant);
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
        public abstract IAgentState OnUpdate();
        public abstract IAgentState OnPredatorFound(IPredatorCharacter predator);
        public abstract IAgentState OnPredatorLost(IPredatorCharacter predator);
        public abstract IAgentState OnAntFound(IAntCharacter ant);
        public abstract IAgentState OnAntLost(IAntCharacter ant);
    }

    private class Idle : AgentState
    {
        public Idle(AntAgentScareable<TCharacter> agent) : base(agent) { }

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

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            AntAgentScareable scareable = ant.agent as AntAgentScareable;
            if (scareable != null)
            {
                agent.Communicate(scareable);
                if (agent.isScared)
                    return new Scared(agent);
            }
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            return this;
        }
    }

    private class Evasive : AgentState
    {
        public Evasive(AntAgentScareable<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.Panic();
        }

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
                return new Scared(agent);
            return this;
        }

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            return this;
        }
    }

    private class Scared : AgentState
    {
        public Scared(AntAgentScareable<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.MoveForward();
        }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            agent.CalmDown();
            if (agent.isScared)
            {
                agent.TurnAtRandom();
                return this;
            }
            else
            {
                return new Lost(agent);
            }
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

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            return this;
        }
    }

    private class Lost : AgentState
    {
        public Lost(AntAgentScareable<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.character.moveForce /= 3;
            agent.MoveForward();
        }

        public override void OnExit()
        {
            agent.character.moveForce *= 3;
        }

        public override IAgentState OnUpdate()
        {
            agent.TurnAtRandom();
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

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            AntAgentScareable scareable = ant.agent as AntAgentScareable;
            if (scareable != null)
            {
                agent.Communicate(scareable);
                if (agent.isScared)
                    return new Scared(agent);
                else
                    return new Idle(agent);
            }

            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            return this;
        }
    }

    #endregion

    const float FearEpsilon = 0.01f;

    public AntAgentScareable(TCharacter character, float chanceToTurn, float maxTurningAngle, float maxFear, float fearDecayRate) : base(character)
    {
        character.Updated += character_Updated;
        character.PredatorFound += character_PredatorFound;
        character.PredatorLost += character_PredatorLost;
        character.AntFound += character_AntFound;
        character.AntLost += character_AntLost;

        state = new Idle(this);

        this.chanceToTurn = chanceToTurn;
        this.maxTurningAngle = maxTurningAngle;
        this.maxFear = maxFear;
        this.fearDecayRate = fearDecayRate;
        this.fear = 0;

        this.threats = new List<Transform>();
    }

    [Header("Random Movement")]

    public float chanceToTurn;
    public float maxTurningAngle;

    [Header("Fear")]

    public float maxFear;
    public float fearDecayRate;
    public float fear { get; private set; }

    public List<Transform> threats { get; private set; }
    
    public void CalmDown()
    {
        fear -= fear * fearDecayRate * Time.deltaTime;
        if (fear < FearEpsilon)
            fear = 0;
    }

    public bool isScared
    {
        get { return fear > 0; }
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

    public void Escape()
    {
        Vector3 escapeDirection = Vector3.zero;
        Vector3 position = character.transform.position;
        for (int i = 0; i < threats.Count; ++i)
            escapeDirection += (position - threats[i].transform.position);

        escapeDirection.y = 0;
        Move(escapeDirection.normalized);
    }

    public void Panic()
    {
        fear = maxFear;
    }

    public void Communicate(AntAgentScareable other)
    {
        fear = other.fear;
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

    private void character_AntFound(IAntCharacter ant)
    {
        state = state.OnAntFound(ant);
    }

    private void character_AntLost(IAntCharacter ant)
    {
        state = state.OnAntLost(ant);
    }

    #endregion
}
