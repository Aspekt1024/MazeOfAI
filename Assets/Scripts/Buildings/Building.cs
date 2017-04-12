using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    public string Name = "unnamed building";

    protected float completion;
    protected bool completed;

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
}
