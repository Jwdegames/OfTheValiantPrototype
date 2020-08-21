using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    //Variables
    public bool isMakingLevel = true;
    public GameObject tilePrefab;
    public SparseMatrix<Tile> tiles = new SparseMatrix<Tile>();
    public List<Tile> tileList = new List<Tile>();

    public List<GameObject> unitPrefabs = new List<GameObject>();
    public List<GameObject> buildingPrefabs = new List<GameObject>();
    public List<GameObject> tileEffectPrefabs = new List<GameObject>();
    [SerializeField]
    private GameObject selectedTile;
    public GameObject currentTile;
    public GameObject currentUnit;
    public GameObject currentBuilding;
    public Tile currentTS;
    public Tile adjacentTS;
    public GameManager gM;
    public UIManager ui;
    public AIManager ai;
    private GameCamera camera;
    public int levelNum = 1;
    public int bonusLevelNum = 1;
    public int x = 0;
    public int y = 0;
    public int tileSizeMod = 10;
    public float outlineThickness = 0.0025f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Player makePlayer(string side, string faction, int metal, int people, Color color)
    {
        Player player = new Player();
        player.side = side;
        player.faction = faction;
        player.metal = metal;
        player.people = people;
        player.color = color;
        player.gM = gM;
        return player;
    }

    public void makeGrid(int xSize, int ySize)
    {
        tiles = new SparseMatrix<Tile>();
        tileList = new List<Tile>();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int xVal = x * tileSizeMod;
                if (y % 2 == 1)
                {
                    xVal = x * tileSizeMod + tileSizeMod / 2;
                }
                //Debug.Log(xVal+"");
                currentTile = Instantiate(tilePrefab, new Vector3(xVal, y * tileSizeMod * 0.75f, 0), Quaternion.identity) as GameObject;
                currentTS = currentTile.GetComponent<Tile>();
                currentTS.setXSize(tileSizeMod);
                currentTS.setYSize(tileSizeMod);
                currentTS.board = this;
                currentTS.ui = ui;
                currentTS.gM = gM;
                currentTS.mapX = x;
                currentTS.mapY = y;
                //Set the adjacent tiles
                //The ternary operator is used for switiching between rows, because not all rows are lined with hexes the same way
                //So for example, downleft on even y rows is x-1, but on odd y rows it's x
                currentTS.setAdj(tiles.getValue(x - 1, y), "left");
                currentTS.setAdj(tiles.getValue(x + 1, y), "right");
                currentTS.setAdj(tiles.getValue((y % 2 == 1) ? x : x - 1, y - 1), "downleft");
                currentTS.setAdj(tiles.getValue((y % 2 == 1) ? x + 1 : x, y - 1), "downright");
                currentTS.setAdj(tiles.getValue((y % 2 == 1) ? x : x - 1, y + 1), "upleft");
                currentTS.setAdj(tiles.getValue((y % 2 == 1) ? x + 1 : x, y + 1), "upright");
                tiles.insertNode(currentTS, x, y);
                tileList.Add(currentTS);


            }
        }
    }

    public void makeGridLevel(string type, int num)
    {
 
        gM.playerDictionary = new Dictionary<string, Player>();
        gM.playerDictionary.Add("null", makePlayer("null", "null", 0, 0, Color.white));
        gM.teams = new List<List<string>>();
        ai.resetAIs();
        //if (true == true) return;
        switch (type)
        {
            case "bonus":
                switch (num)
                {
                    case 1:
                        makeGrid(25, 10);



                        gM.playerDictionary.Add("Red", makePlayer("Red", "Ignis", 100, 10, Color.red));
                        gM.playerDictionary.Add("Green", makePlayer("Green", "Vita",100, 10, Color.green));
                        gM.teams.Add(new List<string>() { "Red" });
                        gM.teams.Add(new List<string>() { "Green" });
                        gM.resetAllDictionaries();

                        //makeUnit("Droid Trooper", gM.playerDictionary["Red"], 1, 3);
                        //makeUnit("Shielded Droid", gM.playerDictionary["Red"], 2, 4);
                        //makeUnit("Droid Gattler", gM.playerDictionary["Red"], 2, 3);
                        //makeUnit("S-Jet Trooper", gM.playerDictionary["Red"], 1, 4);

                        makeUnit("Droid Gattler", gM.playerDictionary["Red"], 17, 3);
                        makeUnit("Droid Gattler", gM.playerDictionary["Red"], 17, 4);
                        makeUnit("Droid Gattler", gM.playerDictionary["Red"], 16, 3);

                        makeUnit("Slime", gM.playerDictionary["Green"], 22, 3);
                        //makeUnit("Masked Trooper", gM.playerDictionary["Green"], 22, 4);
                        //makeUnit("Grenadier", gM.playerDictionary["Green"], 23, 4);
                        //makeUnit("Eyesore", gM.playerDictionary["Green"], 21, 3);
                        //makeUnit(1, 3, gM.playerDictionary["Green"], 24, 5);

                        makeBuilding(0,"House", gM.playerDictionary["Red"], 7, 2);
                        makeBuilding(0, "House", gM.playerDictionary["Red"], 2, 2);
                        makeBuilding(0, "Apartment", gM.playerDictionary["Red"], 7, 5);
                        makeBuilding(0, "Barracks", gM.playerDictionary["Red"], 3, 7);
                        makeBuilding(0, "Barracks", gM.playerDictionary["Red"], 5, 6);
                        makeBuilding(0, "Factory", gM.playerDictionary["Red"], 3, 4);
                        makeBuilding(0, "Mine MK1", gM.playerDictionary["Red"], 5, 9);
                        makeBuilding(0, "Mine MK2", gM.playerDictionary["Red"], 6, 3);
                        makeBuilding(0, "Laboratory", gM.playerDictionary["Red"], 5, 2);
                        makeBuilding(0, "Mine MK3", gM.playerDictionary["Red"], 6, 2);

                        makeBuilding(0, "House", gM.playerDictionary["Green"], 17, 2);
                        makeBuilding(0, "House", gM.playerDictionary["Green"], 22, 2);
                        makeBuilding(0, "Apartment", gM.playerDictionary["Green"], 17, 5);
                        makeBuilding(0, "Barracks", gM.playerDictionary["Green"], 21, 7);
                        makeBuilding(0, "Barracks", gM.playerDictionary["Green"], 19, 6);
                        makeBuilding(0, "Factory", gM.playerDictionary["Green"], 21, 4);
                        makeBuilding(0, "Mine MK1", gM.playerDictionary["Green"], 19, 9);
                        makeBuilding(0, "Mine MK2", gM.playerDictionary["Green"], 18, 3);
                        makeBuilding(0, "Laboratory", gM.playerDictionary["Green"], 19, 2);
                        makeBuilding(0, "Apartment", gM.playerDictionary["Green"], 20, 2);
                        makeBuilding(0, "House", gM.playerDictionary["Green"], 18, 2);

                        makeBuilding(0, "House", null, 9, 1);
                        makeBuilding(0, "House", null, 14, 1);
                        makeBuilding(0, "Mine MK3", null, 11, 8);
                        makeBuilding(0, "Mine MK3", null, 14, 8);
                        makeBuilding(0, "Mine MK1", null, 13, 5);
                        makeBuilding(0, "Mine MK1", null, 11, 5);
                        makeBuilding(0, "Mine MK1", null, 14, 2);
                        makeBuilding(0, "Mine MK1", null, 10, 2);
                        makeBuilding(0, "Factory", null, 12, 3);
                        makeBuilding(0, "Factory", null, 12, 7);
                        makeBuilding(0, "Barracks", null, 10, 4);
                        makeBuilding(0, "Barracks", null, 15, 4);
                        makeBuilding(0, "Apartment", null, 10, 6);
                        makeBuilding(0, "Apartment", null, 15, 6);


                        gM.camScript.centerMap();
                        ai.addAI("Green", "Utility AI");
                        ai.getUnitsFromGM();

                        ui.player = gM.playerDictionary["Red"];
                        break;
                }
                gM.currentPlayer = null;
                gM.endTurn();
                break;

            case "tutorial":
                break;
            case "normal":
                break;
        }


    }

    public void setCamera(GameCamera cam)
    {
        camera = cam;
    }

    public void setUnitStats(Unit unit, Tile tile, int variant)
    {
        currentTile.GetComponent<Tile>().setUnit(currentUnit);
        unit.useTemplate(variant);
        unit.setTile(tile);
        unit.applyScale();
        unit.matchWeapon("Build Unit");
        tile.unitHP = unit.getCurrentHP();
    }

    /**
     *Makes a unit from prefab at the selected tile
     * 
     * <param name="num"> Determines which prefab is used for the unit. See Unit prefabs for all unit prefabs</param> 
     **/
    public Unit makeUnit(int num, int variant, Player player, int x, int y)
    {
        Tile tile = tiles.getValue(x, y);
        if (tile == null || tile.getUnit() != null || player == null)
            return null;
        currentTile = tile.gameObject;
        currentUnit = Instantiate(unitPrefabs[num], new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1), Quaternion.identity) as GameObject;
        Unit unit = currentUnit.GetComponent<Unit>();
        setUnitStats(unit,tile,variant);
        gM.unitDictionary[player.side].Add(unit);
        unit.setSide(player.side);
        unit.makeShadow();
        unit.setOutlineColor(player.color, 10f);
        unit.setOutlineThickness(outlineThickness);
        return unit;
    }
    
    public Unit makeUnit(string unitName, Player player, int x, int y)
    {
        Tile tile = tiles.getValue(x, y);
        if (tile == null || tile.getUnit() != null || player == null)
            return null;
        currentTile = tile.gameObject;
        UnitsList unitsList = new UnitsList();
        currentUnit = Instantiate(unitsList.getUnitPrefab(player.faction,this,unitName), new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1), Quaternion.identity) as GameObject;
        Unit unit = currentUnit.GetComponent<Unit>();

        //Get rid of all children
        if (unit.getCWObject() != null) Destroy(unit.getCWObject().gameObject);
        if (unit.cw2Object != null) Destroy(unit.cw2Object.gameObject);
        if (unit.turretObjects != null)
        {
            foreach (GameObject turretObject in unit.turretObjects)
            {
                Destroy(turretObject);
            }
        }
        Destroy(unit.shadow.gameObject);
        unit.useTemplate(unitsList.templateDictionary[unitName]);

        //setUnitStats(unit, tile, variant);
        gM.unitDictionary[player.side].Add(unit);
        unit.setSide(player.side);
        unit.setTile(tile);
        tile.setUnit(unit.gameObject);
        
        tile.unitHP = unit.getCurrentHP();
        unit.applyScale();
        unit.matchWeapon("Make Unit");
        unit.makeShadow();
        unit.setOutlineColor(player.color, 10f);
        unit.setOutlineThickness(outlineThickness);
        return unit;
    }

    //Builds a unit from prefab, supposed to be used with BuildUnitCostsButton
    public Unit buildUnit(GameObject unit,Player player, int x, int y, bool ui)
    {
        Tile tile = tiles.getValue(x, y);
        if (tile == null || tile.getUnit() != null || player == null)
            return null;
        currentTile = tile.gameObject;
        currentUnit = Instantiate(unit, new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1), Quaternion.identity) as GameObject;
        //Get rid of all children
        Unit u = currentUnit.GetComponent<Unit>();
        Unit unitScript = unit.GetComponent<Unit>();
        if (unitScript.boostedHP)
        {
            u.setHP(unitScript.initialHP);
            u.setCurrentHP(unitScript.initialCurrentHP);
            u.boostedHP = false;
            u.hpBoost = 0;
        }
        if (u.getCWObject() != null) Destroy(u.getCWObject().gameObject);
        if (u.cw2Object != null) Destroy(u.cw2Object.gameObject);
        if (u.turretObjects != null)
        {
            foreach(GameObject turretObject in u.turretObjects)
            {
                Destroy(turretObject);
            }
        }
        Destroy(u.shadow.gameObject);
        u.turretObjects = new List<GameObject>();
        u.turretObjectScripts = new List<WeaponObject>();
        u.setSide(player.side);
        gM.unitDictionary[player.side].Add(u);


        tile.setUnit(u.gameObject);
        tile.unitHP = u.getCurrentHP();
        u.setTile(tile);
        u.matchWeapon("Build Unit");
        u.startTurn();
        u.setCurrentMP(0);
        u.setCurrentAP(0);
        if (ui)
        {
            u.undoUIScale();
        }
        else
        {
            u.transform.localScale = new Vector3(u.getSizeMultiplier(), u.getSizeMultiplier(), 1);
        }
        u.makeShadow();
        u.setOutlineColor(player.color, 10f);
        u.setOutlineThickness(outlineThickness);
        return u;

    }


    public void makeBuilding(int num, string buildingType, Player player, int x, int y)
    {
        Tile tile = tiles.getValue(x, y);
        if (tile == null || tile.getBuilding() != null)
            return;
        currentTile = tile.gameObject;
        currentBuilding = Instantiate(buildingPrefabs[num], new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -0.5f), Quaternion.identity) as GameObject;
        Building building = currentBuilding.GetComponent<Building>();
        building.gM = gM;
        building.name = buildingType;
        building.setSide(player);
        building.tile = tile;

        currentTile.GetComponent<Tile>().setBuilding(building);
        if (player != null)
        {
            gM.buildingDictionary[player.side].Add(building);
        }
        else
        {
            gM.buildingDictionary["null"].Add(building);
        }

        building.setSize();
        building.setSizeAccordingly();
    }

    public IEnumerator startTiles()
    {
        foreach(Tile tile in tileList)
        {
            StartCoroutine(tile.startTurn());
        }
        yield break;
    }

    public void setSelectedTile(GameObject s)
    {
        if (selectedTile == s)
        {
            return;
        }
        if (selectedTile != null)
        {
            selectedTile.GetComponent<Tile>().deleteSelector();
        }
        selectedTile = s;
    }

    public GameObject getSelectedTile()
    {
        return selectedTile;
    }
}
