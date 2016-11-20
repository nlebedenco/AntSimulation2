using UnityEngine;

//using System;
using System.Collections;

public interface IUpdatableState<TCharacter> : IState<TCharacter>
{
    void Updated(TCharacter character);
}

public class AntAgentErratic : Agent<IAntCharacter, IUpdatableState<IAntCharacter>>
{
    #region States

    private class Lost : IUpdatableState<IAntCharacter>
    {
        AntAgentErratic agent;

        public Lost(AntAgentErratic agent)
        {
            this.agent = agent;
        }
            
        public void Enter(IAntCharacter character)
        {
            character.Move(character.transform.forward);
        }

        public void Exit(IAntCharacter character) { }

        public void Updated(IAntCharacter character)
        {
            if (Random.value < agent.chanceToTurn)
            {
                Quaternion rotation = Quaternion.AngleAxis(Random.Range(-agent.maxTurningAngle, agent.maxTurningAngle), character.transform.up);
                character.Turn(rotation);
            }

            if (Random.value < agent.chanceToJump)
                character.Jump();
        }
    }

    #endregion

    public float chanceToJump;
    public float chanceToTurn;
    public float maxTurningAngle;

    public AntAgentErratic(IAntCharacter character, float chanceToJump = 0, float chanceToTurn = 0, float maxTurningAngle = 90) : base(character)
    {
        character.Updated += HandleUpdated;

        State = new Lost(this);

        this.chanceToJump = chanceToJump;
        this.chanceToTurn = chanceToTurn;
        this.maxTurningAngle = maxTurningAngle;
}

    private void HandleUpdated()
    {
        State.Updated(Character);
    }
}
