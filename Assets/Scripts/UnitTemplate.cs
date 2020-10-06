using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System;

[Serializable]
public class UnitTemplate
{
    
    public float hp = 10f, sizeMultiplier, ap, mp, currentHP, currentAP, currentMP;

    public int pplCost, mtCost;

    public string name, description, side, armor, movementType;

    //Start Drone Control Variables
    public bool deploysDrones = false, dronesAreReliant = false;

    public string deployType = "Adjacent";

    public List<string> droneTypes = null;

    public int maxDronesAtTime = 0, maxDronesAtAll = 0,maxDeploysAtTime = 0, maxDeploysAtAll = 0, currentDeploys = 0, currentDrones = 0, totalDeploys = 0, totalDrones = 0;

    //First vector 4 relates to maxDronesAtTime, maxDronesAtAll, maxDeploysAtTime, maxDeploysAtAll
    //Second vector 4 relates to currentDrones, totalDrones, currentDeploys, totalDeploys
    //Third Vector 4 relates to deployType, dronesAreReliant, extraAttribute1, extraAttribute2
    //Fourth Vector 4 relates to maxDronesThisTurn, currentDronesThisTurn, maxDeployThisTurn, currentDeploysThisTurn
    //Fifth Vector 4 relates to extraAttribute1, extraAttribute2, extraAttribute3, extraAttribute4
    public Dictionary<string, List<Vector4>> dronesDict = new Dictionary<string, List<Vector4>>(); 
    //End Drone Control Variables

    //Start Unit Loading/Unloading Variables

    public bool transportsUnits = false;

    public string loadType = "Adjacent", unloadType = "Adjacent";

    public List<string> includeList = new List<string>(), excludeList = new List<string>();

    public int maxCapacity = 0,  currentCapacity = 0;



    //End Unit Loading/Unloading Variables


    //Start OnDeath Variables
    public bool doesDamageOnDeath = false;
    public float damageOnDeath = 0;
    public int damageOnDeathAOE = 0;
    public int damageOnDeathType = 0;

    public string deathAEType = "";

    public bool leavesPoisonGasOnDeath = false;
    public int poisonGasOnDeathAOE = 0;
    public int poisonGasOnDeathAOEType = 0;

    public Dictionary<string, Vector3> unitsMadeOnDeathDict = new Dictionary<string, Vector3>();
    //End OnDeath Variables


    public List<string> possibleActions;

    public Weapon currentWeapon, currentWeapon2, currentWeapon3;

    public List<Weapon> weapons, turrets;

    public Dictionary<string, float> extraAttributes;
    public UnitTemplate(float h, float s, float a, float m, float cHP, float cAP, float cMP, int pplC, int mtC, string n, string desc, string si, string arm, string mT,
        List<string> pA, Weapon w, Weapon w2, Weapon w3, List<Weapon> ws, List<Weapon> ts, Dictionary<string, float> eA) {
        hp = h;
        sizeMultiplier = s;
        ap = a;
        mp = m;
        currentHP = cHP;
        currentAP = cAP;
        currentMP = cMP;
        pplCost = pplC;
        mtCost = mtC;
        name = n;
        description = desc;
        side = si;
        armor = arm;
        movementType = mT;
        possibleActions = pA;
        currentWeapon = w;
        currentWeapon2 = w2;
        currentWeapon3 = w3;
        weapons = ws;
        turrets = ts;
        extraAttributes = eA;
    }

