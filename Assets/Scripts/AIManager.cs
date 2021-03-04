using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using System.Threading;

// This handles running AIs after being called from the GameManager
// This class is needed because AI classes themselves are not GameObjects, so they're unable to use coroutines
public class AIManager : MonoBehaviour
{
    //Dictionary to contain all units
    public Dictionary<string, List<Unit>> unitDictionary;
    public List<Unit> unitsUsed;
    public List<Task> tasksToWaitFor = new List<Task>();
    [SerializeField]
    public List<Thread> threadsToWaitFor = new List<Thread>();
    public int patcCompletedCount = 0;
    public List<PAThreadedChecker> paTCs = new List<PAThreadedChecker>();
    public GameManager gM;
    public string currentSide = "";
    //Dictionary to tell which AI has what difficulty
    public Dictionary<string, string> aiDifficulties;
    public Dictionary<Player, UtilityAI> ais = new Dictionary<Player, UtilityAI>();
    public UtilityAI currentUtilityAI;
    public IEnumerator currentCoroutine;
    public IEnumerator taskCoroutine;
    public IEnumerator currentLoopRequest;
    public AITask currentTask;
    public Controllable currentTaskDoer;
    public bool needToRestart = false;
    int limitedActions = 100, actions = 0;
    public float limitedActionTime = 10, actionTime = 0;
    bool limitActions = true;
    bool limitActionTime = true;
    bool completedAction = false;
    public WaitForSeconds shortWaitTime = new WaitForSeconds(0.1f);
    // 0 is not started, 1 is non-unit makers, 2 is unit Makers
    public int currentMode = 0;
    public int threadsFinished = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initGetAssignmentScore(List<PossibleAssignment> assignments, AITask aiTask, Controllable asset, bool bU, Tile bT)
    {

        //Task tempTask = new Task(getAssignmentScore(assignments, aiTask, asset, bU, bT));
        //Thread tempThread = new Thread(getAssignmentScore);
        //tasksToWaitFor.Add(tempTask);
        PAThreadedChecker paTC = new PAThreadedChecker(assignments, this, gM, aiTask, asset, bU, bT);
        Thread tempThread = new Thread(paTC.checkPA);
        tempThread.IsBackground = true;
        tempThread.Start();
        paTCs.Add(paTC);
        threadsToWaitFor.Add(tempThread);
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

    public IEnumerator getAssignmentScore(List<PossibleAssignment> assignments, AITask aiTask, Controllable asset, bool bU, Tile bT)
    {
        
        float duration = Time.realtimeSinceStartup;
        TPReturnData tprd = asset.isTaskSuitable(gM, aiTask, bU, bT);
        /*if (asset.isBuilding)
        {
            Building building = (Building)asset;
            if (building.makesUnits)
            {
                Debug.Log("building on " + building.tile + " is allowed to make units? " + checkForSuitability(tprd));
            }
        }*/
        if (checkForSuitability(tprd))
        {
            PossibleAssignment assignment = new PossibleAssignment(gM, aiTask, asset, bU, bT);
            assignment.tprd = tprd;
            assignment.initGetScore();
            while (!assignment.scoreCalculated)
            {
                yield return shortWaitTime;
            }
            assignments.Add(assignment);
            /*if (asset.isBuilding)
            {
                Building building = (Building)asset;
                if (building.makesUnits)
                {
                    Debug.Log(assignment);
                    Debug.Log(assignments.Count);
                }
            }*/
        }
        duration = Time.realtimeSinceStartup - duration;
        if (duration > 0.5f)
        {
            Debug.Log("Notice: The following task took over 1 second: Task of " +aiTask.taskType +" for objective "+aiTask.objective +" given asset "+ asset);
        }
        yield break;
    }

    public IEnumerator performUtilityTask(PossibleAssignment order, UtilityAI aiM)
    {
        //Debug.Log("PUT!");
        taskCoroutine = order.possibleTaskDoer.performTask(this, aiM);
        yield return StartCoroutine(taskCoroutine);
    }

    public List<PossibleAssignment> getUtilityOrders(UtilityAI ai)
    {
        return ai.createAssignments();
    }

    public List<PossibleAssignment> getUtilityAssignments(UtilityAI ai)
    {
        return ai.getPossibleAssignments();
    }

    // When unit makers are needed to be done
    public List<PossibleAssignment> getUnitMakerOrders(UtilityAI ai)
    {
        return ai.createAssignmentsFromCustomList(ai.getPossibleAssignmentsFromUnitMakers(ai.generateUnitMakerTasks(), ai.generateAllUnitMakers()));
    }

    public List<PossibleAssignment> getUnitMakerAssignments(UtilityAI ai)
    {
        return ai.getPossibleAssignmentsFromUnitMakers(ai.generateUnitMakerTasks(), ai.generateAllUnitMakers());
    }

    public List<PossibleAssignment> getCustomOrders(UtilityAI ai, List<PossibleAssignment> assignments)
    {
        float duration = Time.realtimeSinceStartup;
        List<PossibleAssignment> orders = ai.createAssignmentsFromCustomList(assignments);
        duration = Time.realtimeSinceStartup - duration;
        Debug.Log("It took " + duration + "s to get all orders!");
        return orders;
    }


    //Method for handling loop requests
    public IEnumerator loopUtilityRequest(string side, UtilityAI ai)
    {
        actions++;
        tasksToWaitFor = new List<Task>();
        //Debug.Log("Looping");
        if ((limitActions && actions <= limitedActions) || !limitActions)
        {
            needToRestart = false;
            List<PossibleAssignment> assignments = getUtilityAssignments(ai);
            yield return StartCoroutine(waitForThreads(threadsToWaitFor));
            List<PossibleAssignment> orders = getCustomOrders(ai, assignments);
            if (orders == null || orders.Count == 0) yield break;
            foreach (PossibleAssignment order in orders)
            {
                //yield return new WaitForSeconds(1f);
                Debug.Log("Out of "+orders.Count+" orders, performing "+order.aiTask.taskType);
                currentTaskDoer = order.possibleTaskDoer;
                currentCoroutine = performUtilityTask(order, ai);
                completedAction = false;
                StartCoroutine(clockActionTime());
                yield return StartCoroutine(currentCoroutine);
                StopCoroutine(clockActionTime());
                completedAction = true;
                if (needToRestart)
                {
                    break;
                }

            }

            if (needToRestart)
            {
                currentLoopRequest = loopUtilityRequest(side, ai);
                yield return StartCoroutine(currentLoopRequest);
            }
        }
    }

    public IEnumerator loopUnitMakerRequest(string side, UtilityAI ai)
    {
        actions++;
        tasksToWaitFor = new List<Task>();
        //Debug.Log("Looping");
        if ((limitActions && actions <= limitedActions) || !limitActions)
        {
            needToRestart = false;
            List<PossibleAssignment> assignments = getUnitMakerAssignments(ai);
            yield return StartCoroutine(waitForThreads(threadsToWaitFor));
            List<PossibleAssignment> orders = getCustomOrders(ai, assignments);
            if (orders == null || orders.Count == 0) yield break;
            foreach (PossibleAssignment order in orders)
            {
                //yield return new WaitForSeconds(1f);
                Debug.Log("Out of "+orders.Count+" orders, performing "+order.aiTask.taskType);
                currentTaskDoer = order.possibleTaskDoer;
                currentCoroutine = performUtilityTask(order, ai);
                completedAction = false;
                StartCoroutine(clockActionTime());
                yield return StartCoroutine(currentCoroutine);
                StopCoroutine(clockActionTime());
                completedAction = true;
                if (needToRestart)
                {
                    break;
                }

            }

            if (needToRestart)
            {
                currentLoopRequest = loopUnitMakerRequest(side, ai);
                yield return StartCoroutine(currentLoopRequest);
            }
        }
    }

    public void stopCurrentCoroutine()
    {
        StopCoroutine(currentCoroutine);
        StartCoroutine(requestAIAction(currentSide));
    }

    public IEnumerator clockActionTime()
    {
        actionTime = 0;
        yield return new WaitForSeconds(limitedActionTime);
        if (!completedAction)
        {
            StopCoroutine(currentCoroutine);
            StopCoroutine(taskCoroutine);
            Debug.Log("Exceeded allotted action time!");
            StartCoroutine(requestAIAction(currentSide));
        }
    }

    public IEnumerator requestAIAction(string side)
    {
        actions = 0;
        actionTime = 0;
        tasksToWaitFor = new List<Task>();
        if (currentMode == 0)
        {
            currentMode = 1;
        };
        //bool finished = true;
        //Prevent while loops from going in infinite loops
        Player aiPlayer = gM.playerDictionary[side];
        currentSide = side;
        //printUnitDictionary();
        Debug.Log("Doing action!");
        switch (aiDifficulties[side])
        {
            case "Utility AI":
                actions++;
                if (limitedActions >= actions)
                {
                    float duration = 0;
                    if (!ais.ContainsKey(aiPlayer))
                    {
                        UtilityAI tempAI = new UtilityAI(this, gM, aiPlayer, aiPlayer.side, gM.getTeam(side));
                        ais.Add(aiPlayer, tempAI);
                    }
                    UtilityAI currentAI = ais[aiPlayer];
                    currentUtilityAI = currentAI;
                    // First do all non unit makers
                    if (currentMode == 1)
                    {
                        //duration = Time.realtimeSinceStartup;
                        List<PossibleAssignment> assignments = getUtilityAssignments(currentAI);
                        //duration = Time.realtimeSinceStartup - duration;
                        //Debug.Log("Initializing took " + duration + " seconds");
                        yield return StartCoroutine(waitForThreads(threadsToWaitFor));
                        //Debug.Log("Finished getting assignments!");
                        List<PossibleAssignment> orders = getCustomOrders(currentAI, assignments);

                        if (orders == null || orders.Count == 0)
                        {
                            gM.endTurn();
                            yield break;
                        }
  
                        foreach (PossibleAssignment order in orders)
                        {
                            //yield return new WaitForSeconds(1f);
                            currentTaskDoer = order.possibleTaskDoer;
                            currentCoroutine = performUtilityTask(order, currentAI);
                            completedAction = false;
                            StartCoroutine(clockActionTime());
                            yield return StartCoroutine(currentCoroutine);
                            StopCoroutine(clockActionTime());
                            completedAction = true;
                            if (needToRestart)
                            {
                                break;
                            }

                        }

                        if (needToRestart)
                        {
                            currentLoopRequest = loopUtilityRequest(side, currentAI);
                            yield return StartCoroutine(currentLoopRequest);
                        }
                        currentMode = 2;
                    }
                    threadsToWaitFor = new List<Thread>();
                    paTCs = new List<PAThreadedChecker>();
                    patcCompletedCount = 0;
                    // Once all non - unit makers are done, begin unit making process
                    if (currentMode == 2)
                    {
                        List<PossibleAssignment> assignments = getUnitMakerAssignments(currentAI);
                        yield return StartCoroutine(waitForThreads(threadsToWaitFor));
                        //Debug.Log("Unit makers have " + assignments.Count + " assignments");
                        List<PossibleAssignment> orders = getCustomOrders(currentAI, assignments);
                        if (orders == null || orders.Count == 0)
                        {
                            Debug.Log("No unit makers available");
                            gM.endTurn();
                            yield break;
                        }
                        foreach (PossibleAssignment order in orders)
                        {
                            //yield return new WaitForSeconds(1f);
                            currentTaskDoer = order.possibleTaskDoer;
                            currentCoroutine = performUtilityTask(order, currentAI);
                            completedAction = false;
                            StartCoroutine(clockActionTime());
                            yield return StartCoroutine(currentCoroutine);
                            StopCoroutine(clockActionTime());
                            completedAction = true;
                            if (needToRestart)
                            {
                                break;
                            }

                        }

                        if (needToRestart)
                        {
                            currentLoopRequest = loopUnitMakerRequest(side, currentAI);
                            yield return StartCoroutine(currentLoopRequest);
                        }
                        currentMode = 0;

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
            Debug.Log("We're in range!");
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

    public IEnumerator beginAttackSequenceAOE(Unit attacker, Tile attackTile, Tile defendTile, List<Weapon> weapons)
    {
        //First determine if we can begin attacking right now
        //Debug.Log(attacker);
        if (gM.getAttackTilesWithWeapons(attacker, attackTile, weapons).Contains(defendTile))
        {
            Debug.Log("We're in AOE range!");
            yield return StartCoroutine(gM.doAOEAttack(attacker, attackTile, defendTile, weapons));
        }
        else
        {

            //Debug.Log("Attacker is "+ attacker);
            //Debug.Log("Attack tile is " + attackTile.getPos());
            //Debug.Log("Defend tile is " + defendTile.getPos());
            //If we can't attack enemy, move to an adjacent tile and begin attack
            yield return StartCoroutine(gM.moveInRange(attacker, attackTile, defendTile, weapons));
            yield return StartCoroutine(gM.doAOEAttack(attacker, attackTile, defendTile, weapons));
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

    //Method to wait for a list of task coroutines to finish
    public IEnumerator waitForTasks(List<Task> tasks)
    {
        float duration = Time.realtimeSinceStartup;
        foreach (Task task in tasks)
        {
            while (task.Running)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
        duration = Time.realtimeSinceStartup - duration;
        Debug.Log("It took " + duration + "s to get all tasks done!");
        //Debug.Log("Finished Tasks");
    }

    public IEnumerator waitForThreads(List<Thread> threads)
    {
        int i = 0;
        int loopCount = 0;
        foreach(Thread thread in threads)
        {
            loopCount = 0;
            while (thread.IsAlive)
            {
                if (loopCount > 100)
                {
                    Debug.LogError(thread + "#"+i+" is taking too long for " +paTCs[i]);
                }
                yield return null;
                yield return shortWaitTime;
                loopCount++;
            }
            i++;
        }
        Debug.Log("Threads complete!");
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
