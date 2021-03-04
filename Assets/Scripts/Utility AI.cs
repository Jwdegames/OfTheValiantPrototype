using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Priority_Queue;
using System.Linq;
using UnityEditor;
using Unity.Jobs;

[Serializable]
public class UtilityAI
{
    public List<Controllable> visibleControllables;
    //Assets are the AI's controllables
    public List<Controllable> assets;
    public List<Controllable> allControllables;
    public List<Controllable> unitMakers;
    public List<AITask> tasks;
    public List<AITask> unitMakerTasks;
    public List<PossibleAssignment> possibleAssignments;
    public List<PossibleAssignment> customAssignments;
    public SimplePriorityQueue<PossibleAssignment, float> assignmentQueue;
    public List<PossibleAssignment> orderedAssignments;
    public List<PossibleAssignment> assignmentExecutionOrder;
    float actions = 0, limitedActions = 100;
    //Give different buildings with different defense modifiers different names
    public Dictionary<string, float> buildingDefenseModifiers;
    /*Regarding Tasks:
     * 1 - Defend Building - Enemy within capture distance of our building
     * 2 - Capture Enemy Building - Our unit within capture distance of enemy building
     * 3 - Capture Non-Player Building - Our unit within capture distance of neutral building
     * 4 - Deploy Units - Player unit with deploy capabilities
     * 5 - Attack Enemies - Enemy exists 
     * 5b - Attack Enemies with main abilities - Enemy exists
     * 6 - Approach Enemies and Sentry - Enemy exists 
     * 7 - Approach Enemies and Guard - Enemy exists 
     * 8 - Repair Allies - Ally exists 
     * 8b - Boost Allies via abilites/commands - Ally exists
     * 9 - Transport Units - Player unit with transport capabilities
     * 10 - Retreat to friendly Space and Guard
     * 11 - Transport Units - Player unit with transport capabilities
     * 12 - Search  - Can't do tasks 1-11
    
    Unit Maker Priority List
     13 - Build units to fufill the other tasks
    */

    // Probably needs execution order to work like this:
    // Start a separate coroutine for each possible assignment to handle the calculations
    // Create ordrered assignments after each courtine is finished - Use Task class


    //assignment score = (14 - general priority + modifier) / distance to Controllable that is assigned
    //if (distance < 1) than distance = 0.5 + distance / 2

    public GameManager gM;
    public AIManager aiM;
    public Player player;
    public string side;
    public int team;

    public UtilityAI(AIManager a, GameManager g, Player p, string s, int t)
    {
        gM = g;
        player = p;
        side = s;
        team = t;
        aiM = a;
    }
    
    //4 steps

    //1 - Generate Tasks
    //2 - Generate Possible Assignments
    //3 - Sort Possible Assignments According To Score
    //4 - Assign to Task Orders

    public List<Controllable> generateAssets(bool custom)
    {
        List<Controllable> customAssets = new List<Controllable>();
        customAssets.AddRange(gM.unitDictionary[side]);
        customAssets.AddRange(gM.buildingDictionary[side]);
        
        foreach(Controllable asset in customAssets)
        {
            if (!custom)
            {
                if (!asset.isBuilding)
                {

                }
                //Debug.Log(asset);
            }
            asset.assignedTask = null;
        }
        if (!custom)
        {
            assets = customAssets;
        }
        customAssets.RemoveAll(item => item.finished);
        return customAssets;
    }

    public List<Controllable> generateAllControlables(bool custom)
    {
        allControllables = new List<Controllable>();
        foreach (string key in gM.unitDictionary.Keys)
        {
            allControllables.AddRange(gM.unitDictionary[key]);
        }
        foreach (string key in gM.buildingDictionary.Keys)
        {
            allControllables.AddRange(gM.buildingDictionary[key]);
        }
        return allControllables;
    }

    // Gets a list of all unit makers
    public List<Controllable> generateAllUnitMakers()
    {
        unitMakers = new List<Controllable>();
        foreach(string key in gM.buildingDictionary.Keys)
        {
            unitMakers.AddRange(gM.buildingDictionary[key]);
        }
        unitMakers.RemoveAll(building => !(((Building) building).makesUnits));
        return unitMakers;
    }



