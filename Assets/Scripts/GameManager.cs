using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Priority_Queue;
using System.Linq;
//using UnityEngine.Networking.NetworkSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject camera;
    public GameObject board;
    public GameObject ui;
    public GameObject endTurnButtonObject;
    public GameCamera camScript;
    public GameObject ai;
    public AIManager aiScript;
    public BoardManager boardScript;
    public UIManager uiScript;
    public DijakstraCalculator dCalc;
    public Button endTurnButton;
    public string humanSide = "Red";
    public string currentPlayer = null;
    public List<string> sides = new List<string>();
    public int currentSide = 0;
    public Tile selectedTile;
    public List<Tile> availableTiles = new List<Tile>();
    public List<Tile> path;
    public WeaponsList weaponsList = new WeaponsList();
    public Unit unit;
    public int index;
    public bool moving = false;
    public bool finishedMoving = false;
    public bool animated = true;
    public bool weaponAnimated = true;
    public bool animationInProgress = false;
    private int actions = 0;
    public int limitedActions = 1000;
    public int day = 1;
    public List<List<string>> teams;
    public string levelType;
    public int levelNum;


    public bool doULAnimations = true;
    public bool doUUnLAnimations = true;
    //Handle Battle UI Functions


    public List<GameObject> weaponPrefabs;
    public List<GameObject> unitEffectPrefabs;
    //Dictionary to contain all units
    public Dictionary<string, List<Unit>> unitDictionary;
    public Dictionary<string, List<Building>> buildingDictionary;

    public Dictionary<string, Player> playerDictionary;
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
           
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        //boardScript = GetComponent<BoardManager>();

    }

    // Start is called before the first frame update
    void Start()
    {
        camScript = camera.GetComponent<GameCamera>();
        boardScript = board.GetComponent<BoardManager>();
        boardScript.setCamera(camScript);
        aiScript = ai.GetComponent<AIManager>();
        aiScript.gM = this;
        boardScript.gM = this;
        boardScript.ai = aiScript;
        uiScript = ui.GetComponent<UIManager>();
        boardScript.ui = uiScript;
        uiScript.gM = this;
        boardScript.makeGridLevel("bonus", 1);
        levelType = "bonus";
        levelNum = 1;
        endTurnButton = endTurnButtonObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Resets the unit dictionary with blank lists
    public void resetUnitDictionary()
    {
        unitDictionary = new Dictionary<string, List<Unit>>();
        foreach (string side in playerDictionary.Keys)
        {
            unitDictionary.Add(side, new List<Unit>());
        }
    }

    //Resets the building dictionary with blank lists
    public void resetBuildingDictionary()
    {
        buildingDictionary = new Dictionary<string, List<Building>>();
        foreach (string side in playerDictionary.Keys)
        {
            buildingDictionary.Add(side, new List<Building>());
        }
    }

    //Reset all dictionaries
    public void resetAllDictionaries()
    {
        resetUnitDictionary();
        resetBuildingDictionary();
    }

    public void destroyEverything()
    {
        //Clear units
        foreach(string side in unitDictionary.Keys)
        {
            foreach(Unit unit in unitDictionary[side])
            {
                Destroy(unit.gameObject);
            }
        }

        uiScript.destroyBattleMenu();

        //Clear buildings
        foreach (string side in buildingDictionary.Keys)
        {
            foreach (Building building in buildingDictionary[side])
            {
                Destroy(building.gameObject);
            }
        }

        foreach(Tile tile in boardScript.tileList)
        {
            Destroy(tile.gameObject);
        }
    }

    public void restart()
    {
        destroyEverything();
        day = 1;
        boardScript.makeGridLevel(levelType, levelNum);

    }

    //Ends the player's turn and moves on to the next player
    public void endTurn()
    {
        Debug.Log(currentPlayer);
        uiScript.destroyBattleMenu();
        bool updateTiles = false;
        uiScript.updateStats(null, null, null);
        if (endTurnButton == null) endTurnButton = endTurnButtonObject.GetComponent<Button>();
        clearAvailableTiles();
        if (selectedTile != null)
        selectedTile.deleteSelector();
        if (currentPlayer == null)
        {
            currentSide = 0;
        }
        else
        {
            //Handle Resetting all unit move points and action points
            foreach (Unit unit in unitDictionary[currentPlayer])
            {
                unit.whiteScale();
            }
            if (currentSide == sides.Count - 1)
            {
                currentSide = 0;
                day++;
                updateTiles = true;
            }
            else currentSide++;
        }
        currentPlayer = sides[currentSide];

        clearAvailableTiles();
        aiScript.resetUsedUnits();
        if (currentPlayer != humanSide)
            endTurnButton.interactable = false;
        else endTurnButton.interactable = true;

        foreach (Unit unit in unitDictionary[currentPlayer])
        {
            unit.startTurn();
            unit.finished = false;
        }


        bool starting = false;
        if (day == 1) starting = true;

        foreach(Building building in buildingDictionary[currentPlayer])
        {
            building.beginTurn(starting);
            building.finished = false;
        }

        if (updateTiles)
        {
            StartCoroutine(boardScript.startTiles());
        }

        uiScript.getBPEconStats();
        //Handle AI functions
        actions = -1;
        limitedActions = unitDictionary[currentPlayer].Count*2;


        if (currentPlayer != humanSide)
        {
            //aiScript.requestAIAction(currentPlayer);
            //doAI();
            StartCoroutine(doAIAction());
        }
        
    }

    public IEnumerator doAIAction()
    {
        /*actions++;
        if (actions > limitedActions)
        {
            endTurn();
            return;
        }
        if (!aiScript.requestAIAction(currentPlayer))
        {
            actions++;
        }
        else
        {
            endTurn();
        }*/
        StartCoroutine(aiScript.requestAIAction(currentPlayer));
        yield return null;
    }

    //Clear the available tiles List and revert the tiles to normal
    public void clearAvailableTiles()
    {
        foreach (Tile tile in availableTiles)
        {
            tile.deleteSelector();
        }
        availableTiles = new List<Tile>();
    }

    public void clearAvailableTilesExceptSelected()
    {
        List<Tile> availTiles = new List<Tile>(availableTiles);
        foreach (Tile tile in availTiles)
        {
            if (tile != selectedTile) {
                availableTiles.Remove(tile);
                tile.deleteSelector();
            }
        }

    }

    public IEnumerator moveUnit(Unit unit, Tile start, Tile dest)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        path = dCalc.findPath(true, false);
        //Debug.Log(path);
        //moving = true;
        //finishedMoving = true;
        unit.move((float)Math.Round(getPathDistance(unit, path),3));
        path[path.Count-1].unitToArrive = unit.gameObject;
        this.unit = unit;
        if (path == null)
        {
            yield return null;
        }
        else
        {
            yield return StartCoroutine(MoveOverSecondsPath(unit, path, 1 / 3f));
        }
        start.unitHP = 0;

        Debug.Log("Moving unit!");
        
    }

    //Used to move the unit for the current human player
    //Disables end turn button to prevent AI from thinking unit is still on a certain tile
    public IEnumerator moveUnitAsPlayer(Unit unit, Tile start, Tile dest)
    {
        endTurnButton.interactable = false;
        animationInProgress = true;
        yield return StartCoroutine(moveUnit(unit, start, dest));
        endTurnButton.interactable = true;
        animationInProgress = false;
    }

    //Move the unit until it is in range of the enemy tile.
    /*public IEnumerator moveInRange(Unit unit, Tile start, Tile dest, List<Weapon> weapons)
    {
        Tile moveTo;
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        path = dCalc.findInRangePath(weapons);
        int i = 1;
        for (i = 1; i < path.Count - 1; i++)
        {
            moveTo = path[i];
            dCalc.setValues(moveTo, dest);
            dCalc.reset();
            List<Tile> tempPath = dCalc.findPath(false, false);
            //Get the path distance
            float pathDist = getAttackPathDistance(tempPath);
            if (pathDist <= unit.getCurrentWeapon().maxRange)
            {
                break;
            }

        }
        path.RemoveRange(i + 1, path.Count - (i + 1));
        unit.move((float)Math.Round(getPathDistance(unit, path), 3));
        start.unitHP = 0;
        path[i].unitToArrive = unit.gameObject;
        this.unit = unit;
        if (path == null)
        {
            yield break;
        }
        yield return StartCoroutine(MoveOverSecondsPath(unit, path, 1 / 3f));

        Debug.Log("Moving unit!");

    }*/

    //Moves unit in range and then attacks
    public IEnumerator moveInRangeAttack(Unit unit, Tile start, Unit defender, Tile dest, bool finalUnitAction, List<Weapon> attackerWeapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        path = dCalc.findInRangePath(unit, attackerWeapons, false, false);
        //Debug.Log(path);
        /*int i = 1;
        for (i = 1; i < path.Count - 1; i++)
        {
            moveTo = path[i];
            dCalc.setValues(moveTo, dest);
            dCalc.reset();
            List<Tile> tempPath = dCalc.findPath(false);
            //Get the path distance
            float pathDist = getAttackPathDistance(tempPath);
            if (pathDist <= unit.getCurrentWeapon().maxRange)
            {
                break;
            }

        }
        path.RemoveRange(i + 1, path.Count - (i + 1));*/
        unit.move((float)Math.Round(getPathDistance(unit, path), 3));
        start.unitHP = 0;
        path[path.Count-1].unitToArrive = unit.gameObject;
        //printPath(path);
        this.unit = unit;
        if (path == null)
        {
            yield return null;
        }
        else
        {
            //Add expected damages for AI
            foreach (Weapon attackerWeapon in attackerWeapons)
            {
                dest.expectedDamage += attackerWeapon.getActualDamagePerAttack(unit, defender);
            }

            yield return StartCoroutine(MoveOverSecondsPathAttack(unit, path, defender, 1 / 3f, attackerWeapons));
        }

    }

    //Gives the tiles to the closest in range path
    //Absolute is true if we don't care about MP
    //Hypothetical is true if we don't care that the unit is actually on the start
    public List<Tile> getInRangePathAH(Unit unit, Tile start, Tile dest, List<Weapon> attackerWeapons, bool absolute, bool hypothetical)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        return dCalc.findInRangePath(unit, attackerWeapons, absolute, hypothetical);
    }

    public List<Tile> getTilesInAbsoluteRange(Tile start, int minRange, int maxRange)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getInAbsoluteRange(minRange,maxRange);
    }

    //Moves unit in range
    public IEnumerator moveInRange(Unit unit, Tile start, Tile dest, List<Weapon> attackerWeapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        path = dCalc.findInRangePath(unit, attackerWeapons, false, false);
        //Debug.Log(path);
        /*int i = 1;
        for (i = 1; i < path.Count - 1; i++)
        {
            moveTo = path[i];
            dCalc.setValues(moveTo, dest);
            dCalc.reset();
            List<Tile> tempPath = dCalc.findPath(false);
            //Get the path distance
            float pathDist = getAttackPathDistance(tempPath);
            if (pathDist <= unit.getCurrentWeapon().maxRange)
            {
                break;
            }

        }
        path.RemoveRange(i + 1, path.Count - (i + 1));*/
        unit.move((float)Math.Round(getPathDistance(unit, path), 3));
        start.unitHP = 0;
        path[path.Count - 1].unitToArrive = unit.gameObject;
        //printPath(path);
        this.unit = unit;
        if (path == null)
        {
            yield return null;
        }
        else
        {
            /*//Add expected damages for AI
            foreach (Weapon attackerWeapon in attackerWeapons)
            {
                dest.expectedDamage += attackerWeapon.getActualDamagePerAttack(unit, defender);
            }
            
            yield return StartCoroutine(MoveOverSecondsPathAttack(unit, path, defender, 1 / 3f, attackerWeapons));*/
            yield return StartCoroutine(MoveOverSecondsPath(unit, path, 1 / 3f));
        }

    }

    //Move the unit as close to the destination tile as allowed by the unit's current MP
    public IEnumerator moveAsFarAsPossible(Unit unit, Tile start, Tile dest, bool finalUnitAction)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        path = dCalc.getAsFarAsPossiblePath();
        float pathDist = getPathDistance(unit, path);
        //Stop if we can't actually move
        if (pathDist > unit.getCurrentMP())
        {
            Debug.Log(pathDist);
            Debug.Log(path.Count);
            Debug.Log("Cancelling!");
            yield break;
        }
        unit.move((float)Math.Round(getPathDistance(unit, path), 3));
        start.unitHP = 0;
        path[path.Count-1].unitToArrive = unit.gameObject;
        this.unit = unit;
        if (path == null)
        {
            yield return null;
        }
        else
        {
            yield return StartCoroutine(MoveOverSecondsPath(unit, path, 1 / 3f));
        }
        /*
         * Old code for finding path
        path = dCalc.findPath(true, true);
        int i = 1;
        float distTraveledLast = 0, distTraveled = 0;
        for (i = 1; i < path.Count - 1; i++)
        {
            distTraveledLast = distTraveled;
            distTraveled += path[i].moveCost;
            //Get the path distance
            if (distTraveled > unit.getCurrentMP())
            {
                moveTo = path[i - 1];
                break;
            }

        }
        path.RemoveRange(i + 1, path.Count - (i + 1));
        unit.move((float)Math.Round(getPathDistance(path), 3));
        start.unitHP = 0;
        path[i].unitToArrive = unit.gameObject;
        this.unit = unit;
        if (path == null)
        {
            return;
        }
        StartCoroutine(MoveOverSecondsPath(unit, path, 1 / 3f));
        */
    }


    public float getPathDistance(Unit unit, List<Tile> path)
    {
        if (path == null) return 0f;
        //Debug.Log("Null path");
        float dist = 0f;
        for (int i = path.Count - 1; i > 0; i--)
        {
            dist += path[i-1].getMoveCost(unit,path[i]);
        }
        return dist;
    }

    public int getAttackPathDistance(List<Tile> path)
    {
        int dist = 0;
        for (int i = path.Count - 1; i > 0; i--)
        {
            dist++;
        }
        return dist;
    }

    //Get units that can be attacked
    public List<Tile> getAttackbleUnits(Unit unit, Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllEnemyUnitsAttackable();
    }

    public List<Tile> getAttackableUnitsWithWeapons(Unit unit, Tile start, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllEnemyUnitsAttackableWithWeapons(weapons);
    }

    public List<Tile> getMoveTiles(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllMoveables();
    }

    public List<Tile> getAttackTiles(Unit unit, Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllAttacks(unit);
    }

    public List<Tile> getAttackTilesWithWeapons(Unit unit, Tile start, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllAttacksWithWeapons(unit, weapons);
    }

    //For hand weapons only
    public bool canAttackEnemy(Unit attacker, Tile start, Unit defender, Tile end)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemy(attacker, defender);
    }

    //For all active weapons
    public bool canAttackEnemyAtAll(Unit attacker, Tile start, Unit defender, Tile end)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemyAtAll(attacker, defender);
    }

    public bool canAttackEnemyWithWeapon(Unit attacker, Tile start, Unit defender, Tile end, Weapon weapon)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemyWithWeapon(attacker, weapon, defender);
    }

    public bool canAttackEnemyWithWeapons(Unit attacker, Tile start, Unit defender, Tile end, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemyWithWeapons(attacker, weapons, defender);
    }

    public bool canAttackEnemyExactlyWithWeapon(Unit attacker, Tile start, Unit defender, Tile end, Weapon weapon)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemyExactlyWithWeapon(attacker, weapon, defender);
    }

    public bool canAttackEnemyExactlyWithWeapons(Unit attacker, Tile start, Unit defender, Tile end, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canAttackEnemyExactlyWithWeapons(attacker, weapons, defender);
    }

    //Healing variants
    public List<Tile> getHealTiles(Unit unit, Tile start, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, dCalc.getDest());
            dCalc.reset();
        }
        return dCalc.findAllHealables(unit, weapons);
    }

    public bool canHealAlly(Tile start, Tile end)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canHealRepairAlly();
    }

    public bool canHealAllyWithWeapon(Tile start, Tile end, Weapon weapon)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canHealRepairAllyWithWeapon(weapon);
    }

    public bool canHealAllyWithWeapons(Tile start, Tile end, List<Weapon> weapons)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, end);
        }
        else
        {
            dCalc.setValues(start, end);
            dCalc.reset();
        }
        return dCalc.canHealRepairAllyWithWeapons(weapons);
    }

    public List<Tile> getTilesInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getTilesInMoveRange();
    }


    public List<Tile> getBuildingsInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getBuildingsInMR();
    }

    public List<Tile> getAllyBuildingsInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getAllyBuildingsInMR();
    }

    public List<Tile> getNeutralBuildingsInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getNeutralBuildingsInMR();
    }

    public List<Tile> getEnemyBuildingsInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getEnemyBuildingsInMR();
    }

    public List<Tile> getCapturableBuildingsInMoveRange(Tile start)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getCapturableBuildingsInMR();
    }

    public float getMovementDistance(Unit unit, Tile start, Tile dest)
    {
        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, dest);
        }
        else
        {
            dCalc.setValues(start, dest);
            dCalc.reset();
        }
        return dCalc.getMovementDistance(unit);
    }

    public Dictionary<Tile, float> getInfluenceDictionary(Tile start)
    {

        if (dCalc == null)
        {
            dCalc = new DijakstraCalculator(this, start, null);
        }
        else
        {
            dCalc.setValues(start, null);
            dCalc.reset();
        }
        return dCalc.getInfluenceMapAsDict();
    }


    //Used to determine if a unit that needs to do some action to a tile, that unit is facing the direction of that tile
    public string facingRightDir(Unit unit, Tile dest)
    {
        //Debug.Log(unit.transform.position.x);
        //Debug.Log(dest.transform.position.x);
        //Debug.Log(unit.gameObject.transform.rotation.eulerAngles);
        if (dest.transform.position.x < unit.transform.position.x)
        {
            if (unit.gameObject.transform.eulerAngles.y == 180) return "true";
            return "turn left";
        }
        else if (dest.transform.position.x > unit.transform.position.x)
        {
            if (unit.gameObject.transform.eulerAngles.y == 0) return "true";
            return "turn right";
        }
       return "true";
    }

    public void lookAtRightDir(Unit unit, Tile dest)
    {
        string dirToFace = facingRightDir(unit, dest);
        if (dirToFace == "turn left")
        {
            //Debug.Log("Turning left");
            unit.transform.eulerAngles = new Vector3(0, 180, 0);
            WeaponObject cwObject = unit.getCWObject();
            if (cwObject != null)
            {
                cwObject.transform.localPosition = new Vector3(cwObject.transform.localPosition.x, cwObject.transform.localPosition.y, cwObject.transform.localPosition.z + 5.5f);
                cwObject = unit.cw2Script;
                if (cwObject != null)
                {
                    cwObject.transform.localPosition = new Vector3(cwObject.transform.localPosition.x, cwObject.transform.localPosition.y, cwObject.transform.localPosition.z + 3.5f);
                }

            }
            if (unit.turretObjects != null)
            {
                foreach (GameObject turret in unit.turretObjects)
                {
                    //Rotating turret
                    //Debug.Log("Rotating Turret");
                    turret.transform.position = new Vector3(turret.transform.position.x, turret.transform.position.y, -3);

                }
            }
        }
        else if (dirToFace == "turn right")
        {
            //
            unit.transform.eulerAngles = new Vector3(0, 0, 0);
            WeaponObject cwObject = unit.getCWObject();
            if (cwObject != null)
            {
                cwObject.transform.localPosition = new Vector3(cwObject.transform.localPosition.x, cwObject.transform.localPosition.y, cwObject.transform.localPosition.z - 5.5f);
                cwObject = unit.cw2Script;
                if (cwObject != null)
                {
                    cwObject.transform.localPosition = new Vector3(cwObject.transform.localPosition.x, cwObject.transform.localPosition.y, cwObject.transform.localPosition.z - 3.5f);
                }
                
            }
            if (unit.turretObjects != null)
            {
                foreach (GameObject turret in unit.turretObjects)
                {
                    turret.transform.position = new Vector3(turret.transform.position.x, turret.transform.position.y, -3);

                }
            }
        }
    }

    public IEnumerator MoveOverSecondsPath(Unit unit, List<Tile> ends, float seconds)
    {
        GameObject objectToMove = unit.gameObject;
        for (int i = 1; i < ends.Count; i++)
        {
            unit.gameObject.GetComponent<Animator>().SetBool("Moving", true);
            //Debug.Log(i);
            //ends[i - 1].setUnit(null);
            Vector3 end = ends[i].gameObject.transform.position;
            float elapsedTime = 0;
            Vector3 relEnd = new Vector3(end.x, end.y, objectToMove.transform.position.z) + unit.displacementVector;
            Vector3 startingPos = objectToMove.transform.position;
            //Debug.Log(dirToFace);
            lookAtRightDir(unit, ends[i]);
            while (elapsedTime < seconds)
            {
                objectToMove.transform.position = Vector3.Lerp(startingPos, relEnd, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            objectToMove.transform.position = relEnd;
            yield return StartCoroutine(ends[i].enterUnit(unit));
            //ends[i].setUnit(unit.gameObject);
        }
        Tile endTile = ends[ends.Count - 1];
        ends[0].setUnit(null);
        endTile.setUnit(unit.gameObject);
        unit.gameObject.GetComponent<Animator>().SetBool("Moving", false);
        endTile.unitHP = unit.getCurrentHP();
        unit.setTile(endTile);
        endTile.unitToArrive = null;
        unit.checkIfActionPossible();
    }

    public IEnumerator MoveOverSecondsPathAttack(Unit unit, List<Tile> ends, Unit defender, float seconds,List<Weapon> attackerWeapons)
    {
        yield return StartCoroutine(MoveOverSecondsPath(unit, ends, seconds));
        yield return StartCoroutine(attackEnemy(unit, defender, attackerWeapons));
    }

    //Returns the damage the unit would do
    //isSentry - whether or not the defending unit is in sentry mode
    //getAttacker - whether or not to get the attacker's damage instead of the defender's damage
    public float calculateDamage(Unit attacker, Unit defender, bool getAttacker, bool isSentry, List<Weapon> attackerWeapons)
    {
        if (!isSentry)
        {
            float weaponDamage = 0;
            float multiplier = 1f;
            string weaponType;
            foreach (Weapon attackerWeapon in attackerWeapons)
            {

                 weaponDamage += attackerWeapon.getActualDamagePerAttack(attacker, defender);
            }
            //Handle if the unit is guarding
            //if (defender.getGuard()) weaponDamage /= 2;
            if (getAttacker) return (float)Math.Round(weaponDamage,3);

            //If not getAttacker, than return defense damage
            multiplier = 1f;
            float attackerDamage = weaponDamage;
            foreach (Weapon defenderWeapon in defender.getAllActiveWeapons())
            {

                weaponDamage += defenderWeapon.getActualDamagePerAttack(defender, attacker) * (defender.getCurrentHP() - attackerDamage) / defender.getHP() / defender.getHPRatio();
            }
            return (float)Math.Round(weaponDamage,3);
        }
        else
        {
            float weaponDamage = 0;
            float multiplier = 1f;
            string weaponType;
            foreach (Weapon defenderWeapon in defender.getAllActiveWeapons())
            {

                weaponDamage += defenderWeapon.getActualDamagePerAttack(defender, attacker);
            }

            if (!getAttacker) return weaponDamage;

            float defenderDamage = weaponDamage;
            weaponDamage = 0f;
            foreach (Weapon attackerWeapon in attackerWeapons){
                //If not getAttacker, than return defense damage

                weaponDamage += attackerWeapon.getActualDamagePerAttack(attacker, defender) * (attacker.getCurrentHP() - defenderDamage) / attacker.getHP() / attacker.getHPRatio();
                
            }
            return (float)Math.Round(weaponDamage,3);
        }
    }

    public float calculateHeal(Unit healer, Unit healee, List<Weapon> weapons)
    {
        float healed = 0;
        Debug.Log(weapons[0].name);
        foreach (Weapon weapon in weapons)
        {
            healed += weapon.getActualHPPerHeal(healer, healee);
        }
        return healed;
    }

    public IEnumerator attackEnemy(Unit attacker, Unit defender , List<Weapon> attackerWeapons)
    {
        if (attackerWeapons != null && attackerWeapons.Count > 0)
        {
            string dirToFace;
            //if (!canAttackEnemy(attacker.getTile(), defender.getTile())) Debug.LogError("We shouldn't be able to attack " + defender + " as " + attacker);
            if (defender == null) yield return null;
            else
            {
                if (!weaponAnimated)
                {
                    if (!defender.getSentry())
                    {
                        float attackerDmg = calculateDamage(attacker, defender, true, false, attackerWeapons);
                        float defenderDmg = calculateDamage(attacker, defender, false, false, attackerWeapons);

                        lookAtRightDir(attacker, defender.getTile());
                        StartCoroutine(defender.loseHP(attackerDmg));

                        //Do counter attack if the unit is in range
                        if (defender != null && defender.getCurrentHP() > 0 && canAttackEnemyAtAll(attacker, defender.getTile(), defender, attacker.getTile()))
                        {
                            lookAtRightDir(defender, attacker.getTile());
                            StartCoroutine(attacker.loseHP(defenderDmg));
                        }
                        if (attacker != null)
                        {
                            attacker.useAP(1);
                        }
                        defender.getTile().expectedDamage -= attackerDmg;
                    }
                    else
                    {
                        float attackerDmg = calculateDamage(attacker, defender, true, true, attackerWeapons);
                        float defenderDmg = calculateDamage(attacker, defender, false, true, attackerWeapons);

                        lookAtRightDir(defender, attacker.getTile());
                        StartCoroutine(attacker.loseHP(defenderDmg));

                        //Do counter attack if the unit is in range
                        //Implied we could already attack enemy
                        if (attacker != null && attacker.getCurrentHP() > 0 && canAttackEnemyWithWeapons(attacker, attacker.getTile(), defender, defender.getTile(),attackerWeapons))
                        {

                            StartCoroutine(defender.loseHP(attackerDmg));
                        }
                        if (attacker != null)
                        {
                            attacker.useAP(1);
                        }
                        defender.getTile().expectedDamage -= attackerDmg;
                    }
                    //if (currentPlayer != humanSide) doAI();
                    yield return null;
                }
                else
                {
                    if (!defender.getSentry())
                    {
                        float attackerDmg = calculateDamage(attacker, defender, true, false, attackerWeapons);
                        float defenderDmg = calculateDamage(attacker, defender, false, false, attackerWeapons);

                        //Do animation
                        yield return StartCoroutine(animateAttack(attacker, defender, attackerDmg, defenderDmg, defender.getSentry(), attackerWeapons));
                    }
                    else
                    {
                        float attackerDmg = calculateDamage(attacker, defender, true, true, attackerWeapons);
                        float defenderDmg = calculateDamage(attacker, defender, false, true, attackerWeapons);

                        //Do animation
                        yield return StartCoroutine(animateAttack(attacker, defender, attackerDmg, defenderDmg, defender.getSentry(), attackerWeapons));
                    }
                }
            }
            foreach (Weapon weapon in attackerWeapons)
            {
                weapon.currentAttacks++;
            }
        }
        attacker.checkIfActionPossible();
    }

    public IEnumerator healAlly(Unit healer, Unit healee, List<Weapon> attackerWeapons)
    {
        if (attackerWeapons != null && attackerWeapons.Count > 0)
        {
            lookAtRightDir(healer, healee.getTile());
            float healedHP = calculateHeal(healer, healee, attackerWeapons);
            Debug.Log(healedHP);
            if (!weaponAnimated)
            {
                if (healedHP > 0)
                {
                    yield return StartCoroutine(healee.healHP(healedHP));
                }
            }
            else
            {
                if (healedHP > 0)
                {
                    yield return StartCoroutine(healer.doHealAnimation(healee.getTile(),attackerWeapons));
                }
                //StartCoroutine(healee.healHP(healedHP));


            }
            healer.useAP(1);
        }
        healer.checkIfActionPossible();
        //yield return null;
    }

    //Gets AOE tiles
    public HashSet<Tile> getAOETiles(Unit unit, Tile start, Tile target, Weapon weapon)
    {
        HashSet<Tile> toAttack = new HashSet<Tile>();
        switch (weapon.aoeType)
        {
            //Target all tiles around the tile to be fired on

            case 0:
                int aoe = unit.targetingWeapon.aoe;
                SimplePriorityQueue<Tile> frontier = new SimplePriorityQueue<Tile>();
                HashSet<Tile> explored = new HashSet<Tile>();
                SimpleNode<Tile, float> current, temp, temp2;
                toAttack = new HashSet<Tile>();
                frontier.Enqueue(target, 0);
                int actions = 0, limitedActions = 100;
                while (frontier.Count > 0)
                {
                    actions++;
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;
                    //Try to target the tile
                    if (current.Priority <= aoe && currentTile.getUnit() != null)
                    {
                        toAttack.Add(currentTile);
                    }
                    foreach (Tile tile in currentTile.adjacent)
                    {
                        if (tile != null)
                        {
                            temp = new SimpleNode<Tile, float>(tile);
                            temp.Priority = current.Priority + 1;
                            if (temp.Priority > aoe) continue;
                            if (!explored.Contains(tile) && !frontier.Contains(tile))
                            {
                                //Add the node to the frontier if we didn't explore it already
                                frontier.Enqueue(tile, current.Priority + 1);
                                tile.predecessor = current.Data;
                            }
                            //If we have a move path that is shorter than what is in the frontier, replace it
                            else if (frontier.Contains(tile))
                            {
                                temp2 = frontier.RemoveNode(tile);
                                if (temp.Priority < temp2.Priority)
                                {
                                    frontier.Enqueue(temp.Data, temp.Priority);
                                    tile.predecessor = currentTile;
                                }
                                else
                                {
                                    frontier.Enqueue(temp2.Data, temp2.Priority);
                                }

                            }
                        }
                    }

                }
                break;
            case 1:
                //The range should always be 1-1 otherwise we will have is
                //Get all tiles in one dir
                //The target may not be one away

                List<string> dirs = new List<string>() { "upleft", "upright", "left", "right", "downleft", "downright" };
                Tile tempTile, previous = start;
                bool foundDest = false;
                //We need to find the dir that we should be looking at from the start
                foreach(string dir in dirs)
                {
                    List<Tile> tempAttack = new List<Tile>();
                    previous = start;
                    for (int i = 0; i < weapon.aoe + 1; i++)
                    {
                        tempTile = previous.getAdjacentTileOnSide(dir);
                        if (tempTile == null)
                        {
                            break;
                        }
                        if (tempTile == target)
                        {
                            foundDest = true;
                        }
                        previous = tempTile;
                        tempAttack.Add(tempTile);
                    }
                    if (foundDest)
                    {
                        toAttack = new HashSet<Tile>(tempAttack);
                        break;
                    }
                }
                break;
        }
        //printPath(toAttack.ToList<Tile>());
        return toAttack;
    }

    //Does an AOE attack
    public IEnumerator doAOEAttack(Unit unit, Tile start, Tile target, List<Weapon> weapons)
    {
        if (weapons != null && weapons.Count > 0)
        {
            foreach (Weapon weapon in weapons)
            {
                HashSet<Tile> toAttack = getAOETiles(unit, start, target, weapon);
                printPath(toAttack.ToList<Tile>());

                //Handle attacking those tiles now
                //First determine if the target tile attacks us
                //unit is attacker variable in this case
                if (target.getUnit() != null)
                {
                    Unit defender = target.getUnitScript();
                    //Handle if the defending unit on the target tile is defending
                    if (defender.getSentry() && canAttackEnemy(defender, target, unit, start))
                    {
                        float defenderDMG = calculateDamage(unit, defender, false, true, new List<Weapon>() { weapon });
                        float attackerDMG = calculateDamage(unit, defender, true, true, new List<Weapon>() { weapon });
                        lookAtRightDir(defender, start);
                        yield return StartCoroutine(defender.doDefenseAnimation(start));
                        //StartCoroutine(unit.loseHP(defenderDMG));

                        //Check if  unit is null or not
                        if (unit != null)
                        {
                            //Begin attack
                            lookAtRightDir(unit, target);
                            if (weaponAnimated)
                            {
                                yield return StartCoroutine(unit.weaponDictionary[weapon].performAOEAnimation(target));
                            }
                            defender.loseHP(attackerDMG);
                            toAttack.Remove(target);

                            //Now attack all aoe'd tiles
                            foreach (Tile tile in toAttack)
                            {
                                if (tile.getUnit() == null) continue;
                                defender = tile.getUnitScript();
                                float dmg = calculateDamage(unit, defender, true, false, new List<Weapon>() { weapon });
                                StartCoroutine(defender.loseHP(dmg));

                            }
                        }

                    }
                    else
                    {
                        //Debug.Log("Attacking with AOE!");
                        float defenderDMG = calculateDamage(unit, defender, false, false, new List<Weapon>() { weapon });
                        float attackerDMG = calculateDamage(unit, defender, true, false, new List<Weapon>() { weapon });
                        Debug.Log(attackerDMG);
                        lookAtRightDir(unit, target);
                        if (weaponAnimated)
                        {
                            yield return StartCoroutine(unit.weaponDictionary[weapon].performAOEAnimation(target));
                        }
                        StartCoroutine(defender.loseHP(attackerDMG));
                        toAttack.Remove(target);

                        //Now attack all aoe'd tiles
                        foreach (Tile tile in toAttack)
                        {
                            if (tile.getUnit() == null) continue;
                            defender = tile.getUnitScript();
                            if (defender.flying && !weapon.canTargetAir) continue;
                            if (defender.isSubmerged && !weapon.canTargetSub) continue;
                            Debug.Log(tile.getPos());
                            float dmg = calculateDamage(unit, defender, true, false, new List<Weapon>() { weapon });
                            Debug.Log(dmg);
                            StartCoroutine((defender.loseHP(dmg)));

                        }

                        if (defender != null && canAttackEnemy(defender, target, unit, start))
                        {
                            //Debug.Log("Counter Attacking!");
                            lookAtRightDir(defender, start);
                            yield return StartCoroutine(defender.doDefenseAnimation(start));
                            //StartCoroutine(unit.loseHP(defenderDMG));
                        }

                    }
                }
                else
                {
                    lookAtRightDir(unit, target);
                    if (weaponAnimated)
                    {
                        yield return StartCoroutine(unit.weaponDictionary[weapon].performAOEAnimation(target));
                    }
                    toAttack.Remove(target);
                    //Now attack all aoe'd tiles
                    foreach (Tile tile in toAttack)
                    {
                        if (tile.getUnit() == null) continue;
                        Unit defender = tile.getUnitScript();
                        float dmg = calculateDamage(unit, defender, true, false, new List<Weapon>() { weapon });
                        StartCoroutine(defender.loseHP(dmg));

                    }
                }
                weapon.currentAttacks++;

            }
            unit.useAP(1);
        }
        unit.checkIfActionPossible();

    }


    public IEnumerator doAOEHeal(Unit unit, Tile start, Tile target, List<Weapon> weapons)
    {
        if (weapons != null && weapons.Count > 0)
        {
            foreach (Weapon weapon in weapons)
            {
                HashSet<Tile> toAttack = getAOETiles(unit, start, target, weapon);

                //Handle attacking those tiles now
                //First determine if the target tile attacks us
                //unit is attacker variable in this case
                if (target.getUnit() != null)
                {
                    Unit defender = target.getUnitScript();
                    //Handle if the defending unit on the target tile is defending

                    //Debug.Log("Attacking with AOE!");

                    float attackerDMG = calculateHeal(unit, defender, new List<Weapon>() { weapon });
                    //Debug.Log(attackerDMG);
                    lookAtRightDir(unit, target);
                    if (weaponAnimated)
                    {
                        yield return StartCoroutine(unit.weaponDictionary[weapon].performAOEAnimation(target));
                    }
                    StartCoroutine(defender.healHP(attackerDMG));
                    toAttack.Remove(target);

                    //Now attack all aoe'd tiles
                    foreach (Tile tile in toAttack)
                    {
                        if (tile.getUnit() == null) continue;
                        defender = tile.getUnitScript();
                        if (defender.flying && !weapon.canTargetAir) continue;
                        if (defender.isSubmerged && !weapon.canTargetSub) continue;
                        float dmg = calculateHeal(unit, defender, new List<Weapon>() { weapon });
                        StartCoroutine((defender.loseHP(dmg)));

                    }



                }
                else
                {
                    lookAtRightDir(unit, target);
                    if (weaponAnimated)
                    {
                        yield return StartCoroutine(unit.weaponDictionary[weapon].performAOEAnimation(target));
                    }
                    toAttack.Remove(target);
                    //Now attack all aoe'd tiles
                    foreach (Tile tile in toAttack)
                    {
                        if (tile.getUnit() == null) continue;
                        Unit defender = tile.getUnitScript();
                        float dmg = calculateHeal(unit, defender, new List<Weapon>() { weapon });
                        StartCoroutine(defender.healHP(dmg));

                    }
                }
                weapon.currentAttacks++;
            }
            unit.useAP(1);
        }
        unit.checkIfActionPossible();
    }

    public IEnumerator animateAttack(Unit attacker, Unit defender, float attackerDmg, float defenderDmg, bool isSentry, List<Weapon> attackerWeapons)
    {
        if (isSentry)
        {
            if (canAttackEnemyAtAll(defender, defender.getTile(), attacker, attacker.getTile())) {
                lookAtRightDir(defender, attacker.getTile());
                yield return StartCoroutine(defender.doDefenseAnimation(attacker.getTile()));
                //StartCoroutine(attacker.loseHP(defenderDmg));
            }
            if (attacker != null && attacker.getCurrentHP() > 0 && canAttackEnemyWithWeapons(attacker, attacker.getTile(), defender, defender.getTile(),attackerWeapons))
            {
                lookAtRightDir(attacker, defender.getTile());
                yield return StartCoroutine(attacker.doAttackAnimation(defender.getTile(),attackerWeapons));
                //StartCoroutine(defender.loseHP(attackerDmg));
            }
            if (attacker != null)
            {
                attacker.useAP(1);
            }
            defender.getTile().expectedDamage -= attackerDmg;
        }
        else
        {
            lookAtRightDir(attacker, defender.getTile());
            yield return StartCoroutine(attacker.doAttackAnimation(defender.getTile(), attackerWeapons));
            //StartCoroutine(defender.loseHP(attackerDmg));
            if (defender != null && defender.getCurrentHP() > 0 && canAttackEnemyAtAll(defender, defender.getTile(), attacker, attacker.getTile()))
            {
                lookAtRightDir(defender, attacker.getTile());
                yield return StartCoroutine(defender.doDefenseAnimation(attacker.getTile()));
                //StartCoroutine(attacker.loseHP(defenderDmg));
            }
            if (attacker != null)
            {
                attacker.useAP(1);
            }
            defender.getTile().expectedDamage -= attackerDmg;
        }
        //wait a bit
        yield return new WaitForSeconds(0.01f);
        //if (currentPlayer != humanSide) doAI();
    }

    public IEnumerator attackEnemyAsPlayer(Unit attacker, Unit defender, List<Weapon> weapons)
    {
        endTurnButton.interactable = false;
        animationInProgress = true;
        yield return StartCoroutine(attackEnemy(attacker, defender, weapons));
        endTurnButton.interactable = true;
        animationInProgress = false;
    }

    public IEnumerator healAllyAsPlayer(Unit healer, Unit healee, List<Weapon> weapons)
    {
        endTurnButton.interactable = false;
        animationInProgress = true;
        yield return StartCoroutine(healAlly(healer, healee, weapons));
        endTurnButton.interactable = true;
        animationInProgress = false;
    }

    public void disableButtons()
    {
        endTurnButton.interactable = false;
    }

    public void enableButtons()
    {
        endTurnButton.interactable = true;
    }

    //Coroutine Method - Straightline move only
    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        Vector3 relEnd = new Vector3(end.x, end.y, objectToMove.transform.position.z) ;
        Vector3 startingPos = objectToMove.transform.position;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(startingPos, relEnd, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        objectToMove.transform.position = relEnd;

        finishedMoving = true;
        Debug.Log("finishedMoving");

    }

    public int getTeam(string side)
    {
        if (side == null || side == "null") return -1;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].Contains(side))
            {
                return i;
            }
        }
        return -1;
    }

    public int getTeam(Player player)
    {
        string side = player.side;
        if (side == null || side == "null") return -1;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i].Contains(side))
            {
                return i;
            }
        }
        return -1;
    }

    //Prints a path
    public void printPath(List<Tile> path)
    {
        String toPrint = "Printing a path: \n";
        if (path == null)
        {
            Debug.Log("There are no tiles in the path!");
            return;
        }
        foreach (Tile tile in path)
        {
            //Unit unit = tile.getUnitScript();
            toPrint += tile + ",";
        }
        toPrint = toPrint.Substring(0, toPrint.Length - 1);
        toPrint += "\n";
        Debug.Log(toPrint);
    }
}
