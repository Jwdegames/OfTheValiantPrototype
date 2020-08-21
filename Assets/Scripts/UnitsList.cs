using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class UnitsList
{
    //List to store weapons
    public List<UnitTemplate> unitsList = new List<UnitTemplate>();
    public Dictionary<string, Dictionary<string, List<UnitTemplate>>> unitTypes = new Dictionary<string, Dictionary<string, List<UnitTemplate>>>();
    public Dictionary<string, UnitTemplate> templateDictionary = new Dictionary<string, UnitTemplate>();
    public Dictionary<UnitTemplate, GameObject> unitPrefabs = new Dictionary<UnitTemplate, GameObject>();
    private WeaponsList weapons = new WeaponsList();
    public UnitsList()
    {
        //Add stuff to the unitTypes Dictionary for making units
        unitTypes.Add("Ignis", new Dictionary<string, List<UnitTemplate>>());
        unitTypes.Add("Vita", new Dictionary<string, List<UnitTemplate>>());

        unitTypes["Ignis"].Add("Infantry", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Infantry", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Advanced Infantry", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Advanced Infantry", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Token Advanced Infantry", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Vehicles", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Vehicles", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Advanced Vehicles", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Advanced Vehicles", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Airplanes", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Airplanes", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Advanced Airplanes", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Advanced Airplanes", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Gunships", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Gunships", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Advanced Gunships", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Advanced Gunships", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Ships", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Ships", new List<UnitTemplate>());
        unitTypes["Ignis"].Add("Advanced Ships", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Advanced Ships", new List<UnitTemplate>());



        //Deployable Units
        setUnitForFaction("Ignis", "Advanced Gunships", new UnitTemplate(125, 5, 1, 8f, 125, 1, 8f, 0, 5, "Drone", "Cheap flying gunship that can travel fast.", "Red", "Light", "Flying",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.weaponList[13], null, null, new List<Weapon>() { weapons.weaponList[13] }, null, null));


        //Side is irrelevant
        //Infantry
        setBasicUnit("Infantry", (new UnitTemplate(200, 5, 1, 6, 200, 1, 6, 1, 7, "Trooper", "Infantry that's good against lightly armored targets.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(0), null, null, new List<Weapon>() { weapons.getWeaponCopy(0) }, null, null)));
        setBasicUnit("Infantry", (new UnitTemplate(200, 5, 1, 5, 200, 1, 5, 1, 10, "Rocketeer", "Infantry that's good against heavily armored units.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(1), null, null, new List<Weapon>() { weapons.getWeaponCopy(1) }, null, null)));
        setBasicUnit("Infantry", (new UnitTemplate(100, 5, 1, 5, 100, 1, 5, 1, 30, "Sniper", "Low health infantry that excels at fighting light armored targets.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(2), null, null, new List<Weapon>() { weapons.getWeaponCopy(2) }, null, null)));
        setBasicUnit("Infantry", (new UnitTemplate(175, 5, 1, 4.5f, 175, 1, 4.5f, 1, 25, "Mortarman", "Infantry that excels at fighting groups of non-aerial light armored units.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(3), null, null, new List<Weapon>() { weapons.getWeaponCopy(3) }, null, null)));
        setBasicUnit("Infantry", (new UnitTemplate(200, 5, 1, 5, 200, 1, 5, 1, 15, "Shielded Trooper", "Frontline anti-light infantry that can take a lot of damage.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(4), weapons.getWeaponCopy(5), null,
            new List<Weapon>() { weapons.getWeaponCopy(4), weapons.getWeaponCopy(5) }, null, null)));

        //Vehicles
        setBasicUnit("Vehicles", (new UnitTemplate(300, 6, 1, 9, 300, 1, 9, 2, 35, "Tank", "Heavily armored vehicle that does a lot of damage.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(14), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(14) }, null, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(400, 6.5f, 1, 8, 400, 1, 8, 2, 45, "Heavy Tank", "Extremely heavily armored vehicle that does an extreme amount of damage.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(15), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(15) }, null, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(180, 7f, 4, 11, 180, 4, 11, 1, 20, "Transporter", "Vehicle that can transport up to 4 units across the map.", "Red", "Medium",
            "Wheeled", true, "Adjacent", "Adjacent", new List<string>() { "Infantry" }, null, 4, 0, new List<string>() { "Move", "Fortify", "Load Units", "Unload Units" }, null, null, null,
            null, null, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(125, 7f, 1, 12, 125, 1, 12, 2, 40, "Rocket Truck", "Mid Range vehicle that is great at taking out heavily armored units.",
            "Red", "Heavy", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(17), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(17) }, null, null)));


        //Handle Ignis Infantry
        setUnitForFaction("Ignis", "Infantry", new UnitTemplate(210, 5, 2, 4.2f, 210, 2, 4.2f, 1, 35, "Gattler", "Anti-light infantry that can attack twice.", "Red", "Light", "Legged",
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(6), null, null, new List<Weapon>() { weapons.getWeaponCopy(6) }, null,
            null));
        setUnitForFaction("Ignis", "Infantry", new UnitTemplate(210, 5, 1, 5, 210, 1, 5, 1, 21, "Shielded Rocketeer", "Frontline anti-heavy infantry.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(1), weapons.getWeaponCopy(5), null,
            new List<Weapon>() { weapons.getWeaponCopy(1), weapons.getWeaponCopy(5) }, null, null));
        setUnitForFaction("Ignis", "Infantry", (new UnitTemplate(190, 5, 1, 5, 200, 1, 5, 1, 15, "Field Medic", "Infantry that can heal biological units and defend itself.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Heal" }, weapons.getWeaponCopy(4), weapons.getWeaponCopy(7), null,
            new List<Weapon>() { weapons.getWeaponCopy(4), weapons.getWeaponCopy(7) }, null, null)));
        setUnitForFaction("Ignis", "Infantry", (new UnitTemplate(190, 5, 1, 5, 200, 1, 5, 1, 15, "Field Engineer", "Infantry that can heal mechanical units and defend itself.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Repair" }, weapons.getWeaponCopy(4), weapons.getWeaponCopy(8), null,
            new List<Weapon>() { weapons.getWeaponCopy(4), weapons.getWeaponCopy(8) }, null, null)));


        //Handle Vita Infantry
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(205, 5, 1, 6, 205, 1, 6, 1, 12, "Masked Trooper", "Anti-light infantry that poisons enemy units.", "Green",
            "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(19), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(19) }, null, null));
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(205, 5, 3, 6, 205, 3, 6, 1, 13, "Grenadier", "Anti-heavy infantry that creates poison gas.", "Green",
            "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(20), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(20) }, null, null));
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(300, 5, 1, 8, 300, 1, 8, 2, 9, "Mutant Trooper", "High-health infantry that poisons enemey units.", "Green",
            "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(21), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(21) }, null, null));
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(300, 5, 1, 8, 300, 1, 8, 1, 22, "MG Mortarman", "High-health infantry that poisons enemey units.", "Green",
            "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(21), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(21) }, null, null));
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(190, 5, 1, 6, 180, 1, 6, 1, 12, "Scientist", "Infantry that excels at fighting groups of non-aerial " +
            "light armored units. Additionally, it creates poison gas.", "Green", "Light", "Legged", new List<string>() { "Move", "Heal", "Fortify", "Capture" },
            weapons.getWeaponCopy(22), null, null, new List<Weapon>() { weapons.getWeaponCopy(22) }, null, null));



        //Handle Ignis Advanced Infantry
        setUnitForFaction("Ignis", "Advanced Infantry", (new UnitTemplate(205, 5, 1, 6, 205, 1, 6, 1, 14, "Jet Trooper", "Flying infantry that's good against lightly armored targets.",
            "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(0), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(0) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Infantry", (new UnitTemplate(205, 5, 1, 5, 205, 1, 5, 1, 17, "Jet Rocketeer", "Flying infantry that's good against heavily armored units.",
            "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(0), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(0) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Infantry", (new UnitTemplate(200, 5, 1, 5, 200, 1, 5, 1, 22, "S-Jet Trooper", "Flying frontline anti-light infantry that can take a lot of damage.", "Red", 
            "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(4), weapons.getWeaponCopy(5), null, 
            new List<Weapon>() { weapons.getWeaponCopy(4), weapons.getWeaponCopy(5) }, null, null)));

        setUnitForFaction("Ignis", "Advanced Infantry", (new UnitTemplate(220, 5, 1, 7, 220, 1, 7, 0, 17, "Droid Trooper", "Infantry that's good against lightly armored targets " +
            "and is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(9),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(9) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(220, 5, 1, 6, 220, 1, 6, 0, 25, "Shielded Droid", "Frontline infantry that is immune to poison." +
            "and is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(10),
            weapons.getWeaponCopy(5), null, new List<Weapon>() { weapons.getWeaponCopy(10), weapons.getWeaponCopy(5) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(231, 5, 1, 6, 231, 1, 6, 0, 33, "S-Droid Rocketeer", "Frontline anti-heavy infantry that is immune to poison.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(11), weapons.getWeaponCopy(5), null,
            new List<Weapon>() { weapons.getWeaponCopy(11), weapons.getWeaponCopy(5) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(231, 5, 2, 5.4f, 231, 2, 5.4f, 0, 47, "Droid Gattler", "Anti-light infantry that can attack twice and " +
            "is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(12),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(12) }, null, null));

        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(225.5f, 5, 1, 7f, 225.5f, 1, 7f, 0, 22, "Jet Droid", "Flying infantry that's good against lightly armored " +
            "targets and is immune to poison.", "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(9),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(9) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(225.5f, 5, 1, 6f, 225.5f, 1, 6f, 0, 25, "Jet Droid Rocketeer", "Flying infantry that's good against heavily " +
            "armored units is immune to poison.", "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(11),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(11) }, null, null));

        Dictionary<string, float> temp = new Dictionary<string, float>();
        //temp.Add("");
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(225.5f, 5, 1, 7f, 225.5f, 1, 7f, 0, 33, "Jet Droid UAV Carrier", "Flying infantry that deploy drones to all " +
            "adjacent tiles once and immune to poison.", "Red", "Light", "Flying", true, true, "Adjacent", new List<string>() { "Drone" }, 0, 0, 0, 0, 0, 1, 0, 0,
            new List<string>() { "Move", "Deploy Drones", "Fortify", "Capture", "Toggle Jetpack" }, null,
            null, null, new List<Weapon>(), null, null));

        //Handle Vita Advanced Infantry
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(300, 5, 1, 8, 300, 1, 8, 2, 5, "Screamer", "Infantry that emits screams that hits multiple units,",
            "Green", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" },
            weapons.getWeaponCopy(23), null, null, new List<Weapon>() { weapons.getWeaponCopy(23) }, null, null));
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(550, 5.5f, 5, 10, 550, 5, 10, 2, 15, "Brewer", "High health infantry that emits poison gas.",
            "Green", "Light", "Legged", false, 0, 0, 0, "", true, 0, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" },
            weapons.getWeaponCopy(24), null, null, new List<Weapon>() { weapons.getWeaponCopy(24) }, null, null));
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(675, 5.75f, 1, 12, 675, 1, 12, 4, 0, "Eyesore", "The eyesore does adequate damage and deals an impressive poisonous " +
            "explosion on death. Additionally, its high health makes it hard to kill.", "Green", "Light", "Legged", true, 100, 0, 0, "Explosion", true, 1, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(25), null, null, new List<Weapon>() { weapons.getWeaponCopy(25) }, null, null));

        Dictionary<string, Vector3> unitsMadeOnDeath = new Dictionary<string, Vector3>();
        unitsMadeOnDeath.Add("Small Slime", new Vector3(1,2,0));

        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(400, 6f, 2, 5, 400, 2, 5, 7, 0, "Slime", "The slime's unique molecular structure helps to protect it from attacks." +
            " Additionally, the slime produces two small slimes on death, which each spawn two mini slimes on death.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "", false, 0, 0, unitsMadeOnDeath, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(25), null, null, new List<Weapon>() { weapons.getWeaponCopy(26) }, null, null));

        unitsMadeOnDeath = new Dictionary<string, Vector3>();
        unitsMadeOnDeath.Add("Mini Slime", new Vector3(1, 2, 0));
        setUnitForFaction("Vita", "Token Advanced Infantry", new UnitTemplate(300, 5f, 2, 6f, 300, 2, 6f, 3, 0, "Small Slime", "The small slime's unique molecular structure helps to protect it from attacks." +
            " Additionally, the slime produces two mini slimes on death. The small slime is a weaker version of the slime.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "", false, 0, 0, unitsMadeOnDeath, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(27), null, null, new List<Weapon>() { weapons.getWeaponCopy(27) }, null, null));
        setUnitForFaction("Vita", "Token Advanced Infantry", new UnitTemplate(225, 4f, 2, 7f, 225, 2, 7f, 1, 0, "Mini Slime", "The slime's unique molecular structure helps to protect it from attacks." +
            " Additionally, the mini slime is a weaker version of the small slime.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "", false, 0, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(28), null, null, new List<Weapon>() { weapons.getWeaponCopy(28) }, null, null));

        //Handle Ignis Vehicles
        setUnitForFaction("Ignis", "Vehicles", (new UnitTemplate(215, 7f, 7, 10, 215, 7, 10, 2, 33, "Assault Transporter", "Vehicle that can transport up to 6 units across the map" +
            " and can defend itself.", "Red", "Medium", "Wheeled", true, "Adjacent", "Adjacent", new List<string>() { "Infantry" }, null, 6, 0,
            new List<string>() { "Move", "Fortify", "Load Units", "Unload Units", "Fire Turret 1" }, null, null, null, null, new List<Weapon>() { weapons.getWeaponCopy(17) }, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(125, 7f, 1, 12, 125, 1, 12, 2, 55, "Artillery Truck", "Long ranged vehicle that can devastate closely packed heavily armored units.",
            "Red", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(18), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(18) }, null, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(300, 6, 2, 9, 300, 2, 9, 3, 50, "Assault Tank", "Heavily armored vehicle that does a lot of damage and has a small turret.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1" }, weapons.getWeaponCopy(14), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(14) }, new List<Weapon>() { weapons.getWeaponCopy(17) }, null)));
        setBasicUnit("Vehicles", (new UnitTemplate(400, 6.5f, 3, 8, 400, 3, 8, 4, 75, "Assault Heavy Tank", "Extremely heavily armored vehicle that does an extreme amount of damage" +
            "and has two turrets.", "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1", "Fire Turret 2" },
            weapons.getWeaponCopy(15), null, null, new List<Weapon>() { weapons.getWeaponCopy(15) },
            new List<Weapon>() { weapons.getWeaponCopy(17), weapons.getWeaponCopy(17) }, null)));
        //Handle Vita Vehicles

        //Handle Ignis Advanced Vechicles

        //handle Vita Advanced Vehicles

        //Handle Ignis Airplanes

        //Handle Vita Airplances

        //Handle Ignis Advanced Airplanes

        //Handle Vita Advanced Airplances

        //Handle Ignis Gunships

        //Handle Vita Gunships

        //Handle Ignis Advanced Gunsips


        //Handle Vita Advanced Gunships

        //Handle Ignis Ships

        //Handle Vita Ships

        //Handle Ignis Advanced Ships


        //Handle Vita Advanced Ships

    }

    public void setBasicUnit(string type, UnitTemplate template)
    {
        foreach (string faction in unitTypes.Keys)
        {
            unitTypes[faction][type].Add(template);

        }
        unitsList.Add(template);
        templateDictionary.Add(template.name, template);
    }

    public void setUnitForFaction(string faction, string type, UnitTemplate template)
    {
        unitTypes[faction][type].Add(template);
        unitsList.Add(template);
        templateDictionary.Add(template.name, template);
    }

    //Template should be from this instance of UnitsList, otherwise this method may fail
    public GameObject getUnitPrefab(UnitTemplate template, string faction, BoardManager bM)
    {

        //Debug.Log(template);
        //Debug.Log("Faction:" +faction);
        //Debug.Log("Template:"+unitTypes["Ignis"]["Advanced Gunships"][0]);
        foreach (string type in unitTypes[faction].Keys)
        {
            //Debug.Log(type);

            if (unitTypes[faction][type].Contains(template))
            {
                switch (faction)
                {
                    case "Ignis":
                        switch (type)
                        {
                            case "Infantry":
                                if (!(template.name == "Gattler" || template.name == "Shielded Rocketeer"))
                                {
                                    return bM.unitPrefabs[0];
                                }
                                else
                                {
                                    return bM.unitPrefabs[2];
                                }
                            case "Advanced Infantry":
                                if (template.name == "Jet Trooper" || template.name == "Jet Rocketeer" || template.name == "S-Jet Trooper")
                                {
                                    return bM.unitPrefabs[3];
                                }
                                else if (template.name == "Droid Trooper" || template.name == "Shielded Droid")
                                {
                                    return bM.unitPrefabs[4];
                                }
                                else if (template.name == "S-Droid Rocketeer" || template.name == "Droid Gattler")
                                {
                                    return bM.unitPrefabs[5];
                                }
                                else
                                {
                                    return bM.unitPrefabs[6];
                                }
                            case "Vehicles":
                                if (template.name == "Tank" || template.name == "Assault Tank")
                                {
                                    return bM.unitPrefabs[8];
                                }
                                else if (template.name == "Heavy Tank" || template.name == "Assault Heavy Tank")
                                {
                                    return bM.unitPrefabs[9];
                                }
                                else if (template.name == "Transporter")
                                {
                                    return bM.unitPrefabs[10];
                                }
                                else if (template.name == "Assault Transporter")
                                {
                                    return bM.unitPrefabs[11];
                                }
                                else if (template.name == "Rocket Truck")
                                {
                                    return bM.unitPrefabs[12];
                                }
                                else
                                {
                                    return bM.unitPrefabs[13];
                                }

                            case "Advanced Gunships":
                                //Debug.Log(template);
                                //Debug.Log(template.name == "Drone");
                                if (template.name == "Drone")
                                {
                                    return bM.unitPrefabs[7];
                                }
                                return null;

                        }
                        break;
                    case "Vita":
                        switch (type)
                        {
                            case "Infantry":
                                if (template.name == "Masked Trooper" || template.name == "Grenadier")
                                {
                                    return bM.unitPrefabs[14];
                                }
                                else if (template.name == "Mutant Trooper")
                                {
                                    return bM.unitPrefabs[15];
                                }
                                else if (template.name == "MG Mortarman")
                                {
                                    return bM.unitPrefabs[16];
                                }
                                else if (template.name == "Scientist")
                                {
                                    return bM.unitPrefabs[17];
                                }
                                return bM.unitPrefabs[1];
                            case "Advanced Infantry":
                                if (template.name == "Screamer")
                                {
                                    return bM.unitPrefabs[18];
                                }
                                else if (template.name == "Brewer")
                                {
                                    return bM.unitPrefabs[19];
                                }
                                else if (template.name == "Eyesore")
                                {
                                    return bM.unitPrefabs[20];
                                }
                                else if (template.name == "Slime")
                                {
                                    return bM.unitPrefabs[21];
                                }
                                return null;
                        }
                        break;
                }
            }
        }
        //We couldn't find a prefab
        return null;


    }

    //Alternate method
    public GameObject getUnitPrefab(string faction, BoardManager bM, string unitName)
    {
        return getUnitPrefab(templateDictionary[unitName], faction, bM);
    }


}
