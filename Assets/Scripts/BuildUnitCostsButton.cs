using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildUnitCostsButton : MonoBehaviour
{
    public Button buildUnitButton;
    public Button buildUnitCostsButton;
    public Image buildUnitImage;
    public Sprite active;
    public Sprite inactive;
    public Unit unit;
    public GameObject unitGameObject;
    public string faction;
    public Text pplCost;
    public Text mtCost;
    public BoardManager bM;
    public UIManager ui;
    public GameManager gM;
    public Tile tile;

    void Start()
    {

    }

    public void makeUnitTemplate(Unit u, UnitTemplate template)
    {
        tile = ui.bpTile;
        unitGameObject = Instantiate(u.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        unitGameObject.transform.SetParent(buildUnitButton.gameObject.transform);
        unitGameObject.transform.localPosition = new Vector3(0, 0, -1);
        
        unit = unitGameObject.GetComponent<Unit>();
        unit.useTemplate(template);
        unit.matchWeapon("Unit Template");
        unit.startTurn();
        pplCost.text = unit.getPPLCost() + "";
        mtCost.text = unit.getMTCost() + "";
        unitGameObject.transform.localScale = new Vector3(unit.getSizeMultiplier() * 40f / 5f, unit.getSizeMultiplier() * 40 / 5f, 1);
        unit.setOutlineColor(gM.playerDictionary[gM.humanSide].color, 1);
        unit.setOutlineThickness(0);
    }

    public void updateUnitStats()
    {
        ui.updateStats(tile, unit, ui.bpBuilding);
    }

    public void buildUnit()
    {
        Unit u = bM.buildUnit(unit.gameObject, gM.playerDictionary[gM.humanSide], tile.mapX, tile.mapY, true);
        ui.destroyUnitBuilderMenu();
        gM.selectedTile.deleteSelector();
        gM.playerDictionary[gM.humanSide].metal -= unit.getMTCost();
        gM.playerDictionary[gM.humanSide].people -= unit.getPPLCost();
        ui.getBPEconStats();
        u.grayScale();
        ui.updateStats(null, null, null);
    }

    public void makeActive()
    {
        buildUnitImage.sprite = active;
        buildUnitButton.enabled = true;
        buildUnitCostsButton.enabled = true;
    }

    public void makeInactive()
    {
        buildUnitImage.sprite = inactive;
        buildUnitButton.enabled = false;
        buildUnitCostsButton.enabled = false;
    }
}
