using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facility : Building {

    private bool trainingUnit;
    private float unitCompletion;
    private float timeToComplete;

	private void Start ()
    {
        completed = true;
        completion = 1;
        Name = "Facility";
	}

    private void Update()
    {
        if (!trainingUnit) return;

        unitCompletion += Time.deltaTime / timeToComplete;
        if (unitCompletion >= 1)
        {
            trainingUnit = false;
            SpawnUnit();
        }
    }

    public void BeginTrainingUnit()
    {
        trainingUnit = true;
        unitCompletion = 0;
        timeToComplete = 2f;
    }

    public void CancelTrainingUnit()
    {
        trainingUnit = false;
    }

    private void SpawnUnit()
    {
        Transform unitParent = GameObject.Find("Units").transform;
        GameObject unitToSpawn = Resources.Load<GameObject>("Units/DroneType1");
        Transform unit = Instantiate(unitToSpawn, transform.position, Quaternion.identity, unitParent).transform;
        
        unit.GetComponent<Unit>().MoveUnitToPoint(transform.position, transform.position + Vector3.right * 3.5f);  // TODO waypoint
    }

    public override string GetProgressString()
    {
        if (!trainingUnit)
            return "";
        else
            return "Training: " + Mathf.RoundToInt(unitCompletion * 100) + "%";
    }
}
