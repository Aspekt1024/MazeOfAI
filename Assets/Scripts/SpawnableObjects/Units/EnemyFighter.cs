using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFighter : Unit {

    public Cannon Cannon;

    private float shootRange = 3f;

    private enum FighterStates
    {
        Idle, Searching, Approaching, Attacking
    }
    private FighterStates state;

    private enum FighterTasks
    {
        Defend = 0, SeekAndDestroy, Idle
    }
    private FighterTasks task;
    
    void Start () {
        Friendly = false;
	}

    protected override void SetupAttributes()
    {
        UnitActive = true;
        Name = "Fighter";
        elevation = 0.5f;
        baseSpeed = 10f;
        baseTurnSpeed = 15f;

        task = FighterTasks.SeekAndDestroy;
        state = FighterStates.Idle;
    }

    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Aggressive", (int)FighterTasks.SeekAndDestroy));
        tasks.Add(new Task("Defensive", (int)FighterTasks.Defend));
        tasks.Add(new Task("Passive", (int)FighterTasks.Idle));

        return tasks;
    }

    public override List<string> GetStatsList()
    {
        List<string> statsList = new List<string>();

        statsList.Add("Level: " + xpHandler.level + " (" + xpHandler.experience + "/" + xpHandler.GetNextLevelXP() + ")");
        statsList.Add("Task: " + task);

        return statsList;
    }

    private void Update()
    {
        if (!UnitActive) return;
        
        CancelActionsIfSetToIdle();
        CheckTargetsExist();

        if (state == FighterStates.Idle && task == FighterTasks.SeekAndDestroy)
        {
            LocateDrone();
        }
        else if (task == FighterTasks.Defend || task == FighterTasks.SeekAndDestroy)
        {
            CheckTargetInRange();
            if (state == FighterStates.Attacking)
                ShootTarget();
        }
    }

    private void ShootTarget()
    {
        if (Cannon !=  null)
            Cannon.Shoot(Target);
    }
    
    private void CancelActionsIfSetToIdle()
    {
        if (task == FighterTasks.Idle && state != FighterStates.Idle)
        {
            Target = null;
            state = FighterStates.Idle;
        }
    }
    
    private void LocateDrone()
    {
        Transform unitParent = GameObject.FindGameObjectWithTag("Units").transform;
        if (unitParent == null) return;

        foreach (Transform tf in unitParent)
        {
            if (tf.GetComponent<Drone>() != null)
            {
                Target = tf;
                state = FighterStates.Searching;
                pathfinder.FindPath();
                return;
            }
        }
    }

    private void CheckTargetInRange()
    {
        if (Target != null && Vector3.Distance(Target.position, transform.position) < shootRange)
        {
            state = FighterStates.Attacking;
            pathfinder.ForceTargetReached();
        }
        else
        {
            if (task == FighterTasks.SeekAndDestroy && state == FighterStates.Attacking)
            {
                state = FighterStates.Searching;
                pathfinder.FindPath();
            }
            else if (task == FighterTasks.Defend)
            {
                state = FighterStates.Idle;
            }
        }
    }

    public override void SetTask(int taskId)
    {
        task = GetEnumFromId<FighterTasks>(taskId);
    }

    public override void FindAnotherPath()
    {
        Target = null;
        state = FighterStates.Idle;
    }

    public override void TargetReached()
    {

    }

    public override void UpdateAttributesForLevel(int level)
    {
        switch (level)
        {
            case 2:
                speedMultiplier = 1.5f;
                turnSpeedMultiplier = 3f;
                break;
            case 3:
                speedMultiplier = 2f;
                turnSpeedMultiplier = 4.5f;
                break;
        }
    }

    private void CheckTargetsExist()
    {
        if (Target == null)
        {
            state = FighterStates.Idle;
        }
    }

}
