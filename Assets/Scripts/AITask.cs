using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

//Class for Utility AI
public class AITask
{
    public int priority;
    public float modifier;
    public string taskType;
    //Objective and task doers should be 
    public Controllable objective;
    public Controllable taskDoer;
    public bool assigned = false;
    public GameManager gM;
    public PossibleAssignment assignment;
    public Vector3 laterMods;

    public List<HashSet<Weapon>> orderedWeapons = new List<HashSet<Weapon>>();

    public void assign(PossibleAssignment assignment, Controllable assignee)
    {
        assigned = true;
        taskDoer = assignee;
        taskDoer.assignTask(this);
        gM = assignment.gM;
        orderedWeapons = assignment.orderedWeapons;
        this.assignment = assignment;

    }

    public AITask()
    {

    }

    public AITask(int p, float m, Controllable o, string t)
    {
        priority = p;
        modifier = m;
        objective = o;
        taskType = t;
  
    }
   
}
