using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using Priority_Queue;
using System.Linq;
using System;

public class PossibleAssignment
{
    public float distance;
    public float assignmentScore;
    public float modifier = 0;
    public Controllable possibleTaskDoer;
    public AITask aiTask;
    public GameManager gM;
    public bool shouldChangeWeapon = false;
    public List<Weapon> preferredHandCombo = new List<Weapon>();
    public List<HashSet<Weapon>> orderedWeapons = new List<HashSet<Weapon>>(), actualOrderedWeapons = new List<HashSet<Weapon>>();
    public float killedBonus = 15;
    public float exhaustRetreatBonus = 10;
    public float canAttackBonus = 10;
    public float exhaustAttackRetreatDrawback = 10;
    public float exhaustHealRepairRetreatDrawback = 10;
    public float exhaustCaptureDrawback = 10;
    public float scientistHealMod = -10f;
    public float scientistSelfHealMod = -30f;
    public float healRepairMod = -10f;
    public float costMod = 0.018f;
    public bool buildingUnit = false;
    public Tile buildingTile = null;
    public float maxMod = -9999999999999;
    public string taskSubType = "";
    public bool careAboutCost = false;
    public Dictionary<string, float> buildingDefenseModifiers;
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

    public PossibleAssignment()
    {

    }

    public PossibleAssignment(GameManager g, AITask task,Controllable pTD, bool bU, Tile bUTile)
    {
        aiTask = task;
        possibleTaskDoer = pTD;
        buildingUnit = bU;
        buildingTile = bUTile;
        if (!bU && aiTask.taskType != "Make Unit")
        {
            //Debug.Log(aiTask.taskType + " is considering " + pTD);
        }
        this.gM = g;
        if (pTD.tile == null) distance = 0.5f;
        if (task.priority == 13)
        {
            distance = 0.5f;
        }
        else
        {
            Unit pTDUnit = (Unit)pTD;
            //task doer must be a unit
            if (task.priority == 4 || ((task.priority == 8 || task.priority == 11) && task.objective == pTD))
            {
                distance = 0.5f;
            }
            else
            {
                if (!bU)
                {
                    distance = g.getMovementDistance(pTDUnit, pTDUnit.getTile(), task.objective.tile);
                }
                else
                {
                    //Debug.Log("Bu tiles " + bUTile.getPos());
                    //Debug.Log("Dest tile " + task.objective.tile.getPos());
                    distance = g.getMovementDistance(pTDUnit, bUTile, task.objective.tile);
                    //Debug.Log(distance);
                }
                //Prevent dividing by 0
                if (distance < 1) distance = 0.5f + distance / 2;
            }
        }
        modifier = aiTask.modifier + getMod();
        if (buildingUnit)
        {
            //Debug.Log("Modifier for " + (Unit)possibleTaskDoer + " is " + modifier);
        }
        getScore();
    }

    public float getScore()
    {
        if (aiTask.taskType == "Attack Enemy" && taskSubType == "Attack Enemy")
        {
            modifier += aiTask.laterMods.x;
        }
        assignmentScore = (14 - aiTask.priority + modifier) / distance;
        
        //Debug.Log("Score of " + assignmentScore);
        if (buildingUnit)
        {
            Player p = gM.playerDictionary[possibleTaskDoer.side];
            Unit u = (Unit)possibleTaskDoer;

            float metalC;
            float pplC;
            if (u.getMTCost() > 0)
            {
                metalC = p.metal / u.getMTCost();
            }
            else
            {
                metalC = 10;
            }
            if (u.getPPLCost() > 0)
            {
                pplC = p.people / u.getPPLCost();
            }
            else
            {
                pplC = 10;
            }
            if (careAboutCost)
            {
                assignmentScore = assignmentScore + (metalC / 2 + pplC / 2) * costMod;
            }
            //Debug.Log("Score of "+assignmentScore+" for "+u);
            if (metalC < 1) assignmentScore = -9999999;
            if (pplC < 1) assignmentScore = -9999999;

        }
        /*if (aiTask.taskType == "Sentry Near Enemy" || aiTask.taskType == "Attack Enemy Near Building" || aiTask.taskType == "Attack Enemy")
        {
            Debug.Log("Score of " + assignmentScore + " for " + aiTask.taskType);
        }*/
        return assignmentScore;
    }

    public float getMod()
    {
        float mod = aiTask.modifier + getHypotheticalMod(aiTask.taskType, possibleTaskDoer, aiTask.objective);
        //Add stuff to modifier
        return mod;
    }

