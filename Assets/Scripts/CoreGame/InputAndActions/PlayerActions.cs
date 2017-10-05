using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour {
    
    private InputHandler input;
    private BuildHandler buildHandler;

    private void Awake()
    {
        buildHandler = GameObject.FindGameObjectWithTag("BuildingParent").GetComponent<BuildHandler>();
    }

    private void Start()
    {
         input = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().input;
    }

	private void Update ()
    {
        if (input.ActionPressed(InputHandler.ActionInput.Cancel))
        {
            buildHandler.CancelBuild();
        }

        if (input.ActionPressed(InputHandler.ActionInput.Build))
        {
            buildHandler.EnterBuildMode();
        }
    }


}
