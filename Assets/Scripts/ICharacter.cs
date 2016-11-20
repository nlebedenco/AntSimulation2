using UnityEngine;

using System.Collections;

public interface ICharacter
{
    Transform transform { get; }

    bool isGrounded { get; }
    bool isJumping { get; }
    bool isLanding { get; }

    void Stop();

    void Move(Vector3 direction);

    void Turn(Quaternion rotation);

    void Jump();
}
