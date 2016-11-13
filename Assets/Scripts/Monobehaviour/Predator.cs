using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ICharacterMovement))]
public class Predator: MonoBehaviour
{
    private ICharacterMovement character;

    #region Unity Events

    void Awake()
    {
        character = GetComponent<ICharacterMovement>();
    }
	
	void Update()
    {
        if (character.isGrounded)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            direction = Camera.main.transform.TransformDirection(direction);
            direction.y = 0;
            character.Move(direction.normalized);

            if (Input.GetButton("Jump"))
                character.Jump();
        }
    }

    #endregion
}
