
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Main Unit Class
public class Unit : Controllable
{
    [SerializeField]
    private float hp = 10f, sizeMultiplier, ap, mp, currentHP, currentAP, currentMP, xWeapon1Scale = 1, yWeapon1Scale = 1, xWeapon2Scale = 1, yWeapon2Scale = 1;

    [SerializeField]
    private int level, vision, xSize, ySize, pplCost, mtlCost;

    [SerializeField]
    private string name, description, armor, tempArmor = null, movementType;

    [SerializeField]
    private List<string> possibleActions;

    //Weapon used for choosing to attack
    public Weapon targetingWeapon;

    //[SerializeField]
    public Weapon currentWeapon;

    public Weapon currentWeapon2;

    [SerializeField]
    private List<Weapon> weapons;


    public List<Weapon> turrets;

    public List<Weapon> allWeapons;

    private Sprite sprite;

    public GameManager gM;

    private GameObject gMObject;

    private UIManager ui;

    [SerializeField]
    private Material rMaterial;

    //Current Weapon object
    [SerializeField]
    private GameObject cwObject;
    [SerializeField]
    private WeaponObject cwScript;

    public GameObject cw2Object;
    public WeaponObject cw2Script;

    public List<GameObject> turretObjects = new List<GameObject>();
    public List<WeaponObject> turretObjectScripts = new List<WeaponObject>();

    
    public Dictionary<Weapon, WeaponObject> weaponDictionary = new Dictionary<Weapon, WeaponObject>(); 

    private WeaponsList weaponsList;

    private List<Weapon> wL;

    [SerializeField]
    private GameObject healthBarPrefab;
    [SerializeField]
    private GameObject healthBar;
    private Slider healthBarSlider;

    //Canvas for drawing the healthbar on
    [SerializeField]
    private Canvas canvas;

    private bool guard = false;
    public float guardCover = 0.5f;
    private bool sentry = false;
    private bool grayScaled;

    public Dictionary<string, float> extraAttributes;
    public bool didInitialCheck = false;

    //Booleans for healing/repairing other units
    //Because of how prefabs are made, these should be controlled via prefab and not unit list
    public bool biological = false;
    public bool mechanical = false;
    public string unitType = "Human";
    public bool healing = false;
    public bool repairing = false;

    //Flying variables - Control in prefabs
    public bool flying = false, canLand = false, hasJetpack = false;
    public Vector3 displacementVector = Vector3.zero;

    public int maxJetToggles = 1, currentJetToggles = 0;
    

    public bool hasShadow = false;
    public GameObject shadowPrefab;
    public GameObject shadow;
    public Vector3 shadowVector;
    public Vector3 shadowScale;

    //Deploy Drone variables - Control using attributes
    public bool deploysDrones = false;
    public string deployType = "Adjacent";
    public List<string> droneTypes = new List<string>();
    public bool dronesAreReliant = false;

    public int maxDronesAtTime = 0;
    public int maxDronesAtAll = 0;
    public int maxDeploysAtTime = 0;
    public int maxDeploysAtAll = 0;
    public int currentDeploys = 0;
    public int currentDrones = 0;
    public int totalDeploys = 0;
    public int totalDrones = 0;

    //Transport Variables
    public string mainTransportType;
    public string secondaryTransportType;
    public string tertiaryTransportType;
    public List<string> transportTypes;

    public bool transportsUnits = false;

    public string loadType = "Adjacent", unloadType = "Adjacent";

    public List<string> includeList, excludeList;

    public List<Unit> loadedUnits = new List<Unit>();

    public int maxCapacity, currentCapacity;

    public float loadTime = 0.5f, unloadTime = 0.5f;


    //Some attribute variables
    public bool dissolvesOnDeath = true;
    public float dissolveTime = 1f;

    //Permanent HP boost
    public float hpBoost, initialHP, initialCurrentHP;

    public bool boostedHP;

    //Sub variables
    public bool isSubmerged = false;

    //OnDeath Variables
    public bool doesDamageOnDeath = false;
    public float damageOnDeath = 0;
    public int damageOnDeathAOE = 0;
    public int damageOnDeathAOEType;

    public string deathAEType = "";

    public bool leavesPoisonGasOnDeath = false;
    public int poisonGasOnDeathAOE = 0;
    public int poisonGasOnDeathAOEType = 0;



    public Dictionary<string, Vector3> unitsMadeOnDeathDict = new Dictionary<string, Vector3>();

    // Use this for initialization
    void Awake()
    {
        //applyScale();
        gMObject = GameObject.FindGameObjectWithTag("GameManager");
        gM = gMObject.GetComponent<GameManager>();
        weaponsList = gM.weaponsList;
        wL = weaponsList.weaponList;
        weapons = new List<Weapon>();
        ui = gM.uiScript;
        gameObject.transform.position += displacementVector;
        isBuilding = false;
        transportTypes = new List<string>() { mainTransportType, secondaryTransportType, tertiaryTransportType };
        //matchWeapon();
        rMaterial = GetComponent<SpriteRenderer>().material;
        GetComponent<Animator>().SetBool("Flying", flying);


    }

