using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Selectable {

    public int GridSizeX = 1;
    public int GridSizeY = 1;

    protected float completion;
    protected bool completed;

    private void Awake()
    {
        ObjRadius = 70;
        completed = false;
    }

    public virtual void Deploy()
    {
        completed = true;
    }

    private void Update()
    {
        if (completed) return;
        transform.position = new Vector3(transform.position.x, (completion - 0.5f), transform.position.z);
        transform.localScale = Vector3.one * completion;
    }

    public void IncreaseCompletion(float percent)
    {
        completion += percent;

        if (completion >= 1)
        {
            completion = 1;
            completed = true;
        }
    }

    public virtual string GetProgressString()
    {
        return "";
    }

    public virtual Vector3 GetEntryPoint()
    {
        return transform.position;
    }
}
