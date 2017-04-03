using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KeyboardInput = InputHandler.KeyboardInput;

public class PlayerMovement : MonoBehaviour {

    public float speed = 5f;
    
    private float camRayLength = 100f;
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

	}
}
