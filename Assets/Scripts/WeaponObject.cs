using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WeaponObject : MonoBehaviour
{
    public int minRange, maxRange, aoe, aoeType, roundsFired;
    public float damage, xSize = 1, ySize = 1;
    public string weaponType, name, description, quote;
    public bool usesExternalSprite = true;

    public GameObject muzzleSprite;
    public GameObject muzzleSprite2;
    public GameObject muzzleSprite3;
    public GameObject muzzleSprite4;
    public GameObject muzzleSprite5;
    public GameObject rpgProjectile;
    public GameObject rpgProjectileExhaust;
    public GameObject explosionPrefab;
    public GameObject soundWavePrefab;
    public GameObject poisonGasPrefab;

    public List<GameObject> missiles = new List<GameObject>();
    public GameObject currentMissile;

    public List<GameObject> soundWaves = new List<GameObject>();
    public GameObject currentSoundWave;

    public List<GameObject> poisonGasClouds = new List<GameObject>();
    public GameObject currentPoisonGasCloud;

    public List<Sprite> sprites;

    public Sprite uiSprite;
    public GameManager gM;
    private UIManager ui;
    private Unit u;
    public Weapon weapon;
    public Weapon template;
    public bool firstTime = true;
    //
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSprite()
    {
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        ui = gM.uiScript;
        switch (name)
        {
            case "Rifle":
                usesExternalSprite = true;
                renderer.sprite = sprites[0];
                uiSprite = ui.uiWeaponSprites[0];
                break;
            case "RPG":
                usesExternalSprite = true;
                renderer.sprite = sprites[0];
                uiSprite = ui.uiWeaponSprites[1];
                break;
            case "Sniper Rifle":
                usesExternalSprite = true;
                renderer.sprite = sprites[2];
                uiSprite = ui.uiWeaponSprites[2];
                break;
            case "Mortar":
                usesExternalSprite = true;
                renderer.sprite = sprites[3];
                uiSprite = ui.uiWeaponSprites[3];
                break;
            case "Uzi":
                usesExternalSprite = true;
                renderer.sprite = sprites[4];
                uiSprite = ui.uiWeaponSprites[4];
                break;
            case "Shield":
                usesExternalSprite = true;
                renderer.sprite = sprites[5];
                uiSprite = ui.uiWeaponSprites[5];
                break;
            case "Minigun":
                usesExternalSprite = true;
                renderer.sprite = sprites[6];
                uiSprite = ui.uiWeaponSprites[6];
                break;
            case "Medkit":
                usesExternalSprite = true;
                renderer.sprite = sprites[7];
                uiSprite = ui.uiWeaponSprites[7];
                break;
            case "Wrench":
                usesExternalSprite = true;
                renderer.sprite = sprites[8];
                uiSprite = ui.uiWeaponSprites[8];
                break;
            case "Laser Rifle":
                usesExternalSprite = true;
                renderer.sprite = sprites[9];
                uiSprite = ui.uiWeaponSprites[9];
                break;
            case "Laser Uzi":
                usesExternalSprite = true;
                renderer.sprite = sprites[10];
                uiSprite = ui.uiWeaponSprites[10];
                break;
            case "Advanced RPG":
                usesExternalSprite = true;
                renderer.sprite = sprites[0];
                uiSprite = ui.uiWeaponSprites[11];
                break;
            case "Laser Minigun":
                usesExternalSprite = true;
                renderer.sprite = sprites[12];
                uiSprite = ui.uiWeaponSprites[12];
                break;
            case "Drone Gun":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[13];
                break;
            case "Tank Cannon":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[14];
                break;
            case "Heavy Tank Cannon":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[15];
                break;
            case "Assault Turret":
                usesExternalSprite = true;
                uiSprite = ui.uiWeaponSprites[16];
                renderer.sprite = sprites[16];
                break;
            case "Rocket Burster":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[17];
                //renderer.sprite = sprites[17];
                break;
            case "Artillery Cannon":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[18];
                break;
            case "Poison Rifle":
                usesExternalSprite = true;
                renderer.sprite = sprites[19];
                uiSprite = ui.uiWeaponSprites[19];
                break;
            case "Gas Grenade":
                usesExternalSprite = true;
                renderer.sprite = sprites[20];
                uiSprite = ui.uiWeaponSprites[20];
                break;
            case "Gas Mortar":
                usesExternalSprite = true;
                renderer.sprite = sprites[0];
                uiSprite = ui.uiWeaponSprites[21];
                break;
            case "Syringe":
                usesExternalSprite = true;
                renderer.sprite = sprites[0];
                uiSprite = ui.uiWeaponSprites[22];
                break;
            case "Screamer Mouth":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[23];
                break;
            case "Brewer Head":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[24];
                break;
            case "Blast Eye x3":
                usesExternalSprite = false;
                uiSprite = ui.uiWeaponSprites[25];
                break;

        }
    }

    public void applyScale()
    {
        transform.localScale = new Vector3(xSize, ySize, 1f);
    }

    public void useWeapon(int type, Unit unit)
    {
        WeaponsList weapons = new WeaponsList();
        template = weapons.weaponList[type];
        minRange = template.minRange;
        maxRange = template.maxRange;
        aoe = template.aoe;
        aoeType = template.aoeType;
        roundsFired = template.roundsFired;
        damage = template.damage;
        weaponType = template.weaponType;
        name = template.name;
        description = template.description;
        quote = template.quote;
        xSize = template.xScale;
        ySize = template.yScale;
        weapon = template;
        u = unit;
        applyScale();
        updateSprite();
    }

    public void useWeapon(Weapon template, Unit unit)
    {

        minRange = template.minRange;
        maxRange = template.maxRange;
        this.template = template;
        aoe = template.aoe;
        aoeType = template.aoeType;
        roundsFired = template.roundsFired;
        damage = template.damage;
        weaponType = template.weaponType;
        name = template.name;
        description = template.description;
        quote = template.quote;
        //If it's already set ignore the template
        if ((xSize == 1 || xSize == 0) || (weapon != null && !firstTime))
        {
            xSize = template.xScale;
        }
        else
        {
            template.xScale = xSize;
        }
        if ((ySize == 1 || ySize == 0) || (weapon != null && !firstTime) )
        {
            ySize = template.yScale;
        }
        else
        {
            template.yScale = ySize;
        }
        weapon = template;
        applyScale();
        if (sprites != null && sprites.Count > 0)
        updateSprite();
        u = unit;
        firstTime = false;
    }

    public void scaleToUI(float xScale, float yScale)
    {
        transform.localScale = new Vector3(xSize * xScale, ySize * yScale, 1f);
    }

    public void useTemplateRaw(int miR, int maR, int a, int aT, int rF, float dmg, string wT, string n, string desc, string q)
    {
        minRange = miR;
        maxRange = maR;
        aoe = a;
        aoeType = aT;
        roundsFired = rF;
        damage = dmg;
        weaponType = wT;
        name = n;
        description = desc;
        quote = q;

    }

    //For UIManager 
    public void makeUI(GameObject originalWeapon, int order)
    {
        //transform.localPosition = new Vector3(originalWeapon.transform.localPosition.x, originalWeapon.transform.localPosition.y, -2);
        if (!usesExternalSprite) return;
        GetComponent<SpriteRenderer>().sortingOrder = order;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1);
        if (rpgProjectile != null)
        {
            rpgProjectile.transform.localScale = new Vector3(rpgProjectile.transform.localScale.x, rpgProjectile.transform.localScale.y, 1);
        }

    }
    

    public void doAnimation(bool endAITurn, Unit defender)
    {
        StartCoroutine(performWeaponAnimation(endAITurn, defender));
    }

    public IEnumerator performWeaponAnimation(bool endAITurn, Unit defender)
    {
        float elapsedTime = 0;
        int shotCount = 0;
        switch (name)
        {
            case "Rifle":
                //Shoot 5 times
                while (shotCount < 5)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.white);
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0,0,0,0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "RPG":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                rpgProjectileExhaust.GetComponent<SpriteRenderer>().enabled = true;
                Vector3 origPos = rpgProjectile.transform.position;
                yield return StartCoroutine(launchRPGProjectile(1f,3f,18f, defender));
                //yield return new WaitForSeconds(0.1f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                rpgProjectileExhaust.GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(reloadRPGProjectile(0.25f, 0.25f, 3));
                break;
            case "Sniper Rifle":
                defender.setTintColor(Color.white);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(0.15f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                defender.setTintColor(new Color(0, 0, 0, 0));
                yield return new WaitForSeconds(0.15f);
                break;
            case "Uzi":
                //Shoot 3 times
                while (shotCount < 3)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.white);
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Minigun":
                //Shoot 7 times
                while (shotCount < 7)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.white);
                    yield return new WaitForSeconds(0.04f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.04f);
                    shotCount++;

                }
                break;
            case "Laser Rifle":
                //Shoot 5 times
                while (shotCount < 5)
                {
                    defender.setTintColor(Color.red);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Laser Uzi":
                //Shoot 3 times
                while (shotCount < 3)
                {
                    defender.setTintColor(Color.red);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Advanced RPG":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                rpgProjectileExhaust.GetComponent<SpriteRenderer>().enabled = true;
                origPos = rpgProjectile.transform.position;
                yield return StartCoroutine(launchRPGProjectile(1f, 3f, 18f, defender));
                //yield return new WaitForSeconds(0.1f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                 rpgProjectileExhaust.GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(reloadRPGProjectile(0.25f, 0.25f, 3));
                break;
            case "Laser Minigun":
                //Shoot 7 times
                while (shotCount < 7)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.red);
                    yield return new WaitForSeconds(0.04f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.04f);
                    shotCount++;

                }
                break;
            case "Drone Gun":
                //Shoot 3 times
                while (shotCount < 3)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.red);
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Tank Cannon":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(0.15f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(0.15f);
                yield return makeExplosion(defender.transform.position, explosionPrefab.transform.localScale, -3);
                break;
            case "Heavy Tank Cannon":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSeconds(0.15f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(0.15f);
                yield return makeExplosion(defender.transform.position, explosionPrefab.transform.localScale, -3);
                break;
            case "Assault Turret":
                while (shotCount < 5)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.white);
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Rocket Burster":
                while (shotCount < 5)
                {
                    currentMissile = Instantiate(rpgProjectile,rpgProjectile.transform.position,Quaternion.identity) as GameObject;
                    currentMissile.transform.SetParent(transform);
                    currentMissile.transform.localPosition = rpgProjectile.transform.localPosition;
                    currentMissile.transform.eulerAngles = rpgProjectile.transform.eulerAngles;
                    currentMissile.transform.localScale = rpgProjectile.transform.localScale;
                    currentMissile.GetComponent<SpriteRenderer>().enabled = true;
                    foreach (Transform child in currentMissile.transform)
                    {
                        child.gameObject.SetActive(true);
                        child.GetComponent<SpriteRenderer>().enabled = true;
                    }
                    missiles.Add(currentMissile);
                    StartCoroutine(launchMissile(currentMissile, rpgProjectile, 0.35f, 8f, 32f, defender));
                    shotCount++;
                    yield return new WaitForSeconds(0.25f);
                }
                yield return StartCoroutine(waitForAllMissiles());
                break;
            case "Poison Rifle":
                //Shoot 5 times
                while (shotCount < 5)
                {
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                    defender.setTintColor(Color.green);
                    yield return new WaitForSeconds(0.05f);
                    muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                    defender.setTintColor(new Color(0, 0, 0, 0));
                    yield return new WaitForSeconds(0.05f);
                    shotCount++;

                }
                break;
            case "Gas Grenade":
                //Debug.Log("Throwing grenade");
                currentMissile = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity) as GameObject;
                currentMissile.transform.SetParent(transform);
                currentMissile.transform.localPosition = gameObject.transform.localPosition;
                currentMissile.transform.eulerAngles = gameObject.transform.eulerAngles;
                currentMissile.transform.localScale = gameObject.transform.localScale;
                currentMissile.GetComponent<SpriteRenderer>().enabled = true;
                /*foreach (Transform child in currentMissile.transform)
                {
                    child.gameObject.SetActive(true);
                    if (child.GetComponent<SpriteRenderer>() != null)
                    {
                        child.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }*/
                missiles.Add(currentMissile);
                yield return StartCoroutine(launchMissile(currentMissile, gameObject, 0.35f, 8f, 32f, defender));
                //yield return new WaitForSeconds(0.25f);
                //yield return StartCoroutine(waitForAllMissiles());
                break;
            case "Brewer Head":
                u.gameObject.GetComponent<Animator>().SetBool("Moving", true);
                yield return new WaitForSeconds(1f);
                u.gameObject.GetComponent<Animator>().SetBool("Moving", false);
                //yield return new WaitForSeconds(1 / 6f);
                currentPoisonGasCloud = Instantiate(poisonGasPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                currentPoisonGasCloud.transform.SetParent(transform);
                currentPoisonGasCloud.transform.localPosition = gameObject.transform.localPosition;
                currentPoisonGasCloud.transform.eulerAngles = gameObject.transform.eulerAngles;
                currentPoisonGasCloud.transform.localScale = gameObject.transform.localScale;
                currentPoisonGasCloud.GetComponent<SpriteRenderer>().enabled = true;

                yield return StartCoroutine(movePoisonGasCloud(currentPoisonGasCloud, new List<Tile>() { defender.getTile() }, 1 / 2f));
                Destroy(currentPoisonGasCloud);
                break;
            case "Blast Eye x3":
                //Shoot 5 times
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                defender.setTintColor(Color.green);
                yield return new WaitForSeconds(0.05f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                defender.setTintColor(new Color(0, 0, 0, 0));
                yield return new WaitForSeconds(0.05f);

                muzzleSprite2.GetComponent<SpriteRenderer>().enabled = true;
                defender.setTintColor(Color.green);
                yield return new WaitForSeconds(0.05f);
                muzzleSprite2.GetComponent<SpriteRenderer>().enabled = false;
                defender.setTintColor(new Color(0, 0, 0, 0));
                yield return new WaitForSeconds(0.05f);

                muzzleSprite3.GetComponent<SpriteRenderer>().enabled = true;
                defender.setTintColor(Color.green);
                yield return new WaitForSeconds(0.05f);
                muzzleSprite3.GetComponent<SpriteRenderer>().enabled = false;
                defender.setTintColor(new Color(0, 0, 0, 0));
                yield return new WaitForSeconds(0.05f);
                break;
                //Notice




        }
        yield return null;
        if (endAITurn)
        {
            //gm.doAI();
        }

    }

    //For AOE weapons
    public IEnumerator performAOEAnimation(Tile target)
    {
        float elapsedTime = 0;
        int shotCount = 0;
        switch (name)
        {
            case "Mortar":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", true);
                yield return new WaitForSeconds(1 / 6f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", false);
                yield return StartCoroutine(makeExplosion(target.transform.position, explosionPrefab.transform.localScale * 4,-3));
                break;
            case "Artillery Cannon":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                Debug.Log(muzzleSprite.GetComponent<SpriteRenderer>().enabled);
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", true);
                yield return new WaitForSeconds(1/6f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", false);
                yield return StartCoroutine(makeExplosion(target.transform.position, explosionPrefab.transform.localScale * 4, -3));
                //yield return new WaitForSeconds(0.5f);
                break;
            case "Gas Mortar":
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = true;
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", true);
                yield return new WaitForSeconds(1 / 6f);
                muzzleSprite.GetComponent<SpriteRenderer>().enabled = false;
                muzzleSprite.GetComponent<Animator>().SetBool("Firing", false);
                yield return StartCoroutine(makeExplosion(target.transform.position, explosionPrefab.transform.localScale * 4, -3));
                break;
            case "Screamer Mouth":
                currentSoundWave = Instantiate(soundWavePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                currentSoundWave.transform.SetParent(transform);
                currentSoundWave.transform.localPosition = gameObject.transform.localPosition;
                currentSoundWave.transform.eulerAngles = gameObject.transform.eulerAngles;
                currentSoundWave.transform.localScale = gameObject.transform.localScale;
                lookAt(currentSoundWave, target.transform.position, true);
                currentSoundWave.GetComponent<SpriteRenderer>().enabled = true;
                yield return StartCoroutine(moveScreamWave(currentSoundWave, gM.getAOETiles(u,u.getTile(), target, template).ToList<Tile>(),1/3f));
                //Debug.Log("Destroying scream wave");
                Destroy(currentSoundWave);
                break;
        }
        yield return null;
    }

    public void lookAt(GameObject obj, Vector3 targetPos, bool affectZ)
    {
        if (affectZ)
        {
            targetPos.z = 0f;
        }

        Vector3 objectPos = obj.transform.position;
        targetPos.x = targetPos.x - objectPos.x;
        targetPos.y = targetPos.y - objectPos.y;

        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }

    public IEnumerator moveScreamWave(GameObject soundWave, List<Tile> tiles, float animTime) 
    {
        //Debug.Log(tiles.Count);
        for (int i = 0; i < tiles.Count; i++)
        {
            //ends[i - 1].setUnit(null);
            Vector3 end = tiles[i].gameObject.transform.position;
            //Debug.Log(end);
            float elapsedTime = 0;
            Vector3 relEnd = new Vector3(end.x, end.y, soundWave.transform.position.z);
            Vector3 startingPos = soundWave.transform.position;
            //Debug.Log(dirToFace);
            while (elapsedTime < animTime)
            {
                soundWave.transform.position = Vector3.Lerp(startingPos, relEnd, (elapsedTime / animTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            soundWave.transform.position = relEnd;
            if (tiles[i].getUnit() != null)
            {
                StartCoroutine(tiles[i].getUnitScript().blinkTintColor(Color.white, 0.075f, 5));
            }
            //ends[i].setUnit(unit.gameObject);
        }
    }

    public IEnumerator movePoisonGasCloud(GameObject poisonGasCloud, List<Tile> tiles, float animTime)
    {
        //Debug.Log(tiles.Count);
        for (int i = 0; i < tiles.Count; i++)
        {
            //ends[i - 1].setUnit(null);
            Vector3 end = tiles[i].gameObject.transform.position;
            //Debug.Log(end);
            float elapsedTime = 0;
            Vector3 relEnd = new Vector3(end.x, end.y, poisonGasCloud.transform.position.z);
            Vector3 startingPos = poisonGasCloud.transform.position;
            //Debug.Log(dirToFace);
            while (elapsedTime < animTime)
            {
                poisonGasCloud.transform.position = Vector3.Lerp(startingPos, relEnd, (elapsedTime / animTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            poisonGasCloud.transform.position = relEnd;
            if (tiles[i].getUnit() != null)
            {
                StartCoroutine(tiles[i].getUnitScript().blinkTintColor(Color.green, 0.2f, 1));
            }
            poisonGasCloud.transform.parent = tiles[i].transform;
            //ends[i].setUnit(unit.gameObject);
        }
    }


    public IEnumerator makeExplosion(Vector3 pos, Vector3 scale, float zPosOffset)
    {
        GameObject tempExplosion = Instantiate(explosionPrefab, new Vector3(pos.x, pos.y, pos.z + zPosOffset), Quaternion.identity) as GameObject;
        tempExplosion.transform.localScale = scale;
        tempExplosion.SetActive(true);
        yield return new WaitForSeconds(1);
        Destroy(tempExplosion);
    }

    public IEnumerator launchRPGProjectile(float animTime, float speedMultiInit, float speedMultiFinal, Unit defender)
    {
        float elaspedTime = 0f;
        Vector3 origPos = rpgProjectile.transform.position;
        float currentSpeed = speedMultiInit;
        bool changedLanes = false;
        while (elaspedTime < animTime)
        {
            float ratio = elaspedTime / animTime;
            if (ratio >= 0.5 && !changedLanes)
            {
                if (rpgProjectile.transform.parent.eulerAngles.y != 180)
                {
                    rpgProjectile.transform.position = defender.transform.position - new Vector3(10,0,0);
                }
                else
                {
                    rpgProjectile.transform.position = defender.transform.position + new Vector3(10, 0, 0);
                }
                changedLanes = true;
            }
            if (rpgProjectile.transform.parent.eulerAngles.y == 180)
            {
                rpgProjectile.transform.position += Vector3.left * Time.deltaTime * currentSpeed;
            }
            else
            {
                rpgProjectile.transform.position += Vector3.right * Time.deltaTime * currentSpeed;
            }
            currentSpeed = Mathf.Lerp(speedMultiInit, speedMultiFinal, elaspedTime / animTime);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        rpgProjectile.GetComponent<SpriteRenderer>().enabled = false;
        rpgProjectile.transform.position = origPos;
        yield return makeExplosion(defender.transform.position, explosionPrefab.transform.localScale,-3);

    }

    public IEnumerator reloadRPGProjectile(float delayBetweenFlashes, float initDelay, int flashes)
    {
        int flashesDone = 0;
        yield return new WaitForSeconds(initDelay);
        while (flashesDone < flashes)
        {
            rpgProjectile.GetComponent<SpriteRenderer>().enabled = true;

            yield return new WaitForSeconds(delayBetweenFlashes/2);
            rpgProjectile.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(delayBetweenFlashes/2);
            flashesDone++;
        }
        rpgProjectile.GetComponent<SpriteRenderer>().enabled = true;
    }

    public IEnumerator launchMissile(GameObject missile, GameObject baseMissile, float animTime, float speedMultiInit, float speedMultiFinal, Unit defender)
    {
        float elaspedTime = 0f;
        Vector3 origPos = missile.transform.position;
        float currentSpeed = speedMultiInit;
        bool changedLanes = false;
        while (elaspedTime < animTime)
        {
            float ratio = elaspedTime / animTime;
            if (ratio >= 0.3 && !changedLanes)
            {
                if (baseMissile.transform.parent.eulerAngles.y != 180)
                {
                    missile.transform.position = defender.transform.position + new Vector3(-5, 5, 0);
                    missile.transform.localEulerAngles = new Vector3(0, 0, 223.007f);
                }
                else
                {
                    missile.transform.position = defender.transform.position + new Vector3(5, 5, 0);
                    missile.transform.localEulerAngles = new Vector3(0, 0, 223.007f);
                }
                changedLanes = true;
            }
            if (baseMissile.transform.parent.eulerAngles.y == 180)
            {
                //rpgProjectile.transform.position += Vector3.left * Time.deltaTime * currentSpeed;
                Vector3 dir = new Vector3(-1, 1, 0);
                if (changedLanes)
                {
                    dir = new Vector3(-1, -1, 0);
                }
                missile.transform.position += dir * Time.deltaTime * currentSpeed;
            }
            else
            {
                Vector3 dir = new Vector3(1, 1, 0);
                if (changedLanes)
                {
                    dir = new Vector3(1, -1, 0);
                }
                //rpgProjectile.transform.position += Vector3.right * Time.deltaTime * currentSpeed;
                missile.transform.position += dir * Time.deltaTime * currentSpeed;
            }
            
            currentSpeed = Mathf.Lerp(speedMultiInit, speedMultiFinal, elaspedTime / animTime);
            elaspedTime += Time.deltaTime;
            yield return null;
        }
        //rpgProjectile.GetComponent<SpriteRenderer>().enabled = false;
        //rpgProjectile.transform.position = origPos;
        missiles.Remove(missile);
        Destroy(missile);
        yield return makeExplosion(defender.transform.position, explosionPrefab.transform.localScale, -3);

    }

    public IEnumerator waitForAllMissiles()
    {
        while (missiles.Count > 0)
        {
            yield return new WaitForSeconds(0.000001f);
        }
    }
}
