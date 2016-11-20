using UnityEngine;

using System.Collections;

public interface ICharacter
{
    Transform transform { get; }

    void Stop();

    void Move(Vector3 direction);

    void Turn(Quaternion rotation);
}
