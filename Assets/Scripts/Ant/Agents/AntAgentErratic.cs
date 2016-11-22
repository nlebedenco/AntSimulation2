using UnityEngine;


public class AntAgentErratic : AntAgentErratic<IAntCharacter>
{
    public AntAgentErratic(IAntCharacter character, float chanceToTurn = 0, float maxTurningAngle = 90) : base(character, chanceToTurn, maxTurningAngle) { }
}


public class AntAgentErratic<TCharacter> : Agent<TCharacter, AntAgentErratic<TCharacter>.IAgentState>
    where TCharacter: IAntCharacter
{
    public interface IAgentState: IState<TCharacter>
    {
        IAgentState OnUpdate();
    }

    #region States

    private class Erratic : IAgentState
    {
        AntAgentErratic<TCharacter> agent;

        public Erratic(AntAgentErratic<TCharacter> agent)
        {
            this.agent = agent;
        }
            
        public void OnEnter()
        {
            agent.MoveForward();
        }

        public void OnExit() { }

        public IAgentState OnUpdate()
        {
            agent.TurnAtRandom();
            return this;
        }
    }

    #endregion

    public float chanceToTurn;
    public float maxTurningAngle;

    public AntAgentErratic(TCharacter character,  float chanceToTurn, float maxTurningAngle) : base(character)
    {
        character.Updated += character_Updated;

        state = new Erratic(this);

        this.chanceToTurn = chanceToTurn;
        this.maxTurningAngle = maxTurningAngle;
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

    #endregion

}
