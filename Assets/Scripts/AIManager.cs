using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    //Dictionary to contain all units
    public Dictionary<string, List<Unit>> unitDictionary;
    public List<Unit> unitsUsed;
    public GameManager gM;
    //Dictionary to tell which AI has what difficulty
    public Dictionary<string, string> aiDifficulties;
    public Dictionary<Player, UtilityAI> ais = new Dictionary<Player, UtilityAI>();
    public UtilityAI currentUtilityAI;
    public bool needToRestart = false;
    int limitedActions = 1000000, actions = 0;
    bool limitActions = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator performUtilityTask(PossibleAssignment order, UtilityAI aiM)
    {
        //Debug.Log("PUT!");
        
        yield return StartCoroutine(order.possibleTaskDoer.performTask(this, aiM));
    }

    public List<PossibleAssignment> getUtilityOrders(UtilityAI ai)
    {
        return ai.createAssignments();
    }


    //Method for handling loop requests
    public IEnumerator loopUtilityRequest(string side, UtilityAI ai)
    {
        actions++;
        //Debug.Log("Looping");
        if ((limitActions && actions <= limitedActions) || !limitActions)
        {
            needToRestart = false;
            List<PossibleAssignment> orders = getUtilityOrders(ai);

            foreach (PossibleAssignment order in orders)
            {
                //yield return new WaitForSeconds(1f);
                //Debug.Log("Out of "+orders.Count+" orders, performing "+order.aiTask.taskType);
                yield return StartCoroutine(performUtilityTask(order, ai));
                if (needToRestart)
                {
                    break;
                }

            }

            if (needToRestart)
            {
                yield return StartCoroutine(loopUtilityRequest(side, ai));
            }
        }
    }

    public IEnumerator requestAIAction(string side)
    {
        actions = 0;
        //bool finished = true;
        //Prevent while loops from going in infinite loops
        Player aiPlayer = gM.playerDictionary[side];
        //printUnitDictionary();
        Debug.Log("Doing action!");
        switch (aiDifficulties[side])
        {
            case "Utility AI":
                actions++;
                if (limitedActions >= actions)
                {
                        if (!ais.ContainsKey(aiPlayer))
                    {
                        UtilityAI tempAI = new UtilityAI(this, gM, aiPlayer, aiPlayer.side, gM.getTeam(side));
                        ais.Add(aiPlayer, tempAI);
                    }
                    UtilityAI currentAI = ais[aiPlayer];
                    currentUtilityAI = currentAI;
                    List<PossibleAssignment> orders = getUtilityOrders(currentAI);

                    foreach (PossibleAssignment order in orders)
                    {
                        //yield return new WaitForSeconds(1f);
                        yield return StartCoroutine(performUtilityTask(order, currentAI));
                        if (needToRestart)
                        {
                        break;
                        }

                    }

                    if (needToRestart)
                    {
                        yield return StartCoroutine(loopUtilityRequest(side, currentAI));
                    }
                }
                gM.endTurn();
                break;


            //Idea for Armor Aware AI:
            //Get all units that can attack this turn and have them attack
            //However, priortorize units that deal more damage based off on the enemy armor type
            //For all other units, have them move towards the enemy
            case "Armor Aware":
                //break;
                List<Unit> units = new List<Unit>(unitDictionary[side]);
                foreach (Unit unit in units)
                {
                    actions = 0;
                    if (!unitsUsed.Contains(unit))
                    {
                        List<Tile> attackables = gM.getAttackbleUnits(unit, unit.getTile());
                        //printUnitListFromTile(attackables);
                        //Search if this unit can attack something to get a damage multiplier of 2 (Armor and Weapon the same), than 1 (Armor or Weapon are Medium), 
                        //than 0.5 (Armor and Weapon are not the same and both are not Medium)
                        if (attackables != null && attackables.Count > 0)
                        {
                            
                            bool attacked = false;
                            //bool checkedInRange = false;
                            int i = 0;
                            while (!attacked && actions <= limitedActions)
                            {
                                actions++;
                                //Debug.Log("Running loop");
 
                                    
                                    foreach (Tile t in attackables)
                                    {
                                        //Debug.Log("Searching with i = " + i);
                                        Unit u = t.getUnitScript();
                                        if (u.getArmor() == unit.getCurrentWeapon().weaponType)
                                        {
                                            yield return StartCoroutine(beginAttackSequence(unit, unit.getTile(), u, u.getTile(), unit.getAllDamageHandWeapons()));
                                            attacked = true;
                                            break;
                                        }
                                        else if ((unit.getCurrentWeapon().weaponType == "Medium" || u.getArmor() == "Medium") && i == 1)
                                        {
                                            yield return StartCoroutine(beginAttackSequence(unit, unit.getTile(), u, u.getTile(), unit.getAllDamageHandWeapons()));
                                            attacked = true;
                                            break;
                                        }
                                        else if (i == 2)
                                        {
                                            yield return StartCoroutine(beginAttackSequence(unit, unit.getTile(), u, u.getTile(), unit.getAllDamageHandWeapons()));
                                            attacked = true;
                                            break;
                                        }
                                    }
                                    i++;
                                    //Debug.Log("i is "+i);
                                    if (i == 3)
                                    {
                                        //checkedInRange = true;
                                        //Debug.LogError("We didn't attack!");
                                        //return false;
                                    }
                                
                            }
                            //If unit attacked, than we are finished attacking, must check other units
                            unitsUsed.Add(unit);
                            //Debug.Log(unitsUsed);
                            //return false;
                        }
                        else
                        {
                           // Debug.Log("We can't attack the enemy!");
                            bool moved = false;
                            //return false;
                            //No attackable units, just move!
                            List<string> keys = new List<string>(unitDictionary.Keys);
                            keys.Remove(side);
                            int i = 0;
                            while (!moved && actions <= limitedActions)
                            {
                                actions++;
                                foreach (string key in keys)
                                {
                                    
                                    foreach (Unit u in unitDictionary[key])
                                    {
                                        if (u.getArmor() == unit.getCurrentWeapon().weaponType)
                                        {
                                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), u.getTile(), true));
                                            moved = true;
                                            break;
                                        }
                                        else if ((unit.getCurrentWeapon().weaponType == "Medium" || u.getArmor() == "Medium") && i == 1)
                                        {
                                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), u.getTile(), true));
                                            moved = true;
                                            break;
                                        }
                                        else if (i == 2)
                                        {
                                            yield return StartCoroutine(gM.moveAsFarAsPossible(unit, unit.getTile(), u.getTile(), true));
                                            moved = true;
                                            break;
                                        }
                                    }
                                    if (moved == true) break;
                                }
                                i++;
                                if (i == 3) moved = true;
                            }
                            unitsUsed.Add(unit);
                            //return false;
                        }

                    }
                }
                gM.endTurn();
                break;
        }
        //return true;
    }


    //Attack an enemy unit
    public IEnumerator beginAttackSequence(Unit attacker, Tile attackTile, Unit defender, Tile defendTile, List<Weapon> weapons)
    {
        //First determine if we can begin attacking right now
        //Debug.Log(attacker);
        if (gM.canAttackEnemy(attacker, attackTile, defender, defendTile))
        {
            
            yield return StartCoroutine(gM.attackEnemy(attacker, defender, weapons));
        }
        else
        {

            //Debug.Log("Attacker is "+ attacker);
            //Debug.Log("Attack tile is " + attackTile.getPos());
            //Debug.Log("Defend tile is " + defendTile.getPos());
            //If we can't attack enemy, move to an adjacent tile and begin attack
            yield return StartCoroutine(gM.moveInRangeAttack(attacker, attackTile, defender, defendTile, true,weapons));
            //gM.moveInRange(attacker, attackTile, defendTile);
            //gM.attackEnemy(attacker, defender);
        }
    }

    //Gets the enemy armor types of all enemy units in a dictionary
    public Dictionary<string, Dictionary<string, List<Unit>>> getEnemyArmorTypes(string AIside)
    {
        //first string indicates the side of the unit, 2nd string indicates the armor type, and the list of units refers to the units that have that armor type.
        Dictionary<string, Dictionary<string, List<Unit>>> enemyArmors = new Dictionary<string, Dictionary<string, List<Unit>>>();
        List<string> keys = new List<string>(unitDictionary.Keys);
        foreach (string key in keys)
        {
            if (key != AIside)
            {
                Dictionary<string, List<Unit>> tempArmorsDict = new Dictionary<string, List<Unit>>();
                List<Unit> lightArmors = new List<Unit>();
                List<Unit> mediumArmors = new List<Unit>();
                List<Unit> heavyArmors = new List<Unit>();
                //Loop through each unit in the unit dictionary and add the key
                foreach (Unit unit in unitDictionary[key])
                {
                    switch (unit.getArmor())
                    {
                        case "Light":
                            lightArmors.Add(unit);
                            break;
                        case "Medium":
                            mediumArmors.Add(unit);
                            break;
                        case "Heavy":
                            heavyArmors.Add(unit);
                            break;
                    }
                }
                tempArmorsDict.Add("Light", lightArmors);
                tempArmorsDict.Add("Medium", mediumArmors);
                tempArmorsDict.Add("Heavy", heavyArmors);
                enemyArmors.Add(key, tempArmorsDict);
                
            }
        }
        return enemyArmors;
    }

    //Gets the enemy weapon types of all enemy units in a dictionary
    public Dictionary<string, Dictionary<string, List<Unit>>> getEnemyWeaponTypes(string AIside)
    {
        //first string indicates the side of the unit, 2nd string indicates the weapon type, and the list of units refers to the units that have that weapon type.
        Dictionary<string, Dictionary<string, List<Unit>>> enemyWeapons = new Dictionary<string, Dictionary<string, List<Unit>>>();
        List<string> keys = new List<string>(unitDictionary.Keys);
        foreach (string key in keys)
        {
            if (key != AIside)
            {
                Dictionary<string, List<Unit>> tempWeaponsDict = new Dictionary<string, List<Unit>>();
                List<Unit> lightWeapons = new List<Unit>();
                List<Unit> mediumWeapons = new List<Unit>();
                List<Unit> heavyWeapons = new List<Unit>();
                //Loop through each unit in the unit dictionary and add the key
                foreach (Unit unit in unitDictionary[key])
                {
                    switch (unit.getCurrentWeapon().weaponType)
                    {
                        case "Light":
                            lightWeapons.Add(unit);
                            break;
                        case "Medium":
                            mediumWeapons.Add(unit);
                            break;
                        case "Heavy":
                            heavyWeapons.Add(unit);
                            break;
                    }
                }
                tempWeaponsDict.Add("Light", lightWeapons);
                tempWeaponsDict.Add("Medium", mediumWeapons);
                tempWeaponsDict.Add("Heavy", heavyWeapons);
                enemyWeapons.Add(key, tempWeaponsDict);

            }
        }
        return enemyWeapons;
    }

    //Get all the weapon types of the AI units
    public Dictionary<string, List<Unit>> getAIWeapons(string AIside)
    {
        Dictionary<string, List<Unit>> aiWeapons = new Dictionary<string, List<Unit>>();
        List<Unit> lightWeapons = new List<Unit>();
        List<Unit> mediumWeapons = new List<Unit>();
        List<Unit> heavyWeapons = new List<Unit>();
        foreach (Unit unit in unitDictionary[AIside])
        {
            if (!unitsUsed.Contains(unit))
            {
                switch (unit.getCurrentWeapon().weaponType)
                {
                    case "Light":
                        lightWeapons.Add(unit);
                        break;
                    case "Medium":
                        mediumWeapons.Add(unit);
                        break;
                    case "Heavy":
                        heavyWeapons.Add(unit);
                        break;
                }
            }
        }
        aiWeapons.Add("Light", lightWeapons);
        aiWeapons.Add("Medium", mediumWeapons);
        aiWeapons.Add("Heavy", heavyWeapons);
        return aiWeapons;
    }

    //Get unit dictionary from game manager
    public void getUnitsFromGM()
    {
        unitDictionary = gM.unitDictionary;
    }

    public void resetUsedUnits()
    {
        unitsUsed = new List<Unit>();
    }

    //Reset our AI dictionary
    public void resetAIs()
    {
        aiDifficulties = new Dictionary<string, string>();
    }

    //Add an AI to the ai dictionary
    public void addAI(string side, string difficulty)
    {
        aiDifficulties.Add(side, difficulty);
    }

    public void printUnitDictionary()
    {
        String toPrint = "Printing Unit Dictionary: \n";
        foreach (string side in unitDictionary.Keys)
        {
            toPrint += "{Side = " + side+",";
            foreach (Unit unit in unitDictionary[side])
            {
                toPrint += " Unit:" + unit+",";
            }
            toPrint = toPrint.Substring(0, toPrint.Length - 1);
            toPrint += "}\n";
        }
        Debug.Log(toPrint);
    }

    public void printUnitList(List<Unit> list)
    {
        String toPrint = "Printing a unit list: \n";
        foreach (Unit unit in list)
        {
            toPrint += unit + ",";
        }
        toPrint = toPrint.Substring(0, toPrint.Length - 1);
        toPrint += "\n";
        Debug.Log(toPrint);
    }

    public void printUnitListFromTile(List<Tile> list)
    {
        String toPrint = "Printing a unit list: \n";
        if (list == null) {
            Debug.Log("There are no units in the list!");
            return;
        }
        foreach (Tile tile in list)
        {
            Unit unit = tile.getUnitScript();
            toPrint += unit + ",";
        }
        toPrint = toPrint.Substring(0, toPrint.Length - 1);
        toPrint += "\n";
        Debug.Log(toPrint);
    }
}
