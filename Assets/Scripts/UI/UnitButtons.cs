using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitButtons : MonoBehaviour {

    private CanvasGroup canvas;

    private Unit unit;
    private Building building;

    [System.Serializable]
    public enum WorkerTasks
    {
        Idle, Gather
    }

    private void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 0f;
    }
    
    public void UnitSelected(GameObject newObj)
    {
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;

        unit = newObj.GetComponent<Unit>();
        building = newObj.GetComponent<Building>();
        // TODO buttons based on unit type

    }

    public void Deselected()
    {
        canvas.alpha = 0f;
        canvas.blocksRaycasts = false;
        unit = null;
    }

    public void SetIdle()
    {
        if (unit != null)
            unit.SetTask(WorkerTasks.Idle);

        if (building != null)
            ((Facility)building).CancelTrainingUnit();
    }
    public void SetGather()
    {
        if (unit != null)
            unit.SetTask(WorkerTasks.Gather);

        if (building != null)
            ((Facility)building).BeginTrainingUnit();
    }
}
