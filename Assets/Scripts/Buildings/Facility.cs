using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facility : Building {

    private bool trainingUnit;
    private int unitsInTrainingQueue;
    private float unitCompletion;
    private float timeToComplete;

    private enum FacilityTasks
    {
        TrainDrone, CancelTraining
    }

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
                BeginTrainingUnit();
            }
        }
    }
    
    // TODO implement GUI displays

    private void TrainUnit()
    {
        if (GameData.Instance.Capsules < 10)
        {
            AlertEvents.InsufficientFunds();
            return;
        }

        GameData.Instance.Capsules -= 10;
        if (trainingUnit)
            unitsInTrainingQueue++;
        else
            BeginTrainingUnit();
    }

    private void BeginTrainingUnit()
    {
        trainingUnit = true;
        unitCompletion = 0;
        timeToComplete = 1f;
    }

    public void CancelTrainingUnit()
    {
        GameData.Instance.Capsules += 10 * (unitsInTrainingQueue + (trainingUnit ? 1 : 0));
        trainingUnit = false;
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

    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Train Drone", (int)FacilityTasks.TrainDrone));
        tasks.Add(new Task("Cancel Training", (int)FacilityTasks.CancelTraining));

        return tasks;
    }

    public override List<string> GetStatsList()
    {
        List<string> statsList = new List<string>();
        
        statsList.Add("Queue: " + unitsInTrainingQueue);
        statsList.Add(GetProgressString());

        return statsList;
    }

    public override void SetTask(int taskId)
    {
        switch (GetEnumFromId<FacilityTasks>(taskId))
        {
            case FacilityTasks.TrainDrone:
                TrainUnit();
                break;
            case FacilityTasks.CancelTraining:
                CancelTrainingUnit();
                break;
        }
    }

}
