using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health {

    private int currentHP;
    private int maxHP;

    public int CurrentHP
    {
        get { return currentHP; }
        set { currentHP = Mathf.Clamp(value, 0, maxHP); }
    }
    public int MaxHP
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    public bool IsDead()
    {
        return currentHP == 0;
    }
    public bool IsAlive()
    {
        return currentHP > 0;
    }
    
}
