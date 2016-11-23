using UnityEngine;
using System.Collections;

public interface IPredatorCharacter : ICharacter
{
    Agent agent { get; set; }

    bool isStealthed { get; }
    bool isGrounded { get; }
    bool isJumping { get; }
    bool isLanding { get; }

    void Jump();
    void Stealth();
}
