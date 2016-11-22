using UnityEngine;

[RequireComponent(typeof(IAntCharacter))]
public class Ant: MonoBehaviour
{
    public IAntCharacter character { get; protected set; }

    #region Unity Events

    protected virtual void Awake()
    {
        character = GetComponent<IAntCharacter>();
    }

    #endregion
}
