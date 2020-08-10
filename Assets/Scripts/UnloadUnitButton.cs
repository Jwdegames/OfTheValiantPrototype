using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnloadUnitButton : MonoBehaviour
{
    public Unit transporter;
    public Unit unit;
    public GameObject unitGameObject;
    public string faction;
    public BoardManager bM;
    public UIManager ui;
    public GameManager gM;
    public Tile tile;

    //Width is 100, Height is 100
    // Start is called before the first frame update
    void Start()
    {
        tile = ui.bpTile;

    }

    public void updateUnitStats()
    {
        ui.updateStats(tile, unit, ui.bpBuilding);
    }

    public void unloadUnit()
    {
        ui.unloadingUnit.markTilesForUnloading(unit);
        ui.destroyUnitBuilderMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
