using UnityEngine;

using System;
using System.Collections;

public class AntCharacter : RigidbodyCharacter, IAntCharacter
{
    public event Action Updated;
    public event Action<Predator> PredatorFound;
    public event Action<Predator> PredatorLost;

    protected void OnUpdated()
    {
        var handler = Updated;
        if (handler != null)
            handler();
    }

    protected void OnPredatorFound(Predator predator)
    {
        var handler = PredatorFound;
        if (handler != null)
            handler(predator);
    }

    protected void OnPredatorLost(Predator predator)
    {
        var handler = PredatorLost;
        if (handler != null)
            handler(predator);
    }

    #region Unity Events 

    protected override void FixedUpdate()
    {
        OnUpdated();

        base.FixedUpdate();
    }

    #endregion
}
