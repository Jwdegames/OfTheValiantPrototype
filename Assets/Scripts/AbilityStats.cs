using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

// For general use abilities
public class AbilityStats 
{
    public string name;
    public string title;
    public string description;
    //-1 = infinite duration
    public int duration;
    public int apCost;
    public List<Vector4> waitTimes = new List<Vector4>();
    public int currentWaitIndex = 0;
    public int currentWaitTime = 0;
    public AbilityStats(string n, string t, string d, int dur, int ap, List<Vector4> wt)
    {
        name = n;
        title = t;
        description = d;
        duration = dur;
        apCost = ap;
        waitTimes = wt;
    }
    public AbilityStats(string n, string t, string d, int dur, int ap, List<Vector4> wt, int cWI, int cWT)
    {
        
        name = n;
        title = t;
        description = d;
        duration = dur;
        apCost = ap;
        waitTimes = wt;
        currentWaitIndex = cWI;
        currentWaitTime = cWT;
    }
}
