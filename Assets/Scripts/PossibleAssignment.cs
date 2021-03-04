using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using Priority_Queue;
using System.Linq;
using System;

[System.Serializable]
public class PossibleAssignment
{
    public float distance;
    public float assignmentScore;
    public float modifier = 0;
    public Controllable possibleTaskDoer;
    public AITask aiTask;
    public GameManager gM;
    public bool shouldChangeWeapon = false;
    public bool shouldTakeIntoAccountDistance = true;
    public List<Weapon> preferredHandCombo = new List<Weapon>();
    public List<HashSet<Weapon>> orderedWeapons = new List<HashSet<Weapon>>(), actualOrderedWeapons = new List<HashSet<Weapon>>();
    public float killedBonus = 35;
    public float exhaustRetreatBonus = 10;
    public float canAttackBonus = 10;
    public float exhaustAttackRetreatDrawback = 10;
    public float exhaustHealRepairRetreatDrawback = 10;
    public float exhaustCaptureDrawback = 10;
    public float scientistHealMod = -10f;
    public float scientistSelfHealMod = -30f;
    public float healRepairMod = -10f;
    public float costMod = 0.018f;
    public float apInUseBonus = 20f;
    public bool buildingUnit = false;
    public Tile buildingTile = null;
    public float maxMod = -9999999999999;
    private string taskSubType = "";
    public bool canDoSubType1 = false;
    public bool canDoSubType2 = false;
    public bool canDoSubType3 = false;
    public bool canDoSubType4 = false;
    private string taskSubType2 = "";
    public string optimalAbility = "";
    public Dictionary<int, Dictionary<Weapon, Tile>> firingDictionary = new Dictionary<int, Dictionary<Weapon, Tile>>();
    public bool careAboutCost = false;
    public Dictionary<string, float> buildingDefenseModifiers;
    public bool scoreCalculated = false;
    public TPReturnData tprd;
    public PAThreadedChecker paTC;
    public DijakstraCalculator dCalc;

    // Need to save data to reduce processing speed
    public Dictionary<HashSet<Weapon>, List<Tile>> attackerirHSPaths = null; 
    public Dictionary<HashSet<Weapon>, List<Tile>> defenderirHSPaths = null;
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
        dCalc = new DijakstraCalculator(g, null, null);

