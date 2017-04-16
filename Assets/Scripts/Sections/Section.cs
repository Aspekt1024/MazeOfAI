using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour {

    public int rows = 10;
    public int cols = 10;

    [HideInInspector]
    public Vector2 lowerBound;
    [HideInInspector]
    public Vector2 upperBound;

    protected Level level;
    
    private static List<string> UnwalkableLayers = new List<string>()
    {
        {"MazeWall"}, {"BuildingBounds"}
    };
    private LayerMask unwalkableMask;

    private void Awake()
    {
        level = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Level>();
        SetupUnwalkableMask();
    }

    public virtual void DeploySection() { }
    
    protected void AlignSectionToGrid()
    {
        Level level = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Level>();
        float xPos = transform.position.x - (transform.position.x - Mathf.FloorToInt(transform.position.x / level.SegmentLength) * level.SegmentLength);
        float zPos = transform.position.z - (transform.position.z - Mathf.FloorToInt(transform.position.z / level.SegmentLength) * level.SegmentLength);

        transform.position = new Vector3(xPos, 0f, zPos);
    }
    
    private void SetupUnwalkableMask()
    {
        foreach (string layer in UnwalkableLayers)
        {
            unwalkableMask |= 1 << LayerMask.NameToLayer(layer);
        }
    }

    public bool IsType<T>()
    {
        return GetType().Equals(typeof(T));
    }
}
