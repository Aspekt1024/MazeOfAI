using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float BaseSpeed = 20f;
    [HideInInspector]
    public InputHandler input;
    
    private void Awake()
    {
        gameObject.AddComponent<Selector>();
        gameObject.AddComponent<PlayerMovement>();
        gameObject.AddComponent<PlayerActions>();
        input = new InputHandler();
    }
}
