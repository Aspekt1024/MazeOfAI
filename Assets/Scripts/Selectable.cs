using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Selectable : MonoBehaviour {

    public string Name = "unnamed object";
    public float ObjRadius;
    
    public struct Task
    {
        public string taskName;
        public int taskId;

        public Task(string task, int id)
        {
            taskName = task;
            taskId = id;
        }
    }

    public bool IsType<T>()
    {
        return GetType().Equals(typeof(T));
    }
    
    public static T GetEnumFromId<T>(int id)
    {
        foreach (object e in Enum.GetValues(typeof(T)))
        {
            if ((int)e == id)
                return (T)e;
        }
        return default(T);
    }

    public virtual List<Task> GetTaskList() { return new List<Task>(); }
    public virtual List<string> GetStatsList() { return new List<string>(); }
    public virtual void SetTask(int taskId) { }
}
