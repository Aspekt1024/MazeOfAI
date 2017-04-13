using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour {

    public float ObjRadius;

    public bool IsType<T>()
    {
        return GetType().Equals(typeof(T));
    }
}
