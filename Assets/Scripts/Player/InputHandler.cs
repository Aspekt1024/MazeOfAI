using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler {
    
    public enum KeyboardInput
    {
        Forward, Back, StrafeLeft, StrafeRight,
        Run
    }

    public enum ActionInput
    {
        Jump
    }

    static Dictionary<KeyboardInput, KeyCode> keyDict = new Dictionary<KeyboardInput, KeyCode>
    {
        { KeyboardInput.Forward, KeyCode.W },
        { KeyboardInput.Back, KeyCode.S },
        { KeyboardInput.StrafeLeft, KeyCode.A },
        { KeyboardInput.StrafeRight, KeyCode.D },
        { KeyboardInput.Run, KeyCode.LeftShift }
    };

    static Dictionary<ActionInput, KeyCode> actionDict = new Dictionary<ActionInput, KeyCode>
    {
        { ActionInput.Jump, KeyCode.Space }
    };

    private Dictionary<ActionInput, bool> actionPressed = new Dictionary<ActionInput, bool>();

    public InputHandler()
    {
        SetupKeyPresses();
    }

    public bool ActionPressed(ActionInput actionKey)
    {
        if (!actionPressed[actionKey]) return false;
        actionPressed[actionKey] = false;
        return true;
    }

    public bool KeyPressed(KeyboardInput key)
    {
        return Input.GetKey(keyDict[key]);
    }

    public void ClearInput()
    {
        foreach (ActionInput keyInput in Enum.GetValues(typeof(ActionInput)))
        {
            actionPressed[keyInput] = false;
        }
    }
    
	public void RefreshInputs ()
    {
        GetActionPresses();
	}

    private void SetupKeyPresses()
    {
        foreach (ActionInput keyInput in Enum.GetValues(typeof(ActionInput)))
        {
            actionPressed.Add(keyInput, false);
        }
    }
    
    private void GetActionPresses()
    {
        foreach (ActionInput keyInput in Enum.GetValues(typeof(ActionInput)))
        {
            if (Input.GetKeyDown(actionDict[keyInput]))
            {
                actionPressed[keyInput] = true;
            }
        }
    }

}
