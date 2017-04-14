using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Drone : Unit {
    
    private enum DroneStates
    {
        Idle, Gathering, Building, Returning
    }
    private DroneStates state;

    private enum WorkerTasks
    {
        Idle = 0, Gather
    }
    private WorkerTasks task;

    private Transform cargo;
    private Collider intersectingTargetCollider;

    protected override void SetupAttributes()
    {
        Name = "Drone";
        elevation = 0.5f;
        baseSpeed = 5f;
        task = WorkerTasks.Gather;
        state = DroneStates.Idle;
    }
    
    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Idle", (int)WorkerTasks.Idle));
        tasks.Add(new Task("Gather", (int)WorkerTasks.Gather));

        return tasks;
    }

    private void Update()
    {
        if (!UnitActive) return;

        PositionHeldCargo();
        CheckIfAlreadyIntersectingTarget();
        CancelActionsIfSetToIdle();

        if (state == DroneStates.Idle && task == WorkerTasks.Gather)
        {
            FindResourceToGather();
        }
    }
    
    private void PositionHeldCargo()
    {
        if (state == DroneStates.Building && cargo != null)
        {
            cargo.transform.position = transform.position + transform.forward * 0.3f;
        }
    }

    private void CheckIfAlreadyIntersectingTarget()
    {
        if (state == DroneStates.Building && intersectingTargetCollider != null && intersectingTargetCollider.transform.parent == Target)
        {
            pathfinder.ForceTargetReached();
        }
    }

    private void CancelActionsIfSetToIdle()
    {
        if (task == WorkerTasks.Idle && state != DroneStates.Idle)
        {
            if (Target != null && Target.GetComponent<Capsule>() != null)
            {
                Target.GetComponent<Capsule>().Assigned = false;
                Target = null;
            }
            if (cargo !=  null && cargo.GetComponent<Capsule>() != null)
            {
                cargo.GetComponent<Capsule>().Assigned = false;
                cargo = null;
            }
            pathfinder.Stop();
            state = DroneStates.Idle;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cargo == null || other.transform.parent != Target || state != DroneStates.Building) return;
        intersectingTargetCollider = other;
        DepositCargo();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other == intersectingTargetCollider)
            intersectingTargetCollider = null;
    }
    
    private void FindResourceToGather()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        if (resources.Length == 0) return;

        for (int i = 0; i < resources.Length; i++)
        {
            if (!resources[i].GetComponent<Capsule>().Assigned && resources[i].GetComponent<Capsule>().Landed)
            {
                Target = resources[i].transform;
                Target.GetComponent<Capsule>().Assigned = true;
                state = DroneStates.Gathering;

                pathfinder.FindPath();
                break;
            }
        }
    }

    private void DepositCargo()
    {
        pathfinder.Stop();
        state = DroneStates.Idle;
        Destroy(cargo.gameObject);
        Target.GetComponentInParent<Building>().IncreaseCompletion(0.1f);
        GameData.Instance.Capsules += 1;
    }
    
    public override void SetTask(int taskId)
    {
        task = GetEnumFromId<WorkerTasks>(taskId);
    }
    
    public override void TargetReached()
    {
        switch (state)
        {
            case DroneStates.Gathering:
                state = DroneStates.Building;
                cargo = Target;
                Target = FindFacilityEntry();
                pathfinder.FindPath();
                break;
            case DroneStates.Building:
                DepositCargo();
                break;
        }
    }

    private Transform FindFacilityEntry()
    {
        Transform facility = FindObjectOfType<Facility>().transform;
        foreach (Transform tf in facility)
        {
            if (tf.tag == "EntryPoint")
                return tf;
        }
        return null;
    }

    public override void FindAnotherPath()
    {
        Target = null;
        state = DroneStates.Idle;
    }
}
