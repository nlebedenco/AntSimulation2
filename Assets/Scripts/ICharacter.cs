using UnityEngine;

using System.Collections;

public interface ICharacter
{
    Transform transform { get; }

    float moveForce { get; set; }
    ForceMode moveForceMode { get; set; }
    float moveDrag { get; set; }

    void Stop();

    void Move(Vector3 direction);

    void Turn(Quaternion rotation);
}
