using UnityEngine;
using System.Collections;

public interface ICharacterMovement
{
    bool isGrounded { get; }

    void Stop();

    void Move(Vector3 direction);

    void Jump();
}
