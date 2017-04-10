using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float BaseSpeed = 8f;
    [HideInInspector]
    public InputHandler input;

    private PlayerMovement movement;
    private PlayerActions actions;
    private Selector selector;

    private void Awake()
    {
        selector = gameObject.AddComponent<Selector>();
        movement = gameObject.AddComponent<PlayerMovement>();
        actions = gameObject.AddComponent<PlayerActions>();
        input = new InputHandler();
    }
}
