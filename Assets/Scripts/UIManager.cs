using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public GameObject battleCanvasObject;
    public GameManager gM;
    public Canvas battleCanvas;
    public GameObject battleMenuPrefab;
    public GameObject battleMenu = null;
    public UnitActionMenu bmUI;
    public bool selectingAction = false;

    public List<Button> inBattleButtons;

    //Handle Battle Panel UI Functions
    public GameObject battlePanel;
    public Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    public GameObject attributePrefab;
    public List<GameObject> attributeIcons = new List<GameObject>();
    public List<Sprite> attributeSprites;
    public Font effectFont;

    public Text pplCountTXT;
    public Text mtCountTXT;

    public Tile bpTile;
    public Unit bpUnit;
    public Building bpBuilding;

    public Image tileIMG;
    public Button expandTileButton;
    public SpriteRenderer weaponUpperSprite;
    public WeaponObject weaponUpperSpriteObject;
    public Text expandTileButtonText;
    public Text tileTXT;
    public Text tileTypeTXT;
    public Text tileStatsTXT;
    public Text tileExtraTXT;

    public Image unitIMG;
    public Image unitMWIMG;
    public Button expandUnitButton;
    public Text expandUnitButtonText;
    public GameObject unitGameObject;
    public List<Sprite> uiWeaponSprites;
    public Text unitTXT;
    public Text unitTypeTXT;
    public Text unitStatsTXT;
    public Text unitAttributesTXT;
    public List<Image> unitAttributes;

    public Image buildingIMG;
    public SpriteRenderer weaponSprite;
    public WeaponObject weaponSpriteObject;
    public Text buildingTXT;
    public Button expandBuildingButton;
    public Text expandBuildingButtonText;

    //Weapon variables
    public GameObject weaponChooserDropdownObject;
    public Dropdown weaponChooserDropdownMenu;
    public Dictionary<string, Weapon> wcdDictionary = new Dictionary<string, Weapon>();

    public Image bpMiddleDiv;
    public Image bpLowerDiv;

    private int pplCount;
    private int mtCount;
    private int pplCountPDay;
    private int mtCountPDay;
    private bool expandedBP = false;
    private string thingExpanded = "";

    public Player player;

    //Building Unit Panel variables
    public GameObject buPanel;
    public Text buText;
    public Unit buUnit;
    public GameObject buildUnitButtonPrefab;
    public GameObject buildUnitButton;
    public BuildUnitCostsButton buildUnitButtonScript;
    public List<GameObject> buildUnitButtons;

    //Transporting variables
    public GameObject unloadUnitButtonPrefab;
    public Unit unloadingUnit;
    public GameObject unloadUnitButton;
    public UnloadUnitButton unloadUnitButtonScript;
    public List<GameObject> unloadUnitButtons;
    public Dictionary<GameObject, Vector3> originalWeaponPositions;

    // Use this for initialization
    void Start()
    {
        battleCanvas = battleCanvasObject.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void uiCoords()
    {

    }

    public void setPPLCount(int num)
    {
        pplCount = num;
    }

    public void setPPLCountPDay(int num)
    {
        pplCountPDay = num;
    }

    public void setMTCount(int num)
    {
        mtCount = num;
    }
    public void setMTCountPDay(int num)
    {
        mtCountPDay = num;
    }

    public int getPPLCount()
    {
        return pplCount;
    }

    public int getMTCount()
    {
        return mtCount;
    }

    //Sets the text for the economy in the battle panel
    public void updateBPEconTXT()
    {
        pplCountTXT.text = pplCount + " PPL\n (+" + pplCountPDay + "/Day)";
        mtCountTXT.text = mtCount + " MT\n (+" + mtCountPDay + "/Day)";
    }

    //Alternate method for getting text for the econ in battle panel.
    public void getBPEconStats()
    {
        pplCountTXT.text =  player.people + " PPL\n (+" + player.getPPLPDay() + "/3 Days)";
        mtCountTXT.text = player.metal + " MT\n (+" + player.getMTPDay() + "/Day)";
    }

    public string getTileMPTXTs(Tile tile, string type)
    {
        if (tile.aqueousType == "Land")
        {
            if (type == "Legged")
            {
                if (tile.moveCostLegged > 0) return tile.moveCostLegged + "";
                return "N/A";
            }
            else if (type == "Tracked")
            {
                if (tile.moveCostTracked > 0) return tile.moveCostTracked + "";
                return "N/A";
            }
            else if (type == "Wheeled")
            {
                if (tile.moveCostWheeled > 0) return tile.moveCostWheeled + "";
                return "N/A";
            }
            else
            {
                return "1";
            }
        }
        return "N/A";

    }

    public void updateTileInfo(Tile tile)
    {

        if (tile != null)
        {
            tileIMG.enabled = true;
            expandTileButton.gameObject.SetActive(true);
            tileIMG.sprite = tile.GetComponent<SpriteRenderer>().sprite;
            tileTXT.text = tile.description;
            tileTypeTXT.text = tile.type;

            if (tile.aqueousType == "Land")
            {
                string temp = "MP Costs: " + getTileMPTXTs(tile, "Legged") + "L," + getTileMPTXTs(tile, "Tracked") + "T," + getTileMPTXTs(tile, "Wheeled") + "W";
                temp += "\nRoads To: N/A";
                temp += "\nCover Bonus: " + tile.coverBonus + "%";
                tileStatsTXT.text = temp;
            }
        }
        else
        {
            tileIMG.enabled = false;
            expandTileButton.gameObject.SetActive(false);
            tileTXT.text = "";
            tileTypeTXT.text = "";
            tileStatsTXT.text = "";


        }
    }

    //Provides more info on Tile
    public void toggleExpandedTileInfo()
    {
        if (!expandedBP)
        {
            updateUnitInfo(null);
            updateBuildingInfo(null);
            bpMiddleDiv.enabled = false;
            bpLowerDiv.enabled = false;
            expandedBP = true;
            thingExpanded = "Tile";

            expandTileButtonText.text = "Contract";
            tileStatsTXT.text = "";
            originalPositions[tileTXT.gameObject] = tileTXT.gameObject.transform.localPosition;
            tileTXT.gameObject.transform.localPosition = tileStatsTXT.transform.localPosition;

            string temp = "This tile is a "+bpTile.aqueousType+" tile. ";
            if (bpTile.aqueousType == "Land")
            {
                /*temp += "That means this tile can only be traveled on by land and air units.\n\n";

                //Handle legged units
                string temp2 = getTileMPTXTs(bpTile, "Legged");
                string oldTemp = temp2;
                if (temp2 != "N/A") {
                    temp += "A legged unit requires " + temp2 + " MP to leave this tile. ";
                }
                else
                {
                    temp += "Legged units cannot enter this tile. ";
                }

                //Handle tracked units
                temp2 = getTileMPTXTs(bpTile, "Tracked");
                if (temp2 != "N/A")
                {
                    temp += "A tracked unit requires " + temp2 + " MP to leave this tile. ";
                }
                else
                {
                    temp += "Tracked units cannot enter this tile. ";
                }

                //Handle wheeled units
                temp2 = getTileMPTXTs(bpTile, "Wheeled");
                if (temp2 != "N/A")
                {
                    temp += "A wheeled unit requires " + temp2 + " MP to leave this tile.";
                }
                else
                {
                    temp += "Wheeled units cannot enter this tile.";
                }*/

                temp += "\n\nThere are no roads to offer movement bonuses.";

                temp += "\n\nUnits here will take "+bpTile.coverBonus+"% less damage due to cover.";

            }
            else if (bpTile.aqueousType == "Aqueous")
            {
                temp += "That means this tile can only be traveled on by sea and air units.";
            }

            temp += "\n\nThis tile has the following attributes:";
            if (bpTile.extraAttributes != null)
            {
                Dictionary<string, float> attributeDict = bpTile.extraAttributes;
                if (attributeDict.ContainsKey("Poison Gas"))
                {
                    makeUnitAttribute("Poison Gas", "This tile poisons units.", "Attribute Lifetime: "+attributeDict["Poison Gas"]+" Days", "", 6,"Tile");
                }

            }
            tileExtraTXT.text = temp;
        }
        else
        {
            if (thingExpanded == "Tile")
            {
                thingExpanded = "";
                expandedBP = false;
                updateUnitInfo(bpUnit);
                updateBuildingInfo(bpBuilding);
                bpMiddleDiv.enabled = true;
                bpLowerDiv.enabled = true;

                expandTileButtonText.text = "Expand";
                tileExtraTXT.text = "";
                tileTXT.gameObject.transform.localPosition = originalPositions[tileTXT.gameObject];

                foreach (GameObject attribute in attributeIcons)
                {
                    Destroy(attribute);
                }

                attributeIcons = new List<GameObject>();

                if (bpTile.aqueousType == "Land")
                {
                    string temp = "MP Costs: " + getTileMPTXTs(bpTile, "Legged") + "L," + getTileMPTXTs(bpTile, "Tracked") + "T," + getTileMPTXTs(bpTile, "Wheeled") + "W";
                    temp += "\nRoads To: N/A";
                    temp += "\nCover Bonus: " + bpTile.coverBonus + "%";
                    tileStatsTXT.text = temp;
                }
            }
        }
    }

    //Gets the armor prefix for an armor
    public string getArmorPrefix(string armorType)
    {
        //Debug.Log("Armor: "+armorType);
        if (armorType == "Light") return "L";
        else if (armorType == "Medium") return "M";
        else if (armorType == "Heavy") return "H";
        else return "";
    }

    public void updateUnitInfo(Unit unit)
    {
        
        if (unit != null)
        {
            expandUnitButton.gameObject.SetActive(true);
            if (unitGameObject != null) Destroy(unitGameObject);
            unitGameObject = Instantiate(unit.gameObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            unitGameObject.transform.SetParent(unitIMG.transform);


            //Now set the weapon for the game object
            Unit ugoScript = unitGameObject.GetComponent<Unit>();

            //Delete all children of the game object
            foreach (Transform child in unitGameObject.transform)
            {
                GameObject childGO = child.gameObject;
                WeaponObject tempWO = childGO.GetComponent<WeaponObject>();
                Destroy(child.gameObject);
            }
            ugoScript.resetWeaponsList();
            ugoScript.matchWeapon("UI");
            unitGameObject.transform.localPosition = new Vector3(0, 0, -2);
            unitGameObject.transform.localScale = new Vector3(33, 33, 0);
            List<Weapon> unitWeapons = ugoScript.getAllActiveWeapons();
            int i = 0;
            if (unitWeapons != null && unitWeapons.Count > 0)
            {
                foreach (Weapon unitWeapon in unitWeapons)
                {
                    
                    //Debug.Log(i+" Unit Weapon: " + unitWeapon.name +". Does it exist in the dictionary? "+ugoScript.weaponDictionary.ContainsKey(unitWeapon));
                    ugoScript.weaponDictionary[unitWeapon].makeUI(null,unitWeapons.Count - i);
                    i++;

                }
            }

            //Debug.Log(ugoScript.getCurrentWeaponObject());
            unitTXT.text = unit.getDescription();
            unitTypeTXT.text = unit.getType();

            string temp = "HP(" + getArmorPrefix(unit.getArmor()) + "): " + Math.Round( unit.getCurrentHP(), 3)+ "/" + unit.getHP() + ",MP(" + unit.getMovementType() + "): " + unit.getCurrentMP() + "/" + unit.getMP() + ",AP: " + unit.getCurrentAP() + "/" + unit.getAP();
            temp += "\nMain Weapon| Dmg(" + getArmorPrefix(unit.getCurrentWeapon().weaponType) + "):" + unit.getCurrentWeapon().getDamagePerSalvo() + ",Range:" + unit.getCurrentWeapon().minRange + "-" + unit.getCurrentWeapon().maxRange;
            unitStatsTXT.text = temp;
        }
        else
        {

            if (unitGameObject != null) Destroy(unitGameObject);
            //Debug.Log(unitGameObject);
            expandUnitButton.gameObject.SetActive(false);
            unitTXT.text = "";
            unitTypeTXT.text = "";
            unitStatsTXT.text = "";
        }

    }

    public Vector3 getNextAttributePosition()
    {
        return new Vector3(-80f + 30*(attributeIcons.Count%6), -12.6f + 30 * (attributeIcons.Count/6) , -1);
    }

    public string getWeaponCarriedSuffix(Weapon weapon)
    {
        if (weapon.isPrimary) return "(Primary)";
        if (weapon.isSecondary) return "(Secondary)";
        if (weapon.isTertiary) return "(Tetiary)";
        if (weapon.isTurret) return "(Turret)";
        return "(Inactive)";
    }

    public void scaleWOToUI(WeaponObject weaponSpriteObj)
    {
        switch (weaponSpriteObj.name)
        {
            case "Drone Gun":
            case "Assault Turret":
            case "Laser Assault Turret":
            case "Blast Eye x3":
            case "Blast Eye x5":
                weaponSpriteObj.scaleToUI(80, 80);
                break;
            case "Tank Cannon":
            case "Heavy Tank Cannon":
            case "Rocket Burster":
            case "Artillery Cannon":
            case "Duality Tank Cannon":
            case "Duality Rocket Burster":
            case "Laser Tank Cannon":
            case "Duality Artillery Cannon":
            case "Duality Laser Tank Cannon":
            case "Gas Mortar MK 2":
            case "Flak Gun":
            case "Venom Tank Cannon":
            case "Gas Mortar MK 3":
            case "P Flak Gun":
            case "DP Rocket Burster":
            case "Tall Brewer":
            case "Slime Launcher":
                weaponSpriteObj.scaleToUI(50, 50);
                break;
            default:
                weaponSpriteObj.scaleToUI(30, 30);
                break;
        }
    } 

    public void offsetToolTip(AttributeIcon icon)
    {
        if (attributeIcons.Count % 5 > 2)
        {
            icon.offsetToolTip(new Vector3(((attributeIcons.Count % 5) - 2) * -5, 0, 0));
        }
    }

    public AttributeIcon makeUnitAttribute(string title, string text, string duration, string displayText, int sprite, string type)
    {
        GameObject tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        AttributeIcon tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
        tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
        tempAttribute.transform.localPosition = getNextAttributePosition();

        tempAttribute.transform.localScale = new Vector3(1, 1, 1);
        attributeIcons.Add(tempAttribute);
        //offsetToolTip(tempAttributeScript);

        tempAttributeScript.updateToolTipTitle(title);
        tempAttributeScript.updateToolTipTXT(text);
        tempAttributeScript.updateToolTipDuration(duration);
        tempAttributeScript.updateText(displayText);
        tempAttributeScript.updateSprite(attributeSprites[sprite]);
        return tempAttributeScript;
    }

    //Shows expanded unit info
    public void toggleExpandedUnitInfo()
    {
        if (!expandedBP)
        {
            bpMiddleDiv.enabled = false;
            updateBuildingInfo(null);
            updateTileInfo(null);
            unitStatsTXT.text = unitTXT.text;
            unitTXT.text = "";

            expandedBP = true;
            thingExpanded = "Unit";

            expandUnitButtonText.text = "Contract";

            originalPositions[unitIMG.gameObject] = unitIMG.gameObject.transform.localPosition;
            unitIMG.gameObject.transform.localPosition = tileIMG.transform.localPosition;

            List<Weapon> unitWeapons = bpUnit.getAllWeapons();

            wcdDictionary = new Dictionary<string, Weapon>();
            if (unitWeapons != null && unitWeapons.Count > 0)
            {
                expandBuildingButton.gameObject.SetActive(true);
                //Handle the weapon chooser menu
                weaponChooserDropdownObject.SetActive(true);
                weaponChooserDropdownMenu.ClearOptions();

                //Debug.Log(unitWeapons[0]);
                List<string> weaponNames = new List<string>();

                foreach (Weapon weapon in unitWeapons)
                {
                    string wName = weapon.name + " " + getWeaponCarriedSuffix(weapon);
                    if (!wcdDictionary.ContainsKey(wName))
                    {
                        wcdDictionary.Add(wName, weapon);
                        weaponNames.Add(wName);
                    }
                    else
                    {
                        //We must add an int until we can use it
                        while (true)
                        {
                            //Debug.Log("Checking");
                            int i = 2;
                            if (!wcdDictionary.ContainsKey(wName + "-" + i))
                            {
                                wcdDictionary.Add(wName + "-" + i, weapon);
                                weaponNames.Add(wName + "-" + i);
                                break;
                            }
                            i++;
                        }
                    }
                }

                //buildingIMG.sprite = unitWeapons[0].uiSprite;
                //buildingIMG.enabled = true;
                buildingIMG.enabled = false;
                weaponSprite.sprite = unitWeapons[0].uiSprite;
                weaponSprite.enabled = true;
                weaponSpriteObject.useWeapon(unitWeapons[0],bpUnit);
                scaleWOToUI(weaponSpriteObject);
                weaponChooserDropdownMenu.value = 0;
                weaponChooserDropdownMenu.AddOptions(weaponNames);
            }

            //Begin writing the expanded info text
            string temp = "This unit has "+Math.Round(bpUnit.getCurrentHP(),3)+"/"+bpUnit.getHP()+" Hit Points. ";


            temp += "\n\nThis unit has " + bpUnit.getCurrentMP() + "/" + bpUnit.getMP() + " Movement Points. This unit is " + bpUnit.getMovementType()+".";
            temp += "\n\nThis unit has " + bpUnit.getCurrentAP() + "/" + bpUnit.getAP() + " Action Points.";

            tileExtraTXT.text = temp;

            unitAttributesTXT.text = "This unit has the following attributes:";

            //Make all the attributes

            //Make the armor attribute 
            GameObject tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            AttributeIcon tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
            tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
            tempAttribute.transform.localPosition = getNextAttributePosition();
            tempAttribute.transform.localScale = new Vector3(1, 1, 1);
            attributeIcons.Add(tempAttribute);
            switch (bpUnit.getArmor())
            {
                case "Light":
                    temp = "This unit has light armor, meaning it will take twice as much damage from anti-light weapons and half as much damage from anti-heavy weapons.";
                    tempAttributeScript.updateText("L");
                    tempAttributeScript.updateToolTipTitle("Light Armor");
                    break;
                case "Heavy":
                    temp = "This unit has heavy armor, meaning it will take twice as much damage from anti-heavy weapons and half as much damage from anti-light weapons.";
                    tempAttributeScript.updateText("H");
                    tempAttributeScript.updateToolTipTitle("Heavy Armor");
                    break;
                case "Medium":
                    temp = "This unit has medium armor, meaning it has no defensive weaknesses or strengths.";
                    tempAttributeScript.updateText("M");
                    tempAttributeScript.updateToolTipTitle("Medium Armor");
                    break;
                case "Slime":
                    temp = "This unit is a slime, meaning it takes 20% less damage from all attacks.";
                    tempAttributeScript.updateText("S");
                    tempAttributeScript.updateToolTipTitle("Slime Armor");
                    break;

            }
            
            tempAttributeScript.updateToolTipTXT(temp);
            tempAttributeScript.updateToolTipDuration("Attribute Lifetime: Forever");

            //Handle guard/sentry attributes
            if (bpUnit.getGuard())
            {
                tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
                tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
                tempAttribute.transform.localPosition = getNextAttributePosition();
                tempAttribute.transform.localScale = new Vector3(1, 1, 1);
                attributeIcons.Add(tempAttribute);
                tempAttributeScript.updateToolTipTitle("Guard");
                tempAttributeScript.updateToolTipTXT("This unit is currently fortified, meaning it takes"+bpUnit.guardCover*100+"% less damage");
                tempAttributeScript.updateToolTipDuration("Attribute Lifetime: 1 Day");
            }
            else if (bpUnit.getSentry())
            {
                tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
                tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
                tempAttribute.transform.localPosition = getNextAttributePosition();
                tempAttribute.transform.localScale = new Vector3(1, 1, 1);
                attributeIcons.Add(tempAttribute);
                tempAttributeScript.updateToolTipTitle("Sentry");
                tempAttributeScript.updateToolTipTXT("This unit is currently patrolling, meaning it attacks enemies before they attack this unit.");
                tempAttributeScript.updateToolTipDuration("Attribute Lifetime: 1 Day");
                tempAttributeScript.updateSprite(attributeSprites[1]);

            }
            //Debug.Log(buUnit);
            if (bpUnit.extraAttributes != null)
            {
                Dictionary<string, float> attributeDict = bpUnit.extraAttributes;
                if (attributeDict.ContainsKey("Poisoned"))
                {
                    float poisonHPEffect = bpUnit.getPoisonHPEffect();
                    if (poisonHPEffect > 0)
                    {
                        temp = "This unit has been poisoned and will lose "+(poisonHPEffect*100)+"% of its hp at the start of its turn.";
                    }
                    else
                    {
                        temp = "This unit has been poisoned and will gain " + (-poisonHPEffect * 100) + "% of its hp at the start of its turn.";
                    }
                    makeUnitAttribute("Poisoned", temp, "Attribute Lifetime: "+attributeDict["Poisoned"]+" Days", "", 5,"Unit");
                }
                if (attributeDict.ContainsKey("Self Heal"))
                {
                    float val = attributeDict["Self Heal"];
                    if (val > 0)
                    {
                        temp = "This unit self heals " + (val * 100) + "% of its hp at the start of its turn.";
                        makeUnitAttribute("Self Heals", temp, "Attribute Lifetime: Forever", "", 9, "Unit");
                    }
                    else
                    {
                        temp = "This unit automatically loses " + (-val * 100) + "% of its hp at the start of its turn.";
                        makeUnitAttribute("Self Damages", temp, "Attribute Lifetime: Forever", "", 9, "Unit");
                    }
                    
                }
            }
            //Handle attributes not in the dictionary
            if (bpUnit.leavesPoisonGasOnDeath)
            {
                temp = "This unit leaves a poisonous gas cloud on death with an AOE of "+bpUnit.poisonGasOnDeathAOE+".";
                makeUnitAttribute("Poisonous Death", temp, "Attribute Lifetime: Forever","D",6,"Unit");
            }
            if (bpUnit.doesDamageOnDeath)
            {
                temp = "This unit deals " + bpUnit.damageOnDeath + " damage to adjacent units on death with an AOE of " + (bpUnit.damageOnDeathAOE + 1) + " tiles.";
                makeUnitAttribute("Explosive Death", temp, "Attribute Lifetime: Forever", "D", 8, "Unit");

            }
            if (bpUnit.hasJetpack)
            {
                temp = "This unit has a jetpack that can be toggled on and off. While flying, this unit cannot capture buildings. This jetpack has used "+bpUnit.currentJetToggles+"/"+bpUnit.maxJetToggles+
                    " toggles left this turn.";
                makeUnitAttribute("Jetpack", temp, "Attribute Lifetime: Forever", "", 7, "Unit");
            }


        }
        else
        {
            if (thingExpanded == "Unit")
            {
                thingExpanded = "";
                expandedBP = false;

                bpMiddleDiv.enabled = true;
                tileExtraTXT.text = "";

                expandBuildingButton.gameObject.SetActive(false);
                weaponSprite.enabled = false;

                expandUnitButtonText.text = "Expand";
                unitIMG.gameObject.transform.localPosition = originalPositions[unitIMG.gameObject];
                updateBuildingInfo(bpBuilding);
                updateTileInfo(bpTile);
                updateUnitInfo(bpUnit);
                unitAttributesTXT.text = "";
                

                foreach (GameObject attribute in attributeIcons)
                {
                    Destroy(attribute);
                }

                attributeIcons = new List<GameObject>();

                weaponChooserDropdownObject.SetActive(false);
            }
            else if (thingExpanded == "Weapon")
            {
                thingExpanded = "Unit";
                weaponUpperSprite.enabled = false;
                //Fully reset the panel to show unit stats by first clearing everything and then calling the toggle a 2nd time to redo the unit stats
                toggleExpandedUnitInfo();
                toggleExpandedUnitInfo();
            }
        }
    }

    public void updateWeaponImage()
    {
        //buildingIMG.sprite = wcdDictionary[weaponChooserDropdownMenu.options[weaponChooserDropdownMenu.value].text].uiSprite;
        Weapon temp = wcdDictionary[weaponChooserDropdownMenu.options[weaponChooserDropdownMenu.value].text];
        weaponSprite.sprite = temp.uiSprite;
        weaponSpriteObject.useWeapon(temp,bpUnit);
        scaleWOToUI(weaponSpriteObject);
    }

    //Updates building info on the battle panel
    public void updateBuildingInfo(Building building) {
 
        if (building != null)
        {
            buildingIMG.enabled = true;
            //Debug.Log(building.GetComponent<SpriteRenderer>());
            buildingIMG.sprite = building.GetComponent<SpriteRenderer>().sprite;
            buildingIMG.color = building.GetComponent<SpriteRenderer>().color;
            buildingTXT.text = building.name;
            expandBuildingButton.gameObject.SetActive(true);
        }
        else
        {
            buildingIMG.enabled = false;
            buildingTXT.text = "";
            expandBuildingButton.gameObject.SetActive(false);
            buildingIMG.color = Color.white;
        }
    }

    //Toggles expanded building/weapon info
    public void toggleExpandedBWInfo()
    {
        if (!expandedBP)
        {
            //Expand the building info
            updateTileInfo(null);
            updateUnitInfo(null);

            originalPositions[buildingIMG.gameObject] = buildingIMG.gameObject.transform.localPosition;
            buildingIMG.gameObject.transform.localPosition = tileIMG.gameObject.transform.localPosition;
            tileTXT.text = bpBuilding.description;

            expandBuildingButtonText.text = "Contract";
            thingExpanded = "Building";
            expandedBP = true;
        }
        else
        {
            if (thingExpanded == "Building")
            {
               buildingIMG.gameObject.transform.localPosition = originalPositions[buildingIMG.gameObject];
                updateTileInfo(bpTile);
                updateUnitInfo(bpUnit);
                expandBuildingButtonText.text = "Expand";

                expandedBP = false;
                thingExpanded = "";
            }
            //Switch expanded thing to weapon
            else if (thingExpanded == "Unit" || thingExpanded == "Weapon")
            {
                thingExpanded = "Weapon";
                Destroy(unitGameObject);

                //tileIMG.enabled = true;
                Weapon weapon = wcdDictionary[weaponChooserDropdownMenu.options[weaponChooserDropdownMenu.value].text];
                //tileIMG.sprite = weapon.uiSprite;
                weaponUpperSprite.enabled = true;
                weaponUpperSprite.sprite = weapon.uiSprite;
                weaponUpperSpriteObject.useWeapon(weapon,bpUnit);
                Debug.Log(weaponSpriteObject.name);
                scaleWOToUI(weaponUpperSpriteObject);
                unitStatsTXT.text = weapon.description;
                unitTypeTXT.text = weapon.name;

                string temp = "";
                if (!weapon.heals && !weapon.repairs)
                {
                    temp = "This weapon does " + weapon.getDamagePerSalvo() + " damage per attack.";
                }
                else
                {
                    if (weapon.healRepairAmountBefore > 0)
                    {
                        if (weapon.healRepairPercent > 0)
                        {
                            if (weapon.healRepairAmountAfter > 0)
                            {
                                temp = "This weapon heals " + weapon.healRepairAmountBefore + " hp, then heals for " + weapon.healRepairPercent * 100 + "% of the unit's health," +
                                    " and then heals the unit for " + weapon.healRepairAmountAfter + " hp";  
                            }
                            else
                            {
                                temp = "This weapon heals " + weapon.healRepairAmountBefore + " hp, and then heals for " + weapon.healRepairPercent * 100 + "% of the unit's health";
                            }
                        }
                        else
                        {
                            if (weapon.healRepairAmountAfter > 0)
                            {
                                temp = "This weapon heals " + (weapon.healRepairAmountBefore + weapon.healRepairAmountAfter) + " hp";
                            }
                            else
                            {
                                temp = "This weapon heals " + weapon.healRepairAmountBefore + " hp";
                            }
                        }
                    }
                    else
                    {
                        if (weapon.healRepairPercent > 0)
                        {
                            if (weapon.healRepairAmountAfter > 0)
                            {
                                temp = "This weapon heals for " + weapon.healRepairPercent * 100 + "% of the unit's health," +
                                    " and then heals the unit for " + weapon.healRepairAmountAfter + " hp";
                            }
                            else
                            {
                                temp = "This weapon heals for " + weapon.healRepairPercent * 100 + "% of the unit's health";
                            }
                        }
                        else
                        {
                            temp = "This weapon heals " + weapon.healRepairAmountAfter + "hp";
                        }
                    }
                    if (weapon.heals && !weapon.repairs)
                    {
                        temp += " and heals only biological units.";
                    }
                    else if (weapon.heals && weapon.repairs)
                    {
                        temp += " and heals both mechanical and biological units.";
                    }
                    else
                    {
                        //Debug.Log(weapon.heals);
                        temp += " and heals only mechanical units.";
                    }
                }

                

                temp += "\n\nThis weapon has a range of " + weapon.minRange + "-" + weapon.maxRange + " tiles ";
                if (weapon.aoe == 0)
                {
                    temp += "and only affects 1 tile.";
                }
                else
                {
                    temp += "and has an Area of Effect range of " + weapon.aoe;
                }

                tileExtraTXT.text = temp;

                unitAttributesTXT.text = "This weapon has the following attributes:";

                //Destroy all previous attributes so we can make a new set
                foreach (GameObject attribute in attributeIcons)
                {
                    Destroy(attribute);
                }

                attributeIcons = new List<GameObject>();

                //Make all the attributes

                //Make the armor attribute 
                GameObject tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                AttributeIcon tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
                tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
                tempAttribute.transform.localPosition = getNextAttributePosition();
                tempAttribute.transform.localScale = new Vector3(1, 1, 1);
                attributeIcons.Add(tempAttribute);
                switch (weapon.weaponType)
                {
                    case "Light":
                        temp = "This weapon is anti-light, meaning it will deal twice as much damage to anti-light armor and half as much damage to anti-heavy armor.";
                        tempAttributeScript.updateText("L");
                        tempAttributeScript.updateToolTipTitle("Anti-Light");
                        break;
                    case "Heavy":
                        temp = "This weapon is anti-heavy, meaning it will deal twice as much damage to anti-heavy armor and half as much damage to anti-light armor.";
                        tempAttributeScript.updateText("H");
                        tempAttributeScript.updateToolTipTitle("Anti-Heavy");
                        break;
                    case "Medium":
                        temp = "This weapon is anti-medium, meaning it has no defensive weaknesses or strengths.";
                        tempAttributeScript.updateText("M");
                        tempAttributeScript.updateToolTipTitle("Anti-Medium");
                        break;

                }
                tempAttributeScript.updateToolTipTXT(temp);
                tempAttributeScript.updateToolTipDuration("Attribute Lifetime: Forever");
                tempAttributeScript.updateSprite(attributeSprites[2]);
                offsetToolTip(tempAttributeScript);

                if (weapon.canTargetAir)
                {
                    temp = "This weapon can target air units.";
                    makeUnitAttribute("Targets Air Units", temp, "Attribute Lifetime: Forever", "A", 2, "Unit");
                }
                if (weapon.canTargetSub)
                {
                    temp = "This weapon can target submerged units.";
                    makeUnitAttribute("Targets Air Units", temp, "Attribute Lifetime: Forever", "S", 2, "Unit");
                }
                //Handle anti-armor bonuses
                if (weapon.extraAttributes != null)
                {
                    foreach (string attribute in weapon.extraAttributes.Keys)
                    {
                        /*tempAttribute = Instantiate(attributePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        tempAttributeScript = tempAttribute.GetComponent<AttributeIcon>();
                        tempAttribute.transform.SetParent(unitAttributesTXT.gameObject.transform);
                        tempAttribute.transform.localPosition = getNextAttributePosition();
                        tempAttribute.transform.localScale = new Vector3(1, 1, 1);
                        attributeIcons.Add(tempAttribute);
                        offsetToolTip(tempAttributeScript);*/
                        float value = weapon.extraAttributes[attribute];
                        Debug.Log(attribute);
                        switch (attribute)
                        {
                            //Handle permanent armor attributes
                            case "Anti-Light Multi-Bonus":
                                temp = "This weapon does " + weapon.extraAttributes[attribute] + " times as much damage against light armored targets.";
                                if (value > 1)
                                {
                                    makeUnitAttribute("Bonus Multiplier (L)", temp, "Attribute Lifetime: Forever", "▲L", 2, "Unit");
                                }
                                else
                                {
                                    makeUnitAttribute("Drawback Multiplier (L)", temp, "Attribute Lifetime: Forever", "▼L", 2, "Unit");
                                }
                                break;
                            case "Anti-Medium Multi-Bonus":
                                temp = "This weapon does " + weapon.extraAttributes[attribute] + " times as much damage against medium armored targets.";
                                if (value > 1)
                                {
                                    makeUnitAttribute("Bonus Multiplier (M)", temp, "Attribute Lifetime: Forever", "▲M", 2, "Unit");
                                }
                                else
                                {
                                    makeUnitAttribute("Drawback Multiplier (M)", temp, "Attribute Lifetime: Forever", "▼M", 2, "Unit");
                                }
                                break;
                            case "Anti-Heavy Multi-Bonus":
                                temp = "This weapon does " + weapon.extraAttributes[attribute] + " times as much damage against heavy armored targets.";
                                if (value > 1)
                                {
                                    makeUnitAttribute("Bonus Multiplier (H)", temp, "Attribute Lifetime: Forever", "▲H", 2, "Unit");
                                }
                                else
                                {
                                    makeUnitAttribute("Drawback Multiplier (H)", temp, "Attribute Lifetime: Forever", "▼H", 2, "Unit");
                                }
                                break;
                            case "Anti-Slime Multi-Bonus":
                                temp = "This weapon does " + weapon.extraAttributes[attribute] + " times as much damage against slimes.";
                                if (value > 1)
                                {
                                    makeUnitAttribute("Bonus Multiplier (S)", temp, "Attribute Lifetime: Forever", "▲S", 2, "Unit");
                                }
                                else
                                {
                                    makeUnitAttribute("Drawback Multiplier (S)", temp, "Attribute Lifetime: Forever", "▼S", 2, "Unit");
                                }
                                break;
                            case "Override To X Armor":
                                temp = "This overrides the unit's armor to ";
                                switch (value+"")
                                {
                                    //0 is Light, 1 is Medium, 2 is Heavy, 3 is Slime
                                    case "0":
                                        temp += "light armor.";
                                        makeUnitAttribute("Override Armor(L)", temp, "Attribute Lifetime: Forever", "OL", 3, "Unit");
                                        break;
                                    case "1":
                                        temp += "medium armor.";
                                        makeUnitAttribute("Override Armor(M)", temp, "Attribute Lifetime: Forever", "OM", 3, "Unit");
                                        break;
                                    case "2":
                                        temp += "heavy armor.";
                                        makeUnitAttribute("Override Armor(H)", temp, "Attribute Lifetime: Forever", "OH", 3, "Unit");
                                        break;
                                    case "3":
                                        temp += "slime armor.";
                                        makeUnitAttribute("Override Armor(S)", temp, "Attribute Lifetime: Forever", "OS", 3, "Unit");
                                        break;
                                }
                                //tempAttributeScript.updateSprite(attributeSprites[2]);
                                break;
                            case "Guard Damage Reduction Percentage Override":
                                temp = "This unit gets a "+value*100+"% damage reduction bonus instead of a 50% damage reduction bonus from guarding.";
                                if (value > 0.5)
                                {
                                    makeUnitAttribute("Guard Defense Boost", temp, "Attribute Lifetime: Forever", "▲G", 3, "Unit");
                                }
                                else
                                {
                                    makeUnitAttribute("Guard Defense Decrease", temp, "Attribute Lifetime: Forever", "▼G", 3, "Unit");
                                }
                                
                                break;
                            case "HP Bonus":
                                if (value > 0)
                                {
                                    temp = "This unit gets " + value + " extra HP.";
                                    makeUnitAttribute("HP Bonus", temp, "Attribute Lifetime: Forever", "▲♥", 3, "Unit");
                                }
                                else
                                {
                                    temp = "This unit loses " + value + " HP.";
                                    makeUnitAttribute("HP Debuff", temp, "Attribute Lifetime: Forever", "▼♥", 3, "Unit");
                                    
                                }
                                break;
                            case "Poisons":
                                temp = "This poisons humans that are not masked and vehicles that aren't sealed nor autonomous for 3 days.";
                                makeUnitAttribute("Poisons", temp, "Attribute Lifetime: Forever", "", 5, "Unit");
                                break;
                            case "Makes Poison Gas":
                                temp = "This unit creates poison gas on tiles it hits that lasts 5 days.";
                                makeUnitAttribute("Makes Poison Gas", temp, "Attribute Lifetime: Forever", "", 6, "Unit");
                                break;
                        }
                        //tempAttributeScript.updateToolTipTXT(temp);

                    }
                }
            }
        }
    }
    //Update all stats from one function
    public void updateStats(Tile tile, Unit unit, Building building)
    {
        //Disable the expanded mode of the battle panel
        if (expandedBP)
        {
            switch (thingExpanded)
            {
                case "Tile":
                    toggleExpandedTileInfo();
                    break;
                case "Unit":
                    toggleExpandedUnitInfo();
                    break;
                case "Weapon":
                    thingExpanded = "Unit";
                    weaponUpperSprite.enabled = false;
                    toggleExpandedUnitInfo();
                    break;
                case "Building":
                    toggleExpandedBWInfo();
                    break;
            }
        }
        updateTileInfo(tile);
        updateUnitInfo(unit);
        updateBuildingInfo(building);

        bpTile = tile;
        bpUnit = unit;
        bpBuilding = building;
    }


    //Make the unit action menu when the tile is right clicked
    public void makeBattleMenu(Tile tile, Vector3 coords)
    {
        if (battleMenu != null)
        {
            Destroy(battleMenu);
        }

        battleMenu = Instantiate(battleMenuPrefab, coords, Quaternion.identity) as GameObject;
        battleMenu.transform.SetParent(battleCanvasObject.transform);
        //Debug.Log("Battle Menu coords is at " + coords);
        bmUI = battleMenu.GetComponent<UnitActionMenu>();
        bmUI.ui = this;
        bmUI.gM = gM;
        bmUI.tile = tile;
        bmUI.unitObject = tile.getUnit();
        bmUI.setUnit();
        bmUI.makeButtons();

    }

    public void destroyBattleMenu()
    {
        if (battleMenu != null)
        {
            Destroy(battleMenu);
        }

    }

    public Vector3 getNextUnitButtonPosition()
    {

        return new Vector3(-300f + 150 * (buildUnitButtons.Count%5), 100 - 75 * (buildUnitButtons.Count/5), -1);
    }

    //Make the unit builder menu when selected
    public void makeUnitBuilderMenu()
    {
        disableInBattleButtons();
        buPanel.SetActive(true);
        selectingAction = true;
        buildUnitButtons = new List<GameObject>();
        buText.text = "Build Unit";

        //Prevent the camera from moving while we are making a unit
        gM.camScript.changeable = false;

        //Make a units list to grab templates from
        UnitsList unitsList = new UnitsList();
        Player humanPlayer = gM.playerDictionary[gM.humanSide];
        //Determine which units we need to build
        if (bpBuilding.name == "Barracks")
        {
            //Produce infantry units
            string faction = gM.playerDictionary[gM.humanSide].faction;
            List<UnitTemplate> unitTemplates = new List<UnitTemplate>(unitsList.unitTypes[faction]["Infantry"]);
            
            if (gM.playerDictionary[gM.humanSide].hasLab())
            {
                //Debug.Log("Adding infantry!");
                unitTemplates.AddRange(unitsList.unitTypes[faction]["Advanced Infantry"]);
            }
            foreach (UnitTemplate template in unitTemplates)
            {
                buildUnitButton = Instantiate(buildUnitButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                buildUnitButton.transform.SetParent(buPanel.transform);
                buildUnitButton.transform.localPosition = getNextUnitButtonPosition();
                buildUnitButton.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                buildUnitButtons.Add(buildUnitButton);
                buildUnitButtonScript = buildUnitButton.GetComponent<BuildUnitCostsButton>();
                buildUnitButtonScript.ui = this;
                buildUnitButtonScript.bM = gM.boardScript;
                buildUnitButtonScript.gM = gM;
                buildUnitButtonScript.makeUnitTemplate(unitsList.getUnitPrefab(template,faction,gM.boardScript).GetComponent<Unit>(),template);
                if (template.mtCost > humanPlayer.metal || template.pplCost > humanPlayer.people)
                {
                    Debug.Log("Failed cost check!");
                    buildUnitButtonScript.makeInactive();
                }


            }

        }
        else if (bpBuilding.name == "Factory")
        {
            //Produce infantry units
            string faction = gM.playerDictionary[gM.humanSide].faction;
            List<UnitTemplate> unitTemplates = new List<UnitTemplate>(unitsList.unitTypes[faction]["Vehicles"]);

            if (gM.playerDictionary[gM.humanSide].hasLab())
            {
                //Debug.Log("Adding infantry!");
                unitTemplates.AddRange(unitsList.unitTypes[faction]["Advanced Vehicles"]);
            }
            foreach (UnitTemplate template in unitTemplates)
            {
                buildUnitButton = Instantiate(buildUnitButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                buildUnitButton.transform.SetParent(buPanel.transform);
                buildUnitButton.transform.localPosition = getNextUnitButtonPosition();
                buildUnitButton.transform.localScale = new Vector3(0.8f, 0.8f, 1);
                buildUnitButtons.Add(buildUnitButton);
                buildUnitButtonScript = buildUnitButton.GetComponent<BuildUnitCostsButton>();
                buildUnitButtonScript.ui = this;
                buildUnitButtonScript.bM = gM.boardScript;
                buildUnitButtonScript.gM = gM;
                buildUnitButtonScript.makeUnitTemplate(unitsList.getUnitPrefab(template, faction, gM.boardScript).GetComponent<Unit>(), template);
                if (template.mtCost > humanPlayer.metal || template.pplCost > humanPlayer.people)
                {
                    buildUnitButtonScript.makeInactive();
                }


            }
        }
    }

    public Vector3 getNextUnloadUnitButtonPosition()
    {

        return new Vector3(-300f + 150 * (unloadUnitButtons.Count % 5), 100 - 75 * (unloadUnitButtons.Count / 5), -1);
    }

    public void makeUnloadingMenu(Unit transporter)
    {
        disableInBattleButtons();
        originalWeaponPositions = new Dictionary<GameObject, Vector3>();
        buPanel.SetActive(true);
        selectingAction = true;
        unloadUnitButtons = new List<GameObject>();
        buText.text = "Unload Unit";
        unloadingUnit = transporter;
        //Prevent the camera from moving while we are unloading a unit
        gM.camScript.changeable = false;

        foreach (Unit unit in transporter.loadedUnits)
        {
            unloadUnitButton = Instantiate(unloadUnitButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            unloadUnitButton.transform.SetParent(buPanel.transform);
            unloadUnitButton.transform.localPosition = getNextUnloadUnitButtonPosition();
            unloadUnitButton.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            unloadUnitButtons.Add(unloadUnitButton);
            unit.gameObject.SetActive(true);
            unit.transform.parent = unloadUnitButton.transform;
            unit.transform.localPosition = new Vector3(0, 0, -1);
            unloadUnitButtonScript = unloadUnitButton.GetComponent<UnloadUnitButton>();
            unloadUnitButtonScript.unit = unit;
            //unloadUnitButtonScript.makeUnitTemplate(unitsList.getUnitPrefab(template, faction, gM.boardScript).GetComponent<Unit>(), template);
            unloadUnitButtonScript.ui = this;
            unloadUnitButtonScript.bM = gM.boardScript;
            unloadUnitButtonScript.gM = gM;

            //Make all weapons visisble
            List<WeaponObject> wOList = unit.getAllWeaponObjects();
            foreach (WeaponObject wO in wOList)
            {
                originalWeaponPositions[wO.gameObject] = wO.transform.localPosition;
                wO.transform.localPosition = new Vector3(wO.transform.localPosition.x, wO.transform.localPosition.y, 0);
            }
        }
    }

    public void disableInBattleButtons()
    {
        foreach(Button button in inBattleButtons)
        {
            button.interactable = false;
        }
    }

    public void enableInBattleButtons()
    {
        foreach (Button button in inBattleButtons)
        {
            button.interactable = true;
        }
    }

    //Disable the unit builder menu
    public void destroyUnitBuilderMenu()
    {
        switch (buText.text) {
            case "Build Unit":
                buPanel.SetActive(false);
                selectingAction = false;
                gM.camScript.changeable = true;

                foreach (GameObject button in buildUnitButtons)
                {
                    Destroy(button);
                }
                break;
            case "Unload Unit":
                buPanel.SetActive(false);
                selectingAction = false;
                gM.camScript.changeable = true;

                foreach (GameObject button in unloadUnitButtons)
                {
                    unloadUnitButtonScript = button.GetComponent<UnloadUnitButton>();
                    Unit unit = unloadUnitButtonScript.unit;
                    unit.transform.parent = unloadingUnit.transform;
                    unit.transform.localPosition = new Vector3(0, 0, -1);
                    List<WeaponObject> wOList = unit.getAllWeaponObjects();
                    foreach (WeaponObject wO in wOList)
                    {
                        wO.transform.localPosition = originalWeaponPositions[wO.gameObject];
                    }
                    unit.gameObject.SetActive(false);
                    Destroy(button);

                }
                break;

        }
        enableInBattleButtons();
    }


}
