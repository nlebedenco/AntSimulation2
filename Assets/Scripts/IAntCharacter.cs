using UnityEngine;
using System;
using System.Collections;

public interface IAntCharacter: ICharacter
{
    event Action Updated;
}