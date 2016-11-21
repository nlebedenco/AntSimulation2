using System;

public abstract class Agent
{

}

public interface IState<TCharacter>
{
    void OnEnter();
    void OnExit();
}

public abstract class Agent<TCharacter, TState> : Agent
    where TState: IState<TCharacter>
{
    public Agent(TCharacter character)
    {
        this.character = character;
    }

    public TCharacter character { get; protected set; }

    private TState _state;
    public TState state
    {
        get { return _state; }
        protected set
        {
            if (!value.Equals(_state))
            {
                if (_state != null)
                    _state.OnExit();
                _state = value;
                if (_state != null)
                    _state.OnEnter();
            }
        }
    }
}


