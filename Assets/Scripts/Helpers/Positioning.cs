using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Positioning {

	public static void SnapObjectToGrid(Transform obj)
    {
        obj.transform.position = GetSnappedPosition(obj.position);
    }

    public static void SnapObjectToGrid(GameObject obj)
    {
        SnapObjectToGrid(obj.transform);
    }

    public static Vector3 GetSnappedPosition(Transform obj)
    {
        Vector3 pos = obj.position;
        return GetSnappedPosition(pos);
    }

    public static Vector3 GetSnappedPosition(Vector3 pos)
    {
        return new Vector3(Mathf.Round(pos.x) + 0.5f, Mathf.Round(pos.y), Mathf.Round(pos.z) + 0.5f);
    }

}
