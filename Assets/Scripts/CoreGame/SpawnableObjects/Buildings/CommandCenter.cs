using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : Building {
    
	private void Start ()
    {
        completed = true;
        Name = "CommandCenter";
        ObjRadius = 80;
    }
	
	private void Update ()
    {
		
	}


    public override List<Task> GetTaskList()
    {
        List<Task> tasks = new List<Task>();

        return tasks;
    }

    public override List<string> GetStatsList()
    {
        List<string> statsList = new List<string>();

        statsList.Add("doing CC stuff...");

        return statsList;
    }

    public override Vector3 GetEntryPoint()
    {
        return transform.position;
    }
}