        if (buildingUnit)
        {
            //Debug.Log("Modifier for " + (Unit)possibleTaskDoer + " is " + modifier);
        }
        
    }

    public void initGetScore()
    {
        paTC.subMode = 0;
        modifier = aiTask.modifier + getMod();
        paTC.subMode = 25;
        getScore();
        paTC.subMode = 100;
        scoreCalculated = true;
    }

    public float getScore()
    {
        if (aiTask.taskType == "Attack Enemy" && taskSubType == "Attack Enemy")
        {
            modifier += aiTask.laterMods.x;
        }
        assignmentScore = (14 - aiTask.priority + modifier);
        
        if (shouldTakeIntoAccountDistance)
        {
            assignmentScore /= distance;
        }
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
            //Debug.Log("Score of " + assignmentScore + " for " + aiTask.taskType);
            //Debug.Log("Task " + aiTask.taskType + " of " + possibleTaskDoer + " has task subtype " + taskSubType);
        }*/
        if (taskSubType == null)
        {
            optimalAbility = null;
        }
        return assignmentScore;
    }

    public float getMod()
    {
        float mod = aiTask.modifier + getHypotheticalMod(aiTask.taskType, possibleTaskDoer, aiTask.objective);
        //Add stuff to modifier
        return mod;
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

    public float getHypotheticalMod(string type, Controllable pTD, Controllable obj)
    {
        float mod = 0;
        paTC.subMode = 2;
        Unit objUnit = (!obj.isBuilding) ? (Unit)obj : null;
        //Debug.Log("Checking hypothetical:"+ type);
        if (type == "Make Unit")
        {
            return mod;
        }
        switch (type)
        {
            case "Attack Enemy Near Building":
            case "Attack Enemy":
                maxMod = float.MinValue;
                float tempMod = 0;
                Unit ptdUnit = (Unit)pTD;
                taskSubType = "Attack Enemy";
                float attackAdvantage;
                pTD.taskSubType = "Attack Enemy";
                paTC.subMode = 3;
                if (tprd.subSuitability.x > 0)
                {
                    attackAdvantage = 0;
                    if (tprd.attackPath != null && tprd.attackPath.Count <= 15)
                    {
                        paTC.subMode = 4;
                        attackAdvantage = determineAttackAdvantage(ptdUnit, objUnit, false, false, false);
                        paTC.subMode = 5;

                    } 
                    else
                    {
                        paTC.subMode = 6;
                        attackAdvantage = determineQuickAA(ptdUnit, objUnit, false, false, false);
                        paTC.subMode = 7;
                    }

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

                        taskSubType = pTD.taskSubType;
                        maxMod = tempMod;
                        actualOrderedWeapons = orderedWeapons;
                        paTC.subMode = 8;

                }
                else
                {
                    //Debug.Log(ptdUnit + " can't attack " + (Unit)obj);
                }
                if (type == "Attack Enemy Near Building") return maxMod;
                //Reverse the attack order for sentry
                paTC.subMode = 9;
                pTD.taskSubType = "Sentry";
                if (tprd.subSuitability.y > 0)
                {
                    paTC.subMode = 10;
                    tempMod = -3;
                    if (ptdUnit.getCurrentAP() < ptdUnit.getAP() && ptdUnit.getCurrentAP() > 0)
                    {
                        tempMod -= apInUseBonus;
                    }
                    float sentryAdvantage = 0;
                    if (tprd.attackPath != null && tprd.attackPath.Count <= 15)
                    {
                        paTC.subMode = 11;
                        sentryAdvantage = determineAttackAdvantage(objUnit, ptdUnit, true, false, false);

                    }
                    else
                    {
                        paTC.subMode = 12;
                        sentryAdvantage = determineQuickAA(objUnit, ptdUnit, true, false, false);
                    }
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
                    paTC.subMode = 13;
                    //This gives the retreat variant
                    //Ignore if we are building a unit, because we shouldn't be focusing on retreating when building a unit
                    if (!buildingUnit)
                    {
                        tempMod += 1;
                        tempMod *= -1;
                        tempMod -= 4;
                        if (ptdUnit.getCurrentAP() < ptdUnit.getAP() && ptdUnit.getCurrentAP() > 0)
                        {
                            tempMod -= apInUseBonus*4;
                        }
                        if (tempMod > maxMod)
                        {
                            taskSubType = "Retreat and Sentry";
                            maxMod = tempMod;
                        }
                        paTC.subMode = 14;
                    }
                }

                pTD.taskSubType = "Fortify";
                if (tprd.subSuitability.z > 0)
                {
                    paTC.subMode = 15;
                    tempMod = -5;
                    if (ptdUnit.getCurrentAP() < ptdUnit.getAP() && ptdUnit.getCurrentAP() > 0)
                    {
                        tempMod -= apInUseBonus;
                    }
                    bool lastGuard = ptdUnit.getGuard();
                    ptdUnit.setGuard(true);
                    float guardAdvantage = 0;
                    if (tprd.attackPath != null && tprd.attackPath.Count <= 15)
                    {
                        paTC.subMode = 16;
                        guardAdvantage = determineAttackAdvantage(objUnit, ptdUnit, false, true, false);
                        paTC.subMode = 17;

                    }
                    else
                    {
                        paTC.subMode = 18;
                        guardAdvantage = determineQuickAA(objUnit, ptdUnit, false, true, false);
                        paTC.subMode = 19;
                    }
                    tempMod -= guardAdvantage;
                    ptdUnit.setGuard(lastGuard);
                    //if (ptdUnit.getCurrentAP() < ptdUnit.getAP())
                    if (!ptdUnit.canAttackThisTurn())
                    {
                        tempMod -= exhaustAttackRetreatDrawback;
                    }
                    if (ptdUnit.getCurrentAP() < ptdUnit.getAP() && ptdUnit.getCurrentAP() > 0)
                    {
                        tempMod -= apInUseBonus;
                    }
                    if (tempMod > maxMod)
                    {
                        taskSubType = "Fortify Near Enemy";
                        maxMod = tempMod;
                    }
                    paTC.subMode = 20;
                    //Ignore if we are building a unit, because we shouldn't be focusing on retreating when building a unit
                    if (!buildingUnit)
                    {
                        tempMod += 2;
                        tempMod *= -1;
                        tempMod -= 5;
                        if (ptdUnit.getCurrentAP() < ptdUnit.getAP() && ptdUnit.getCurrentAP() > 0)
                        {
                           // Debug.Log(tempMod);
                            tempMod -= apInUseBonus*4;
                            //Debug.Log(tempMod);
                        }
                        if (tempMod > maxMod)
                        {
                            taskSubType = "Retreat and Fortify";
                            maxMod = tempMod;
                            
                        }
                    }
                    paTC.subMode = 21;
                }

                pTD.taskSubType = "Use Ability";
                if (tprd.subSuitability.w > 0)
                {
                    paTC.subMode = 22;
                    tempMod = 0;
                    // Add a bonus if the unit can use ap for abilities, but can't use weapons
                    if (!Mathf.Approximately(ptdUnit.getCurrentAP(),0) && !ptdUnit.canAttackThisTurn())
                    {
                        tempMod += 10;
                    }
                    if (pTD.sentryAI)
                    {
                        tempMod += 10;
                    }
                    if (tempMod > maxMod)
                    {
                        taskSubType = "Use Ability";
                        taskSubType2 = taskSubType;
                        maxMod = tempMod;
                        // We need to determine the optimal ability to use
                        List<string> abilities = ptdUnit.getOffensiveAbilities();
                        string effect = "";
                        float tempMod2 = 0.1f;
                        float maxMod2 = float.MinValue;
                        foreach (string ability in abilities) {
                            tempMod = 0.1f;
                            if (StatsManager.commandEffectDictionary.ContainsKey(ability)) {
                                effect = StatsManager.commandEffectDictionary[ability];
                            }
                            // If the associated effect is already in the unit, then we should target another unit
                            if (effect != "" && ((Unit)obj).extraAttributes != null && ((Unit)obj).extraAttributes.ContainsKey(effect))
                            {
                                tempMod2 -= 10;
                            }
                            if (tempMod2 > maxMod2)
                            {
                                maxMod2 = tempMod2;
                                optimalAbility = ability;
                            }
                        }
                        // Change maxmod so this goes first
                        Dictionary<Tile, float> influenceDict = objUnit.gM.influenceDict[objUnit.getTeam()];
                        maxMod *= -influenceDict[objUnit.getTile()];
                        maxMod *= 10;
                        paTC.subMode = 23;
                    }
                }
                if (tempMod == 0) Debug.Log("temp mod has a value of 0!");
                //Debug.Log(maxMod);
                ptdUnit.taskSubType = "";
                paTC.subMode = 24;
                return maxMod;
            case "Heal Unit":
            case "Repair Unit":
                ptdUnit = (Unit)pTD;

                pTD.taskSubType = "Heal/Repair";
                tempMod = 0;
                maxMod = float.MinValue;
                if (tprd.subSuitability.x > 0)
                {
                    if (ptdUnit.getCurrentAP() <= 0)
                    {
                        tempMod -= exhaustHealRepairRetreatDrawback;
                    }
                    if (objUnit.unitName == "Scientist")
                    {
                        tempMod += scientistHealMod;
                    }
                    tempMod += healRepairMod;
                     maxMod = tempMod;
                     taskSubType = pTD.taskSubType;
                    
                }

                pTD.taskSubType = "Boost";
                if (tprd.subSuitability.w > 0)
                {
                    tempMod = 0;
                    // Add a bonus if the unit can use ap for abilities, but can't use weapons
                    if (!Mathf.Approximately(ptdUnit.getCurrentAP(), 0) && !ptdUnit.canHealThisTurn())
                    {
                        tempMod += 10;
                    } 
                    if (ptdUnit.sentryAI)
                    {
                        tempMod += 10;
                    }
                    if (tempMod > maxMod)
                    {
                        maxMod = tempMod;
                        taskSubType = "Boost";
                        taskSubType2 = taskSubType;
                        List<string> abilities = ptdUnit.getBoostingAbilities();
                        string effect = "";
                        float tempMod2 = 0;
                        float maxMod2 = float.MinValue;
                        foreach (string ability in abilities)
                        {
                            tempMod = 0;
                            if (StatsManager.commandEffectDictionary.ContainsKey(ability))
                            {
                                effect = StatsManager.commandEffectDictionary[ability];
                            }
                            // If the associated effect is already in the unit, then we should target another unit
                            if (effect != "" && ((Unit)obj).extraAttributes != null && ((Unit)obj).extraAttributes.ContainsKey(effect))
                            {
                                tempMod2 -= 10;
                            }
                            if (tempMod2 > maxMod2)
                            {
                                maxMod2 = tempMod2;
                                optimalAbility = ability;
                                taskSubType = "Boost";
                            }
                        }
                        // Reduce distance divider by multiplying by distance
                        // Additionally the influence dict should be used to give help to endangered units
                        Dictionary<Tile, float> influenceDict = objUnit.gM.influenceDict[objUnit.getTeam()];
                        //Debug.Log(objUnit + " is in influence of " + influenceDict[objUnit.getTile()] + " with maxMod" + maxMod);
                        maxMod *= distance-influenceDict[objUnit.getTile()];
                        maxMod *= 10;
                        if (StatsManager.executeCommandsLast.Contains(optimalAbility))
                        {
                            maxMod /= 100;
                        }
                        shouldTakeIntoAccountDistance = false;
                        //Debug.Log(objUnit + " now has a maxMod of " + maxMod);
                        //if (optimalAbility != "")
                           // Debug.Log("Mod: Assigning" + aiTask.taskType + " of subtype " + taskSubType + " to " + possibleTaskDoer);
                    }
                }
                ptdUnit.taskSubType = "";
                // Debug.Log(taskSubType);
                return maxMod;
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

    /*  Determines how well an attacker does against a defender depending on the attack mode - attack, sentry, or guarding
        Algorithm: 
        1) Get all sets of weapons that can be used to attack
        2) Go through all sets of weapons that can be used to attack
        3) Find the list of tiles that the unit can be at and fire at the attacker
            4) Store the damage done to the defender
            5) Determine if the defender can attack the attacker at the current tile
            6) Use influenceMap/Distance to determine how well the attacker did
        7) Rank the sets of weapons that were the most effective


    */
    
    // Quick attack advantage determiner for when the unit can't directly attack so time isn't wasted
    public float determineQuickAA(Unit attacker, Unit defender, bool sentry, bool guard, bool flipped)
    {
        float attackerDamage = 0;
        float defenderHP = defender.getCurrentHP();
        List<Weapon> attackerWeapons = attacker.getAllDamageActiveWeapons();
        if (!sentry)
        {
            foreach(Weapon weapon in attackerWeapons)
            {
                attackerDamage += weapon.getHypotheticalDamagePerAttack(attacker, defender) * weapon.maxAttacksPerTurn;
            }
            defenderHP -=  attackerDamage;
            if (defenderHP < 0) return killedBonus;
        }
        float defenderDamage = 0;
        List<Weapon> defenderWeapons = defender.getAllDamageActiveWeapons();

        foreach (Weapon weapon in defenderWeapons)
        {
            defenderDamage += (weapon.getHypotheticalDamagePerAttack(defender, attacker) * weapon.maxAttacksPerTurn) * (defender.getCurrentHP() - attackerDamage)/defender.getHP();
        }
        float attackerHP = attacker.getCurrentHP() - defenderDamage;
        if (attackerHP < 0) return killedBonus;
        
        if (sentry)
        {
            foreach (Weapon weapon in attackerWeapons)
            {
                attackerDamage += weapon.getHypotheticalDamagePerAttack(attacker, defender) * weapon.maxAttacksPerTurn * attackerHP/attacker.getHP();
            }
            defenderHP -= attackerDamage;
            if (defenderHP < 0) return killedBonus;
        }
        float attackerRatio = defenderHP / defender.getHP();
        float defenderRatio = attackerHP / attacker.getHP();
        float difference = attackerRatio - defenderRatio;
        return difference * killedBonus;

    }

    // FIXME: TOO SLOW NEEDS TO BE REDONE
    public float determineAttackAdvantage(Unit attacker, Unit defender, bool sentry, bool guard, bool flipped)
    {
        float advantage = 0;
        bool doDebug = false;
        //if (defender.getName().Equals("Asher") || attacker.getName().Equals("Asher")) doDebug = true;

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
        /*if (!flipped)
        {
            if (attackerirHSPaths != null)
            {
                irHSPaths = attackerirHSPaths;
            } 
            else
            {
                attackerirHSPaths = irHSPaths;
            }
        }
        else
        {
            if (defenderirHSPaths != null)
            {
                irHSPaths = defenderirHSPaths;
            }
            else
            {
                defenderirHSPaths = irHSPaths;
            }
        }*/
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
                dCalc.reset();
                if (buildingUnit)
                {
                    dCalc.setValues(attacker.getTile() != null ? attacker.getTile() : buildingTile, defender.getTile() == null ? buildingTile : defender.getTile());
                    tempPath = dCalc.findInRangePath(attacker, new List<Weapon>() { weapon }, true, true);
                    if (tempPath != null)
                    {
                        addDamage = true;
                    }
                }
                else if (!buildingUnit) 
                {
                    dCalc.setValues(attacker.getTile(), defender.getTile());
                    tempPath = dCalc.findInRangePath(attacker, new List<Weapon>() { weapon }, true, true);
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
        Dictionary<Weapon, Tile> firingDict = new Dictionary<Weapon, Tile>();
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
                    float attackDamageThisTime = weapon.getHypotheticalDamagePerAttack(attacker, defender) * attackerHealth / attacker.getHP() / attacker.getHPRatio();
                    // We also need to handle adjacent tiles if the weapon is an AOE weapon
                    // We only care though if the unit is attacking, and not sentrying or guarding
                    if (weapon.aoe > 0 && !sentry && !flipped)
                    {
                        float maxAttackDamage = attackDamageThisTime;
                        Tile maxAttackTile = defender.getTile();
                        // First go through all aoe tiles
                        Tile currentTile;
                        HashSet<Tile> explored = new HashSet<Tile>();
                        SimplePriorityQueue<Tile, int> frontier = new SimplePriorityQueue<Tile, int>();
                        SimpleNode<Tile, int> currentNode, tempNode, tempNode2;
                        List<Tile> adjacentTiles = defender.getTile().getAdjacent(), aoeTiles;
                        if (doDebug)
                        {
                            Debug.Log("Planning to AOE Attack " + defender);
                        }
                        foreach (Tile t in adjacentTiles)
                        {
                            if (t != null) 
                            frontier.Enqueue(t, 1);
                        }
                        while (frontier.Count > 0)
                        {
                            float tempDamage = 0;
                            currentNode = frontier.DequeueNode();
                            currentTile = currentNode.Data;
                            explored.Add(currentTile);
                            dCalc.reset();
                            dCalc.setValues(attacker.getTile() != null ? attacker.getTile() : buildingTile, currentTile);

                            aoeTiles = dCalc.getPresetAOETiles(weapon, currentTile, true);
                            Tile attackerTile = (buildingUnit) ? buildingTile : attacker.getTile();
                            if (aoeTiles != null)
                            {
                                foreach (Tile aoeT in aoeTiles)
                                {
                                    if (aoeT.getUnitScript() == null) continue;
                                    Unit aoeTUnit = aoeT.getUnitScript();
                                    tempDamage += weapon.getHypotheticalDamagePerAttack(attacker, aoeTUnit) * attackerHealth / attacker.getHP() / attacker.getHPRatio();
                                }
                            }

                            // See if the tempDamage is higher than maxDamage or if the tempDamage is equal, but is closer to the attacker
                            // if so, then it becomes the firing tile of the weapon
                            dCalc.reset();
                            dCalc.setValues(currentTile, attackerTile);
                            float currentDist = dCalc.getAbsoluteDist();
                            dCalc.reset();
                            dCalc.setValues(maxAttackTile, attackerTile);
                            float bestDist = dCalc.getAbsoluteDist();
                            if (tempDamage > maxAttackDamage || (Mathf.Approximately(tempDamage, maxAttackDamage) && currentDist < bestDist)) {
                                maxAttackDamage = tempDamage;
                                if (doDebug)
                                Debug.Log("More efficient for " + attacker + " to attack " + currentTile + " instead of " + maxAttackTile + " for attacking "+ defender);
                                maxAttackTile = currentTile;

                            }

                            adjacentTiles = currentTile.getAdjacent();
                            foreach (Tile tile in adjacentTiles)
                            {
                                //Debug.Log(tile);
                                if (tile != null)
                                {
                                    tempNode = new SimpleNode<Tile, int>(tile);
                                    tempNode.Priority = currentNode.Priority + 1;
                                    //Debug.Log(temp.Priority);
                                    if (tempNode.Priority > weapon.aoe) continue;
                                    //Debug.Log("Test 2");
                                    if (!explored.Contains(tile) && !frontier.Contains(tile))
                                    {
                                        //Add the node to the frontier if we didn't explore it already
                                        frontier.Enqueue(tile, currentNode.Priority + 1);
                                        tile.predecessor = currentNode.Data;
                                        //Debug.Log("Enqueing");
                                    }
                                    //If we have a move path that is shorter than what is in the frontier, replace it
                                    else if (frontier.Contains(tile))
                                    {
                                        tempNode2 = frontier.RemoveNode(tile);
                                        if (tempNode.Priority < tempNode2.Priority)
                                        {
                                            frontier.Enqueue(tempNode.Data, tempNode.Priority);
                                            tile.predecessor = currentTile;
                                        }
                                        else
                                        {
                                            frontier.Enqueue(tempNode2.Data, tempNode2.Priority);
                                        }

                                    }
                                }
                            }
                        }
                        firingDict.Add(weapon, maxAttackTile);
                        int type = 0;
                        firingDictionary.Add(type,firingDict);
                        attackerDamage += 1;
                        attackerDamage += (maxAttackDamage * weapon.maxAttacksPerTurn);
                        if (doDebug)
                        {
                            Debug.Log("Doing "+attackerDamage+" damage");
                        }
                    } else
                    {
                        attackerDamage += attackDamageThisTime * weapon.maxAttacksPerTurn;
                    }

                    // Also add attack damage if weapon does DoT
                    if (weapon.extraAttributes != null && weapon.extraAttributes.ContainsKey("Makes Poison Gas") && defender.getPoisonHPEffect() > 0 && defender.extraAttributes != null && !defender.extraAttributes.ContainsKey("Poisoned"))
                    {
                        attackerDamage += defender.getPoisonHPEffect() * weapon.extraAttributes["Makes Poison Gas"];
                    }
                    else if (weapon.extraAttributes != null && weapon.extraAttributes.ContainsKey("Poisons") && defender.getPoisonHPEffect() > 0 && defender.extraAttributes != null && !defender.extraAttributes.ContainsKey("Poisoned"))
                    {
                        attackerDamage += defender.getPoisonHPEffect() * weapon.extraAttributes["Poisons"];
                    }
                 
                }
            }

            if (doDebug)
            {
                Debug.Log(attackerDamage);
            }
            defenderHealth -= attackerDamage;
            if (defenderHealth < 0) return killedBonus;
            defenderDamage = 0;

            List<Tile> path = irHSPaths[weaponSet];
            Tile firingTile = path[path.Count - 1];
            foreach (Weapon weapon in weaponSet)
            {
                if (firingDict.ContainsKey(weapon))
                {
                    dCalc.reset();
                    if (buildingUnit)
                    {

                        dCalc.setValues(attacker.getTile() != null ? attacker.getTile() : buildingTile, defender.getTile() == null ? buildingTile : firingDict[weapon]);
                        tempPath = dCalc.findInRangePath(attacker, new List<Weapon>() { weapon }, true, true);
                        if (tempPath != null)
                        {
                            addDamage = true;
                        }
                    }
                    else if (!buildingUnit)
                    {
                        dCalc.setValues(attacker.getTile(), firingDict[weapon]);
                        tempPath = dCalc.findInRangePath(attacker, new List<Weapon>() { weapon }, true, true);
                        if (tempPath != null)
                        {
                            addDamage = true;
                        }
                    }
                    if (tempPath != null)
                    firingTile = tempPath[tempPath.Count - 1];
                    break;
                }
            }
            if (firingTile == path[path.Count - 1])
            {
                foreach (Weapon weapon in defender.getAllActiveWeapons())
                {
                    /*if (gM.getInRangePathAH(defender, (defender.getTile() != null) ? defender.getTile() : buildingTile, firingTile, new List<Weapon>() { weapon }, true, false) != null)
                    {
                        defenderDamage += weapon.getHypotheticalDamagePerAttack(defender, attacker) * defenderHealth / defender.getHP() / defender.getHPRatio();
                    }*/
                    dCalc.reset();
                    dCalc.setValues((defender.getTile() != null) ? defender.getTile() : buildingTile, firingTile);
                    if (dCalc.canAttackEnemyExactlyWithWeapons(defender, new List<Weapon>() { weapon }, attacker))
                    {
                        /*if (!buildingUnit)
                        {
                            Debug.Log(weapon + " can fire on " + firingTile + " from " + defender);
                        }*/
                        defenderDamage += weapon.getHypotheticalDamagePerAttack(defender, attacker) * defenderHealth / defender.getHP() / defender.getHPRatio();
                    }
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
                        if (weapon.extraAttributes != null && weapon.extraAttributes.ContainsKey("Makes Poison Gas") && defender.getPoisonHPEffect() > 0 && defender.extraAttributes != null && !defender.extraAttributes.ContainsKey("Poisoned"))
                        {
                            attackerDamage += defender.getPoisonHPEffect() * weapon.extraAttributes["Makes Poison Gas"];
                        }
                        else if (weapon.extraAttributes != null && weapon.extraAttributes.ContainsKey("Poisons") && defender.getPoisonHPEffect() > 0 && defender.extraAttributes != null && !defender.extraAttributes.ContainsKey("Poisoned"))
                        {
                            attackerDamage += defender.getPoisonHPEffect() * weapon.extraAttributes["Poisons"];
                        }
                    }
                }
            }
        }
        //If they didn't kill each other, get the respective damage ratios
        float attackerRatio = attackerHealth / attacker.getCurrentHP();
        float defenderRatio = defenderHealth / defender.getCurrentHP();
        float difference = attackerRatio - defenderRatio;
        if (doDebug)
        {
            Debug.Log("Attacker " + attacker + " has attacker ratio of " + attackerRatio + " and defender " + defender + " has defense ratio of " + defenderRatio);
            Debug.Log("This is for sentry " + sentry + " and guard " + guard);
        }
        advantage = difference * killedBonus;
        //Debug.Log(advantage);
        return advantage;
    }

    public void setTaskSubType(string subType)
    {
        taskSubType = subType;
    }

    //Assign the task if we have the highest priority
    public bool assign()
    {
        lock (possibleTaskDoer)
        {
            if (possibleTaskDoer.assignedTask == null)
            {
                //Debug.Log("Assigning " + aiTask.taskType + " of subtype " + taskSubType+" to " + possibleTaskDoer);
                aiTask.assign(this, possibleTaskDoer);
                aiTask.taskDoer.orderedWeapons = actualOrderedWeapons;
                // Debug.Log(taskSubType+ " with optimal ability "+optimalAbility);
                possibleTaskDoer.optimalAbility = optimalAbility;
                possibleTaskDoer.taskSubType = taskSubType;
                possibleTaskDoer.firingDictionary = firingDictionary;
                if ((optimalAbility.Length > 1) && (taskSubType == "" || taskSubType == null))
                {
                    Debug.LogError("Should have a subtask if optimal ability highlighted: " + taskSubType2);
                }
                Debug.Log(possibleTaskDoer + " assigned to task (" + aiTask.taskType + ")");
                return true;
            }
            //Debug.Log(possibleTaskDoer + " already has a task (" + aiTask.taskType + ")");
            return false;
        }

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