    public void makeShadow()
    {
        if (hasShadow)
        {
            shadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity) as GameObject;
            shadow.transform.SetParent(transform);
            shadow.transform.localPosition = shadowVector;
            shadow.transform.localScale = shadowScale;
        }

    }
    public void resetWeaponsList()
    {
        gMObject = GameObject.FindGameObjectWithTag("GameManager");
        gM = gMObject.GetComponent<GameManager>();
        weaponsList = gM.weaponsList;
        wL = weaponsList.weaponList;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void scale(int s)
    {
        sizeMultiplier = s;
        applyScale();
    }

    public void applyScale()
    {
        transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, 1f);
    }

    public float getSizeMultiplier()
    {
        return sizeMultiplier;
    }

    public List<string> getPossibleActions()
    {
        return possibleActions;
    }
    public void setSide(string s)
    {
        side = s;
        setTeam();
    }

    public void setTeam()
    {
        team = getTeam();
    }

    public float getMP()
    {
        return mp;
    }

    public void resetCurrentMP()
    {
        currentMP = mp;
    }


    public float getCurrentMP()
    {
        return currentMP;
    }

    public void setCurrentMP(float m)
    {
        currentMP = m;
    }

    public void move(float m)
    {
        currentMP = (float)Math.Round(currentMP - m,3);
    }

    public Vector3 getPos()
    {
        return transform.position;
    }

    public string getSide()
    {
        return side;
    }

    public int getTeam()
    {
        if (side == null) return -1;
        for (int i = 0; i < gM.teams.Count; i++)
        {
            if (gM.teams[i].Contains(side))
            {
                return i;
            }
        }
        return -1;
    }

    public bool isSameTeam(Unit otherUnit)
    {
        return getTeam() == otherUnit.getTeam();
    }

    public void setTile(Tile t)
    {
        tile = t;
    }

    public Tile getTile()
    {
        return tile;
    }
    public void setCurrentWeapon(int index)
    {
        if (index >= weapons.Count)
        {
            return;
        }
        else
        {
            currentWeapon = weapons[index];
        }
    }

    public Weapon getCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponObject getCurrentWeaponObject()
    {
        return cwScript;
    }
    public void addWeapon(Weapon weapon)
    {
        weapons.Add(weapon);
    }

    public Weapon getWeapon(int index)
    {
        if (index >= weapons.Count)
        {
            return null;
        }
        return weapons[index];
    }

    public Weapon removeWeapon(int index)
    {
        if (index >= weapons.Count)
        {
            return null;
        }
        Weapon temp = weapons[index];
        weapons.RemoveAt(index);
        return temp;
    }

    public void setWeapon(int index, Weapon weapon)
    {
        if (index >= weapons.Count + 1)
        {
            return;
        }
        weapons.Insert(index, weapon);
    }

    public WeaponObject getCWObject()
    {
        return cwScript;
    }

    public void setArmor(string s)
    {
        armor = s;
    }

    //Gets the current armor of the unit
    public string getArmor()
    {
        if (tempArmor != null && tempArmor != "")
        {
            //Debug.Log("Returning " + tempArmor);
            return tempArmor;
        }
        return armor;
    }

    public string getOriginalArmor()
    {
        return armor;
    }

    public void setHP(float h)
    {
        hp = h;
    }

    public float getHP()
    {
        return hp;
    }

    public void setCurrentHP(float h)
    {
        currentHP = h;
    }

    public float getCurrentHP()
    {
        return currentHP;
    }

    public void setSentry(bool s)
    {
        sentry = s;
    }

    public IEnumerator blinkObject(GameObject bO, float blinkSpread, int maxBlinks)
    {
        int blinks = 0;
        while (maxBlinks > blinks)
        {
            if (bO == null) yield break;
            bO.SetActive(false);
            yield return new WaitForSeconds(blinkSpread / 2);
            if (bO == null) yield break;
            bO.SetActive(true);
            yield return new WaitForSeconds(blinkSpread / 2);
            blinks++;
        }
    }

    public IEnumerator showEffect(Sprite sprite, string effectText)
    {
        GameObject effectIO = new GameObject();
        Image effectImage = effectIO.AddComponent<Image>();
        effectImage.sprite = sprite;
        effectImage.GetComponent<RectTransform>().SetParent(canvas.transform);
        effectIO.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5);
        effectIO.SetActive(true);
        effectImage.transform.localScale = new Vector3(0.004f,0.004f,0);

        GameObject effectTO = new GameObject();
        Text effectTXT = effectTO.AddComponent<Text>();
        effectTXT.text = effectText;
        effectTXT.GetComponent<RectTransform>().SetParent(canvas.transform);
        effectTXT.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 6);
        effectTO.SetActive(true);
        effectTXT.transform.localScale = new Vector3(0.015f, 0.015f, 0);
        effectTXT.alignment = TextAnchor.MiddleCenter;
        effectTXT.horizontalOverflow = HorizontalWrapMode.Overflow;
        effectTXT.fontSize = 30;
        effectTXT.font = ui.effectFont;

        float minSpeed = 0.0000075f, maxSpeed = 0.000075f, elapsedTime = 0f, animTime = 0.75f;
        StartCoroutine(blinkObject(effectTO, 0.25f, 3));
        while (elapsedTime < animTime)
        {
            
            float speed = Mathf.Lerp(minSpeed, maxSpeed, elapsedTime / animTime);
            if (effectImage != null)
            {
                effectImage.transform.localScale = new Vector3(effectImage.transform.localScale.x + speed, effectImage.transform.localScale.y + speed, effectImage.transform.localScale.z);
            }
            else
            {
                if (effectTXT != null) Destroy(effectTXT);
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(effectIO);
        Destroy(effectTXT);
    }

    public void startActuallySentry()
    {
        StartCoroutine(actuallySentry());
    }

    public IEnumerator actuallySentry()
    {

        sentry = true;
        useAP(currentAP);
        yield return StartCoroutine(showEffect(ui.attributeSprites[1], "Sentry"));
        checkIfActionPossible();
    }
    public bool getSentry()
    {
        return sentry;
    }

    public void setGuard(bool g)
    {
        guard = g; 
    }

    public void startActuallyGuard()
    {
        StartCoroutine(actuallyGuard());
    }

    public IEnumerator actuallyGuard()
    {
        yield return StartCoroutine(showEffect(ui.attributeSprites[0], "Guard"));
        guard = true;
        useAP(currentAP);
        checkIfActionPossible();
    }


    public bool getGuard()
    {
        return guard;
    }

    public void setMovementType(string mT)
    {
        movementType = mT;
    }

    public string getMovementType()
    {
        return movementType;
    }

    public string getDescription()
    {
        return description;
    }

    public string getType()
    {
        return name;
    }
    //Method used when another unit attacks another unit
    //If HP falls to 0 or less, destroys the unit
    //Must use StartCoroutine to run
    public IEnumerator loseHP(float h)
    {
        if (h > 0)
        {
            currentHP -= h;
            //Debug.Log("Unit "+name+" at "+tile.getPos()+ "is losing " + h + " hp!");
            if (currentHP <= 0) yield return StartCoroutine(die());
            else
            {
                //Debug.Log("Losing "+h+" hp!");
                if (healthBar == null)
                {
                    healthBar = Instantiate(healthBarPrefab, new Vector3(transform.position.x, transform.position.y + 4.5f, transform.position.z), Quaternion.identity) as GameObject;
                    healthBar.transform.SetParent(canvas.transform);
                }
                //Debug.Log(healthBar);
                if (healthBar == null) Debug.LogError("Health bar doesn't exist!");
                healthBarSlider = healthBar.GetComponent<Slider>();
                //Debug.Log(currentHP);
                //Debug.Log(hp);
                healthBarSlider.value = (float)Math.Round(currentHP / hp, 3);
                //yield return null;
            }
        }
    }

    public IEnumerator healHP(float h)
    {
        //Debug.Log("Healing");
        currentHP += h;
        if (currentHP > hp) currentHP = hp;
        if (currentHP == hp && healthBar != null) Destroy(healthBar);

        else if (healthBar != null)
        {
            healthBarSlider = healthBar.GetComponent<Slider>();
            //Debug.Log(currentHP);
            //Debug.Log(hp);
            healthBarSlider.value = (float)Math.Round(currentHP / hp, 3);
        }
        yield return null;
    }


    public void resetAP()
    {
        currentAP = ap;
    }

    public float getAP()
    {
        return ap;
    }
    public float getCurrentAP()
    {
        return currentAP;
    }
    public void useAP(float a)
    {
        currentAP = (float)Math.Round(currentAP - a, 3);
        if (currentAP < 0) currentAP = 0;
    }

    public void setCurrentAP(float a)
    {
        currentAP = a;
    }
    //Play death animation and then kill the unit

    public IEnumerator die()
    {
        if (healthBar != null) Destroy(healthBar);
        if (dissolvesOnDeath)
        {
            float elaspedTime = 0f;
            while (elaspedTime < dissolveTime)
            {
                rMaterial.SetFloat("_Fade", 1 - elaspedTime / dissolveTime);
                elaspedTime += Time.deltaTime;
                yield return null;
            }
        }
        //Perform dissolve animation
        if (leavesPoisonGasOnDeath)
        {
            switch (poisonGasOnDeathAOEType)
            {
                case 0:
                    List<Tile> poisonTiles = gM.getTilesInAbsoluteRange(tile, 0, poisonGasOnDeathAOE);
                    foreach(Tile t in poisonTiles)
                    {
                        StartCoroutine(t.addEffect("Poison Gas", 5, true));
                    }
                    break;
            }
        }
        switch (deathAEType)
        {
            case "Explosion":
                GameObject explosion = Instantiate(gM.unitEffectPrefabs[0], new Vector3(transform.position.x, transform.position.y, transform.position.z - 4), Quaternion.identity);
                yield return new WaitForSeconds(1f);
                Destroy(explosion);
                break;
        }
        if (doesDamageOnDeath)
        {
            List<Tile> explosionTiles = gM.getTilesInAbsoluteRange(tile, 1, 1+damageOnDeathAOE);
            foreach (Tile t in explosionTiles)
            {
                if (t.getUnit() != null)
                {
                    Unit u = t.getUnitScript();
                    float lostHP = damageOnDeath;
                    if (u.getGuard())
                    {
                        lostHP *= (1 - u.guardCover);
                    }
                    if (u.armor == "Slime")
                    {
                        lostHP *= 0.8f;
                    }
                    StartCoroutine(u.loseHP(lostHP));
                }
            }
        }

        gM.unitDictionary[side].Remove(this);
        gM.aiScript.unitDictionary[side].Remove(this);
        Destroy(gameObject);
        yield break;
     
    }

    public float getHPRatio()
    {
        return currentHP / hp;
    }

    public void setupCWObject()
    {
        cwObject.transform.parent = gameObject.transform;
        cwObject.transform.localPosition = new Vector3(0,0,cwObject.transform.localScale.z);
        putWeaponAbove(cwObject);
        cwScript = cwObject.GetComponent<WeaponObject>();
        cwScript.gM = gM;
    }

    public void setupCW2Object()
    {
        cw2Object.transform.parent = gameObject.transform;
        cw2Object.transform.localPosition = new Vector3(0, 0, cwObject.transform.localScale.z);
        putWeaponAbove(cw2Object);
        cw2Script = cw2Object.GetComponent<WeaponObject>();
        cw2Script.gM = gM;
    }

    public void setUpTurret(GameObject turret)
    {
        turret.transform.parent = gameObject.transform;
        turret.transform.localPosition = new Vector3(0, 0, turret.transform.localScale.z);
        putWeaponAbove(turret);
        WeaponObject turretScript = turret.GetComponent<WeaponObject>();
        turretScript.gM = gM;
        turretObjects.Add(turret);
        turretObjectScripts.Add(turretScript);
    }

    //Determines how much hp % is lost from poison from this unit
    public float getPoisonHPEffect()
    {
        switch(mainTransportType)
        {
            case "Infantry":
                switch(secondaryTransportType)
                {
                    case "Human":
                        return 0.25f;
                    case "Experiment":
                        return -0.12f;
                }
                return 0;
            case "Vehicle":
                switch(secondaryTransportType)
                {
                    case "Non-Autonomous":
                        return 0.15f;
                }
                return 0;  
        }
        return 0;
    }

    public void addEffect(string effect, float val, bool overrideCurrent)
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
    }

    public IEnumerator makeEffectsFromWeapon(Weapon weapon, Tile target)
    {
        if (weapon.extraAttributes != null)
        {
            if (target.getUnit() != null)
            {
                Unit effectee = target.getUnitScript();
                if (weapon.extraAttributes.ContainsKey("Poisons") && effectee.getPoisonHPEffect() != 0)
                {
                    effectee.addEffect("Poisoned", weapon.extraAttributes["Poisons"], true);
                    yield return (effectee.showEffect(ui.attributeSprites[5], "Poisoned"));
                }
            }

            //Handle non-unit tile effects
            if (weapon.extraAttributes.ContainsKey("Makes Poison Gas"))
            {
                yield return StartCoroutine(target.addEffect("Poison Gas", weapon.extraAttributes["Makes Poison Gas"],true));
            }
        }
        yield return null;
    }

    //doDefenseAnimation helper method
    public IEnumerator dealDamage(Weapon weapon, Tile target)
    {
        if (target.getUnit() != null)
        {
            Unit enemy = target.getUnitScript();
            yield return StartCoroutine(enemy.loseHP(weapon.getActualDamagePerAttack(this, enemy)));
        }
        yield return StartCoroutine(makeEffectsFromWeapon(weapon, target));

        
    }

    public IEnumerator dealHeal(Weapon weapon, Tile target)
    {
        if (target.getUnit() != null)
        {
            Unit ally = target.getUnitScript();
            //Debug.Log("Deal Heal");
            yield return StartCoroutine(ally.healHP(weapon.getActualHPPerHeal(this, ally)));
        }
        yield return StartCoroutine(makeEffectsFromWeapon(weapon, target));


    }

    public IEnumerator dealAOEDamage(Weapon weapon, Tile target)
    {
        HashSet<Tile> tiles = gM.getAOETiles(this, tile, target, weapon);
        foreach(Tile aoeTile in tiles)
        {
            if (aoeTile != null && aoeTile.getUnit() != null)
            {
                yield return StartCoroutine(dealDamage(weapon, aoeTile));
            }
        }
    }

    public IEnumerator dealAOEHeal(Weapon weapon, Tile target)
    {
        HashSet<Tile> tiles = gM.getAOETiles(this, tile, target, weapon);
        foreach (Tile aoeTile in tiles)
        {
            if (aoeTile != null && aoeTile.getUnit() != null)
            {
                yield return StartCoroutine(dealHeal(weapon, aoeTile));
            }
        }
    }

    //Does the defense animations and deal damage
    public IEnumerator doDefenseAnimation(Tile target)
    {
        //Debug.Log("Defense requested for"+name+"!");
        //Go to the first weapon and check if it does a defense animation
        if (currentWeapon != null && currentWeapon.doesSomething && !currentWeapon.heals)
        {
            if (gM.canAttackEnemyExactlyWithWeapon(this, tile, target.getUnitScript(), target, currentWeapon))
            {
                if (currentWeapon.aoe > 0)
                {
                    yield return StartCoroutine(cwScript.performAOEAnimation(target));
                    yield return StartCoroutine(dealAOEDamage(currentWeapon, target));
                }
                else
                {
                    yield return StartCoroutine(cwScript.performWeaponAnimation(false, target.getUnitScript()));
                    yield return StartCoroutine(dealDamage(currentWeapon, target));
                }

            }
        }

        //Now the 2nd weapon
        if (currentWeapon2 != null && currentWeapon2.doesSomething && !currentWeapon2.heals)
        {
            if (gM.canAttackEnemyExactlyWithWeapon(this, tile, target.getUnitScript(), target, currentWeapon2))
            {
                if (currentWeapon2.aoe > 0)
                {
                    yield return StartCoroutine(cw2Script.performAOEAnimation(target));
                    yield return StartCoroutine(dealAOEDamage(currentWeapon2, target));
                }
                else
                {
                    yield return StartCoroutine(cw2Script.performWeaponAnimation(false, target.getUnitScript()));
                    yield return StartCoroutine(dealDamage(currentWeapon2, target));

                }
            }
        }

        //Debug.Log("Begginning counter attack");
        //Now do all turrets
        if (turrets != null)
        {
            //Debug.Log("Begginning counter attack");
            for (int i = 0; i < turrets.Count; i++)
            {
                //Debug.Log("Checking turret");
                Weapon turret = turrets[i];
                if (turret == null) continue;
                //Debug.Log("turret is not null");
                if (gM.canAttackEnemyExactlyWithWeapon(this, tile, target.getUnitScript(), target, turret))
                {
                    //Debug.Log("Can attack enemy");
                    if (turret.aoe > 0)
                    {
                        yield return StartCoroutine(turretObjectScripts[i].performAOEAnimation(target));
                        yield return StartCoroutine(dealAOEDamage(turret, target));
                    }
                    else
                    {
                        //Debug.Log("NO AOE Turret");
                        yield return StartCoroutine(turretObjectScripts[i].performWeaponAnimation(false, target.getUnitScript()));
                        yield return StartCoroutine(dealDamage(turret, target));
                    }
                }
            }
        }
        //Debug.Log("End counterattack");
        yield return null;
    }

    public IEnumerator doAttackAnimation(Tile target, List<Weapon> weaponsToUse)
    {
        //Debug.Log("Attack animation requested for "+this);
        for (int i = 0; i < weaponsToUse.Count; i++)
        {
            //Technically can also be a primary or secondary weapon
            Weapon turret = weaponsToUse[i];
            //Debug.Log(turret);
            if (turret == null) continue;
            if (gM.canAttackEnemyWithWeapon(this, tile, target.getUnitScript(), target, turret))
            {
                if (turret.aoe > 0)
                {
                    yield return StartCoroutine(weaponDictionary[weaponsToUse[i]].performAOEAnimation(target));
                    yield return StartCoroutine(dealAOEDamage(turret, target));
                }
                else
                {
                    yield return StartCoroutine(weaponDictionary[weaponsToUse[i]].performWeaponAnimation(false, target.getUnitScript()));
                    yield return StartCoroutine(dealDamage(turret, target));
                }
            }
            else
            {
                Debug.Log("Failed check");
                Debug.Log(this + " can't attack " + target + " with " + turret);
            }
        }
    
        yield return null;
    }

    public IEnumerator doHealAnimation(Tile target, List<Weapon> weaponsToUse)
    {
        //Debug.Log("Attack animation requested for "+this);
        for (int i = 0; i < weaponsToUse.Count; i++)
        {
            //Technically can also be a primary or secondary weapon
            Weapon turret = weaponsToUse[i];
            //Debug.Log(turret);
            if (turret == null) continue;
            if (gM.canHealAllyWithWeapon(tile, target, turret))
            {
                if (turret.aoe > 0)
                {
                    yield return StartCoroutine(weaponDictionary[weaponsToUse[i]].performAOEAnimation(target));
                    yield return StartCoroutine(dealAOEHeal(turret, target));
                }
                else
                {
                    yield return StartCoroutine(weaponDictionary[weaponsToUse[i]].performWeaponAnimation(false, target.getUnitScript()));
                    yield return StartCoroutine(dealHeal(turret, target));
                }
            }
            else
            {
                Debug.Log("Failed check");
                Debug.Log(this + " can't attack " + target + " with " + turret);
            }
        }

        yield return null;
    }

    //Generate the weapon associated with this troop
    public void matchWeapon(string use)
    {

        weapons = new List<Weapon>();
        turrets = new List<Weapon>();
        Vector2 pos = Vector2.zero;
        Vector3 pos3D = Vector3.zero;
        switch (name)
        {
            case "Trooper":
            case "Jet Trooper":
                weapons.Add(weaponsList.getWeaponCopy(0));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[0];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1/8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1/8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[0], genWeaponAboveCoords(pos), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.6f;
                cwScript.ySize = 1.6f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                
                break;
            case "Rocketeer":
            case "Jet Rocketeer":
                weapons.Add(weaponsList.getWeaponCopy(1));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[1];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[1], genWeaponAboveCoords(0,-1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 0.6f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Sniper":
                weapons.Add(weaponsList.getWeaponCopy(2));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[2];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[2], genWeaponAboveCoords(0, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1f;
                cwScript.ySize = 1.6f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Mortarman":
                weapons.Add(weaponsList.getWeaponCopy(3));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[3];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(1/8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(1/8f, -1 / 8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[3], genWeaponAboveCoords(1, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.6f;
                cwScript.ySize = 1.25f;
                cwObject.transform.localEulerAngles = new Vector3(0, 0, 30);
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Shielded Trooper":
            case "S-Jet Trooper":
                weapons.Add(weaponsList.getWeaponCopy(4));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[4];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0f/8, -1 / 8f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0f / 8, -1 / 8f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0f/8, -1 / 8f, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[4], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1.25f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(5));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[5];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2/8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2/8f, -1 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[5], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 1.25f;
                cw2Script.ySize = 1.6f;
                cw2Script.useWeapon(weapons[1], this);
                //printAttributeDictionary(weapons[1].extraAttributes);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "Gattler":
                weapons.Add(weaponsList.getWeaponCopy(6));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[6];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[6], genWeaponAboveCoords(0, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Shielded Rocketeer":
                weapons.Add(weaponsList.getWeaponCopy(1));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[0];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[1], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 0.6f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(5));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[5];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[5], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 1.25f;
                cw2Script.ySize = 1.6f;
                cw2Script.useWeapon(weapons[1], this);

                //printAttributeDictionary(weapons[1].extraAttributes);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "Field Medic":
                weapons.Add(weaponsList.getWeaponCopy(4));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[4];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[4], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.25f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(7));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[7];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[7], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 0.45f;
                cw2Script.ySize = 0.45f;
                cw2Script.useWeapon(weapons[1], this);
                //printAttributeDictionary(weapons[1].extraAttributes);
                cw2Object.transform.localEulerAngles = new Vector3(0, 0, -60);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "Field Engineer":
                weapons.Add(weaponsList.getWeaponCopy(4));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[4];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[4], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.25f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(8));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[8];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(-2 / 8f, -2 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[8], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 1f;
                cw2Script.ySize = 1f;
                cw2Script.useWeapon(weapons[1], this);
                //printAttributeDictionary(weapons[1].extraAttributes);
                //cw2Object.transform.localEulerAngles = new Vector3(0, 0, 90);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "Droid Trooper":
            case "Jet Droid":
                weapons.Add(weaponsList.getWeaponCopy(9));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[9];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[9], genWeaponAboveCoords(pos), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.6f;
                cwScript.ySize = 1.6f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                break;
            case "Shielded Droid":
                weapons.Add(weaponsList.getWeaponCopy(10));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[10];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0f / 8, -1 / 8f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0f / 8, -1 / 8f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0f / 8, -1 / 8f, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[10], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;

                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1.25f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(5));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[5];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[5], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 1.25f;
                cw2Script.ySize = 1.6f;
                cw2Script.useWeapon(weapons[1], this);
                //printAttributeDictionary(weapons[1].extraAttributes);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "S-Droid Rocketeer":
                weapons.Add(weaponsList.getWeaponCopy(11));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[11];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[11], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 0.6f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                //Now do the shield
                weapons.Add(weaponsList.getWeaponCopy(5));
                weapons[1].isSecondary = true;
                weapons[1].uiSprite = ui.uiWeaponSprites[5];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2 / 8f, -1 / 8f);
                        break;


                }
                currentWeapon2 = weapons[1];
                cw2Object = Instantiate(gM.weaponPrefabs[5], genWeaponAboveCoords(2f, -1), Quaternion.identity) as GameObject;
                setupCW2Object();
                offsetWeapon(cw2Object, pos);
                cw2Script.xSize = 1.25f;
                cw2Script.ySize = 1.6f;
                cw2Script.useWeapon(weapons[1], this);

                //printAttributeDictionary(weapons[1].extraAttributes);
                weaponDictionary.Add(weapons[1], cw2Script);
                break;
            case "Droid Gattler":
                weapons.Add(weaponsList.getWeaponCopy(12));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[12];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[12], genWeaponAboveCoords(0, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Jet Droid Rocketeer":
                weapons.Add(weaponsList.getWeaponCopy(11));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[11];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, -1 / 8f, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[11], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 0.6f;
                cwScript.ySize = 1.25f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Drone":
                weapons.Add(weaponsList.getWeaponCopy(13));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[13];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[13], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Tank":
                weapons.Add(weaponsList.getWeaponCopy(14));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[14];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[14], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Heavy Tank":
                weapons.Add(weaponsList.getWeaponCopy(15));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[15];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[15], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Assault Transporter":
                turrets.Add(weaponsList.getWeaponCopy(16));
                turrets[0].isTurret = true;
                turrets[0].uiSprite = ui.uiWeaponSprites[16];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0.072f, 0.297f, - 1);
                        break;
                    case "UI":
                        pos3D = new Vector3(-0.072f, 0.297f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(-0.072f, 0.297f, -1);
                        break;


                }
                //setCurrentWeapon(0);

                GameObject turretObject = Instantiate(gM.weaponPrefabs[16], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                //setupCWObject();
                setUpTurret(turretObject);
                offsetWeapon(turretObject, pos3D);
                WeaponObject turretScript = turretObject.GetComponent<WeaponObject>();
                turretScript.xSize = 0.55f;
                turretScript.ySize = 0.55f;
                turretScript.useWeapon(turrets[0], this);
                weaponDictionary.Add(turrets[0], turretScript);
                break;
            case "Rocket Truck":
                weapons.Add(weaponsList.getWeaponCopy(17));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[17];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[17], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Artillery Truck":
                weapons.Add(weaponsList.getWeaponCopy(18));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[18];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[18], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Assault Tank":
                weapons.Add(weaponsList.getWeaponCopy(14));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[14];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[14], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                turrets.Add(weaponsList.getWeaponCopy(16));
                turrets[0].isTurret = true;
                turrets[0].uiSprite = ui.uiWeaponSprites[16];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0.05f, 0.37918f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(-0.05f, 0.37918f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(-0.05f, 0.37918f, -1);
                        break;


                }
                //setCurrentWeapon(0);

                turretObject = Instantiate(gM.weaponPrefabs[16], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                //setupCWObject();
                setUpTurret(turretObject);
                offsetWeapon(turretObject, pos3D);
                turretScript = turretObject.GetComponent<WeaponObject>();
                turretScript.xSize = 0.55f;
                turretScript.ySize = 0.55f;
                turretScript.useWeapon(turrets[0], this);
                weaponDictionary.Add(turrets[0], turretScript);
                break;
            case "Assault Heavy Tank":
                weapons.Add(weaponsList.getWeaponCopy(15));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[15];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[15], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                turrets.Add(weaponsList.getWeaponCopy(16));
                turrets[0].isTurret = true;
                turrets[0].uiSprite = ui.uiWeaponSprites[16];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0.046f, 0.403f, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(-0.046f, 0.403f, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(-0.046f, 0.403f, -1);
                        break;


                }
                //setCurrentWeapon(0);

                turretObject = Instantiate(gM.weaponPrefabs[16], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                //setupCWObject();
                setUpTurret(turretObject);
                offsetWeapon(turretObject, pos3D);
                turretScript = turretObject.GetComponent<WeaponObject>();
                turretScript.xSize = 0.55f;
                turretScript.ySize = 0.55f;
                turretScript.useWeapon(turrets[0], this);
                weaponDictionary.Add(turrets[0], turretScript);

                turrets.Add(weaponsList.getWeaponCopy(16));
                turrets[1].isTurret = true;
                turrets[1].uiSprite = ui.uiWeaponSprites[16];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(-0.045f, 0.275f, -2);
                        break;
                    case "UI":
                        pos3D = new Vector3(-0.045f, 0.275f, -2);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(-0.045f, 0.275f, -2);
                        break;


                }
                //setCurrentWeapon(0);

                turretObject = Instantiate(gM.weaponPrefabs[16], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                //setupCWObject();
                setUpTurret(turretObject);
                offsetWeapon(turretObject, pos3D);
                turretScript = turretObject.GetComponent<WeaponObject>();
                turretScript.xSize = 0.55f;
                turretScript.ySize = 0.55f;
                turretScript.useWeapon(turrets[1], this);
                weaponDictionary.Add(turrets[1], turretScript);
                break;
            case "Masked Trooper":
            case "Mutant Trooper":
                weapons.Add(weaponsList.getWeaponCopy(19));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[19];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(0, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(0, -1 / 8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[19], genWeaponAboveCoords(pos), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.6f;
                cwScript.ySize = 1.6f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                break;
            case "Grenadier":
                weapons.Add(weaponsList.getWeaponCopy(20));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[20];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(1/8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[20], genWeaponAboveCoords(pos), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 0.8f;
                cwScript.ySize = 0.8f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);

                break;
            case "MG Mortarman":
                weapons.Add(weaponsList.getWeaponCopy(21));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[21];
                setCurrentWeapon(0);
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(1 / 8f, -1 / 8f);
                        break;


                }
                cwObject = Instantiate(gM.weaponPrefabs[21], genWeaponAboveCoords(1, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 1.92f;
                cwScript.ySize = 1.5f;
                cwObject.transform.localEulerAngles = new Vector3(0, 0, 30);
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Scientist":
                weapons.Add(weaponsList.getWeaponCopy(22));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[22];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "UI":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;
                    case "Unit Template":
                        pos = new Vector2(2f / 8, -1 / 8f);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[22], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos);
                cwScript.xSize = 0.8f;
                cwScript.ySize = 0.8f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Screamer":
                weapons.Add(weaponsList.getWeaponCopy(23));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[23];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[23], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Brewer":
                weapons.Add(weaponsList.getWeaponCopy(24));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[24];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[24], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;
            case "Eyesore":
                weapons.Add(weaponsList.getWeaponCopy(25));
                weapons[0].isPrimary = true;
                weapons[0].uiSprite = ui.uiWeaponSprites[25];
                switch (use)
                {
                    case "Build Unit":
                    case "Make Unit":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "UI":
                        pos3D = new Vector3(0, 0, -1);
                        break;
                    case "Unit Template":
                        pos3D = new Vector3(0, 0, -1);
                        break;


                }
                setCurrentWeapon(0);
                cwObject = Instantiate(gM.weaponPrefabs[25], genWeaponAboveCoords(-0.5f, -1), Quaternion.identity) as GameObject;
                setupCWObject();
                offsetWeapon(cwObject, pos3D);
                cwScript.xSize = 1f;
                cwScript.ySize = 1f;
                cwScript.useWeapon(weapons[0], this);
                weaponDictionary.Add(weapons[0], cwScript);
                break;


        }
        if (weapons != null && weapons.Count > 0)
        {
            targetingWeapon = weapons[0];
        }
        else
        {
            targetingWeapon = null;
        }

    }

    //Undoes the UI scale
    public void undoUIScale()
    {
        //cwObject.transform.localScale = new Vector3(cwObject.transform.localScale.x / 1, cwObject.transform.localScale.y / 1, 1);
        /*if (cwObject != null)
        {
            cwObject.transform.localPosition = new Vector3(cwObject.transform.localPosition.x / xWeapon1Scale, cwObject.transform.localPosition.y / yWeapon1Scale,
                cwObject.transform.localPosition.z);
        }
        if (cw2Object != null)
        {
            cw2Object.transform.localPosition = new Vector3(cw2Object.transform.localPosition.x / xWeapon2Scale, cw2Object.transform.localPosition.y / yWeapon2Scale,
                 cw2Object.transform.localPosition.z);
        }*/
        transform.localScale = new Vector3(transform.localScale.x * 5 / 40f, transform.localScale.y * 5 / 40f, 1);

    }

    public Vector3 genWeaponAboveCoords(float xOffset, float yOffset)
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
    }

    public Vector3 genWeaponAboveCoords(float xOffset, float yOffset, float zOffset)
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + zOffset);
    }

    public Vector3 genWeaponAboveCoords(Vector2 offsetVector)
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
    }

    public void putWeaponAbove(GameObject weapon)
    {
        weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x, weapon.transform.localPosition.y, transform.localPosition.z - 2);
    }

    public void offsetWeapon(GameObject weapon, Vector2 pos)
    {
        weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + pos.x, weapon.transform.localPosition.y + pos.y, weapon.transform.localPosition.z);
    }

    public void offsetWeapon(GameObject weapon, Vector3 pos)
    {
        weapon.transform.localPosition = new Vector3(weapon.transform.localPosition.x + pos.x, weapon.transform.localPosition.y + pos.y, weapon.transform.localPosition.z + pos.z);
    }

    public List<Weapon> getAllWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (turrets != null) toRet.AddRange(turrets);
        if (weapons != null) toRet.AddRange(weapons);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");

        return toRet;
    }

    public List<WeaponObject> getAllWeaponObjects()
    {
        List<WeaponObject> toRet = new List<WeaponObject>();
        if (cwScript != null) toRet.Add(cwScript);
        if (cw2Script != null) toRet.Add(cw2Script);
        if (turretObjectScripts != null) toRet.AddRange(turretObjectScripts);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");

        return toRet;
    }

    public List<Weapon> getAllHandWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null) toRet.Add(currentWeapon);
        if (currentWeapon2 != null) toRet.Add(currentWeapon2);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    public List<Weapon> getAllDamageHandWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && currentWeapon.damages) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && currentWeapon2.damages) toRet.Add(currentWeapon2);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    public List<Weapon> getAllDamageActiveWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && currentWeapon.damages) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && currentWeapon2.damages) toRet.Add(currentWeapon2);
        if (turrets != null && turrets.Count > 0) toRet.AddRange(turrets);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.damages);
        return toRet;
    }

    public List<Weapon> getAllNonTurretDamageWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (weapons != null && weapons.Count > 0) toRet.AddRange(weapons);
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.damages);
        return toRet;
    }

    public List<Weapon> getAllDamageWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (weapons != null && weapons.Count > 0) toRet.AddRange(weapons);
        if (turrets != null && turrets.Count > 0) toRet.AddRange(turrets);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.damages);
        return toRet;
    }


    public List<Weapon> getAllHealRepairHandWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && (currentWeapon.heals || currentWeapon.repairs)) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && (currentWeapon2.heals || currentWeapon2.repairs)) toRet.Add(currentWeapon2);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    public List<Weapon> getAllHealRepairActiveWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && (currentWeapon.heals || currentWeapon.repairs)) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && (currentWeapon2.heals || currentWeapon2.repairs)) toRet.Add(currentWeapon2);
        if (turrets != null && turrets.Count > 0) toRet.AddRange(turrets);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !(item.heals || item.repairs));
        return toRet;
    }

    public List<Weapon> getAllHealRepairTurretWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (turrets != null && turrets.Count > 0) toRet.AddRange(turrets);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !(item.heals || item.repairs));
        return toRet;
    }

    public List<Weapon> getAllHealRepairNonTurretWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (weapons != null && weapons.Count > 0) toRet.AddRange(weapons);
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !(item.heals || item.repairs));
        return toRet;
    }

    public List<Weapon> getAllHealWeaponsFromList(List<Weapon> weapons)
    {
        List<Weapon> toRet = new List<Weapon>(weapons);
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.heals);
        return toRet;
    }

    public List<Weapon> getAllRepairWeaponsFromList(List<Weapon> weapons)
    {
        List<Weapon> toRet = new List<Weapon>(weapons);
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.repairs);
        return toRet;
    }

    public List<Weapon> getAllDamageWeaponsFromList(List<Weapon> weapons)
    {
        List<Weapon> toRet = new List<Weapon>(weapons);
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "" || !item.damages);
        return toRet;
    }

    public List<Weapon> getAllHealHandWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && (currentWeapon.heals)) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && (currentWeapon2.heals)) toRet.Add(currentWeapon2);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    public List<Weapon> getAllRepairHandWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null && (currentWeapon.repairs)) toRet.Add(currentWeapon);
        if (currentWeapon2 != null && (currentWeapon2.repairs)) toRet.Add(currentWeapon2);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    public IEnumerator captureBuilding()
    {
        //Debug.Log("Capturing building");
        if (tile.getBuilding() != null)
        {
            yield return StartCoroutine(tile.getBuilding().capture(this));
        }
    }
    
    public List<Weapon> getAllActiveWeapons()
    {
        List<Weapon> toRet = new List<Weapon>();
        if (currentWeapon != null) toRet.Add(currentWeapon);
        if (currentWeapon2 != null) toRet.Add(currentWeapon2);
        if (turrets != null && turrets.Count > 0) toRet.AddRange(turrets);
        //Use lambda to remove all null weapons
        toRet.RemoveAll(item => item == null || item.name == null || item.name == "");
        return toRet;
    }

    //Gets only the AOE weapons from a list
    public List<Weapon> getOnlyAOEWeapons(List <Weapon> weapons)
    {
        List<Weapon> temp = new List<Weapon>(weapons);
        //Debug.Log(weapons[0].name + " has an aoe of " + weapons[0].aoe);
        temp.RemoveAll(item => item.aoe <= 0);
        return temp;
    }

    //Gets only the Non-AOE weapons from a list
    public List<Weapon> getOnlyDirectWeapons(List<Weapon> weapons)
    {
        List<Weapon> temp = new List<Weapon>(weapons);
        //Debug.Log(temp[0].name + " has an aoe of " + temp[0].aoe);
        temp.RemoveAll(item => item.aoe > 0);
        //Debug.Log(temp[0].name + " has an aoe of " + temp[0].aoe);
        return temp;
    }

    //Reset AP, MP, and do status attributes
    public void startTurn()
    {
        resetAP();
        resetCurrentMP();
        sentry = false;
        guard = false;
        whiteScale();

        currentJetToggles = 0;

        //Debug.Log("Starting turn for: " + name);
        if (currentWeapon2 != null)
        {
            //Debug.Log("Getting infor about " + currentWeapon2);
            //printAttributeDictionary(currentWeapon2.extraAttributes);
        }
        //Handle weapon attributes
        List<Weapon> allActiveWeapons = getAllActiveWeapons();
        tempArmor = null;

        initialHP = getHP();
        initialCurrentHP = getCurrentHP();
        if (extraAttributes != null)
        {
            if (extraAttributes.ContainsKey("Poisoned"))
            {
                float hpChange = hp * getPoisonHPEffect();
                Debug.Log(hpChange);
                if (hpChange < 0)
                {
                    StartCoroutine(healHP(-hpChange));
                }
                else
                {
                    StartCoroutine(loseHP(hpChange));
                }
                extraAttributes["Poisoned"] -= 1;
                if (extraAttributes["Poisoned"] == 0)
                {
                    extraAttributes.Remove("Poisoned");
                }
            }
        }

        foreach (Weapon weapon in allActiveWeapons)
        {
            weapon.currentAttacks = 0;
           //Debug.Log("Checking " + weapon.name + " and has attribute dictionary: " );
            //printAttributeDictionary(weapon.extraAttributes);
            if (weapon.extraAttributes != null)
            {
                //Debug.Log("Checking weapon attributes");
                foreach(string attribute in weapon.extraAttributes.Keys)
                {
                    if (attribute == "Override To X Armor")
                    {
                        switch(weapon.extraAttributes[attribute])
                        {
                            case 0:
                                tempArmor = "Light";
                                break;
                            case 1:
                                tempArmor = "Medium";
                                break;
                            case 2:
                                tempArmor = "Heavy";
                                break;
                            case 3:
                                tempArmor = "Slime";
                                break;
                        }
                    }
                    else if (attribute == "HP Bonus")
                    {
                        if (!boostedHP)
                        {
                                hpBoost += weapon.extraAttributes[attribute];
                                currentHP = initialCurrentHP + hpBoost;
                                hp = initialHP + hpBoost;
                           
                        }

                    }
                }
            }
            else
            {
                //Debug.Log(weapon.name + " has no attribute dictionary ");
            }
        }
        boostedHP = true;
        didInitialCheck = true;
    }

    public bool checkIfActionPossible()
    {
        //Debug.Log("Checking for possible action");
        foreach (string action in possibleActions)
        {
            switch (action)
            {
                case "Move":
                    List<Tile> moveables = gM.getMoveTiles(tile);
                    //Debug.Log(moveables);
                    if (moveables != null && moveables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Attack":
                    List<Tile> attackbles = gM.getAttackTiles(this, tile);
                    if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Fortify":
                case "Sentry":
                    if (currentAP > 0) return true;
                    break;
                case "Capture":
                    if (currentAP > 0 && tile.getBuilding() != null && tile.getBuilding().team != getTeam())
                    {
                        return true;
                    }
                    break;
                case "Deploy Drones":
                case "Deploy Units":
                    if (canDeploy() && currentAP > 0)
                    {
                        return true;
                    }
                    break;
                case "Heal":
                    List<Tile> healables = gM.getHealTiles(this, tile, getAllHealHandWeapons());
                    if (currentAP > 0 && healables != null && healables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Repair":
                    healables = gM.getHealTiles(this, tile, getAllRepairHandWeapons());
                    if (currentAP > 0 && healables != null && healables.Count > 0)
                    {
                        return true;
                    }
                    break;
                case "Load Units": 
                    if (canLoadUnits() && currentAP > 0)
                    {
                        return true;
                    }
                    break;
                case "Unload Units":
                    if (canUnloadUnits() && currentAP > 0)
                    {
                        return true;
                    }
                    break;
                case "Fire Turret 1":
                    if (turrets != null && turrets.Count > 0)
                    {
                        attackbles = gM.getAttackTilesWithWeapons(this, tile, new List<Weapon>() { turrets[0] });
                        if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 2":
                    if (turrets != null && turrets.Count > 1)
                    {
                        attackbles = gM.getAttackTilesWithWeapons(this, tile, new List<Weapon>() { turrets[1] });
                        if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 3":
                    if (turrets != null && turrets.Count > 2)
                    {
                        attackbles = gM.getAttackTilesWithWeapons(this, tile, new List<Weapon>() { turrets[2] });
                        if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 4":
                    if (turrets != null && turrets.Count > 3)
                    {
                        attackbles = gM.getAttackTilesWithWeapons(this, tile, new List<Weapon>() { turrets[3] });
                        if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
                case "Fire Turret 5":
                    if (turrets != null && turrets.Count > 4)
                    {
                        attackbles = gM.getAttackTilesWithWeapons(this, tile, new List<Weapon>() { turrets[4] });
                        if (currentAP > 0 && attackbles != null && attackbles.Count > 0)
                        {
                            return true;
                        }
                    }
                    break;
            }
        }
        grayScale();
        return false;
    }

    public void startActuallyCapture()
    {
        StartCoroutine(actuallyCapture());

    }
    public IEnumerator actuallyCapture()
    {
        //Debug.Log("Actually capturing");
        StartCoroutine(showEffect(ui.attributeSprites[4], "Capture"));
        yield return StartCoroutine(captureBuilding());
        currentAP = 0;
        checkIfActionPossible();
    }

    //Deploy drones
    public void deployDrones()
    {
        Debug.Log("Deploying Drones");
        if (deploysDrones && droneTypes != null && droneTypes.Count > 0)
        {
            //Return if we reached our limit on drones
            //Debug.Log(maxDeploysAtAll);
            //Debug.Log(totalDeploys);
            //Debug.Log(totalDeploys >= maxDeploysAtAll);
            if (maxDeploysAtAll > 0 && totalDeploys >= maxDeploysAtAll) return;
            //Debug.Log("Cleared to Deploy 1");
            if (maxDronesAtAll > 0 && totalDrones >= maxDronesAtAll) return;
            //Debug.Log("Cleared to Deploy 2");
            if (maxDeploysAtTime > 0 && currentDeploys >= maxDeploysAtTime) return;
            //Debug.Log("Cleared to Deploy 3");
            if (maxDronesAtTime > 0 && currentDrones >= maxDronesAtTime) return;
            UnitsList unitsList = new UnitsList();
            //Debug.Log("Cleared to Deploy 4");
            switch(deployType)
            {
                case "Adjacent":
                    //Get Adjacent Tiles
                    List<Tile> adjacentTiles = tile.getAdjacent();
                    //Debug.Log(adjacentTiles[0]);
                    foreach(Tile adjacentTile in adjacentTiles)
                    {
                        if (adjacentTile.getUnit() == null)
                        {
                            //Debug.Log("Making drone");
                            Unit tempDrone = gM.boardScript.makeUnit(droneTypes[0], gM.playerDictionary[side], adjacentTile.mapX, adjacentTile.mapY);
                            tempDrone.useAP(tempDrone.getCurrentAP());
                            tempDrone.move(tempDrone.getCurrentMP());
                            tempDrone.grayScale();
                            currentDrones++;
                            totalDrones++;
                        }
                    }
                    //Debug.Log("Made drones");
                    currentDeploys++;
                    totalDeploys++;
                    useAP(1);
                    checkIfActionPossible();
                    break;
                    
            }

        }
    }

    //Determine if we can deploy
    public bool canDeploy()
    {
        if (deploysDrones && droneTypes != null && droneTypes.Count > 0)
        {
            if (maxDeploysAtAll > 0 && totalDeploys >= maxDeploysAtAll) return false;
            if (maxDronesAtAll > 0 && totalDrones >= maxDronesAtAll) return false;
            if (maxDeploysAtTime > 0 && currentDeploys >= maxDeploysAtTime) return false;
            if (maxDronesAtTime > 0 && currentDrones >= maxDronesAtTime) return false;
            switch (deployType)
            {
                case "Adjacent":
                    List<Tile> adjacentTiles = tile.getAdjacent();
                    foreach(Tile adjacentTile in adjacentTiles)
                    {
                        if (adjacentTile.getUnit() == null)
                        {
                            return true;
                        }
                    }
                    return false;

            }
            return false;
        }
        return false;
    }

    public IEnumerator loadUnit(Unit unit, bool animated)
    {
        loadedUnits.Add(unit);
        unit.transform.parent = transform;
        //unit.transform.localPosition = new Vector3(0, 0, unit.transform.localPosition.z);
        if (animated)
        {
            gM.animationInProgress = true;
            gM.disableButtons();
            yield return StartCoroutine(loadUnitAnimation(unit));
            gM.animationInProgress = false;
            gM.enableButtons();
        }
        else
        {
            unit.transform.localPosition = new Vector3(0, 0, unit.transform.localPosition.z);
        }
        unit.gameObject.SetActive(false);
        unit.getTile().setUnit(null);
        unit.setTile(null);
        checkIfActionPossible();
        useAP(1);
        currentCapacity++;
    }

    public IEnumerator loadUnitAnimation(Unit unit)
    {
        switch (loadType)
        {
            case "Adjacent":
                unit.gameObject.GetComponent<Animator>().SetBool("Moving", true);
                Vector3 start = unit.transform.localPosition;
                Vector3 end = new Vector3(0, 0, unit.transform.localPosition.z);
                gM.lookAtRightDir(unit, tile);
                float elapsedTime = 0;
                while (elapsedTime < loadTime)
                {
                    unit.transform.localPosition = Vector3.Lerp(start, end, (elapsedTime / loadTime));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                unit.transform.localPosition = end;
                unit.gameObject.GetComponent<Animator>().SetBool("Moving", false);
                break;
        }
    }

    //Determines if we can load units
    public bool canLoadUnits()
    {
        if (currentCapacity < maxCapacity)
        {
            switch(loadType)
            {
                case "Adjacent":
                    foreach (Tile t in tile.adjacent)
                    {
                        if (t!= null & t.getUnit() != null && t.getUnitScript().getSide() == side)
                        {
                            Unit u = t.getUnitScript();
                            bool isExcluded = false;
                            foreach(string excluder in excludeList)
                            {
                                if (excluder == u.mainTransportType || excluder == u.secondaryTransportType || excluder == u.tertiaryTransportType)
                                {
                                    isExcluded = true;
                                    break;
                                }
                            }
                            if (isExcluded) return false;
                            bool isIncluded = false;
                            foreach(string includer in includeList)
                            {
                                if (includer == u.mainTransportType || includer == u.secondaryTransportType || includer == u.tertiaryTransportType)
                                {
                                    isIncluded = true;
                                    break;
                                }
                            }
                            if (isIncluded)
                            {
                                return true;
                            }
                        }
                        
                    }
                    return false;

            }
        }
        return false;
    }

    public IEnumerator unloadUnit(Unit unloadee, Tile t, bool animated)
    {
        unloadee.gameObject.SetActive(true);
        unloadee.transform.parent = null;
        //unit.transform.localPosition = new Vector3(0, 0, unit.transform.localPosition.z);
        if (animated)
        {
            gM.animationInProgress = true;
            gM.disableButtons();
            yield return StartCoroutine(unloadUnitAnimation(unloadee,t));
            gM.animationInProgress = false;
            gM.enableButtons();
        }
        else
        {
            unloadee.transform.position = new Vector3(0, tile.transform.position.y, unloadee.transform.localPosition.z);
        }
        unloadee.setTile(t);
        t.setUnit(unloadee.gameObject);

        useAP(1);
        checkIfActionPossible();
        currentCapacity--;
        loadedUnits.Remove(unloadee);
    }

    public IEnumerator unloadUnitAnimation(Unit unit, Tile t)
    {
        switch (loadType)
        {
            case "Adjacent":
                unit.gameObject.GetComponent<Animator>().SetBool("Moving", true);
                Vector3 start = unit.transform.position;
                Vector3 end = new Vector3(t.getXPos(), t.getYPos(), unit.transform.localPosition.z);
                gM.lookAtRightDir(unit, t);
                float elapsedTime = 0;
                while (elapsedTime < loadTime)
                {
                    unit.transform.localPosition = Vector3.Lerp(start, end, (elapsedTime / loadTime));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                unit.transform.localPosition = end;
                unit.gameObject.GetComponent<Animator>().SetBool("Moving", false);
                break;
        }
    }

    public void markTilesForUnloading(Unit unloadee)
    {
        switch(unloadType)
        {
            case "Adjacent":
                //Get the tiles we can unload on
                foreach (Tile t in tile.adjacent)
                {
                    if (t!= null && t.getUnit() == null)
                    {
                        t.makeUnloadable(this, unloadee);
                    }
                }
                break;
        }
    }

    //Determine if we can unload our unit into a tile
    public bool canUnloadUnits()
    {
        if (currentCapacity > 0)
        {
            switch (unloadType)
            {
                case "Adjacent":
                    foreach (Tile t in tile.adjacent)
                    {
                        if (t != null && t.getUnit() == null)
                        {
                            return true;
                        }
                    }
                    return false;
            }
        }
        return false;
    }

    public void startToggleJetpack()
    {
        StartCoroutine(toggleJetpack());
    }

    public IEnumerator toggleJetpack()
    {
        if (canLand && currentJetToggles < maxJetToggles)
        {
            //Debug.Log("Toggling Jetpack");
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + (flying ? -displacementVector : displacementVector);
            float elaspedTime = 0f;
            float animTime = 1f;
            
            if (flying)
            {

                Vector3 shadowStartPos = shadow.transform.position;
                while(elaspedTime < animTime)
                {
                    transform.position = Vector3.Lerp(startPos,endPos,elaspedTime / animTime);
                    shadow.transform.position = Vector3.Lerp(shadowStartPos, endPos, elaspedTime / animTime / 5);
                    elaspedTime += Time.deltaTime;
                    //Debug.Log("Lerping");
                    yield return null;
                    //if (elaspedTime < animTime) Debug.Log("Log");

                }
                transform.position = endPos;
                //Debug.Log("Test");
                Destroy(shadow.gameObject);
                GetComponent<Animator>().SetBool("Flying", false);
                flying = false;

            }
            else
            {
                makeShadow();
                GetComponent<Animator>().SetBool("Flying", true);
                while (elaspedTime < 1)
                {
                    transform.position = Vector3.Lerp(startPos, endPos, elaspedTime/ animTime);
                    elaspedTime += Time.deltaTime;
                    yield return null;

                }
                transform.position = endPos;

                flying = true;
            }
        }
        currentJetToggles++;
    }

    public void setPPLCost(int c)
    {
        pplCost = c;
    }

    public int getPPLCost()
    {
        return pplCost;
    }

    public void setMTCost(int c)
    {
        mtlCost = c;
    }

    public void grayScale()
    {
        rMaterial.SetColor("_BaseColor",Color.gray);
        

    }

    public Color grayScaleColor(Color color)
    {
        color.r /= 2;
        color.g /= 2;
        color.b /= 2;
        return color;
    }

    public void whiteScale()
    {
        rMaterial.SetColor("_BaseColor", Color.white);
    }
    
    public void setOutlineColor(Color color, float intensity)
    {
        //if (name == "Tank") intensity *= 1 / 5f;
        rMaterial.SetColor("_OutlineColor", new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a));
    }

    public void setOutlineThickness(float thickness)
    {
        rMaterial.SetFloat("_Thickness", thickness);
    }

    public void setTintColor(Color color)
    {
        rMaterial.SetColor("_TintColor", color);
    }

    public IEnumerator blinkTintColor(Color color, float blinkSpread, int numBlinks)
    {
        Color originalColor = rMaterial.GetColor("_TintColor");
        for(int i = 0; i < numBlinks; i++)
        {
            rMaterial.SetColor("_TintColor", color);
            yield return new WaitForSeconds(blinkSpread/2);
            rMaterial.SetColor("_TintColor", originalColor);
            if (i <= numBlinks - 1)
            {
                yield return new WaitForSeconds(blinkSpread / 2);
            }
        }
    }

    public int getMTCost()
    {
        return mtlCost;
    }

    //Uses the UnitList and UnitTemplate classes to generate unit info
    public void useTemplate(int type)
    {
        UnitsList units = new UnitsList();
        useTemplate(units.unitsList[type]);       
    }

    public void useTemplate(UnitTemplate unitData)
    {
        hp = unitData.hp;
        sizeMultiplier = unitData.sizeMultiplier;
        ap = unitData.ap;
        mp = unitData.mp;
        currentHP = unitData.currentHP;
        currentAP = unitData.currentAP;
        currentMP = unitData.currentMP;
        pplCost = unitData.pplCost;
        mtlCost = unitData.mtCost;
        name = unitData.name;
        description = unitData.description;
        side = unitData.side;
        armor = unitData.armor;
        movementType = unitData.movementType;

        //Start Deploy Drone Variables
        deploysDrones = unitData.deploysDrones;
        dronesAreReliant = unitData.dronesAreReliant;
        deployType = unitData.deployType;
        droneTypes = unitData.droneTypes;

        maxDronesAtTime = unitData.maxDronesAtTime;
        maxDronesAtAll = unitData.maxDronesAtAll;
        currentDrones = unitData.currentDrones;
        totalDrones = unitData.totalDrones;

        maxDeploysAtTime = unitData.maxDeploysAtTime;
        maxDeploysAtAll = unitData.maxDeploysAtAll;
        currentDeploys = unitData.currentDeploys;
        totalDeploys = unitData.totalDeploys;

        //End Deploy Drone Variables

        //Transport Variables

        transportsUnits = unitData.transportsUnits;

        includeList = unitData.includeList;
        excludeList = unitData.excludeList;

        maxCapacity = unitData.maxCapacity;
        currentCapacity = unitData.currentCapacity;

        //End Transport Variables

        //Start On Death Variables
        doesDamageOnDeath = unitData.doesDamageOnDeath;
        damageOnDeath = unitData.damageOnDeath;
        damageOnDeathAOE = unitData.damageOnDeathAOE;
        damageOnDeathAOEType = unitData.damageOnDeathType;

        deathAEType = unitData.deathAEType;

        leavesPoisonGasOnDeath = unitData.leavesPoisonGasOnDeath;
        poisonGasOnDeathAOE = unitData.poisonGasOnDeathAOE;
        poisonGasOnDeathAOEType = unitData.poisonGasOnDeathAOEType;

        unitsMadeOnDeathDict = unitData.unitsMadeOnDeathDict;

        //End On Death Variables

        possibleActions = unitData.possibleActions;
        currentWeapon = unitData.currentWeapon;
        currentWeapon2 = unitData.currentWeapon2;
        weapons = unitData.weapons;
        turrets = unitData.turrets;
        extraAttributes = unitData.extraAttributes;
    }

    //Determines if this unit has any attack capabilities regardless of AP/MP
    public bool canAttackAbsolute()
    {
         
        if (possibleActions.Contains("Attack"))
        {
            List<Weapon> dmgHandWeapons = getAllDamageHandWeapons();
            if (dmgHandWeapons != null && dmgHandWeapons.Count > 0)
            {
                return true;
            }
            else if (possibleActions.Contains("Change Weapon"))
            {
                List<Weapon> dmgWeapons = getAllNonTurretDamageWeapons();
                if (dmgWeapons != null)
                {
                    dmgWeapons.RemoveAll(item => !item.damages);
                    if (dmgWeapons.Count > 0)
                    {
                        return true;
                    }
                }
            }
            
        }
        if (possibleActions.Contains("Fire Turret 1"))
        {
            if (turrets != null && turrets.Count > 0 && turrets[0].damages)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 2"))
        {
            if (turrets != null && turrets.Count > 1 && turrets[1].damages)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 3"))
        {
            if (turrets != null && turrets.Count > 2 && turrets[2].damages)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 4"))
        {
            if (turrets != null && turrets.Count > 3 && turrets[3].damages)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 5"))
        {
            if (turrets != null && turrets.Count > 4 && turrets[4].damages)
            {
                return true;
            }
        }
        return false;
    }

    public bool canTargetAir(List<Weapon> weapons)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.canTargetAir)
            {
                return true;
            }
        }
        return false;
    }

    public bool canTargetSub(List<Weapon> weapons)
    {
        foreach (Weapon weapon in weapons)
        {
            if (weapon.canTargetSub)
            {
                return true;
            }
        }
        return false;
    }


    //Determines if this unit can heal regardless of AP/MP
    public bool canHealAbsolute()
    {
        if (possibleActions.Contains("Heal"))
        {
            List<Weapon> healHandWeapons = getAllHealHandWeapons();
            if (healHandWeapons != null && healHandWeapons.Count > 0)
            {
                return true;
            }
            else if (possibleActions.Contains("Change Weapon"))
            {
                List<Weapon> healWeapons = getAllHealWeaponsFromList(getAllHealRepairNonTurretWeapons());
                if (healWeapons != null)
                {
                    healWeapons.RemoveAll(item => !item.heals);
                    if (healWeapons.Count > 0)
                    {
                        return true;
                    }
                }
            }

        }
        if (possibleActions.Contains("Fire Turret 1"))
        {
            if (turrets != null && turrets.Count > 0 && turrets[0].heals)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 2"))
        {
            if (turrets != null && turrets.Count > 1 && turrets[1].heals)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 3"))
        {
            if (turrets != null && turrets.Count > 2 && turrets[2].heals)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 4"))
        {
            if (turrets != null && turrets.Count > 3 && turrets[3].heals)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 5"))
        {
            if (turrets != null && turrets.Count > 4 && turrets[4].heals)
            {
                return true;
            }
        }
        return false;
    }

    public bool canRepairAbsolute()
    {
        if (possibleActions.Contains("Repair"))
        {
            List<Weapon> healHandWeapons = getAllRepairHandWeapons();
            if (healHandWeapons != null && healHandWeapons.Count > 0)
            {
                return true;
            }
            else if (possibleActions.Contains("Change Weapon"))
            {
                List<Weapon> healWeapons = getAllRepairWeaponsFromList(getAllHealRepairNonTurretWeapons());
                if (healWeapons != null)
                {
                    healWeapons.RemoveAll(item => !item.repairs);
                    if (healWeapons.Count > 0)
                    {
                        return true;
                    }
                }
            }

        }
        if (possibleActions.Contains("Fire Turret 1"))
        {
            if (turrets != null && turrets.Count > 0 && turrets[0].repairs)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 2"))
        {
            if (turrets != null && turrets.Count > 1 && turrets[1].repairs)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 3"))
        {
            if (turrets != null && turrets.Count > 2 && turrets[2].repairs)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 4"))
        {
            if (turrets != null && turrets.Count > 3 && turrets[3].repairs)
            {
                return true;
            }
        }
        if (possibleActions.Contains("Fire Turret 5"))
        {
            if (turrets != null && turrets.Count > 4 && turrets[4].repairs)
            {
                return true;
            }
        }
        return false;
    }

    public bool canHealRepairAbsolute()
    {
        return canHealAbsolute() || canRepairAbsolute();
    }

    public void printAttributeDictionary(Dictionary<string,float> attributes)
    {
        if (attributes == null)
        {
            Debug.Log("Dictionary is null");
            return;
        }
        string temp = "Printing an attribute dictionary:";
        foreach(string key in attributes.Keys)
        {
            temp += "\nAttribute: "+key+" with value: "+attributes[key];

        }
        Debug.Log(temp);
    }
    override
    public string ToString()
    {
        string temp = "Unit " + name + " on side " + side + " with " + currentHP + " hp";
        if (tile != null) temp +=" at position "+ tile.getPos();
        return temp;
    }

}
