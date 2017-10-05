using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleSpring : Building {

    private float spawnTimer;
    private float spawnsPerSecond;

    private float capsulesSpawned;
    private float maxCapsules = 500f;

    private GameObject CapsuleModel;
    
    private enum SpringTasks
    {
        IncreaseRate, DecreaseRate
    }

    private void Start()
    {
        spawnTimer = 0;
        spawnsPerSecond = 1;
        capsulesSpawned = 0;
        Name = "Capsule Spring";
        ObjRadius = 26;
        completed = true;

        CapsuleModel = Resources.Load<GameObject>("Spawnables/Capsule");
    }

    private void Update()
    {
        if (capsulesSpawned == maxCapsules || !completed) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 1 / spawnsPerSecond)
        {
            spawnTimer = 0;
            StartCoroutine(SpawnCapsule());
        }
    }

    private IEnumerator SpawnCapsule()
    {
        capsulesSpawned++;

        GameObject capsule = Instantiate(CapsuleModel, transform.position + Vector3.up * 0.5f, transform.rotation, transform);
        capsule.GetComponent<Collider>().enabled = false;
        capsule.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(3f, 5f), Random.Range(-2f, 2f));

        yield return new WaitForSeconds(0.5f);
        capsule.GetComponent<Collider>().enabled = true;

    }
    
    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();
        tasks.Add(new Task("Rate + 10", (int)SpringTasks.IncreaseRate));
        tasks.Add(new Task("Rate - 10", (int)SpringTasks.DecreaseRate));

        return tasks;
    }

    public override List<string> GetStatsList()
    {
        List<string> statsList = new List<string>();

        statsList.Add("Rate: " + spawnsPerSecond + " / sec");
        statsList.Add((maxCapsules - capsulesSpawned) + " / " + maxCapsules);

        return statsList;
    }

    public override void SetTask(int taskId)
    {
        switch (GetEnumFromId<SpringTasks>(taskId))
        {
            case SpringTasks.IncreaseRate:
                spawnsPerSecond += 10;
                break;
            case SpringTasks.DecreaseRate:
                spawnsPerSecond -= 10;
                break;
        }

        spawnsPerSecond = Mathf.Clamp(spawnsPerSecond, 0, 100);
    }
}
