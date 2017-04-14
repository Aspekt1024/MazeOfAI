using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Selectable : MonoBehaviour {

    public float ObjRadius;

    public bool IsType<T>()
    {
        return GetType().Equals(typeof(T));
    }
    
    public static T GetEnumFromId<T>(int id)
    {
        foreach (object e in Enum.GetValues(typeof(T)))
        {
            if ((int)e == id)
                return (T)e;
        }
        return default(T);
    }
}
