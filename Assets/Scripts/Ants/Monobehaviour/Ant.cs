using UnityEngine;

[RequireComponent(typeof(IAntCharacter))]
public class Ant: MonoBehaviour
{
    protected IAntCharacter character;

    #region Unity Events

    protected virtual void Awake()
    {
        character = GetComponent<IAntCharacter>();
    }

    #endregion
}
