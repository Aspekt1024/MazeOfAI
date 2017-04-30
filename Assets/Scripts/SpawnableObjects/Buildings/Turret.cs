using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building {

    public Transform turretPylon;
    public Cannon cannon;

    private Transform target = null;
    private float turretRange = 15f;

    private UnitHandler units;

    private enum TurretTasks
    {
        Passive, Defensive
    }

    private enum TurretStates
    {
        Idle, Aiming, Firing, Passive
    }
    private TurretStates state;

    private void Start()
    {
        GridSizeX = 2;
        GridSizeY = 2;

        completed = true;
        Name = "Turret";
        ObjRadius = 40;

        state = TurretStates.Idle;

        units = GameObject.FindGameObjectWithTag("Units").GetComponent<UnitHandler>();
    }

    private void Update()
    {
        if (state == TurretStates.Passive) return;

        GetTargetInRange();
        if (target == null)
        {
            state = TurretStates.Idle;
        }
        else
        {
            ShootTarget();
        }
    }

    private void GetTargetInRange()
    {
        if (target != null && Vector3.Distance(transform.position, target.position) > turretRange)
        {
            target = null;
        }
        if (target != null) return;

        foreach (Transform tf in units.GetEnemyUnits())
        {
            if (tf.GetComponents<EnemyFighter>() != null && Vector3.Distance(transform.position, tf.position) < turretRange)
            {
                target = tf;
                break;
            }
        }

    }

    private void ShootTarget()
    {
        float currentRotation = turretPylon.eulerAngles.y;

        float xDist = (target.position.x - transform.position.x);
        float yDist = (transform.position.z - target.position.z);

        float radToDeg = 57.295779513f;
        float targetRotation = Mathf.Atan(yDist / xDist) * radToDeg;

        if (xDist < 0)
            targetRotation = 180 + targetRotation;
        else if (yDist < 0)
            targetRotation = 360 + targetRotation;

        float requiredRotation = targetRotation - currentRotation;

        if (requiredRotation > 180)
            requiredRotation -= 360f;

        const float rotationSpeed = 180f; // degrees/sec
        turretPylon.Rotate(Vector3.up, Mathf.Clamp(requiredRotation, -rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime));

        if (requiredRotation < 3f)
        {
            cannon.Shoot(target);
        }
    }

    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Set Passive", (int)TurretTasks.Passive));
        tasks.Add(new Task("Set Active", (int)TurretTasks.Defensive));

        return tasks;
    }

    public override List<string> GetStatsList()
    {
        List<string> statsList = new List<string>();

        string mode = state == TurretStates.Passive ? "Passive" : "Defensive";
        statsList.Add("Mode: " + mode);

        return statsList;
    }

    public override void SetTask(int taskId)
    {
        switch (GetEnumFromId<TurretTasks>(taskId))
        {
            case TurretTasks.Defensive:
                if (state == TurretStates.Passive)
                    state = TurretStates.Idle;
                break;
            case TurretTasks.Passive:
                state = TurretStates.Passive;
                target = null;
                break;
        }
    }

    public override Vector3 GetEntryPoint()
    {
        return transform.position;
    }
}