    public float getHypotheticalMod(string type, Controllable pTD, Controllable obj)
    {
        float mod = 0;
        //Debug.Log("Checking hypothetical:"+ type);
        if (type == "Make Unit")
        {
            return mod;
        }
        switch (type)
        {
            case "Attack Enemy Near Building":
            case "Attack Enemy":
                maxMod = -9999999999999;
                float tempMod = 0;
                Unit ptdUnit = (Unit)pTD;
                float attackAdvantage;
                pTD.taskSubType = "Attack Enemy";
                if (pTD.isTaskSuitable(gM, aiTask, buildingUnit, buildingTile))
                {
                    attackAdvantage = determineAttackAdvantage((Unit)pTD, (Unit)obj, false, false);

                    tempMod = attackAdvantage;
                    if (!buildingUnit)
                    {
                        //Debug.Log(pTD + " has an attack advantage of " + aa + " against " + obj);
                    }
                    if (!ptdUnit.canAttackThisTurn())
                    {
                        tempMod -= exhaustAttackRetreatDrawback;
                    }
                    else
                    {
                        tempMod += canAttackBonus;
                    }
                    if (tempMod > maxMod)
                    {
                        taskSubType = pTD.taskSubType;
                        maxMod = tempMod;
                        actualOrderedWeapons = orderedWeapons;
                    }
                }
                else
                {
                    //Debug.Log(ptdUnit + " can't attack " + (Unit)obj);
                }
                if (type == "Attack Enemy Near Building") return maxMod;
                //Reverse the attack order for sentry
                pTD.taskSubType = "Sentry";
                if (pTD.isTaskSuitable(gM, aiTask, buildingUnit, buildingTile))
                {
                    tempMod = -1;
                 
                    float sentryAdvantage = determineAttackAdvantage((Unit)obj, (Unit)pTD, true, true);
                    if (!buildingUnit)
                    {
                        //Debug.Log(pTD + " has an attack sentry advantage of " + aa + " against " + obj);
                    }
                    //if (ptdUnit.getCurrentAP() < ptdUnit.getAP())
                    if (!ptdUnit.canAttackThisTurn())
                    {
                        tempMod -= exhaustAttackRetreatDrawback;
                    }
                    tempMod -= sentryAdvantage;
                    if (tempMod > maxMod)
                    {
                        taskSubType = "Sentry Near Enemy";
                        maxMod = tempMod;
                    }
                    //This gives the retreat variant
                    //Ignore if we are building a unit, because we shouldn't be focusing on retreating when building a unit
                    if (!buildingUnit)
                    {
                        tempMod += 1;
                        tempMod *= -1;
                        tempMod -= 4;
                        if (tempMod > maxMod)
                        {
                            taskSubType = "Retreat and Sentry";
                            maxMod = tempMod;
                        }
                    }
                }

                pTD.taskSubType = "Fortify";
                if (pTD.isTaskSuitable(gM, aiTask, buildingUnit, buildingTile))
                {
                    tempMod = -2;
                    bool lastGuard = ptdUnit.getGuard();
                    ptdUnit.setGuard(true);
                    tempMod -= determineAttackAdvantage((Unit)obj, (Unit)pTD, false, true);
                    ptdUnit.setGuard(lastGuard);
                    //if (ptdUnit.getCurrentAP() < ptdUnit.getAP())
                    if (!ptdUnit.canAttackThisTurn())
                    {
                        tempMod -= exhaustAttackRetreatDrawback;
                    }
                    if (tempMod > maxMod)
                    {
                        taskSubType = "Fortify Near Enemy";
                        maxMod = tempMod;
                    }
                    //Ignore if we are building a unit, because we shouldn't be focusing on retreating when building a unit
                    if (!buildingUnit)
                    {
                        tempMod += 2;
                        tempMod *= -1;
                        tempMod -= 5;
                        if (tempMod > maxMod)
                        {
                            taskSubType = "Retreat and Fortify";
                            maxMod = tempMod;
                        }
                    }
                }
                if (tempMod == 0) Debug.Log("temp mod has a value of 0!");
                //Debug.Log(maxMod);
                ptdUnit.taskSubType = "";
                return maxMod;
            case "Heal Unit":
            case "Repair Unit":
                ptdUnit = (Unit)pTD;
                Unit objUnit = (Unit)obj; 
                if (ptdUnit.getCurrentAP() <= 0)
                {
                    mod -= exhaustHealRepairRetreatDrawback;
                }
                if (objUnit.name == "Scientist")
                {
                    mod += scientistHealMod;
                }
                mod += healRepairMod;
                break;
            case "Deploy Units":
                mod = 10;
                ptdUnit = (Unit)pTD;
                if (ptdUnit.getCurrentAP() <= 0)
                {
                    mod -= exhaustAttackRetreatDrawback*100;
                }
                break;
            case "Transport Units":
                ptdUnit = (Unit)pTD;
                mod = 1;
                break;
            case "Capture Enemy Building":
            case "Capture Neutral Building":
                ptdUnit = (Unit)pTD;
                if (ptdUnit.getCurrentAP() < ptdUnit.getAP())
                {
                    mod -= exhaustCaptureDrawback;
                }
                break;

        }
        return mod;
    }

