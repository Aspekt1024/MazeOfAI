using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

	private void Update ()
    {
        InputHandler input = player.input;
        if (input.ActionPressed(InputHandler.ActionInput.Jump))
        {
            Instantiate(Resources.Load("Spawnables/Capsule"), transform.position + Vector3.down * 0.6f, Quaternion.identity, transform);
        }
    }
}
