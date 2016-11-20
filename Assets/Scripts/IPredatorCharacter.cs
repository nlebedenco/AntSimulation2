using UnityEngine;
using System.Collections;

public interface IPredatorCharacter : ICharacter
{
    bool isGrounded { get; }
    bool isJumping { get; }
    bool isLanding { get; }

    void Jump();
}
