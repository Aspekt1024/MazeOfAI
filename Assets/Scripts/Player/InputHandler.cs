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
    private Dictionary<KeyboardInput, bool> keyPressed = new Dictionary<KeyboardInput, bool>();

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
        return keyPressed[key];
    }

    public void ClearInput()
    {
        foreach (KeyboardInput keyInput in Enum.GetValues(typeof(KeyboardInput)))
        {
            keyPressed[keyInput] = false;
        }
        foreach (ActionInput keyInput in Enum.GetValues(typeof(ActionInput)))
        {
            actionPressed[keyInput] = false;
        }
    }
    
	public void RefreshInputs ()
    {
        GetKeyPresses();
        GetActionPresses();
	}

    private void SetupKeyPresses()
    {
        foreach (KeyboardInput keyInput in Enum.GetValues(typeof(KeyboardInput)))
        {
            keyPressed.Add(keyInput, false);
        }
        foreach (ActionInput keyInput in Enum.GetValues(typeof(ActionInput)))
        {
            actionPressed.Add(keyInput, false);
        }
    }

    private void GetKeyPresses()
    {
        foreach (KeyboardInput keyInput in Enum.GetValues(typeof(KeyboardInput)))
        {
            if (Input.GetKeyDown(keyDict[keyInput]))
            {
                keyPressed[keyInput] = true;
            }
            else if (Input.GetKeyUp(keyDict[keyInput]))
            {
                keyPressed[keyInput] = false;
            }
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
