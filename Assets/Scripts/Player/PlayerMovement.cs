using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KeyboardInput = InputHandler.KeyboardInput;

public class PlayerMovement : MonoBehaviour {
    
    private Rigidbody body;
    private Player player;

    private bool runEnabled;

    private void Awake()
    {
        player = GetComponent<Player>();
        body = GetComponent<Rigidbody>();
    }
    
	private void Update ()
    {
        InputHandler input = player.input;
        if (input == null) return;
        input.RefreshInputs();
        float speed = player.BaseSpeed * (input.KeyPressed(KeyboardInput.Run) ? 2 : 1);
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
