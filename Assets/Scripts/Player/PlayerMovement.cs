using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KeyboardInput = InputHandler.KeyboardInput;

public class PlayerMovement : MonoBehaviour {

    public float baseSpeed = 8f;
    
    private Rigidbody body;
    private InputHandler input;

    private bool runEnabled;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        input = new InputHandler();
    }
    
	private void Update ()
    {
        input.RefreshInputs();
        float speed = baseSpeed * (input.KeyPressed(KeyboardInput.Run) ? 2 : 1);
        if (input.KeyPressed(KeyboardInput.Forward))
        {
            body.velocity = new Vector3(body.velocity.x, body.velocity.y, speed);
        }
        if (input.KeyPressed(KeyboardInput.Back))
        {
            body.velocity = new Vector3(body.velocity.x, body.velocity.y, -speed);
        }
        if (input.KeyPressed(KeyboardInput.StrafeLeft))
        {
            body.velocity = new Vector3(-speed, body.velocity.y, body.velocity.z);
        }
        if (input.KeyPressed(KeyboardInput.StrafeRight))
        {
            body.velocity = new Vector3(speed, body.velocity.y, body.velocity.z);
        }

        // TODO move to player actions
        if (input.ActionPressed(InputHandler.ActionInput.Jump))
        {
            Instantiate(Resources.Load("Spawnables/Capsule"), transform.position + Vector3.down * 0.6f, Quaternion.identity, transform);
        }

	}
}