    //Makes tasks
    public List<AITask> generateTasks(bool custom, List<Controllable> customAssets)
    {
        //List<Controllable> customAssets = generateAssets(custom);
        float duration = Time.realtimeSinceStartup;
        generateAllControlables(custom);
        bool objectiveVisisble = true;
        List<AITask> customTasks = new List<AITask>();
        if (!custom)
        {
            tasks = new List<AITask>();
        }
        foreach (Controllable con in allControllables)
        {
            float modifier = 0;
            //Determine if con is a unit
            if (!con.isBuilding)
            {
                Unit unit = (Unit)con;
                //Determine if the unit is an enemy
                if (unit.getTeam() != team)
                {
                    //Check to see if the unit is in capture distance of any building
                    List<Tile> allyBuildingsInRange = gM.getAllyBuildingsInMoveRange(unit.getTile());
                    //Add up all the defense priorities and create the task
                    Vector3 modVector = new Vector3();
                    if (allyBuildingsInRange != null && allyBuildingsInRange.Count > 0)
                    {
                        modVector = new Vector3(4 + getAllBuildingDefensePriorities(unit, allyBuildingsInRange), 0, 0);
                    }
                    // Originally was 5 different tasks - way too slow
                    AITask tempTask = new AITask(5, 0, unit, "Attack Enemy");
                    tempTask.laterMods = modVector;
                    customTasks.Add(tempTask);

                    //customTasks.Add(new AITask(6, 0, unit, "Sentry Near Enemy"));
                    //customTasks.Add(new AITask(7, 0, unit, "Fortify Near Enemy"));
                    //customTasks.Add(new AITask(9, 0, unit, "Retreat and Sentry"));
                    //customTasks.Add(new AITask(10, 0, unit, "Retreat and Fortify"));
                    
                }
                else
                {
                    float hpLostPercent = (unit.getHP() - unit.getCurrentHP()) / unit.getHP();
                    float hpMod = hpLostPercent * 100 / 20;
                    if (hpLostPercent == 0)
                    {
                        hpMod -= 10;
                    }
                    //Priortorize healing the unit if it's an ally
                    if (unit.biological)
                    {
                        customTasks.Add(new AITask(8, hpMod, unit, "Heal Unit"));
                    }
                    if (unit.mechanical)
                    {
                        customTasks.Add(new AITask(8, hpMod, unit, "Repair Unit"));
                    }
                    if ((unit.getPossibleActions().Contains("Deploy Units") || unit.getPossibleActions().Contains("Deploy Drones")) && unit.getAP() > 0)
                    {
                        customTasks.Add(new AITask(4, 0, unit, "Deploy Units"));
                    }
  
                    //FIXME
                    //Complex and needs to be implemented
                        //tasks.Add(new AITask(11, 0, unit, "Transport Units"));
                    
                }
                
            }
            else
            {
                //Handle if the con is a building
                Building building = (Building)con;
                if (con.team != team)
                {
                    //Handle if con is enemy building
                    if (con.team >= 0)
                    {
                        customTasks.Add(new AITask(2, getBuildingDefensePriority(null, building)+4, building, "Capture Enemy Building"));
                    }
                    else
                    {
                        customTasks.Add(new AITask(3, getBuildingDefensePriority(null, building)+4, building, "Capture Neutral Building"));
                    }
                }
                else
                {
                    // This needs to be added to a separate command list
                    /*
                    if (con.side == side && building.makesUnits == true)
                    {
                        customTasks.Add(new AITask(13, -100+getBuildingDefensePriority(null, building), building, "Make Unit"));
                    }
                    */
                }
            }
        }
        if (!custom)
        {
            tasks = customTasks;
        }
        duration = Time.realtimeSinceStartup - duration;
        Debug.Log("It took " + duration + "s to get all tasks");
        return customTasks;
    }

    // Handle Unit Makers
    public List<AITask> generateUnitMakerTasks()
    {
        generateAllControlables(false);
        bool objectiveVisisble = true;
        List<AITask> customTasks = new List<AITask>();

        foreach (Controllable con in allControllables)
        {
            if (con.isBuilding)
            {
                //Handle if the con is a building
                Building building = (Building)con;
                
                // This needs to be added to a separate command list
                    
                if (con.side == side && building.makesUnits == true)
                {
                    customTasks.Add(new AITask(13, -100+getBuildingDefensePriority(null, building), building, "Make Unit"));
                }
                    
                
            }
        }
        unitMakerTasks = customTasks;
        return customTasks;
    }

    public List<PossibleAssignment> getPossibleAssignments()
    {
        float duration = Time.realtimeSinceStartup;
        generateTasks(false, generateAssets(false));
        possibleAssignments = new List<PossibleAssignment>();
        //AssignmentJob jobToDo = new AssignmentJob();
        //jobToDo.sendData(gM, possibleAssignments, tasks, assets, false, null);

        //JobHandle handle = jobToDo.Schedule(tasks.Count * assets.Count, 1);
        bool ranOnce = false;
        foreach (AITask task in tasks)
        {
            foreach(Controllable asset in assets)
            {
                asset.taskSubType = "";
                asset.optimalAbility = "";
                if (!ranOnce && !asset.isBuilding)
                {
                    Unit unit = (Unit)asset;
                    if (unit.canAttackAbsolute())
                    {
                       // unit.attackableTiles = gM.getAttackbleUnits(unit, unit.getTile());
                    }
                }
                aiM.initGetAssignmentScore(possibleAssignments, task, asset, false, null);
                /*
                if (asset.isTaskSuitable(gM, task, false, null))
                {
                    PossibleAssignment tempAssignment = new PossibleAssignment(gM, task, asset, false, null);

                    possibleAssignments.Add(tempAssignment);
                }
                else
                {
                    if (!asset.isBuilding && task.taskType == "Attack Enemy")
                    {
                        //Debug.Log(asset + " is unable to complete task: " + task.taskType+ " wrt "+task.objective);
                        //Debug.Log(gM.getInRangePathAH((Unit)asset, asset.tile, task.objective.tile, ((Unit)asset).getAllActiveWeapons(), true, false) == null);
                    }
                }*/
            }
            ranOnce = true;
        }
        duration = Time.realtimeSinceStartup - duration;
        Debug.Log("It took " + duration + "s to get all assignments");
        return possibleAssignments;
    }

