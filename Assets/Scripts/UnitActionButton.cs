using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.CodeDom;

public class UnitActionButton : MonoBehaviour, IPointerClickHandler
{
    public Sprite activated;
    public Sprite deactivated;
    public Sprite move;
    public Sprite attack;
    public Sprite fortify;
    public Sprite sentry;
    public Sprite rotate;
    public Sprite repair;
    public Sprite heal;
    public Sprite capture;
    public Sprite item;
    public Sprite changeWeapons;
    public Sprite fireTurret1;
    public Sprite fireTurret2;
    public Sprite fireTurret3;
    public Sprite fireTurret4;
    public Sprite fireTurret5;
    public Sprite loadUnits;
    public Sprite deployUnits;
    public Sprite deployDrones;
    public Sprite toggleJetpack;
    public Sprite launchSlime;
    public Sprite cRally;
    public Sprite cBolster;
    public Sprite cFocus;
    public Sprite cResetAlly;
    public Sprite cPressure;
    public Sprite cCaution;
    public Sprite cInspire;
    public Sprite cEnergize;
    public Sprite cAnger;
    public Sprite cConfuse;
    public Sprite cDebuff;
    public Sprite cIntimidate;
    public float apCost;
    public Unit unit;
    public Image image;
    public Image buttonImage;
    public GameObject imageObject;
    public GameObject unitObject;
    public string type = "Move";
    public float imageFactor = 7 / 8f;
    public Tile tile;
    public UIManager ui;
    public GameManager gM;

    public GameObject tooltipBox;
    public Text tooltipText;
    public Image tooltipArrow;

    public UnitInfoTooltip infoTooltip;
    //public Vector2 infoTooltipActivatorPos = new Vector2(8.849998f, 2.025301f);

