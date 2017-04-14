using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers {
    
    public static Vector2 V3ToV2(Vector3 V3)
    {
        return new Vector2(V3.x, V3.z);
    }
}
