using UnityEngine;

using System;
using System.Collections;


public class AntAgentStatic: Agent<IAntCharacter, IState<IAntCharacter>>
{
    #region States 

    private class Idle : IState<IAntCharacter>
    {
        public void Enter(IAntCharacter character) { }

        public void Exit(IAntCharacter character) { }
    }

    #endregion

    public AntAgentStatic(IAntCharacter character) : base(character)
    {
        State = new Idle();
    }
}
