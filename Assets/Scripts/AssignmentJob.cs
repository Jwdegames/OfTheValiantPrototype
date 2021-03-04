using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
public struct AssignmentJob : IJobParallelFor
{
    public List<PossibleAssignment> assignments;
    public List<AITask> tasks;
    public List<Controllable> assets;
    public bool buildingUnit;
    public Tile buildTile;
    public GameManager gM;

    public void Execute(int i)
    {
        int taskIndex = i / tasks.Count;
        int assetIndex = i % tasks.Count;
        /*
        Controllable asset = assets[assetIndex];
        AITask task = tasks[taskIndex];
        asset.taskSubType = "";
        asset.optimalAbility = "";
        if (asset.isTaskSuitable(gM, task, buildingUnit, buildTile))
        {
            PossibleAssignment tempAssignment = new PossibleAssignment(gM, task, asset, buildingUnit, buildTile);
            assignments.Add(tempAssignment);
        }
        */
    }

    // SHOULD NOT BE CALLED WHEN JOB IS EXECUTING
    // Initializes the job parameters
    public void sendData(GameManager g, List<PossibleAssignment> p, List<AITask> t, List<Controllable> a, bool bU, Tile bT)
    {
        gM = g;
        assignments = p;
        tasks = t;
        assets = a;
        buildingUnit = bU;
        buildTile = bT;
    }
}
