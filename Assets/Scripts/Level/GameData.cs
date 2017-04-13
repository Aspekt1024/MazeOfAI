using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData> {

    protected GameData() { }

    int capsules;

    public int Capsules
    {
        get { return capsules; }
        set {
            capsules = value;
            EventListener.CurrencyChange();
        }
    }
    

    private void Awake()
    {
        capsules = 10;
    }
}
