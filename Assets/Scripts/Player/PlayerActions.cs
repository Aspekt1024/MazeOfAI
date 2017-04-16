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
            GameObject resParent = GameObject.FindGameObjectWithTag("Resources");
            if (resParent == null)
            {
                resParent = new GameObject("Resources");
                resParent.tag = "Resources";
            }
            Instantiate(Resources.Load("Spawnables/Capsule"), transform.position + Vector3.down * 0.6f, Quaternion.identity, resParent.transform);
        }
    }
}
