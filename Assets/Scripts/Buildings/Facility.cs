using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facility : Building {

    private bool trainingUnit;
    private int unitsInTrainingQueue;
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
            if (unitsInTrainingQueue > 0)
            {
                unitsInTrainingQueue--;
                TrainUnit();
            }
        }
    }
    
    // TODO implement GUI displays

    public void BeginTrainingUnit()
    {
        if (GameData.Instance.Capsules < 10) return; // TODO create message: not enough resources
        GameData.Instance.Capsules -= 10;
        if (trainingUnit)
            unitsInTrainingQueue++;
        else
            TrainUnit();
    }

    private void TrainUnit()
    {
        trainingUnit = true;
        unitCompletion = 0;
        timeToComplete = 1f;
    }

    public void CancelTrainingUnit()
    {
        trainingUnit = false;
        GameData.Instance.Capsules += 10 * (unitsInTrainingQueue + 1);
        unitsInTrainingQueue = 0;
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
