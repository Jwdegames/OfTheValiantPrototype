using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

// Class used to process possible assignments for the Utility AI
// Meant to be used with Multithreading
// Many methods from other classes are copied over here to prevent race conditions of multithreading
[System.Serializable]
public class PAThreadedChecker 
{
    List<PossibleAssignment> pAs;
    public DijakstraCalculator localDCalc;
    public GameManager gM;
    public AIManager aiM;
    public AITask taskToCheck;
    public Controllable asset;
    public PossibleAssignment pA;
    public bool buildingUnit;
    public Tile buildTile;
    public TPReturnData tprd = new TPReturnData();
    public int mode = 0;
    public int subMode = 0;

    public PAThreadedChecker(List<PossibleAssignment> pAs, AIManager ai, GameManager g, AITask task, Controllable a, bool bU, Tile bT)
    {
        this.pAs = pAs;
        aiM = ai;
        gM = g;
        localDCalc = new DijakstraCalculator(g, null, null);
        taskToCheck = task;
        asset = a;
        buildingUnit = bU;
        buildTile = bT;
    }

    public bool checkForSuitability(TPReturnData tprData)
    {
        Vector4 suitabilityVector = tprData.subSuitability;
        if (suitabilityVector.x > 0 || suitabilityVector.y > 0 || suitabilityVector.z > 0 || suitabilityVector.w > 0)
        {
            return true;
        }
        return false;
    }

    public float checkDistance()
    {
        float distance;
        if (asset.tile == null) distance = 0.5f;
        if (taskToCheck.priority == 13)
        {
            distance = 0.5f;
        }
        if (taskToCheck.priority == 4 || ((taskToCheck.priority == 8 || taskToCheck.priority == 11) && taskToCheck.objective == asset))
        {
            distance = 0.5f;
        }
        else if (!(asset.isBuilding))
        {
 
            if (!buildingUnit)
            {
                Unit pTDUnit = (Unit)asset;
                localDCalc.reset();

                    localDCalc.setValues(pTDUnit.getTile(), taskToCheck.objective.tile);
                    distance = localDCalc.getMovementDistance(pTDUnit);
                    
            }
            else
            {
                //Debug.Log("Bu tiles " + bUTile.getPos());
                //Debug.Log("Dest tile " + task.objective.tile.getPos());
                Unit pTDUnit = (Unit)asset;
                localDCalc.reset();
                    localDCalc.setValues(buildTile, taskToCheck.objective.tile);
                    distance = localDCalc.getMovementDistance(pTDUnit);
                    
                //Debug.Log(distance);
            }
            
        }
        else
        {
            distance = 0.5f;
        }
        return distance;
    }

    public void checkPA()
    {
        //Debug.Log("Beginning Check");
        if (asset == null)
        {
            Debug.LogError("Null asset!");
        }
        mode = 1;
        isTaskSuitable();
        mode = 2;
        if (checkForSuitability(tprd))
        {
            if (asset == null)
            {
                Debug.LogError("Null asset!");
            }
            mode = 3;
            float distance = checkDistance();
            mode = 4;
            if (asset == null)
            {
                Debug.LogError("Null asset!");
            }
            pA = new PossibleAssignment(gM, taskToCheck, asset, buildingUnit, buildTile);
            if (asset == null)
            {
                Debug.LogError("Null asset!");
            }
            mode = 5;
            pA.tprd = tprd;
            pA.paTC = this;
            pA.distance = distance;
            pA.initGetScore();
            mode = 6;
            lock (pAs)
            {
                pAs.Add(pA);
            }
            mode = 7;
            }
        
        mode = 8;
        aiM.patcCompletedCount++;
        //Debug.Log("Check complete");
    }

