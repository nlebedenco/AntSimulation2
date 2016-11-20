using System;

public abstract class Agent
{

}

public interface IState<TCharacter>
{
    void Enter(TCharacter character);
    void Exit(TCharacter character);
}

public abstract class Agent<TCharacter, TState> : Agent
    where TState: IState<TCharacter>
{
    public Agent(TCharacter character)
    {
        Character = character;
    }

    public TCharacter Character { get; protected set; }

    private TState _state;
    public TState State
    {
        get { return _state; }
        protected set
        {
            if (!value.Equals(_state))
            {
                if (_state != null)
                    _state.Exit(Character);
                _state = value;
                if (_state != null)
                    _state.Enter(Character);
            }
        }
    }
}


