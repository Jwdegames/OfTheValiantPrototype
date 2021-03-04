using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Linq;
//using System.Security.Policy;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using UnityEditor.Playables;
//using UnityEditorInternal;
using UnityEngine;
//using UnityEngine.Rendering;


//Class for Utility AI
public class Controllable : MonoBehaviour
{
    public string side;
    public Tile tile;
    public int team;
    public bool isBuilding;
    public bool cloaked;
    public bool sentryAI = false;
    public bool canBeAggravated = false;
    public Tile sentryTile;
    //2nd variable for cloaked - determines if everyone knows the location of the cloaked object
    public bool finished = false;
    public bool lastSeen;
    [SerializeField]
    public AITask assignedTask = null;
    [SerializeField]
    public AITask lastTask;
    public string taskSubType;
    public string optimalAbility;
    public Dictionary<int, Dictionary<Weapon, Tile>> firingDictionary;
    public List<HashSet<Weapon>> orderedWeapons = new List<HashSet<Weapon>>();
    // This is used for the UtilityAI
    // The AI preprocesses what units this can attack
    public List<Tile> attackableTiles = new List<Tile>();

    //Regarding Tasks:
    // 1 - Defend Building - Enemy within capture distance of our building
    // 2 - Capture Enemy Building - Our unit within capture distance of enemy building
    // 3 - Capture Non-Player Building - Our unit within capture distance of neutral building
    // 4 - Deploy Units - Player unit with deploy capabilities
    // 5 - Attack Enemies - Enemy exists 
    // 6 - Approach Enemies and Sentry - Enemy exists 
    // 7 - Approach Enemies and Guard - Enemy exists 
    // 8 - Repair Allies - Ally exists
    // 9 - Retreat to friendly Space and Sentry
    // 10 - Retreat to friendly Space and Guard
    // 11 - Transport Units - Player unit with transport capabilities
    // 12 - Search  - Can't do tasks 1-11 
    // 13 - Build units to fufill the other tasks

    public void Awake()
    {
        assignedTask = null;
    }

    public void assignTask(AITask task)
    {
        assignedTask = task;
        //if (!isBuilding) Debug.Log("Task assigned to unit " + (Unit)this);
        //else Debug.Log("Task assigned to building " + (Building)this);
        //Debug.Log(this + " is receiving a task!");
    }

    public void freeUp()
    {
        assignedTask = null;
    }