    public bool canAttackAbsolute(Unit unit)
    {
        lock (unit)
        {
            List<string> possibleActions = new List<string>(unit.getPossibleActions());
            if (possibleActions.Contains("Attack"))
            {
                List<Weapon> dmgHandWeapons = unit.getAllDamageHandWeapons();
                if (dmgHandWeapons != null && dmgHandWeapons.Count > 0)
                {
                    return true;
                }
                else if (possibleActions.Contains("Change Weapon"))
                {
                    List<Weapon> dmgWeapons = unit.getAllNonTurretDamageWeapons();
                    if (dmgWeapons != null)
                    {
                        dmgWeapons.RemoveAll(item => !item.damages);
                        if (dmgWeapons.Count > 0)
                        {
                            return true;
                        }
                    }
                }

            }
            if (possibleActions.Contains("Fire Turret 1"))
            {
                if (unit.turrets != null && unit.turrets.Count > 0 && unit.turrets[0].damages)
                {
                    return true;
                }
            }
            if (possibleActions.Contains("Fire Turret 2"))
            {
                if (unit.turrets != null && unit.turrets.Count > 1 && unit.turrets[1].damages)
                {
                    return true;
                }
            }
            if (possibleActions.Contains("Fire Turret 3"))
            {
                if (unit.turrets != null && unit.turrets.Count > 2 && unit.turrets[2].damages)
                {
                    return true;
                }
            }
            if (possibleActions.Contains("Fire Turret 4"))
            {
                if (unit.turrets != null && unit.turrets.Count > 3 && unit.turrets[3].damages)
                {
                    return true;
                }
            }
            if (possibleActions.Contains("Fire Turret 5"))
            {
                if (unit.turrets != null && unit.turrets.Count > 4 && unit.turrets[4].damages)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Thread Safe method for check if action possible
    public bool checkIfActionPossible(Unit unit)
    {
        //Debug.Log("Checking for possible action");
        List<string> actions = new List<string>(unit.getPossibleActions());
        foreach (string action in actions)
        {
            switch (action)
            {
                case "Move":
                    localDCalc.reset();
                    localDCalc.setValues(unit.tile, null);
                    List<Tile> moveables = localDCalc.findAllMoveables();
                    //Debug.Log(moveables);
                    if (moveables != null && moveables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Attack":
                    localDCalc.reset();
                    localDCalc.setValues(unit.tile, null);
                    List<Tile> attackbles = localDCalc.findAllAttacks(unit);
                    if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Fortify":
                case "Sentry":
                    if (unit.getCurrentAP() > 0) return true;
                    break;
                case "Capture":
                    if (unit.getCurrentAP() > 0 && unit.tile.getBuilding() != null && unit.tile.getBuilding().team != unit.team)
                    {
                        return true;
                    }
                    break;
                case "Deploy Drones":
                case "Deploy Units":
                    if (unit.canDeploy() && unit.getCurrentAP() > 0)
                    {
                        return true;
                    }
                    break;
                case "Heal":
                    List<Tile> healables = localDCalc.findAllHealables(unit, unit.getAllHealHandWeapons());
                    if (unit.getCurrentAP() > 0 && healables != null && healables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Repair":
                    healables = localDCalc.findAllHealables(unit, unit.getAllRepairHandWeapons());
                    if (unit.getCurrentAP() > 0 && healables != null && healables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Load Units":
                    if (unit.canLoadUnits() && unit.getCurrentAP() > 0)
                    {
                        return true;
                    }
                    break;
                case "Unload Units":
                    if (unit.canUnloadUnits() && unit.getCurrentAP() > 0)
                    {
                        return true;
                    }
                    break;
                case "Fire Turret 1":
                    if (unit.turrets != null && unit.turrets.Count > 0)
                    {
                        localDCalc.reset();
                        localDCalc.setValues(unit.tile, null);
                        attackbles = localDCalc.findAllAttacksWithWeapons(unit, new List<Weapon>() { unit.turrets[0] });
                        if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 2":
                    if (unit.turrets != null && unit.turrets.Count > 0)
                    {
                        localDCalc.reset();
                        localDCalc.setValues(unit.tile, null);
                        attackbles = localDCalc.findAllAttacksWithWeapons(unit, new List<Weapon>() { unit.turrets[1] });
                        if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 3":
                    if (unit.turrets != null && unit.turrets.Count > 0)
                    {
                        localDCalc.reset();
                        localDCalc.setValues(unit.tile, null);
                        attackbles = localDCalc.findAllAttacksWithWeapons(unit, new List<Weapon>() { unit.turrets[2] });
                        if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 4":
                    if (unit.turrets != null && unit.turrets.Count > 0)
                    {
                        localDCalc.reset();
                        localDCalc.setValues(unit.tile, null);
                        attackbles = localDCalc.findAllAttacksWithWeapons(unit, new List<Weapon>() { unit.turrets[3] });
                        if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 5":
                    if (unit.turrets != null && unit.turrets.Count > 0)
                    {
                        localDCalc.reset();
                        localDCalc.setValues(unit.tile, null);
                        attackbles = localDCalc.findAllAttacksWithWeapons(unit, new List<Weapon>() { unit.turrets[4] });
                        if (unit.getCurrentAP() > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
            }
        }
        return false;
    }

    // Checks if the asset is capable of doing the task
    // The usage of the local dijkstra calculator prevents race conditions of multithreading
    public void isTaskSuitable()
    {
        //Debug.Log("Checking suitable: " + task.taskType);
        subMode = 0;
        Vector4 subSuitability = new Vector4();
        List<Tile> goalPath = null;
        if (asset.isBuilding)
        {
            Building building = (Building)asset;
            if (taskToCheck.priority != 13)
            {
            }
            else
            {
                switch (taskToCheck.taskType)
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
            subMode = 1;
            Unit unit = (Unit)asset;
            Unit enemy;
            //Return if we can't do anything

            if (!buildingUnit)
            {
                if (unit.getName() == "Slime")
                {
                    //Debug.Log("Checking if slime can do task: " + task.taskType);
                }
                if (!checkIfActionPossible(unit))
                {
                    //Debug.Log(unit + " is unable to perform any more actions!");
                    return;
                }
            }
            //Unit should have AP to complete tasks
            switch (taskToCheck.taskType)
            {
                case "Attack Enemy Near Building":
                case "Attack Enemy":
                    // Determine if the asset can attack enemies without moving
                    localDCalc.reset();
                    //Debug.Log(asset);
                    localDCalc.setValues((buildingUnit)?buildTile:asset.tile, null);
                    List<Tile> attackTiles = localDCalc.findAllAttacksWithWeapons(unit, unit.getAllDamageActiveWeapons());
                    subMode = 2;
                    if (!(asset.sentryAI && (attackTiles == null || attackTiles.Count == 0)))
                    {
                        if (canAttackAbsolute(unit))
                        {
                            enemy = (Unit)taskToCheck.objective;
                            if (!buildingUnit)
                            {
                                //Debug.Log("Checking if " + unit + " can attack " + enemy);
                            }

                            if (!buildingUnit)
                            {
                                subMode = 3;
                                localDCalc.reset();
                                localDCalc.setValues(unit.tile, taskToCheck.objective.tile);
                                subMode = 4;
                                goalPath = localDCalc.findInRangePath(unit, unit.getAllDamageActiveWeapons(), true, false);
                                subMode = 5;
                                if (goalPath != null)
                                {
                                    subMode = 6;
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
                                    subMode = 7;
                                }
                                subMode = 8;

                            }
                            else if (buildingUnit)
                            {
                                subMode = 9;
                                localDCalc.reset();
                                localDCalc.setValues(buildTile, taskToCheck.objective.tile);
                                goalPath = localDCalc.findInRangePath(unit, unit.getAllDamageActiveWeapons(), true, true);
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
                                subMode = 10;
                            }
                        }
                    }
                    subMode = 11;
                    if (!(asset.sentryAI && (attackTiles == null || attackTiles.Count == 0 )))
                    {
                        if (unit.getPossibleActions().Contains("Fortify"))
                        {
                            subSuitability.y = 1;
                        }
                    }
                    subMode = 12;
                    if (!(asset.sentryAI && (attackTiles == null || attackTiles.Count == 0)))
                    {
                        enemy = (Unit)taskToCheck.objective;
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
                    subMode = 13;
                    // NOTICE: Abilities/commands should be capable of targeting all units regardless if they are flying or submerged
                    if (unit.hasOffensiveAbilities() && unit.getCurrentAP() > 0)
                    {
                        subSuitability.w = 1;
                    }
                    subMode = 14;
                    break;
                case "Capture Enemy Building":
                case "Capture Neutral Building":
                    if (!asset.sentryAI && unit.getPossibleActions().Contains("Capture") && taskToCheck.objective.tile.getUnitScript() == null)
                    {
                        subSuitability.x = 1;
                    }
                    break;
                case "Deploy Units":
                    //Only the unit that is deploying the drone is task suitable
                    if ((buildingUnit && unit == taskToCheck.objective) || (!buildingUnit && unit.canDeploy() && unit == taskToCheck.objective && unit.getCurrentAP() > 0))
                    {
                        subSuitability.x = 1;
                    }
                    //Debug.Log(unit + " can't deploy!");
                    break;
                case "Heal Unit":
                case "Repair Unit":
                    Unit u = (Unit)taskToCheck.objective;
                    localDCalc.reset();
                    localDCalc.setValues((buildingUnit)?buildTile:asset.tile, null);
                    List<Tile> healTiles = localDCalc.findAllHealables(u, u.getAllHealRepairActiveWeapons());
                    if (!(asset.sentryAI && (healTiles == null || healTiles.Count == 0)))
                    {

                        if ((u.biological && unit.canHealAbsolute()) || (u.mechanical && unit.canRepairAbsolute()))
                        {
                            List<Weapon> weapons = u.biological ? (u.mechanical ? unit.getAllHealRepairActiveWeapons() : unit.getAllHealWeaponsFromList(unit.getAllActiveWeapons()))
                                : (u.mechanical ? unit.getAllRepairWeaponsFromList(unit.getAllActiveWeapons()) : new List<Weapon>());
                            if (!buildingUnit)
                            {
                                localDCalc.reset();
                                localDCalc.setValues(unit.tile, taskToCheck.objective.tile);
                                goalPath = localDCalc.findInRangePath(unit, weapons, true, false);
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
                            else if (buildingUnit)
                            {
                                localDCalc.reset();
                                localDCalc.setValues(buildTile, taskToCheck.objective.tile);
                                goalPath = localDCalc.findInRangePath(unit, weapons, true, true);
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

                    if (unit.hasBoostingAbilities() && unit.getCurrentAP() > 0)
                    {
                        subSuitability.w = 1;
                    }
                    break;
                case "Transport Units":
                    if (!(asset.sentryAI || !unit.transportsUnits))
                    {
                        u = (Unit)taskToCheck.objective;
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
                    if (!asset.sentryAI && unit.getMovementType() != "Stationary")
                    {
                        subSuitability.x = 1;
                    }
                    break;
                case "Make Unit":
                    if (asset.isBuilding)
                    {
                        Building building = (Building)asset;
                        if (building.makesUnits && asset.tile.getUnitScript() == null)
                        {
                            subSuitability.x = 1;
                        }
                    }
                    break;
            }

        }
        TPReturnData tprData = new TPReturnData
        {
            subSuitability = subSuitability,
            attackPath = goalPath,
            completed = true
        };
        tprd = tprData;
    }

    override
    public string ToString()
    {
        return "PAThreadedChecker for task " + taskToCheck.taskType + " of asset " + asset + " on mode " +mode;
    }
}
