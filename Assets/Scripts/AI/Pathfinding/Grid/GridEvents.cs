using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO remove? not currently used.
public static class GridEvents {

    public delegate void GridEvent();
    public static GridEvent OnPlaceBuilding;

    public static void PlaceBuilding()
    {
        if (OnPlaceBuilding != null)
            OnPlaceBuilding();
    }
	
}
