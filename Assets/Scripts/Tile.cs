using System;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tile : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public string description;
    public BoardManager board;
    public UIManager ui;
    public GameManager gM;
    public SpriteRenderer renderer;
    public GameObject selector;
    public GameObject selectorPrefab;
    public int spriteNum = 1;
    [SerializeField]
    private GameObject unit;
    public GameObject movingUnit;
    
    private float xSize = 100;
    private float ySize = 100;
    private float xPos = 0;
    private float yPos = 0;
    public string type = "Grassland";
    public string aqueousType = "Land";
    public float moveCostLegged = 1.2f;
    public float moveCostTracked = 1.5f;
    public float moveCostWheeled = 1.2f;
    public float coverBonus = 0f;
    public bool isOutlined = false;
    public bool isAOE;
    public bool acting = false;
    public bool firing = false;
    public int outLineNum = 0;
    public float movesLeft;

    public Weapon weapon;

    [SerializeField]
    private Building building;

    //Variables to prevent overlap via AI
    public GameObject unitToArrive;
    public float unitHP;
    public float expectedDamage;

    public Tile upleft = null;
    public Tile left = null;
    public Tile upright = null;
    public Tile right = null;
    public Tile downright = null;
    public Tile downleft = null;
    public Tile predecessor = null;
    public List<Tile> adjacent;
    public int mapX;
    public int mapY;

    public Unit actingUnit;
    public Unit secondaryUnit;
    public List<Weapon> actingWeapons;
    public List<Weapon> directWeapons;
    public List<Weapon> aoeWeapons;
    public Dictionary<string, float> extraAttributes = new Dictionary<string, float>();

    //Tile Effects
    public GameObject poisonGas;
    public List<GameObject> tileObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        preserve();
        setVariation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void destroyEverything()
    {
        foreach(GameObject tileObject in tileObjects)
        {
            Destroy(tileObject);
        }
    }

    public void makeMoveable()
    {
        if (isOutlined && (outLineNum == 0 || outLineNum == 1))
        {
            return;
        }
        if (isOutlined && outLineNum != 1)
        {
            selector.GetComponent<UnitSelector>().setColor(Color.blue);
            outLineNum = 1;
        }
        else if (!isOutlined)
        {
            outLineNum = 1;
            isOutlined = true;
            selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
            selector.GetComponent<UnitSelector>().setColor(Color.blue);

        }
        gM.availableTiles.Add(this);
    }
    
    public void makeLoadable(Unit unit)
    {
        if (isOutlined && (outLineNum == 0 || outLineNum == 7))
        {
            return;
        }
        if (isOutlined && outLineNum != 7)
        {
            selector.GetComponent<UnitSelector>().setColor(new Color(51, 0, 102));
            outLineNum = 7;
        }
        else if (!isOutlined)
        {
            outLineNum = 7;
            isOutlined = true;
            selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
            selector.GetComponent<UnitSelector>().setColor(new Color(51,0,102));

        }
        gM.availableTiles.Add(this);
        actingUnit = unit;
    }

    public void makeUnloadable(Unit unloader, Unit unloadee)
    {
        if (isOutlined && (outLineNum == 0 || outLineNum == 8))
        {
            return;
        }
        if (isOutlined && outLineNum != 8)
        {
            selector.GetComponent<UnitSelector>().setColor(new Color(51, 0, 102));
            outLineNum = 8;
        }
        else if (!isOutlined)
        {
            outLineNum = 8;
            isOutlined = true;
            selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
            selector.GetComponent<UnitSelector>().setColor(new Color(51, 0, 102));

        }
        gM.availableTiles.Add(this);
        actingUnit = unloader;
        secondaryUnit = unloadee;
    }

    public void makeAttackable(Unit unit, List<Weapon> weapons)
    {
        List<Weapon> directWeapons = unit.getOnlyDirectWeapons(weapons);
        List<Weapon> aoeWeapons = unit.getOnlyAOEWeapons(weapons);
        List<Weapon> directThenAOEWeapons = new List<Weapon>(directWeapons);
        directThenAOEWeapons.AddRange(aoeWeapons);
        this.directWeapons = directWeapons;
        this.aoeWeapons = aoeWeapons;
        foreach (Weapon weapon in directThenAOEWeapons)
        {
            this.weapon = weapon;
            if (weapon.aoe <= 0)
            {
                if (isOutlined && (outLineNum == 0 || outLineNum == 2))
                {
                    return;
                }
                if (isOutlined && outLineNum != 2)
                {
                    selector.GetComponent<UnitSelector>().setColor(Color.red);
                    outLineNum = 2;
                }
                else if (!isOutlined)
                {
                    outLineNum = 2;
                    isOutlined = true;
                    selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
                    selector.GetComponent<UnitSelector>().setColor(Color.red);

                }
            }
            else
            {
                if (isOutlined && (outLineNum == 0 || outLineNum == 4))
                {
                    return;
                }
                if (isOutlined && outLineNum != 4)
                {
                    selector.GetComponent<UnitSelector>().setColor(new Color(0.5f, 0, 0));
                    outLineNum = 4;
                }
                else if (!isOutlined)
                {
                    outLineNum = 4;
                    isOutlined = true;
                    selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
                    selector.GetComponent<UnitSelector>().setColor(new Color(0.5f, 0, 0));

                }
            }
        }
        gM.availableTiles.Add(this);
        actingUnit = unit;

    }

    //Works also for repairing units
    public void makeHealable(Unit unit, List<Weapon> weapons)
    {
        List<Weapon> directWeapons = unit.getOnlyDirectWeapons(weapons);
        List<Weapon> aoeWeapons = unit.getOnlyAOEWeapons(weapons);
        List<Weapon> directThenAOEWeapons = directWeapons;
        directThenAOEWeapons.AddRange(aoeWeapons);
        actingWeapons = directThenAOEWeapons;
        foreach (Weapon weapon in directThenAOEWeapons)
        {
            this.weapon = weapon;
            if (weapon.aoe <= 0)
            {
                if (isOutlined && outLineNum == 5)
                {
                    return;
                }
                if (isOutlined && outLineNum != 5)
                {
                    selector.GetComponent<UnitSelector>().setColor(Color.green);
                    outLineNum = 5;
                }
                else if (!isOutlined)
                {
                    outLineNum = 5;
                    isOutlined = true;
                    selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
                    selector.GetComponent<UnitSelector>().setColor(Color.green);

                }
            }
            else
            {
                if (isOutlined && outLineNum == 6)
                {
                    return;
                }
                if (isOutlined && outLineNum != 6)
                {
                    selector.GetComponent<UnitSelector>().setColor(new Color(0f, 0.5f, 0));
                    outLineNum = 6;
                }
                else if (!isOutlined)
                {
                    outLineNum = 6;
                    isOutlined = true;
                    selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
                    selector.GetComponent<UnitSelector>().setColor(new Color(0f, 0.5f, 0));

                }
            }
        }
        gM.availableTiles.Add(this);
    }

    private void OnMouseEnter()
    {
        if (ui.selectingAction || gM.currentPlayer != gM.humanSide || gM.animationInProgress)
        {
            return;
        }

        //If we have an AOE weapon then show affected tiles
        if (isOutlined && outLineNum == 4)
        {
            foreach (Tile tile in adjacent)
            {
                if (tile != null)
                {

                    if (tile.isOutlined && tile.outLineNum == 4)
                    {
                        tile.isAOE = true;

                    }
                    else
                    {
                        tile.isAOE = false;
                        tile.selector = Instantiate(selectorPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z - 9), Quaternion.identity) as GameObject;
                        gM.availableTiles.Add(tile);
                    }
                    tile.selector.GetComponent<UnitSelector>().setColor(new Color(1f, 0, 0));


                }
            }
        }
        else if (isOutlined && outLineNum == 6) {
            foreach (Tile tile in adjacent)
            {
                if (tile != null)
                {

                    if (tile.isOutlined && tile.outLineNum == 6)
                    {
                        tile.isAOE = true;

                    }
                    else
                    {
                        tile.isAOE = false;
                        tile.selector = Instantiate(selectorPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z - 9), Quaternion.identity) as GameObject;
                        gM.availableTiles.Add(tile);
                    }
                    tile.selector.GetComponent<UnitSelector>().setColor(new Color(0f, 1f, 0));


                }
            }
        }
    }

    //0 is select unit
    //1 is attack, 4 is AOE attack
    //2 is move
    //3 is build unit
    //5 is heal, 6 is AOE heal
    //7 is load units
    //8 is unload units

    //Make the tile selected when we click on it
    private void OnMouseOver()
    {

        //Left Click

        if (ui.selectingAction || gM.currentPlayer != gM.humanSide || gM.animationInProgress)
        {
            return;
        }
        


        if (Input.GetMouseButtonDown(0))
        {

            //Debug.Log("We were clicked with outlined:" + isOutlined + " and outline number " + outLineNum);
            if (isOutlined && outLineNum == 1)
            {
                //Handle moving unit
                Tile start = board.getSelectedTile().GetComponent<Tile>();
                StartCoroutine(gM.moveUnitAsPlayer(start.getUnit().GetComponent<Unit>(), start, this));
                gM.clearAvailableTiles();
            }
            else if (isOutlined && outLineNum == 2 || outLineNum == 4)
            {
                //Handle attacking unit
                Tile start = board.getSelectedTile().GetComponent<Tile>();
                
                if (start.getUnit() != null) {
                        if (getUnit() != null)
                        {
                            string attackingSide = start.getUnitScript().getSide();
                            string defendingSide = getUnitScript().getSide();
                            if (attackingSide != defendingSide)
                            {
                                StartCoroutine(gM.attackEnemyAsPlayer(start.getUnitScript(), getUnitScript(), directWeapons));
                            }
                        }
                        StartCoroutine(gM.doAOEAttack(start.getUnitScript(), start, this, aoeWeapons));
                }
                gM.clearAvailableTiles();
            }
            else if (isOutlined && outLineNum == 5 || outLineNum == 6)
            {
                //Handle healing unit
                Tile start = board.getSelectedTile().GetComponent<Tile>();
                
                if (start.getUnit() != null)
                {
                    Unit unit = start.getUnitScript();
                    List<Weapon> healingWeapons;
                    if (unit.healing)
                    {
                        if (unit.repairing)
                        {
                            healingWeapons = unit.getAllHealRepairHandWeapons();
                        }
                        else
                        {
                            healingWeapons = unit.getAllHealHandWeapons();
                        }
                    }
                    else
                    {
                        healingWeapons = unit.getAllRepairHandWeapons();
                    }
                    //Debug.Log(healingWeapons[0]);
                    List<Weapon> directHealingWeapons = unit.getOnlyDirectWeapons(healingWeapons);
                    List<Weapon> aoeHealingWeapons = unit.getOnlyAOEWeapons(healingWeapons);

                        if (getUnit() != null)
                        {
                            string attackingSide = start.getUnitScript().getSide();
                            string defendingSide = getUnitScript().getSide();
                            if (attackingSide == defendingSide)
                            {
                                //Debug.Log("Healing");
                                StartCoroutine(gM.healAllyAsPlayer(start.getUnitScript(), getUnitScript(),  directHealingWeapons));

                            }
                        }

                        StartCoroutine(gM.doAOEHeal(start.getUnitScript(), start, this, aoeHealingWeapons));

                    //Stop healing/repairing
                    unit.healing = false;
                    unit.repairing = false;
                }
                gM.clearAvailableTiles();
            }
            else if (isOutlined && outLineNum == 7)
            {
                
                StartCoroutine(actingUnit.loadUnit(getUnitScript(),gM.doULAnimations));
                gM.clearAvailableTilesExceptSelected();
            }
            else if (isOutlined && outLineNum == 8)
            {
                StartCoroutine(actingUnit.unloadUnit(secondaryUnit, this, gM.doUUnLAnimations));
                gM.clearAvailableTiles();
            }
            else if (isOutlined && outLineNum == 3)
            {

                ui.makeUnitBuilderMenu();
                gM.clearAvailableTiles();
            }
            else if (isOutlined && outLineNum != 0)
            {
                outLineNum = 0;
                gM.clearAvailableTiles();
                ui.updateStats(this, getUnitScript(), building);
            }
            else if (!isOutlined)
            {
                outLineNum = 0;
                isOutlined = true;
                selector = Instantiate(selectorPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 9), Quaternion.identity) as GameObject;
                selector.GetComponent<UnitSelector>().setColor(Color.yellow);
                gM.clearAvailableTiles();
                ui.updateStats(this, getUnitScript(), building);
                gM.selectedTile = this;

                //Switch to outline number 3 (build) if we select a factory
                if (building != null && building.makesUnits && building.side == gM.humanSide && building.enabled && getUnitScript() == null)
                {
                    outLineNum = 3;
                    selector.GetComponent<UnitSelector>().setColor(Color.cyan);
                }
                //if (!gM.availableTiles.Contains(this))
                //{
                    //gM.availableTiles.Add(this);
                //}
            }
            ui.destroyBattleMenu();
            board.setSelectedTile(gameObject);
            acting = false;
        }
        //Right Click
        else if (Input.GetMouseButtonDown(1))
        {
            if (unit != null)
            {
                string unitSide = unit.GetComponent<Unit>().getSide();
                if (isOutlined && outLineNum == 0 && unitSide == gM.humanSide && gM.humanSide == gM.currentPlayer)
                {
                    if (!acting)
                    {
                        Vector3 adjacent = new Vector3(transform.position.x + 200, transform.position.y, -9);
                        ui.makeBattleMenu(this, adjacent);
                        acting = true;
                    }
                    else
                    {
                        acting = false;
                        ui.destroyBattleMenu();
                    }
                }
                gM.clearAvailableTiles();
            }
            else if (outLineNum == 3)
            {
                ui.makeUnitBuilderMenu();
                gM.clearAvailableTiles();
            }
        }
    }

    private void OnMouseExit()
    {
        if (isOutlined && outLineNum == 4)
        {
            foreach (Tile tile in adjacent)
            {
                if (tile != null)
                {
                    if (tile.isAOE)
                    {
                        tile.selector.GetComponent<UnitSelector>().setColor(new Color(0.5f, 0, 0));
                    }
                    else
                    {
                        tile.deleteSelector();
                    }
                }
            }
        }
        else if (isOutlined && outLineNum == 6)
        {
            foreach (Tile tile in adjacent)
            {
                if (tile != null)
                {
                    if (tile.isAOE)
                    {
                        tile.selector.GetComponent<UnitSelector>().setColor(new Color(0f, 0.5f, 0));
                    }
                    else
                    {
                        tile.deleteSelector();
                    }
                }
            }
        }
    }



    //Gets the adjacent tiles and returns the previous adjacent tiles
    public List<Tile> setAdjacent()
    {
        List<Tile> temp = new List<Tile>(adjacent);
        adjacent = new List<Tile>();
        adjacent.Add(upleft);
        adjacent.Add(upright);
        adjacent.Add(right);
        adjacent.Add(downright);
        adjacent.Add(downleft);
        adjacent.Add(left);
        //Get rid of all null items
        temp.RemoveAll(item => item == null);
        return temp;
    }

    //Gets the adjacent tiles
    public List<Tile> getAdjacent()
    {
        if (adjacent == null || adjacent.Count == 0)
            setAdjacent();
        return adjacent;
    }

    public Tile getAdjacentTileOnSide(string side)
    {
        switch (side)
        {
            case "upleft":
                return upleft;
            case "upright":
                return upright;
            case "left":
                return left;
            case "right":
                return right;
            case "downleft":
                return downleft;
            case "downright":
                return downright;

        }
        return null;
    }

    //Gets the string relating to the adjacent tile
    public string getAdjacentTileSide(Tile tile)
    {
        if (tile == upleft) return "upleft";
        if (tile == upright) return "upright";
        if (tile == left) return "left";
        if (tile == right) return "right";
        if (tile == downleft) return "downleft";
        if (tile == downright) return "downright";
        return "";
    }

    //Determines if tile is adjacent
    public bool isAdjacent(Tile t)
    {
        return adjacent.Contains(t);
    }

    //Decide if we could move to the tile disreguarding anything inbetween our tiles
    public bool canMoveTo(Tile dest, bool on)
    {
        if (dest == null)
        {
            return false;
        }
        if ((unit != null && dest.getUnit() != null) && (dest.getUnitScript().getTeam() != getUnitScript().getTeam() || dest.unitToArrive != null))
        {
            return false;
        }
        //If we care about the unit on the destination tile, return false if there is any unit on the destination tile.
        if ((on && dest.unit != null))
        {
            return false;
        }
        return true;
    }

    public bool canMoveToWithUnit(Unit u, Tile dest, bool on)
    {
        if (dest == null)
        {
            return false;
        }
        if ((u != null && dest.getUnit() != null) && (dest.getUnitScript().getTeam() != u.getTeam() || dest.unitToArrive != null))
        {
            return false;
        }
        //If we care about the unit on the destination tile, return false if there is any unit on the destination tile.
        if ((on && dest.unit != null))
        {
            return false;
        }
        return true;
    }

    //Decide if we could move to the tile if it's adjacent
    public bool canMoveToAdjacent(Tile dest, bool on)
    {
        if (!canMoveTo(dest, on))
        {
            return false;
        }
        if (!isAdjacent(dest))
        {
            return false;
        }
        return true;
    }

    //Decide if it's possible for the unit to move to adjacent tiles
    public bool canMoveAtAllAdjacent()
    {
        List<Tile> adjTiles = getAdjacent();
        foreach (Tile t in adjTiles)
        {
            if (canMoveTo(t, true)) return true; 
        }
        return false;
    }

    //Decide if we could move to the adjacent tiles at all given our current MP
    public bool couldMoveToAnyAdjacent()
    {
        List<Tile> adjTiles = getAdjacent();
        //Debug.Log(adjTiles[0]);
        foreach (Tile t in adjTiles)
        {
            if (t == null) continue;
            //Debug.Log("Tile move cost is "+t.moveCost);
            //Debug.Log("Unit move cost is "+getUnitScript().getCurrentMP());
            if (Math.Round(t.getMoveCost(getUnitScript()),3) <= Math.Round(getUnitScript().getCurrentMP(),3)) return true;
        }
        return false;
    }

    //Sets the adjacent node, t, that is to the direction, dir, of this node
    public void setAdj(Tile t, string dir)
    {
        switch(dir)
        {
            case "upleft":
                upleft = t;
                if (t!= null)
                    t.downright = this;
                break;
            case "upright":
                upright = t;
                if (t != null)
                    t.downleft = this;
                break;
            case "right":
                right = t;
                if (t != null)
                    t.left = this;
                break;
            case "downright":
                downright = t;
                if (t != null)
                    t.upleft = this;
                break;
            case "downleft":
                downleft = t;
                if (t != null)
                    t.upright = this;
                break;
            case "left":
                left = t;
                if (t != null)
                    t.right = this;
                break;
        }
    }

    public float getMoveCost(Unit unit)
    {
        if (unit == null) return 1f;
        else
        {
            switch (unit.getMovementType())
            {
                case "Legged":
                    return moveCostLegged;
                case "Tracked":
                    return moveCostTracked;
                case "Wheeled":
                    return moveCostWheeled;
                default:
                    return 1f;
            }
        }
    }

    //In the case of roads
    public float getMoveCost(Unit unit, Tile dest)
    {
        if (unit == null) return 1f;
        else
        {
            switch (unit.getMovementType())
            {
                case "Legged":
                    return moveCostLegged;
                case "Tracked":
                    return moveCostTracked;
                case "Wheeled":
                    return moveCostWheeled;
                default:
                    return 1f;
            }
        }
    }


    public void deleteSelector()
    {
        isOutlined = false;
        isAOE = false;
        outLineNum = 0;
        if (selector != null)
        Destroy(selector);
        selector = null;
    }

    public void setXPos(float x)
    {
        xPos = x;
    }

    public void setYPos(float y)
    {
        yPos = y;
    }

    public float getXPos()
    {
        return xPos;
    }

    public float getYPos()
    {
        return yPos;
    }

    public string getPos()
    {
        return "(" + xPos + "," + yPos + ")";
    }

    public string getMapPos()
    {
        return "(" + mapX + "," + mapY + ")";
    }

    public void setXSize(float x)
    {
        xSize = x;
        scale();
    }

    public void setYSize(float y)
    {
        ySize = y;
        scale();
    }

    public float getX()
    {
        return xSize;
    }

    public float getY()
    {
        return ySize;
    }

    public void scale()
    {
        Vector3 scale = new Vector3(xSize, ySize, 1f);
        transform.localScale = scale;

    }

    public void move()
    {
        Vector3 pos = new Vector3(xPos, yPos, 0);
        transform.position = pos;
    }

    public void updateSprite(int num)
    {
        spriteNum = num;
        renderer.sprite = sprites[num];
    }

    public void preserve()
    {
        xPos = transform.position.x;
        yPos = transform.position.y;
    }

    public void setUnit(GameObject u)
    {
        unit = u;
    }

    public GameObject getUnit()
    {
        return unit;
    }

    public Unit getUnitScript()
    {
        if (unit != null)
        {
            return unit.GetComponent<Unit>();
        }
        return null;
    }

    public int getTeam()
    {
        //strings are treated like primitive objects -> therefore no equals method needed
        if (unit == null) return -1;
        string side = getUnitScript().getSide();
        for (int i = 0; i < gM.teams.Count; i++)
        {
            if (gM.teams[i].Contains(side))
            {
                return i;
            }
        }
        return -1;
    }

    public IEnumerator startTurn()
    {
        if (extraAttributes != null)
        {
            if (extraAttributes.ContainsKey("Poison Gas"))
            {
                extraAttributes["Poison Gas"] -= 1;
                if (extraAttributes["Poison Gas"] == 0)
                {
                    extraAttributes.Remove("Poison Gas");
                    if (poisonGas != null)
                    {
                        tileObjects.Remove(poisonGas);
                        Destroy(poisonGas);
                    }
                }
            }
        }
        if (unit != null)
        {
            yield return StartCoroutine(enterUnit(getUnitScript()));
        }
    }

    public IEnumerator addEffect(string effect, float val, bool overrideCurrent)
    {
        if (extraAttributes != null)
        {
            if (!extraAttributes.ContainsKey(effect))
            {
                extraAttributes.Add(effect, val);
            }
            else if (overrideCurrent == true)
            {
                extraAttributes[effect] = val;
            }
        }
        else
        {
            extraAttributes = new Dictionary<string, float>();
            extraAttributes.Add(effect, val);
        }
        if (effect == "Poison Gas" && val > 0 && poisonGas == null)
        {
            poisonGas = Instantiate(board.tileEffectPrefabs[0], new Vector3(transform.position.x, transform.position.y, transform.position.z - 3), Quaternion.identity) as GameObject;
            tileObjects.Add(poisonGas);
            poisonGas.GetComponent<PoisonGasCloud>().recordPos(poisonGas.transform.position);
            poisonGas.GetComponent<PoisonGasCloud>().animating = true;
            if (unit != null)
            {
                Unit unit = getUnitScript();
                if (unit.getPoisonHPEffect() != 0 && unit.getCurrentHP() > 0)
                {
                    unit.addEffect("Poisoned", 3, true);
                    yield return StartCoroutine(unit.showEffect(ui.attributeSprites[5], "Poisoned"));
                }
            }

        }
        yield return null;
    }

    //Applies effects to a unit when it enters a tile or 
    public IEnumerator enterUnit(Unit unit)
    {
        if (extraAttributes.ContainsKey("Poison Gas"))
        {
            if (unit.getPoisonHPEffect() != 0 && unit.getCurrentHP() > 0)
            {
                unit.addEffect("Poisoned", 3, true);
                yield return StartCoroutine(unit.showEffect(ui.attributeSprites[5], "Poisoned"));
            }
        }
    }

    public void setBuilding (Building b)
    {
        building = b;
        building.tile = this;
    }

    public void setVariation()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Count)];
    }
    public Building getBuilding()
    {
        return building;
    }
    public string getUnitSide()
    {
        return unit.GetComponent<Unit>().getSide();
    }
    override
    public string ToString()
    {
        return "(" + transform.position + ")";
    }
}
