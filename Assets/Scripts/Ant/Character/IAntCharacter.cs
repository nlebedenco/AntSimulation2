using UnityEngine;
using System;
using System.Collections;

public interface IAntCharacter: ICharacter
{
    Agent agent { get; set; }

    event Action Updated;

    event Action<IPredatorCharacter> PredatorFound;
    event Action<IPredatorCharacter> PredatorLost;

    event Action<IAntCharacter> AntFound;
    event Action<IAntCharacter> AntLost;
}