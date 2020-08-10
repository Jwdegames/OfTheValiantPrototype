using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class UnitActionMenu : MonoBehaviour
{
    //prefab for unit action button
    public GameObject uab;
    public GameObject unitObject;
    public Unit unit;
    public Tile tile;
    public GameObject tempButtonObject;
    public UnitActionButton tempButton;
    public UIManager ui;
    public GameManager gM;
    public float buttonSizeX = 0.5f, buttonSizeY = 0.5f, xScale = 0.75f, yScale = 0.75f, xMenuOffset = 15f, xButtonOffset = 3f, yButtonScale = 6f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUnit()
    {
        unit = unitObject.GetComponent<Unit>();
    }

    //These 2 methods are necessary in both the action menu and the buttons
    private void OnMouseOver()
    {
        //Debug.Log("Moused over");
        ui.selectingAction = true;
    }

    private void OnMouseExit()
    {
        ui.selectingAction = false;
    }

    //Make all of our buttons
    public void makeButtons()
    {
        
        int count = unit.getPossibleActions().Count;
        if (count > 1)
        {
            xScale *= 2;
        }
        transform.localScale = new Vector3(xScale, transform.localScale.y * (yScale * Mathf.Ceil(count / 2f)));
        //RectTransform rect = GetComponent<RectTransform>();
        //rect.position.Set(unitObject.transform.position.x, unitObject.transform.position.y, transform.position.z);
        transform.position = new Vector3(unitObject.transform.position.x+xMenuOffset,unitObject.transform.position.y,transform.position.z);
        //Debug.Log(rect.position);
        for (int i = 0; i < count; i++)
        {
            //if (i == 2) break;
            if (count == 1)
            {
                tempButtonObject = Instantiate(uab) as GameObject;

                tempButton = tempButtonObject.GetComponent<UnitActionButton>();
                //Debug.Log("Setting uab type");
                tempButton.setType(unit.getPossibleActions()[i]);
                tempButton.setSize(buttonSizeX, buttonSizeY);
                tempButton.ui = ui;
                tempButton.gM = gM;
                tempButton.tile = tile;
                tempButton.unit = unit;
                tempButtonObject.transform.SetParent(gameObject.transform);

            }
            else
            {
                Vector3 pos;
                float centerRow = Mathf.Floor((count - 1) / 2f) / 2f;
                float currentRow = Mathf.Floor(i / 2) * 1f;
                float yDist = (centerRow - currentRow) * yButtonScale;
                //Debug.Log(currentRow + " vs " + centerRow);
                if (i % 2 == 1)
                {
                    pos = new Vector3(transform.position.x + xButtonOffset, transform.position.y + yDist, 0);
                }
                else
                {
                    float xDist = (i % 2 == 0) ? -125f : 125f;
                    pos = new Vector3(transform.position.x - xButtonOffset, transform.position.y + yDist, 0);
                }
                
                tempButtonObject = Instantiate(uab, pos, Quaternion.identity) as GameObject;

                tempButton = tempButtonObject.GetComponent<UnitActionButton>();
                //Debug.Log("Setting uab type");
                tempButton.setType(unit.getPossibleActions()[i]);
                tempButton.setSize(buttonSizeX, buttonSizeY);
                tempButton.ui = ui;
                tempButton.gM = gM;
                tempButton.tile = tile;
                tempButton.unit = unit;
                tempButtonObject.transform.SetParent(gameObject.transform);

                //Debug.Log(i+":Position for " + unit.getPossibleActions()[i]+ " is " + pos + " vs " + transform.position);
                //Debug.Log("Buttong positon: " + tempButtonObject.transform.position);
            }
            string currentAction = unit.getPossibleActions()[i];
            if (currentAction == "Move" && !tile.couldMoveToAnyAdjacent()) tempButton.disable();
            if (currentAction != "Move")
            {
                if (currentAction == "Attack")
                {
                    List<Tile> attackables = gM.getAttackTiles(unit, unit.getTile());
                    //gM.printPath(attackables);
                    if (unit.getCurrentAP() <= 0f || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Sentry" || currentAction == "Fortify")
                {
                    if (unit.getCurrentAP() < unit.getAP())
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Heal")
                {
                    List<Tile> healables = gM.getHealTiles(unit, unit.getTile(), unit.getAllHealHandWeapons());
                    if (unit.getCurrentAP() <= 0 || healables == null || healables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Repair")
                {
                    List<Tile> healables = gM.getHealTiles(unit, tile, unit.getAllRepairHandWeapons());
                    if (unit.getCurrentAP() <= 0 || healables == null || healables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Deploy Drones")
                {
                    if (unit.getCurrentAP() <= 0 || !unit.canDeploy())
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Load Units")
                {
                    if (unit.getCurrentAP() <= 0 || !unit.canLoadUnits())
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Unload Units")
                {
                    if (unit.getCurrentAP() <= 0 || !unit.canUnloadUnits())
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Fire Turret 1")
                {
                    List<Tile> attackables = gM.getAttackTilesWithWeapons(unit, unit.getTile(),new List<Weapon>() { unit.turrets[0]});
                    if (unit.getCurrentAP() <= 0 || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Fire Turret 2")
                {
                    List<Tile> attackables = gM.getAttackTilesWithWeapons(unit, unit.getTile(), new List<Weapon>() { unit.turrets[1] });
                    if (unit.getCurrentAP() <= 0 || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Fire Turret 3")
                {
                    List<Tile> attackables = gM.getAttackTilesWithWeapons(unit, unit.getTile(), new List<Weapon>() { unit.turrets[2] });
                    if (unit.getCurrentAP() <= 0 || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Fire Turret 4")
                {
                    List<Tile> attackables = gM.getAttackTilesWithWeapons(unit, unit.getTile(), new List<Weapon>() { unit.turrets[3] });
                    if (unit.getCurrentAP() <= 0 || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else if (currentAction == "Fire Turret 5")
                {
                    List<Tile> attackables = gM.getAttackTilesWithWeapons(unit, unit.getTile(), new List<Weapon>() { unit.turrets[4] });
                    if (unit.getCurrentAP() <= 0 || attackables == null || attackables.Count <= 0)
                    {
                        tempButton.disable();
                    }
                }
                else
                {
                    if (unit.getCurrentAP() <= 0f)
                    tempButton.disable();
                    //Prevent capturing if there is no building we can capture
                    if (currentAction == "Capture" && (unit.getTile().getBuilding() == null || unit.getTile().getBuilding().side == unit.getSide()))
                    {
                        tempButton.disable();
                    }
                }
            }
        }
    }

}

