using UnityEngine;

using System;
using System.Collections;

public class AntCharacter : RigidbodyCharacter, IAntCharacter
{
    public event Action Updated;

    protected void OnUpdated()
    {
        var handler = Updated;
        if (handler != null)
            handler();
    }

    #region Unity Events 

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        OnUpdated();
    }

    #endregion
}