    public IEnumerator performTask(AIManager aiM, UtilityAI uAI)
    {

        lastTask = assignedTask;
        if (finished)
        {
            Debug.Log("Finished Task already: "+ assignedTask.taskType + " from  " + this);
            yield break;
        }
        GameManager gM = aiM.gM;
        if (isBuilding)
        {
            Building building = (Building)this;
            //return true;
            switch (assignedTask.taskType)
            {
                case "Make Unit":
                    if (tile.getUnit() || !building.makesUnits)
                    {
                        finished = true;
                        yield break;
                    }
                    UnitsList unitsList = new UnitsList();
                    if (tile.getUnit() != null)
                    {
                        finished = true;
                        yield break;
                    }



                    //Get a list of units that can be used to do the tasks
                    Player player = gM.playerDictionary[side];
                    string faction = player.faction;
                    if (faction == null || team < 0)
                    {
                        yield break;
                    }
                    string unitType = "Infantry";
                    switch(building.name)
                    {
                        case "Barracks":
                            unitType = "Infantry";
                            break;
                        case "Factory":
                            unitType = "Vehicles";
                            break;
                    }
                    Debug.Log("Generating Templates");
                    List<UnitTemplate> unitTemplates = new List<UnitTemplate>(unitsList.unitTypes[faction][unitType]);
                    if (gM.playerDictionary[gM.humanSide].hasLab())
                    {
                        unitTemplates.AddRange(unitsList.unitTypes[faction]["Advanced "+unitType]);
                    }
                    List<GameObject> unitPrefabs = new List<GameObject>();
                    List<Controllable> unitPrefabScripts = new List<Controllable>();
                    if (tile.getUnit())
                    {
                        finished = true;
                        yield break;
                    }
                    Debug.Log("Generating prefabs");
                    foreach (UnitTemplate template in unitTemplates)
                    {
                        GameObject unitObject = Instantiate(unitsList.getUnitPrefab(template, faction, gM.boardScript),Vector3.zero, Quaternion.identity) as GameObject;
                        unitPrefabs.Add(unitObject);
                        Unit unitScript = unitObject.GetComponent<Unit>();
                        unitScript.matchWeapon("UI");
                        unitScript.useTemplate(template);
                        unitPrefabScripts.Add(unitScript);
                    }

                    //Assign scores to each unit

                    //Get all unit related tasks
                    Debug.Log("Generating tasks");
                    bool builtUnit = false;
                    Unit unitBuilt = null;
                    List<PossibleAssignment> executionList;
                    List<AITask> tasksToComplete = uAI.generateTasks(true, unitPrefabScripts);

                    tasksToComplete.RemoveAll(item => item.objective.isBuilding);
                    if (tile == null) Debug.LogError("Invalid tile");
                    //Debug.Log("Generating Assignments");
                    List<PossibleAssignment> pAList = uAI.getPossibleAssignmentsFromCustomLists(tasksToComplete, unitPrefabScripts, tile);
                    //Debug.Log("Waiting for Assignments");
                    yield return StartCoroutine(aiM.waitForThreads(aiM.threadsToWaitFor));
                    //yield break;
                    //Debug.Log("PA: "+pAList.Count);
                    executionList = uAI.createAssignmentsFromCustomList(pAList);
                    //Debug.Log("Units buildable: " + executionList.Count);
                    finished = true;
                    //yield break;
                    //Debug.Log("Units buildable: "+executionList.Count);
                    /*foreach(PossibleAssignment assignment in executionList)
                    {
                        Debug.Log(assignment.assignmentScore + " for " + (Unit)assignment.possibleTaskDoer + " to do task "+assignment.aiTask.taskType +" for objective " +
                            assignment.aiTask.objective);
                    }*/
                    //Debug.Log("Trying to build unit");
                    foreach (PossibleAssignment assignment in executionList)
                    {

                        if (assignment.assignmentScore != -9999999)
                        {
                            unitBuilt = gM.boardScript.buildUnit(((Unit)(assignment.possibleTaskDoer)).gameObject, player, building.tile.mapX, building.tile.mapY, false);
                            //Debug.Log(unitBuilt);
                            builtUnit = true;
                            //unitBuilt.useTemplate(unitsList.templateDictionary[((Unit)(assignment.possibleTaskDoer)).name]);
                            break;
                        }
                    }
                    if (builtUnit && unitBuilt != null) { 
                        player.metal -= unitBuilt.getMTCost();
                        player.people -= unitBuilt.getPPLCost();
                        unitBuilt.grayScale();
                    }
                    else
                    {
                        Debug.Log("Failed to build unit for "+this+"!");
                        Debug.Log(unitBuilt);
                    }

                    foreach(GameObject unitObject in unitPrefabs)
                    {
                        Destroy(unitObject);
                    }

                    finished = true;
                    //Debug.Log("Waiting 10 secs");
                    //yield return new WaitForSeconds(10f);
                    //Debug.Log("Finished 10 secs");
                            
                    
                    break;
            }
        }
        else
        {
            Unit unit = (Unit)this;
            Debug.Log("Performing Task: "+ assignedTask.taskType+ " of subtype "+ taskSubType +" from unit "+unit);
            switch (assignedTask.taskType)
            {
                case "Attack Enemy Near Building":
                case "Attack Enemy":
                    switch (taskSubType) {
                        case "Attack Enemy":
                            //orderedWeapons = assignedTask.orderedWeapons;
                            //Debug.Log(orderedWeapons.Count);
                            Unit obj = (Unit)assignedTask.objective;
                            List<Weapon> weapons = null;
                            if (orderedWeapons != null && orderedWeapons.Count > 0)
                             weapons = new List<Weapon>(orderedWeapons[0]);
                            else
                            {
                                weapons = unit.getAllDamageHandWeapons();
                            }
                            Tile targetTile = obj.getTile();
                            Dictionary<Weapon, Tile> mainFiringDict = (firingDictionary != null && firingDictionary.ContainsKey(0)) ? firingDictionary[0] : null;
                            if (mainFiringDict != null && mainFiringDict.ContainsKey(weapons[0]))
                            {
                                //Debug.Log("Aoe attack: "+ this +" should attack " + mainFiringDict[weapons[0]] + " instead of " + targetTile);
                                targetTile = mainFiringDict[weapons[0]];
                            }
                            List<Tile> attackableUnits = assignedTask.gM.findAllTilesAttackableWithWeapons(unit, unit.getTile(), weapons);

                            if (attackableUnits.Contains(targetTile)) {
                                if (unit.getCurrentAP() > 0)
                                {
                                    //Debug.Log("Attacking with "+unit.getCurrentAP()+" AP with weapon "+weapons[0]+"!");
                                    if (targetTile == obj.getTile())
                                    {
                                        yield return StartCoroutine(aiM.beginAttackSequence(unit, unit.getTile(), obj, obj.getTile(), weapons));
                                    } else
                                    {
                                        //ebug.Log("Initiating aoe attack");
                                        yield return StartCoroutine(aiM.beginAttackSequenceAOE(unit, unit.getTile(), targetTile, weapons));
                                    }
                                    //Debug.Log(unit.getCurrentAP());
                                    aiM.needToRestart = true;
                                }
                                else
                                {
                                    yield return StartCoroutine(gM.moveInRange(unit, unit.getTile(), obj.getTile(), weapons));
                                }
                                //finished = true;
                            }
                            else
                            {
                                //Debug.Log("Unable to attack!");
                                yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), obj.getTile(), false));
                                float sentryMod = assignedTask.assignment.getHypotheticalMod("Sentry Near Enemy", unit, obj);
                                float guardMod = assignedTask.assignment.getHypotheticalMod("Fortify Near Enemy", unit, obj);
                                float diff = sentryMod - guardMod;
                                if (diff > 0)
                                {
                                    if (unit.getCurrentAP() == unit.getAP())
                                    {
                                        yield return StartCoroutine(unit.actuallySentry());
                                    }
                                    finished = true;

                                }
                                else
                                {
                                    if (unit.getCurrentAP() == unit.getAP())
                                    {
                                        yield return StartCoroutine(unit.actuallyGuard());
                                    }
                                    finished = true;
                                }
                            }
                            break;
                        case "Sentry Near Enemy":
                            obj = (Unit)assignedTask.objective;
                            List<Tile> movePath = gM.getInRangePathAH(unit, unit.getTile(), obj.getTile(), unit.getAllActiveWeapons(), true, false);
                            Tile sentryTile = movePath[movePath.Count - 1];
                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), sentryTile, false));
                            if (unit.getCurrentAP() == unit.getAP())
                            {
                                yield return StartCoroutine(unit.actuallySentry());
                            }
                            finished = true;
                            break;
                        case "Fortify Near Enemy":
                            obj = (Unit)assignedTask.objective;
                            movePath = gM.getInRangePathAH(unit, unit.getTile(), obj.getTile(), unit.getAllActiveWeapons(), true, false);
                            sentryTile = movePath[movePath.Count - 1];
                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), sentryTile, false));
                            if (unit.getCurrentAP() == unit.getAP())
                            {
                                yield return StartCoroutine(unit.actuallyGuard());
                            }
                            finished = true;
                            break;
                        case "Retreat and Sentry":
                            Debug.Log("Retreat And Sentry");
                            Tile retreatTile = findRetreatTile();
                            Debug.Log("Found retreat tile");
                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), retreatTile, false));
                            if (unit.getCurrentAP() == unit.getAP())
                            {
                                yield return StartCoroutine(unit.actuallySentry());
                            }
                            finished = true;
                            break;
                        case "Retreat and Fortify":
                            Debug.Log("Retreat And Fortify");
                            retreatTile = findRetreatTile();
                            Debug.Log("Found retreat tile");
                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), retreatTile, false));
                            if (unit.getCurrentAP() == unit.getAP())
                            {
                                yield return StartCoroutine(unit.actuallyGuard());
                            }
                            finished = true;
                            break;
                        case "Use Ability":
                            // NOTE: This needs to be changed when abilities are implemented, only handles commands for now
                            obj = (Unit)assignedTask.objective;
                            yield return StartCoroutine(unit.doCommand(optimalAbility,unit.getAffectedCommandTiles(optimalAbility, obj.getTile()), false));
                            break;
                    }
                    break;
                case "Heal Unit":
                case "Repair Unit":
                    // Debug.Log("Healing unit");
                    Unit taskUnit = (Unit)assignedTask.objective;
                    switch (taskSubType) {
                        case "Heal/Repair":

                        List<Weapon> useWeapons = (assignedTask.taskType == "Heal Unit") ? unit.getAllHealHandWeapons() : unit.getAllRepairHandWeapons();
                        if (gM.canHealAllyWithWeapons(unit.getTile(), taskUnit.getTile(), useWeapons))
                        {
                            yield return StartCoroutine(gM.healAlly(unit, taskUnit, useWeapons));
                        }
                        else
                        {
                            List<Tile> hrPath = gM.getInRangePathAH(unit, unit.getTile(), taskUnit.getTile(), useWeapons, false, false);
                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), hrPath[hrPath.Count - 1], false));
                            if (gM.canHealAllyWithWeapons(unit.getTile(), taskUnit.getTile(), useWeapons))
                            {
                                yield return StartCoroutine(gM.healAlly(unit, taskUnit, useWeapons));
                            }
                            else
                            {
                                //Begin guard sequence if we were unable to heal/repair
                                float sentryMod = assignedTask.assignment.getHypotheticalMod("Sentry Near Enemy", unit, taskUnit);
                                float guardMod = assignedTask.assignment.getHypotheticalMod("Fortify Near Enemy", unit, taskUnit);
                                float diff = sentryMod - guardMod;
                                if (diff > 0)
                                {
                                    if (unit.getCurrentAP() == unit.getAP())
                                    {
                                        yield return StartCoroutine(unit.actuallySentry());
                                    }
                                    finished = true;

                                }
                                else
                                {
                                    if (unit.getCurrentAP() == unit.getAP())
                                    {
                                        yield return StartCoroutine(unit.actuallyGuard());
                                    }
                                    finished = true;
                                }
                            }
                        }
                            break;
                        case "Boost":
                            // NOTE: This needs to be changed when abilities are implemented, only handles commands for now
                            // Debug.Log("Testing "+ optimalAbility);
                            yield return StartCoroutine(unit.doCommand(optimalAbility, unit.getAffectedCommandTiles(optimalAbility, taskUnit.getTile()), false));
                            break;
                    }
                    break;
                case "Deploy Units":
                    foreach(string deployeeType in unit.dronesDict.Keys)
                    {
                        if (unit.canDeploySpecificCustom(deployeeType) && unit.getCurrentAP() > 0)
                        {
                            List<Vector4> deployeeData = unit.dronesDict[deployeeType];
                            switch(deployeeData[2].x)
                            {
                                case 0:
                                    //Retreat and deploy
                                    //Debug.Log("Retreat And Deploy - Adjacent");
                                    //Tile retreatTile = findRetreatTile();
                                    //Debug.Log("Found retreat tile");
                                    //yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), retreatTile, false));
                                    yield return StartCoroutine(unit.deployDronesCustom(new Dictionary<string, List<Tile>>() { { deployeeType, unit.getTile().getAdjacent() } }));

                                    //finished = true;
                                    break;
                                case 1:
                                    //This is the launcher
                                    //Debug.Log("Retreat And Deploy - Launch");
                                    //retreatTile = findRetreatTile();
                                    //Debug.Log("Found retreat tile");
                                    //yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), retreatTile, false));
                                    List<Weapon> launchers = unit.findLaunchers(deployeeType);
                                    Weapon launcher = launchers[0];
                                    yield return StartCoroutine(unit.deployDronesCustom(new Dictionary<string, List<Tile>>() { { deployeeType, new List<Tile>() {gM.findClosestEmptyTileWithinRange(
                                        unit.getTile(), false, launcher.minRange, launcher.maxRange) } } }));
                                    break;
                            }
                        }
                        else
                        {
                            
                        }
                        
                    }
                    break;
                case "Capture Enemy Building":
                case "Capture Neutral Building":
                    yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), assignedTask.objective.tile, false));
                    if (unit.getTile() == assignedTask.objective.tile)
                    {
                        if (unit.getCurrentAP() == unit.getAP())
                        {
                            yield return StartCoroutine(unit.actuallyCapture());
                            aiM.needToRestart = true;
                        }
                        finished = true;
                    }
                    else
                    {
                        /*float sentryMod = assignedTask.assignment.getHypotheticalMod("Sentry Near Enemy", unit, obj);
                        float guardMod = assignedTask.assignment.getHypotheticalMod("Fortify Near Enemy", unit, obj);
                        float diff = sentryMod - guardMod;
                        if (diff > 0)
                        {
                            unit.actuallySentry();

                        }
                        else
                        {
                            unit.actuallyGuard();
                        }*/
                        //Debug.Log("Not at building");
                        if (unit.getCurrentAP() == unit.getAP())
                        {
                            yield return StartCoroutine(unit.actuallyGuard());
                        }
                        finished = true;
                    }
                    break;
            }

            //If unit can do something reset the ai manager
            if (unit.checkIfActionPossible() && !finished)
            {
                aiM.needToRestart = true;
            }
        }
        assignedTask = null;
        taskSubType = "";
        //yield return null;
        //return true;
    }

    //Determines if a unit can do a task
    public TPReturnData isTaskSuitable(GameManager gM,  AITask task, bool buildUnit, Tile buildTile)
    {
        //Debug.Log("Checking suitable: " + task.taskType);
        Vector4 subSuitability = new Vector4();
        List<Tile> goalPath = null;
        if (isBuilding)
        {
            Building building = (Building)this;
            if (task.priority != 13)
            {
            }
            else
            {
                switch (task.taskType)
                {
                    case "Make Unit":
                        if (building.makesUnits && building.enabled)
                        {
                            subSuitability.x = 1;
                        }
                        break;
                }
            }
        }
        else
        {
            Unit unit = (Unit)this;
            Unit enemy;
            //Return if we can't do anything
           
            if (!buildUnit)
            {
                if (unit.getName() == "Slime")
                {
                    //Debug.Log("Checking if slime can do task: " + task.taskType);
                }
                if (!unit.checkIfActionPossible())
                {
                    //Debug.Log(unit + " is unable to perform any more actions!");
                    return new TPReturnData();
                }
            }
            //Unit should have AP to complete tasks
            switch (task.taskType)
            {
                case "Attack Enemy Near Building":
                case "Attack Enemy":

                    if (!(sentryAI && !gM.canAttackEnemiesWithoutMoving(unit)))
                    {
                        if (unit.canAttackAbsolute())
                        {
                            enemy = (Unit)task.objective;
                            if (!buildUnit)
                            {
                                //Debug.Log("Checking if " + unit + " can attack " + enemy);
                            }

                            if (!buildUnit)
                            {
                                goalPath = gM.getInRangePathAH(unit, unit.tile, task.objective.tile, unit.getAllDamageActiveWeapons(), true, false);
                                if (goalPath != null)
                                {

                                    if (enemy.flying && !unit.canTargetAir(unit.getAllDamageActiveWeapons()))
                                    {

                                    }
                                    else if (enemy.isSubmerged && !unit.canTargetSub(unit.getAllDamageActiveWeapons()))
                                    {

                                    }
                                    else
                                    {
                                        subSuitability.x = 1;
                                    }
                                }

                            }
                            else if (buildUnit)
                            {
                                goalPath = gM.getInRangePathAH(unit, buildTile, task.objective.tile, unit.getAllDamageActiveWeapons(), true, true);
                                if (goalPath != null)
                                {
                                    if (enemy.flying && !unit.canTargetAir(unit.getAllDamageActiveWeapons()))
                                    {

                                    }
                                    else if (enemy.isSubmerged && !unit.canTargetSub(unit.getAllDamageActiveWeapons()))
                                    {

                                    }
                                    else
                                    {
                                        subSuitability.x = 1;
                                    }
                                }
                            }
                        }
                    }
                    if (!(sentryAI && !gM.canAttackEnemiesWithoutMoving(unit)))
                    {
                        if (unit.getPossibleActions().Contains("Fortify"))
                        {
                            subSuitability.y = 1;
                        }
                    }
                    if (!(sentryAI && !gM.canAttackEnemiesWithoutMoving(unit)))
                    {
                        enemy = (Unit)task.objective;
                        if (unit.getPossibleActions().Contains("Sentry"))
                        {
                            if (enemy.flying && !unit.canTargetAir(unit.getAllDamageActiveWeapons()))
                            {

                            }
                            else if (enemy.isSubmerged && !unit.canTargetSub(unit.getAllDamageActiveWeapons()))
                            {

                            }
                            else
                            {
                                subSuitability.z = 1;
                            }

                        }
                    }
                    // NOTICE: Abilities/commands should be capable of targeting all units regardless if they are flying or submerged
                        if (unit.hasOffensiveAbilities() && unit.getCurrentAP() > 0)
                        {
                            subSuitability.w = 1;
                        }
                    break;
                case "Capture Enemy Building":
                case "Capture Neutral Building":
                    if (!sentryAI && unit.getPossibleActions().Contains("Capture") && task.objective.tile.getUnit() == null) {
                        subSuitability.x = 1;
                    }
                    break;
                case "Deploy Units":
                    //Only the unit that is deploying the drone is task suitable
                    if ((buildUnit && unit == task.objective) || (!buildUnit && unit.canDeploy() && unit == task.objective && unit.getCurrentAP() > 0))
                    {
                        subSuitability.x = 1;
                    }
                    //Debug.Log(unit + " can't deploy!");
                    break;
                case "Heal Unit":
                case "Repair Unit":
                    Unit u = (Unit)task.objective;
                    if (!(sentryAI && !gM.canHealAlliesWithoutMoving(unit)) && (taskSubType == "Heal/Repair" || taskSubType == ""))
                    {

                        if ((u.biological && unit.canHealAbsolute()) || (u.mechanical && unit.canRepairAbsolute()))
                        {
                            List<Weapon> weapons = u.biological ? (u.mechanical ? unit.getAllHealRepairActiveWeapons() : unit.getAllHealWeaponsFromList(unit.getAllActiveWeapons()))
                                : (u.mechanical ? unit.getAllRepairWeaponsFromList(unit.getAllActiveWeapons()) : new List<Weapon>());
                             if (!buildUnit)
                            {
                                goalPath = gM.getInRangePathAH(unit, unit.tile, task.objective.tile, weapons, true, false);
                                if (goalPath != null)
                                {
                                    if (u.flying && !unit.canTargetAir(weapons))
                                    {

                                    }
                                    else if (u.isSubmerged && !unit.canTargetSub(weapons))
                                    {

                                    }
                                    else
                                    {
                                        subSuitability.x = 1;
                                    }
                                }
                            }
                            else if (buildUnit)
                            {
                                goalPath = gM.getInRangePathAH(unit, buildTile, task.objective.tile, weapons, true, true);
                                if (goalPath != null)
                                {
                                    if (u.flying && !unit.canTargetAir(weapons))
                                    {

                                    }
                                    else if (u.isSubmerged && !unit.canTargetSub(weapons))
                                    {

                                    }
                                    else
                                    {
                                        subSuitability.x = 1;
                                    }
                                }
                            }
                        }
                    } 
                    else if (taskSubType == "Boost" || taskSubType == "")
                    {
                        if (unit.hasBoostingAbilities() && unit.getCurrentAP() > 0)
                        {
                            subSuitability.w = 1;
                        }
                    }
                    break;
                case "Transport Units":
                    if (!(sentryAI || !unit.transportsUnits))
                    {
                        u = (Unit)task.objective;
                        //We don't want to transport ourself
                        if (u == unit) break;
                        foreach (string exclude in unit.excludeList)
                        {
                            if (unit.transportTypes.Contains(exclude))
                            {
                                break;
                            }
                        }
                        bool foundInclusion = false;
                        foreach (string include in unit.includeList)
                        {
                            if (unit.transportTypes.Contains(include))
                            {
                                foundInclusion = true;
                                break;
                            }
                        }
                        if (foundInclusion) subSuitability.x = 1;
                    }
                    break;
                case "Search":
                    if (!sentryAI && unit.getMovementType() != "Stationary")
                    {
                        subSuitability.x = 1;
                    }
                    break;
                case "Make Unit":
                    if (isBuilding)
                    {
                        Building building = (Building)this;
                        if (building.makesUnits && tile.getUnit() == null)
                        {
                            subSuitability.x = 1;
                        } 
                    } 
                    break;
            }

        }
        TPReturnData tprData = new TPReturnData();
        tprData.subSuitability = subSuitability;
        tprData.attackPath = goalPath;
        tprData.completed = true;
        return tprData;
    }

    public Tile findRetreatTile()
    {
        Debug.Log("Beggining fRT");
        Dictionary<Tile, float> influenceDict = ((Unit)this).gM.influenceDict[team];
        //Remove all tiles that have a unit on them
        List<Tile> tiles = new List<Tile>(influenceDict.Keys.ToList<Tile>());
        tiles.RemoveAll(item => item.getUnit() != null || (item.getBuilding() != null && item.getBuilding().makesUnits));
        float max = 0;
        Tile maxTile = tile;
        foreach(Tile t in tiles)
        {
            if (influenceDict[t] >= max)
            {
                max = influenceDict[t];
                maxTile = t;
            }
            
        }
        //Debug.Log(maxTile == null);
        Debug.Log(maxTile.getPos());
        return maxTile;
    }


}


