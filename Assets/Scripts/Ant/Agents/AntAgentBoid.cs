using UnityEngine;
using System.Collections.Generic;

public class AntAgentBoid : AntAgentBoid<IAntCharacter>
{
    public AntAgentBoid(IAntCharacter character, float chanceToTurn, float maxTurningAngle, float minDistanceToFriend, float maxDistanceToFriend) : base(character, chanceToTurn, maxTurningAngle, minDistanceToFriend, maxDistanceToFriend) { }
}

public class AntAgentBoid<TCharacter> : Agent<TCharacter, AntAgentBoid<TCharacter>.IAgentState>
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
        protected AntAgentBoid<TCharacter> agent { get; private set; }

        public AgentState(AntAgentBoid<TCharacter> agent)
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

    private class Safe : AgentState
    {
        public Safe(AntAgentBoid<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.Stop();
        }

        public override void OnExit() { }

        public override IAgentState OnUpdate()
        {
            return agent.ShouldFollow ? new Scared(agent) as IAgentState : this as IAgentState;
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
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Add(boid);
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Remove(boid);

            return agent.friends.Count > 0 ? this as IAgentState : new Lost(agent) as IAgentState;
        }
    }

    private class Evasive : AgentState
    {
        public Evasive(AntAgentBoid<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.CallToFollow(agent.FindEscapeDirection(), agent as AntAgentBoid);
        }

        public override void OnExit()
        {
            agent.Turn(Random.value > 0.5f ? 90 : -90);
            agent.MoveForward();
        }

        public override IAgentState OnUpdate()
        {
            agent.Move(agent.FindEscapeDirection());
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
                return agent.friends.Count > 0 ? new Regroup(agent) as IAgentState : new Lost(agent) as IAgentState;

            return this;
        }

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Add(boid);
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Remove(boid);
            return this;
        }
    }

    private class Scared : AgentState
    {
        public Scared(AntAgentBoid<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.CallToFollow(agent.directionToFollow, agent as AntAgentBoid);
            agent.Move(agent.directionToFollow);
        }

        public override void OnExit()
        {
            agent.Rest();
        }

        public override IAgentState OnUpdate()
        {
            return agent.ShouldFollow ? this as IAgentState : new Regroup(agent) as IAgentState;
        }

        public override IAgentState OnPredatorFound(IPredatorCharacter predator)
        {
            agent.threats.Add(predator.transform);
            return new Evasive(agent);
        }

        public override IAgentState OnPredatorLost(IPredatorCharacter predator)
        {
            agent.threats.Remove(predator.transform);
            if (agent.threats.Count == 0)
                return agent.friends.Count > 0 ? new Safe(agent) as IAgentState : new Lost(agent) as IAgentState;

            return this;
        }

        public override IAgentState OnAntFound(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Add(boid);
            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Remove(boid);

            if (boid == agent.callerToFollow)
                return new Regroup(agent);

            return agent.friends.Count > 0 ? this as IAgentState : new Lost(agent) as IAgentState;
        }
    }

    private class Regroup : AgentState
    {
        public Regroup(AntAgentBoid<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            agent.CallToRest();
            agent.character.moveForce *= 2;
            agent.MoveForward();
        }

        public override void OnExit()
        {
            agent.character.moveForce /= 2;
        }

        public override IAgentState OnUpdate()
        {
            Vector3 optimalPosition = agent.AveragePositionOfMostFriends();
            float distance = Vector3.Distance(optimalPosition, agent.character.transform.position);
            Vector3 directionToCluster = (optimalPosition - agent.character.transform.position) / distance;
            if (distance > agent.maxDistanceToFriends)
            {
                agent.Move(directionToCluster);
                return agent.ShouldFollow ? new Scared(agent) as IAgentState : this as IAgentState;
            }
            
            return agent.ShouldFollow ? new Scared(agent) as IAgentState : new Safe(agent) as IAgentState; 
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
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Add(boid);

            return this;
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Remove(boid);

            return agent.friends.Count > 0 ? this as IAgentState : new Lost(agent) as IAgentState;
        }
    }

    private class Lost : AgentState
    {
        public Lost(AntAgentBoid<TCharacter> agent) : base(agent) { }

        public override void OnEnter()
        {
            Debug.Log("Enter Lost");
            agent.character.moveForce *= 2;
            agent.MoveForward();
        }

        public override void OnExit()
        {
            agent.character.moveForce /= 2;
            Debug.Log("Exit Lost");
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
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Add(boid);

            return new Regroup(agent);
        }

        public override IAgentState OnAntLost(IAntCharacter ant)
        {
            AntAgentBoid boid = ant.agent as AntAgentBoid;
            if (boid != null)
                agent.friends.Remove(boid);
            return this;
        }
    }

    #endregion

    public List<Transform> threats { get; private set; }
    public List<AntAgentBoid> friends { get; private set; }

    public AntAgentBoid(TCharacter character, float chanceToTurn, float maxTurningAngle, float minDistanceToFriend, float maxDistanceToFriend) : base(character)
    {
        character.Updated += character_Updated;
        character.PredatorFound += character_PredatorFound;
        character.PredatorLost += character_PredatorLost;
        character.AntFound += character_AntFound;
        character.AntLost += character_AntLost;

        state = new Safe(this);

        this.chanceToTurn = chanceToTurn;
        this.maxTurningAngle = maxTurningAngle;
        this.minDistanceToFriend = minDistanceToFriend;
        this.maxDistanceToFriends = maxDistanceToFriend;

        this.threats = new List<Transform>();
        this.friends = new List<AntAgentBoid>();
    }

    [Header("Flocking")]

    public float minDistanceToFriend;
    public float maxDistanceToFriends;

    [Header("Random Movement")]

    public float chanceToTurn;
    public float maxTurningAngle;

    public void Stop()
    {
        character.Stop();
    }

    public void Move(Vector3 direction)
    {
        character.Move(direction);
    }

    public void Turn(float angle)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, character.transform.up);
        character.Turn(rotation);
    }

    public void MoveForward()
    {
        character.Move(character.transform.forward);
    }

    public void TurnAtRandom()
    {
        if (Random.value < chanceToTurn)
        {
            Turn(Random.Range(-maxTurningAngle, maxTurningAngle));
        }
    }

    public Vector3 FindEscapeDirection()
    {
        Vector3 escapeDirection = Vector3.zero;
        Vector3 position = character.transform.position;
        for (int i = 0; i < threats.Count; ++i)
            escapeDirection += (position - threats[i].transform.position);

        escapeDirection.y = 0;
        return escapeDirection.normalized;
    }

    public Vector3 DirectionToMostFriends()
    {
        Vector3 directionToMostFriends = Vector3.zero;

        Vector3 position = character.transform.position;
        for (int i = 0; i < friends.Count; ++i)
        {
            directionToMostFriends += (friends[i].character.transform.position - position);
        }

        return directionToMostFriends.normalized;
    }

    public Vector3 AveragePositionOfMostFriends()
    {
        Vector3 direction = DirectionToMostFriends();
        Vector3 position = character.transform.position;
        Vector3 groupPosition = Vector3.zero;
        int count = 0;
        for (int i = 0; i < friends.Count; ++i)
        {
            Vector3 toFriend = (friends[i].character.transform.position - position);
            if (Vector3.Dot(direction, toFriend) > 0)
            {
                groupPosition += friends[i].character.transform.position;
                count++;
            }
            
        }
        groupPosition /= count;
        return groupPosition;
    }

    public Vector3 directionToFollow;
    public AntAgentBoid callerToFollow;
    private bool shouldFollow;
    public bool ShouldFollow
    {
        get { return shouldFollow; }
    }

    public void Rest()
    {
        shouldFollow = false;
        directionToFollow = Vector3.zero;
        callerToFollow = null;
    }

    public void Follow(Vector3 direction)
    {
        shouldFollow = true;
        directionToFollow = direction;
    }

    public void CallToFollow(Vector3 direction, AntAgentBoid caller)
    {
        callerToFollow = caller;
        for (int i = 0; i < friends.Count; ++i)
            friends[i].Follow(direction);
    }

    public void CallToRest()
    {
        for (int i = 0; i < friends.Count; ++i)
            friends[i].Rest();
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