    public float determineAttackAdvantage(Unit attacker, Unit defender, bool sentry, bool flipped)
    {
        float advantage = 0;
        //Deal with primary weapons first

        //We must go through each weapon
        List<HashSet<Weapon>> weapons = new List<HashSet<Weapon>>();
        //Check to make sure there are damage potential in each list 
        List<Weapon> dhWeapons = attacker.getAllDamageHandWeapons();
        dhWeapons.RemoveAll(weapon => weapon.currentAttacks >= weapon.maxAttacksPerTurn || weapon == null);
        HashSet<Weapon> handWeapons = new HashSet<Weapon>(dhWeapons);
        if (handWeapons.Count > 0)
        {
            
            weapons.Add(handWeapons);
        } 
        if (attacker.getPossibleActions().Contains("Fire Turret 1"))
        {
            if (attacker.turrets != null && attacker.turrets.Count > 0 && attacker.turrets[0].damages && attacker.turrets[0].currentAttacks < attacker.turrets[0].maxAttacksPerTurn)
            {
                weapons.Add(new HashSet<Weapon>() { attacker.turrets[0] });
            }
        }
        if (attacker.getPossibleActions().Contains("Fire Turret 2"))
        {
            if (attacker.turrets != null && attacker.turrets.Count > 1 && attacker.turrets[1].damages && attacker.turrets[1].currentAttacks < attacker.turrets[1].maxAttacksPerTurn)
            {
                weapons.Add(new HashSet<Weapon>() { attacker.turrets[1] });
            }
        }
        if (attacker.getPossibleActions().Contains("Fire Turret 3"))
        {
            if (attacker.turrets != null && attacker.turrets.Count > 2 && attacker.turrets[2].damages && attacker.turrets[2].currentAttacks < attacker.turrets[2].maxAttacksPerTurn)
            {
                weapons.Add(new HashSet<Weapon>() { attacker.turrets[2] });
            }
        }
        if (attacker.getPossibleActions().Contains("Fire Turret 4"))
        {
            if (attacker.turrets != null && attacker.turrets.Count > 3 && attacker.turrets[3].damages && attacker.turrets[3].currentAttacks < attacker.turrets[3].maxAttacksPerTurn)
            {
                weapons.Add(new HashSet<Weapon>() { attacker.turrets[3] });
            }
        }
        if (attacker.getPossibleActions().Contains("Fire Turret 5"))
        {
            if (attacker.turrets != null && attacker.turrets.Count > 4 && attacker.turrets[4].damages && attacker.turrets[4].currentAttacks < attacker.turrets[4].maxAttacksPerTurn)
            {
                weapons.Add(new HashSet<Weapon>() { attacker.turrets[4] });
            }
        }
        //If we have to change weapons, something else should be implemented but for now just try to avoid having to do that
        if (weapons.Count == 0) return -killedBonus;


        //Lower priorities are at the front of the queue
        SimplePriorityQueue<HashSet<Weapon>, float> attackerDamages = new SimplePriorityQueue<HashSet<Weapon>, float>();
        SimpleNode<Weapon, float> temp, temp2;
        Dictionary<Weapon, List<Tile>> irPaths = new Dictionary<Weapon, List<Tile>>();
        Dictionary<HashSet<Weapon>, List<Tile>> irHSPaths = new Dictionary<HashSet<Weapon>, List<Tile>>();
        float attackerDamage = 0;
        float attackerHealth = attacker.getCurrentHP();
        float defenderDamage = 0;
        float defenderHealth = defender.getCurrentHP();
        bool addDamage = false;
        bool damageAdded = false;
        List<Tile> tempPath = new List<Tile>();
        List<Tile> tempHSPath = new List<Tile>();
        //Add everything to the priority queue that can attack the enemy
        foreach (HashSet<Weapon> currentWeapons in weapons) {
            attackerDamage = 0;
            damageAdded = false;
            tempHSPath = new List<Tile>();
            foreach (Weapon weapon in currentWeapons)
            {
                if (buildingUnit)
                {
                    tempPath = gM.getInRangePathAH(attacker, attacker.getTile() != null ? attacker.getTile() : buildingTile, defender.getTile() == null ? buildingTile :
                    defender.getTile(), new List<Weapon>() { weapon }, true, true);
                    if (tempPath != null)
                    {
                        addDamage = true;
                    }
                }
                else if (!buildingUnit) 
                {
                    tempPath = gM.getInRangePathAH(attacker, attacker.getTile(), defender.getTile(), new List<Weapon>() { weapon }, true, true);
                    if (tempPath != null)
                    {
                        addDamage = true;
                    }
                }
                irPaths.Add(weapon, tempPath);
                if (tempPath != null)
                {
                    tempHSPath.AddRange(tempPath);
                }
                if (addDamage) {
                    for (int i = 0; i < weapon.maxAttacksPerTurn; i++)
                    {
                        attackerDamage += weapon.getHypotheticalDamagePerAttack(attacker, defender);
                    }
                    damageAdded = true;
                }
            }
            if (damageAdded)
            {
                irHSPaths.Add(currentWeapons, tempHSPath);
                attackerDamages.Enqueue(currentWeapons, attackerDamage);
            }
        }
        orderedWeapons = new List<HashSet<Weapon>>();
        while (attackerDamages.Count > 0)
        {
            HashSet<Weapon> tempSet = attackerDamages.Dequeue();
            orderedWeapons.Insert(0, tempSet);
        }
        
        //Go through the ordered weapons and deal damage
        foreach(HashSet<Weapon> weaponSet in orderedWeapons)
        {
            //Notice: We need to first determine if it's possible to attack the enemy each weapon
            
            if (!sentry)
            {
                attackerDamage = 0;
                attackerHealth -= defenderDamage;
                if (attackerHealth < 0) return -killedBonus;
                foreach (Weapon weapon in weaponSet)
                {
                    for (int i = 0; i < weapon.maxAttacksPerTurn; i++)
                    {
                        attackerDamage += weapon.getHypotheticalDamagePerAttack(attacker, defender) * attackerHealth / attacker.getHP() / attacker.getHPRatio();
                    }
                }
            }

            defenderHealth -= attackerDamage;
            if (defenderHealth < 0) return killedBonus;
            defenderDamage = 0;

            List<Tile> path = irHSPaths[weaponSet];
            Tile firingTile = path[path.Count - 1];
            foreach (Weapon weapon in defender.getAllActiveWeapons())
            {
                /*if (gM.getInRangePathAH(defender, (defender.getTile() != null) ? defender.getTile() : buildingTile, firingTile, new List<Weapon>() { weapon }, true, false) != null)
                {
                    defenderDamage += weapon.getHypotheticalDamagePerAttack(defender, attacker) * defenderHealth / defender.getHP() / defender.getHPRatio();
                }*/
                if (gM.canAttackEnemyExactlyWithWeapons(defender, (defender.getTile() != null) ? defender.getTile() : buildingTile, attacker, firingTile,
                    new List<Weapon>() { weapon }))
                {
                    /*if (!buildingUnit)
                    {
                        Debug.Log(weapon + " can fire on " + firingTile + " from " + defender);
                    }*/
                    defenderDamage += weapon.getHypotheticalDamagePerAttack(defender, attacker) * defenderHealth / defender.getHP() / defender.getHPRatio();
                }
            }
            

            if (sentry)
            {
                attackerDamage = 0;
                attackerHealth -= defenderDamage;
                if (attackerHealth < 0) return -killedBonus;
                foreach (Weapon weapon in weaponSet)
                {
                    for (int i = weapon.currentAttacks; i < weapon.maxAttacksPerTurn; i++)
                    {
                        attackerDamage += weapon.getHypotheticalDamagePerAttack(attacker, defender) * attackerHealth / attacker.getHP() / attacker.getHPRatio();
                    }
                }
            }
        }
        //If they didn't kill each other, get the respective damage ratios
        float attackerRatio = attackerHealth / attacker.getCurrentHP();
        float defenderRatio = defenderHealth / defender.getCurrentHP();
        float difference = attackerRatio - defenderRatio;
        //Debug.Log("Attacker " + attacker + " has attacker ratio of " + attackerRatio + " and defender " + defender + " has defense ratio of " + defenderRatio);
        advantage = difference * killedBonus;
        //Debug.Log(advantage);
        return advantage;
    }

    //Assign the task if we have the highest priority
    public bool assign()
    {
        if (possibleTaskDoer.assignedTask == null)
        {
            //Debug.Log("Assigning to " + possibleTaskDoer.isBuilding);
            aiTask.assign(this, possibleTaskDoer);
            aiTask.taskDoer.orderedWeapons = actualOrderedWeapons;
            possibleTaskDoer.taskSubType = taskSubType;
            //Debug.Log(taskSubType);
            return true;
        }
        return false;

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
        foreach (Tile t in tiles)
        {
            priorities += getBuildingDefensePriority(unit, t.getBuilding());
        }
        return priorities;
    }
}