    //Version for deploying drones
    public UnitTemplate(float h, float s, float a, float m, float cHP, float cAP, float cMP, int pplC, int mtC, string n, string desc, string si, string arm, string mT,
        bool dDs, bool dsAR, string dT, List<string> dTs, int mDAT, int mDAA, int cD, int tDs, int mDepAT, int mDepAA, int cDep, int tDep, Dictionary<string, List<Vector4>> droneD,
        List<string> pA, Weapon w, Weapon w2, Weapon w3, List<Weapon> ws, List<Weapon> ts, Dictionary<string, float> eA)
    {
        hp = h;
        sizeMultiplier = s;
        ap = a;
        mp = m;
        currentHP = cHP;
        currentAP = cAP;
        currentMP = cMP;
        pplCost = pplC;
        mtCost = mtC;
        name = n;
        description = desc;
        side = si;
        armor = arm;
        movementType = mT;
        deploysDrones = dDs;
        dronesAreReliant = dsAR;
        deployType = dT;
        droneTypes = dTs;
        maxDronesAtTime = mDAT;
        maxDronesAtAll = mDAA;
        maxDeploysAtTime = mDepAT;
        maxDeploysAtAll = mDepAA;
        currentDeploys = cDep;
        currentDrones = cD;
        totalDrones = tDs;
        totalDeploys = tDep;
        dronesDict = droneD;
        possibleActions = pA;
        currentWeapon = w;
        currentWeapon2 = w2;
        currentWeapon3 = w3;
        weapons = ws;
        turrets = ts;
        extraAttributes = eA;
    }

    //Template for loading/unloading Units
    public UnitTemplate(float h, float s, float a, float m, float cHP, float cAP, float cMP, int pplC, int mtC, string n, string desc, string si, string arm, string mT, bool tU,
        string lT, string ulT, List<string> includes, List<string> excludes, int maxC, int curC,
        List<string> pA, Weapon w, Weapon w2, Weapon w3, List<Weapon> ws, List<Weapon> ts, Dictionary<string, float> eA)
    {
        hp = h;
        sizeMultiplier = s;
        ap = a;
        mp = m;
        currentHP = cHP;
        currentAP = cAP;
        currentMP = cMP;
        pplCost = pplC;
        mtCost = mtC;
        name = n;
        description = desc;
        side = si;
        armor = arm;
        movementType = mT;

        transportsUnits = tU;
        loadType = lT;
        unloadType = ulT;
        includeList = includes;
        excludeList = excludes;
        maxCapacity = maxC;
        currentCapacity = curC;

        possibleActions = pA;
        currentWeapon = w;
        currentWeapon2 = w2;
        currentWeapon3 = w3;
        weapons = ws;
        turrets = ts;
        extraAttributes = eA;
    }

    //For handling death effects
    public UnitTemplate(float h, float s, float a, float m, float cHP, float cAP, float cMP, int pplC, int mtC, string n, string desc, string si, string arm, string mT, bool dDOD,
        float dOD, int dODAOE, int dODAOEType, string dAEType, bool lPGOD, int pGODAOE, int pGODAOEType, Dictionary<string, Vector3> uMODDict,
        List<string> pA, Weapon w, Weapon w2, Weapon w3, List<Weapon> ws, List<Weapon> ts, Dictionary<string, float> eA)
    {
        hp = h;
        sizeMultiplier = s;
        ap = a;
        mp = m;
        currentHP = cHP;
        currentAP = cAP;
        currentMP = cMP;
        pplCost = pplC;
        mtCost = mtC;
        name = n;
        description = desc;
        side = si;
        armor = arm;
        movementType = mT;

        doesDamageOnDeath = dDOD;
        damageOnDeath = dOD;
        damageOnDeathAOE = dODAOE;
        damageOnDeathType = dODAOEType;

        deathAEType = dAEType;

        leavesPoisonGasOnDeath = lPGOD;
        poisonGasOnDeathAOE = pGODAOE;
        poisonGasOnDeathAOEType = pGODAOEType;

        unitsMadeOnDeathDict = uMODDict;

        possibleActions = pA;
        currentWeapon = w;
        currentWeapon2 = w2;
        currentWeapon3 = w3;
        weapons = ws;
        turrets = ts;
        extraAttributes = eA;
    }

    override
    public string ToString()
    {
        return name;
    }
}