    // Use this for initialization
    void Awake()
    {
        image = imageObject.GetComponent<Image>();
        buttonImage = GetComponent<Image>();
        //Vector3 posVector = new Vector3(infoTooltipActivatorPos.x, infoTooltipActivatorPos.y, transform.position.z);
        //infoTooltipActivator = Instantiate(infoTooltipPrefab, posVector, Quaternion.identity) as GameObject;
        //infoTooltipActivator.transform.SetParent(transform);
        //infoTooltipActivator.GetComponent<RectTransform>().anchoredPosition = infoTooltipActivatorPos;
        //infoTooltip = infoTooltipActivator.GetComponent<UnitInfoTooltip>();
        //infoTooltip.transform.SetParent(null);
        //Debug.Log(image);
        //Debug.Log(imageObject);
        //Debug.Log("Getting image data");
        applyImageScale();
        setType(type);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setType(string t)
    {
        type = t;
        //Debug.Log(image);
        switch (type)
        {
            case "Move":
                image.sprite = move;
                apCost = 0;
                updateInfoTooltip("Move unit to a desired tile");
                break;
            case "Attack":
                image.sprite = attack;
                apCost = 1;
                updateInfoTooltip("Attack an enemy unit with the main weapons");
                break;
            case "Fortify":
                image.sprite = fortify;
                //apCost = unit.getAP();
                updateInfoTooltip("Use up all AP and reduce the damage taken");
                break;
            case "Sentry":
                image.sprite = sentry;
                //apCost = unit.getAP();
                updateInfoTooltip("Use up all AP and attack enemies first instead of counterattacking");
                break;
            case "Repair":
                image.sprite = repair;
                apCost = 1;
                updateInfoTooltip("Repair an ally");
                break;
            case "Heal":
                image.sprite = heal;
                apCost = 1;
                updateInfoTooltip("Heal an ally");
                break;
            case "Capture":
                image.sprite = capture;
                //apCost = unit.getAP();
                updateInfoTooltip("Capture a building");
                break;
            case "Deploy Drones":
                image.sprite = deployDrones;
                apCost = 1;
                updateInfoTooltip("Deploy friendly drones");
                break;
            case "Load Units":
                image.sprite = loadUnits;
                apCost = 1;
                updateInfoTooltip("Load allied units up");
                break;
            case "Unload Units":
                image.sprite = deployUnits;
                apCost = 1;
                updateInfoTooltip("Unload allied units");
                break;
            case "Fire Turret 1":
                image.sprite = fireTurret1;
                apCost = 1;
                updateInfoTooltip("Attack an enemy with Turret 1");
                break;
            case "Fire Turret 2":
                image.sprite = fireTurret2;
                updateInfoTooltip("Attack an enemy with Turret 2");
                apCost = 1;
                break;
            case "Fire Turret 3":
                image.sprite = fireTurret3;
                updateInfoTooltip("Attack an enemy with Turret 3");
                apCost = 1;
                break;
            case "Fire Turret 4":
                image.sprite = fireTurret4;
                updateInfoTooltip("Attack an enemy with Turret 4");
                apCost = 1;
                break;
            case "Fire Turret 5":
                image.sprite = fireTurret5;
                updateInfoTooltip("Attack an enemy with Turret 5");
                apCost = 1;
                break;
            case "Toggle Jetpack":
                image.sprite = toggleJetpack;
                apCost = 1;
                updateInfoTooltip("Use a jetpack toggle point and toggle the jetpack on or off");
                break;
            case "Launch Slime":
                image.sprite = launchSlime;
                apCost = 1;
                updateInfoTooltip("Launch a slime at a tile");
                break;
            case "Rally":
                image.sprite = cRally;
                updateInfoTooltip("Boost allied units' attack and MP within 2 tiles for 3 turns");
                break;
            case "Bolster":
                image.sprite = cBolster;
                updateInfoTooltip("Boost an allied unit's attack and MP for 3 turns");
                break;
            case "Focus":
                image.sprite = cFocus;
                updateInfoTooltip("Boost an allied unit's attack greatly for 3 turns");
                break;
            case "Reset Ally":
                image.sprite = cResetAlly;
                updateInfoTooltip("Reset an allied unit's AP");
                break;
            case "Pressure":
                image.sprite = cPressure;
                updateInfoTooltip("Boost an allied unit's speed greatly for 3 turns");
                break;
            case "Caution":
                image.sprite = cCaution;
                updateInfoTooltip("Make an ally take less damage at the cost of MP for 3 turns");
                break;
            case "Inspire":
                image.sprite = cInspire;
                updateInfoTooltip("Boost an allied unit's speed, defense, and attack for 3 turns");
                break;
            case "Energize":
                image.sprite = cEnergize;
                updateInfoTooltip("Reset an allied unit's MP");
                break;
            case "Anger":
                image.sprite = cAnger;
                updateInfoTooltip("Decrease an enemy unit's defense at the cost of increasing their attack for 3 turns.");
                break;
            case "Confuse":
                image.sprite = cConfuse;
                updateInfoTooltip("Make an enemy unit do random things for 3 turns");
                break;
            case "Debuff":
                image.sprite = cDebuff;
                updateInfoTooltip("Decrease an enemy unit's attack, defense, and speed for 3 turns");
                break;
            case "Intimidate":
                image.sprite = cIntimidate;
                updateInfoTooltip("Decrease the attack of all enemies within 2 tiles for 3 turns");
                break;
            default:
                image.sprite = move;
                updateInfoTooltip("Move unit to a desired tile");
                break;
        }
    }

    public void updateInfoTooltip(string text)
    {
        if (infoTooltip != null)
        {
            infoTooltip.setInfoToolTipText(text);
        }
    }

    public void getMoveables()
    {
        //unit.resetCurrentMP();
        List<Tile> moveTiles = gM.getMoveTiles(tile);
        //moveTiles.RemoveAll(tempTile => tempTile.getUnit());
        foreach(Tile tile in moveTiles)
        {
            tile.makeMoveable();
        }
        //canMoveTo(tile, tile, tile, tile.movesLeft, true);



    }

    //WARNING, if moves is too high, this function will do infinite recursion!!!!!
    //Use djakstra's algorithm instead to prevent recursion
    /*public void canMoveTo(Tile origin, Tile start, Tile dest, float moves, bool first)
    {

        if (dest == null)
        {
            return;
        }
        if (first)
        {
            canMoveTo(tile, tile, dest.upleft, moves, false);
            canMoveTo(tile, tile, dest.upright, moves, false);
            canMoveTo(tile, tile, dest.right, moves, false);
            canMoveTo(tile, tile, dest.downright, moves, false);
            canMoveTo(tile, tile, dest.downleft, moves, false);
            canMoveTo(tile, tile, dest.left, moves, false);
        }
        else
        {
            moves -= dest.getMoveCost(origin.getUnitScript());
            moves = (float)Math.Round(moves, 3);
            //Debug.Log("Before checking: "+moves);
            if (moves >= 0 && origin.canMoveTo(dest, true))
            {
                //Debug.Log("After checking: " + moves);
                dest.makeMoveable();
                if (moves > 0)
                {
                    canMoveTo(tile, dest, dest.upleft, moves, false);
                    canMoveTo(tile, dest, dest.upright, moves, false);
                    canMoveTo(tile, dest, dest.right, moves, false);
                    canMoveTo(tile, dest, dest.downright, moves, false);
                    canMoveTo(tile, dest, dest.downleft, moves, false);
                    canMoveTo(tile, dest, dest.left, moves, false);
                }
            }
        }
    }*/

    public void getAttackables()
    {
        //We need to get attack tiles for each weapon
        List<Weapon> attackWeapons = unit.getAllDamageHandWeapons();
        foreach (Weapon weapon in attackWeapons) {
            List<Tile> attackTiles = gM.getAttackTilesWithWeapons(unit, tile, new List<Weapon> { weapon });


            foreach (Tile t in attackTiles)
            {
                t.makeAttackable(unit, new List<Weapon>() { weapon });
            }
        }
    }

    public void getTurretAttackables(int turret)
    {
        List<Weapon> turrets = new List<Weapon>() { unit.turrets[turret] };
        List<Tile> attackTiles = gM.getAttackTilesWithWeapons(unit, tile, turrets);


        foreach (Tile t in attackTiles)
        {
            t.makeAttackable(unit, turrets);
        }
    }

    public void getHealables()
    {
        List<Tile> healTiles = gM.getHealTiles(unit, tile, unit.getAllHealHandWeapons());
        foreach (Tile t in healTiles)
        {
            t.makeHealable(unit, unit.getAllHealHandWeapons());
        }
        unit.healing = true;

    }

    public void getRepairables()
    {
        List<Tile> healTiles = gM.getHealTiles(unit, tile, unit.getAllRepairHandWeapons());
        foreach (Tile t in healTiles)
        {
            t.makeHealable(unit,unit.getAllRepairHandWeapons());
        }
        unit.repairing = true;
    }

    public void getLoadables()
    {
        if (!unit.canLoadUnits()) return;
        switch (unit.loadType) 
        {
            case "Adjacent":
                List<Tile> adjacentTiles = tile.getAdjacent();
                foreach (Tile t in adjacentTiles)
                {
                    if (t != null && t.getUnit() != null && t.getUnitScript().getSide() == unit.getSide())
                    {
                        t.makeLoadable(unit);
                    }
                }
                break;
        }
    }

    public void beginUnloading()
    {
        ui.makeUnloadingMenu(unit);
    }

    public void getHealRepairables()
    {
        List<Tile> healTiles = gM.getHealTiles(unit, tile, unit.getAllHealRepairHandWeapons());
        foreach (Tile t in healTiles)
        {
            t.makeHealable(unit, unit.getAllHealRepairHandWeapons());
        }
        unit.healing = true;
        unit.repairing = true;
    }

        //These 2 methods are necessary in both the action menu and the buttons
        /*public void OnPointerEnter(PointerEventData eventData)
        {
            ui.selectingAction = true;
            if (buttonImage.sprite.Equals(activated))
            {
                tooltipBox.SetActive(true);
                tooltipText.text = type;
                tooltipArrow.enabled = true;
            }
            Debug.Log("Moused over");
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            ui.selectingAction = false;
            if (buttonImage.sprite.Equals(activated))
            {
                tooltipBox.SetActive(false);
                tooltipArrow.enabled = false;
            }
        }*/

        public void mouseIn()
    {
        ui.selectingAction = true;
        if (buttonImage.sprite.Equals(activated))
        {
            tooltipBox.SetActive(true);
            string temp = type;
            if (type == "Move")
            {
                temp += "(Cost: MP Used After Tile Selected)";
            }
            else if (type == "Toggle Jetpack")
            {
                temp += "(Cost: 1 Jetpack Toggle)";
            }
            else if (type == "Fortify" || type == "Sentry" || type == "Capture")
            {
                temp += "(Cost: "+unit.getAP()+" AP)";
            }
            else if (type == "Attack" || type == "Fire Turret 1" || type == "Fire Turret 2" || type == "Fire Turret 3" || type == "Fire Turret 4" || type == "Fire Turret 5")
            {
                temp += "(Cost: 1 AP After Tile Selected)";
            }
            else
            {
                temp += "(Cost: 1 AP After Action Is Completed)";
            }
            tooltipText.text = temp;
            tooltipArrow.enabled = true;
        }
        //Debug.Log("Moused over");
    }

    public void mouseOut()
    {
        ui.selectingAction = false;
        if (buttonImage.sprite.Equals(activated))
        {
            tooltipBox.SetActive(false);
            tooltipArrow.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    /*
    //These 2 methods are necessary in both the action menu and the buttons
    private void OnMouseOver()
    {
        
    }

    private void OnMouseExit()
    {
       
    }*/


    //Do the unit action when we are clicked
    //NOTICE: We can only keep running coroutines if the script that calls the coroutine isn't destroyed, therefore, use starter methods
    public void doAction()
    {
        if (buttonImage.sprite.Equals(deactivated)) return;
        ui.destroyBattleMenu();
        ui.updateStats(null, null, null);
        ui.selectingAction = false;
        switch (type)
        {
            case "Move":
                getMoveables();
                break;
            case "Attack":
                getAttackables();
                break;
            case "Fortify":
                unit.startActuallyGuard();

                gM.selectedTile.deleteSelector();
                break;
            case "Sentry":
                unit.startActuallySentry();
                gM.selectedTile.deleteSelector();
                break;
            case "Capture":
                unit.startActuallyCapture();
                gM.selectedTile.deleteSelector();
                break;
            case "Heal":
                getHealables();
                break;
            case "Repair":
                getRepairables();
                break;
            case "Deploy Drones":
                unit.deployDrones();
                break;
            //unit.useAP(1);
            case "Load Units":
                getLoadables();
                break;
            case "Unload Units":
                beginUnloading();
                break;
            case "Fire Turret 1":
                getTurretAttackables(0);
                break;
            case "Fire Turret 2":
                getTurretAttackables(1);
                break;
            case "Fire Turret 3":
                getTurretAttackables(2);
                break;
            case "Fire Turret 4":
                getTurretAttackables(3);
                break;
            case "Fire Turret 5":
                getTurretAttackables(4);
                break;
            case "Toggle Jetpack":
                unit.startToggleJetpack();
                gM.selectedTile.deleteSelector();
                break;
            case "Rally":
            case "Bolster":
            case "Focus":
            case "Reset Ally":
            case "Pressure":
            case "Caution":
            case "Inspire":
            case "Energize":
            case "Anger":
            case "Confuse":
            case "Debuff":
            case "Intimidate":
                //print("Unit Rally");
                unit.doCommandAsPlayer(type);
                break;
            default:
                break;
        }

    }


    public void applyImageScale()
    {
        imageObject.transform.localScale = new Vector3(imageObject.transform.localScale.x * imageFactor, imageObject.transform.localScale.y * imageFactor, 1f);
    }

    public void setSize(float x, float y)
    {
        Vector3 size = new Vector3(x, y, 1f);
        setSize(size);
    }

    public void setSize(Vector3 size)
    {
        transform.localScale = size;
        applyImageScale();
    }


    //Disable the button
    public void disable()
    {
        buttonImage.sprite = deactivated;
        GetComponent<Button>().enabled = false;
    }

    //Enable the button
    public void enable()
    {
        buttonImage.sprite = activated;
        GetComponent<Button>().enabled = true;
    }


}