    // For unit makers
    public List<PossibleAssignment> getPossibleAssignmentsFromUnitMakers(List<AITask> customTasks, List<Controllable> unitMakers)
    {
        List<PossibleAssignment> unitMakerPossibleAssignments = new List<PossibleAssignment>();
        foreach (AITask task in customTasks)
        {
           foreach(Controllable unitMaker in unitMakers)
            {
                if (unitMaker.side == player.side)
                aiM.initGetAssignmentScore(unitMakerPossibleAssignments, task, unitMaker, false, null);
            }
        }
        return unitMakerPossibleAssignments;
    }

    //For building use
    public List<PossibleAssignment> getPossibleAssignmentsFromCustomLists(List<AITask> customTasks, List<Controllable> customAssets, Tile bUTile)
    {
        List<PossibleAssignment> customPossibleAssignments = new List<PossibleAssignment>();
        bool ranOnce = false;
        foreach (AITask task in customTasks)
        {
            foreach (Controllable asset in customAssets)
            {
                aiM.initGetAssignmentScore(customPossibleAssignments, task, asset, true, bUTile);
            }
            ranOnce = true;
        }
        return customPossibleAssignments;
    }


    public float getBuildingDefensePriority(Unit unit, Building building)
    {
        float modifier = 0;
        if (buildingDefenseModifiers == null)
        {
            if (building.makesUnits)
            {
                modifier = 1;
            }
        }
        if (unit != null)
        {

        }
        return modifier;
    }

    public float getAllBuildingDefensePriorities(Unit unit, List<Tile> tiles)
    {
        float priorities = 0;
        foreach(Tile t in tiles)
        {
            priorities += getBuildingDefensePriority(unit, t.getBuilding());
        }
        return priorities;
    }

    public List<PossibleAssignment> createAssignments()
    {
        List<PossibleAssignment> assignments = getPossibleAssignments();
        assignmentQueue = new SimplePriorityQueue<PossibleAssignment, float>();
        foreach(PossibleAssignment assignment in assignments)
        {
            assignmentQueue.Enqueue(assignment, assignment.assignmentScore);
        }

        //Now make a list that contains the assignments from highest to lowest priority
        orderedAssignments = new List<PossibleAssignment>();
        while (assignmentQueue.Count > 0)
        {
            orderedAssignments.Insert(0, assignmentQueue.Dequeue());
        }

        assignmentExecutionOrder = new List<PossibleAssignment>();
        //Now we have an ordered list and must assign tasks
        foreach(PossibleAssignment assignment in orderedAssignments)
        {
            if (assignment.assign())
            {
                assignmentExecutionOrder.Add(assignment);
            }
        }
        return assignmentExecutionOrder;
    }

    public List<PossibleAssignment> createAssignmentsFromCustomList(List<PossibleAssignment> customAssignments)
    {
        SimplePriorityQueue<PossibleAssignment,float> customAssignmentQueue = new SimplePriorityQueue<PossibleAssignment, float>();
        foreach (PossibleAssignment assignment in customAssignments)
        {
            if (customAssignmentQueue != null && assignment != null)
            {
                customAssignmentQueue.Enqueue(assignment, assignment.assignmentScore);
                assignment.possibleTaskDoer.assignedTask = null;
            } else
            {
                //Debug.Log(assignment);
            }
        }
        //Debug.Log(customAssignmentQueue.Count);
        //Now make a list that contains the assignments from highest to lowest priority
        List<PossibleAssignment> customOrderedAssignments = new List<PossibleAssignment>();
        while (customAssignmentQueue.Count > 0)
        {
            customOrderedAssignments.Insert(0, customAssignmentQueue.Dequeue());
        }
        //Debug.Log(customOrderedAssignments.Count);
        List<PossibleAssignment> customAssignmentExecutionOrder = new List<PossibleAssignment>();
        //Now we have an ordered list and must assign tasks
        foreach (PossibleAssignment assignment in customOrderedAssignments)
        {
            if (assignment.assign())
            {
                customAssignmentExecutionOrder.Add(assignment);
            } else
            {
                //Debug.Log("Failed to assign");
            }
        }
        Debug.Log(customAssignmentExecutionOrder.Count);
        return customAssignmentExecutionOrder;
    }



    
}
