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
    

    private void Awake()
    {
        level = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Level>();
    }

    public virtual void DeploySection() { }
    
    protected void AlignSectionToGrid()
    {
        Level level = GameObject.FindGameObjectWithTag("Scripts").GetComponent<Level>();
        float xPos = transform.position.x - (transform.position.x - Mathf.FloorToInt(transform.position.x / level.SegmentLength) * level.SegmentLength);
        float zPos = transform.position.z - (transform.position.z - Mathf.FloorToInt(transform.position.z / level.SegmentLength) * level.SegmentLength);

        transform.position = new Vector3(xPos, 0f, zPos);
    }
    
    public bool IsType<T>()
    {
        return GetType().Equals(typeof(T));
    }
}
