using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    // Pathfinding editor variables
    // TODO create custom inspector for Level
    public float SegmentLength = 1f;
    public int SegmentDivisions = 5;
    
	private void Start ()
    {
        GameData.Instance.Capsules = 100;
    }
}
