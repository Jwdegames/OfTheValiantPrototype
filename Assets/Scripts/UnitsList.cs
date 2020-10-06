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

        unitTypes["Ignis"].Add("Commanders", new List<UnitTemplate>());
        unitTypes["Vita"].Add("Commanders", new List<UnitTemplate>());
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
            "Red", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(17), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(17) }, null, null)));

        //Handle Ignis Commanders
        setUnitForFaction("Ignis", "Commanders", new UnitTemplate(250, 5, 1, 6, 250, 1, 6, 1, 20, "Asher", "Asher is a young and energetic commander.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Rally" }, weapons.getWeaponCopy(10),
            weapons.getWeaponCopy(10), null, new List<Weapon>() { weapons.getWeaponCopy(10), weapons.getWeaponCopy(10) }, null, null));
        //Handle Vita Commanders

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
        setUnitForFaction("Vita", "Infantry", new UnitTemplate(205, 5, 1, 8, 205, 1, 8, 1, 22, "MG Mortarman", "High-health infantry that poisons enemy units and makes poison gas.", "Green",
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

        setUnitForFaction("Ignis", "Advanced Infantry", (new UnitTemplate(300, 5, 1, 7, 300, 1, 7, 0, 17, "Droid Trooper", "Infantry that's good against lightly armored targets " +
            "and is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(9),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(9) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(300, 5, 1, 6, 300, 1, 6, 0, 25, "Shielded Droid", "Frontline infantry that is immune to poison." +
            "and is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(10),
            weapons.getWeaponCopy(5), null, new List<Weapon>() { weapons.getWeaponCopy(10), weapons.getWeaponCopy(5) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(315, 5, 1, 6, 315, 1, 6, 0, 33, "S-Droid Rocketeer", "Frontline anti-heavy infantry that is immune to poison.",
            "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(11), weapons.getWeaponCopy(5), null,
            new List<Weapon>() { weapons.getWeaponCopy(11), weapons.getWeaponCopy(5) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(315, 5, 2, 5.4f, 315, 2, 5.4f, 0, 47, "Droid Gattler", "Anti-light infantry that can attack twice and " +
            "is immune to poison.", "Red", "Light", "Legged", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture" }, weapons.getWeaponCopy(12),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(12) }, null, null));

        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(307.5f, 5, 1, 7f, 307.5f, 1, 7f, 0, 22, "Jet Droid", "Flying infantry that's good against lightly armored " +
            "targets and is immune to poison.", "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(9),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(9) }, null, null));
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(307.5f, 5, 1, 6f, 225.5f, 1, 6f, 0, 25, "Jet Droid Rocketeer", "Flying infantry that's good against heavily " +
            "armored units is immune to poison.", "Red", "Light", "Flying", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Capture", "Toggle Jetpack" }, weapons.getWeaponCopy(11),
            null, null, new List<Weapon>() { weapons.getWeaponCopy(11) }, null, null));

        Dictionary<string, float> temp = new Dictionary<string, float>();
        //temp.Add("");
        setUnitForFaction("Ignis", "Advanced Infantry", new UnitTemplate(307.5f, 5, 1, 7f, 307.5f, 1, 7f, 0, 33, "Jet Droid UAV Carrier", "Flying infantry that deploy drones to all " +
            "adjacent tiles once and immune to poison.", "Red", "Light", "Flying", true, true, "Adjacent", new List<string>() { "Drone" }, 0, 0, 0, 0, 0, 1, 0, 0, null,
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
        unitsMadeOnDeath.Add("Small Slime", new Vector3(1,2,1));

        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(400, 6f, 2, 5, 400, 2, 5, 8, 0, "Slime", "The slime has a unique armor and spawns two small slimes on death, which each " +
            "spawn two mini slimes on death.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "Dissolve After Make Unit", false, 0, 0, unitsMadeOnDeath, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(25), null, null, new List<Weapon>() { weapons.getWeaponCopy(26) }, null, null));

        unitsMadeOnDeath = new Dictionary<string, Vector3>();
        unitsMadeOnDeath.Add("Mini Slime", new Vector3(1, 2, 1));
        setUnitForFaction("Vita", "Token Advanced Infantry", new UnitTemplate(300, 5f, 2, 6f, 300, 2, 6f, 4, 0, "Small Slime", "The small slime has a unique armor and spawns two mini slimes on death " +
            "and is weaker than regular slimes.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "Dissolve After Make Unit", false, 0, 0, unitsMadeOnDeath, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(27), null, null, new List<Weapon>() { weapons.getWeaponCopy(27) }, null, null));
        setUnitForFaction("Vita", "Token Advanced Infantry", new UnitTemplate(225, 4f, 2, 7f, 225, 2, 7f, 2, 0, "Mini Slime", "The mini slime has a unique armor and is weaker than small slimes.",
            "Green", "Slime", "Slime", false, 0, 0, 0, "", false, 0, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(28), null, null, new List<Weapon>() { weapons.getWeaponCopy(28) }, null, null));

        temp = new Dictionary<string, float>();
        temp.Add("Self Heal",0.1f);
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(600, 5f, 1, 7, 600, 1, 7, 6, 0, "Eyebat", "The eyebat does an impressive amount of damage and slightly heals each day",
            "Green", "Light", "Flying", false, 0, 0, 0, "", false, 1, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(29), null, null, new List<Weapon>() { weapons.getWeaponCopy(29) }, null, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Self Heal", 0.1f);
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(610, 5.1f, 4, 7, 610, 4, 7, 4, 0, "Rosebat", "The rosebat makes poison gas clouds and slightly heals each day",
            "Green", "Light", "Flying", false, 0, 0, 0, "", false, 1, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(30), null, null, new List<Weapon>() { weapons.getWeaponCopy(30) }, null, temp));

        unitsMadeOnDeath = new Dictionary<string, Vector3>();
        unitsMadeOnDeath.Add("Weak Foci", new Vector3(0, 1, 0));
        temp = new Dictionary<string, float>();
        temp.Add("Self Heal", 0.1f);
        setUnitForFaction("Vita", "Advanced Infantry", new UnitTemplate(625, 5.2f, 1, 6, 625, 1, 6, 3, 60, "Focibat", "The focibat deals massive amounts of damage, self-heals, and makes a weak foci" +
            "after dying.",
            "Green", "Heavy", "Flying", false, 0, 0, 0, "Dissolve After Make Unit", false, 1, 0, unitsMadeOnDeath, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(31), null, null, new List<Weapon>() { weapons.getWeaponCopy(31) }, null, temp));

        setUnitForFaction("Vita", "Token Advanced Infantry", new UnitTemplate(450, 5.2f, 1, 2, 450, 1, 2, 1, 40, "Weak Foci", "The foci deals quite a bit of damage, but moves very slowly",
            "Green", "Heavy", "Flying", false, 0, 0, 0, "", false, 1, 0, null, new List<string>() { "Move", "Attack", "Fortify", "Sentry" },
            weapons.getWeaponCopy(31), null, null, new List<Weapon>() { weapons.getWeaponCopy(32) }, null, null));

        //Handle Ignis Vehicles
        setUnitForFaction("Ignis", "Vehicles", (new UnitTemplate(215, 7f, 7, 10, 215, 7, 10, 2, 43, "Assault Transporter", "Vehicle that can transport up to 6 units across the map" +
            " and can defend itself.", "Red", "Medium", "Wheeled", true, "Adjacent", "Adjacent", new List<string>() { "Infantry" }, null, 6, 0,
            new List<string>() { "Move", "Fortify", "Load Units", "Unload Units", "Fire Turret 1" }, null, null, null, null, new List<Weapon>() { weapons.getWeaponCopy(17) }, null)));
        setUnitForFaction("Ignis", "Vehicles", (new UnitTemplate(125, 7f, 1, 12, 125, 1, 12, 2, 55, "Artillery Truck", "Long ranged vehicle that can devastate closely packed heavily armored units.",
            "Red", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(18), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(18) }, null, null)));
        setUnitForFaction("Ignis", "Vehicles", (new UnitTemplate(300, 6, 2, 9, 300, 2, 9, 3, 50, "Assault Tank", "Heavily armored vehicle that does a lot of damage and has a small turret.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1" }, weapons.getWeaponCopy(14), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(14) }, new List<Weapon>() { weapons.getWeaponCopy(17) }, null)));
        setUnitForFaction("Ignis", "Vehicles", (new UnitTemplate(400, 6.5f, 3, 8, 400, 3, 8, 4, 75, "Assault Heavy Tank", "Extremely heavily armored vehicle that does an extreme amount of damage" +
            " and has two turrets.", "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1", "Fire Turret 2" },
            weapons.getWeaponCopy(15), null, null, new List<Weapon>() { weapons.getWeaponCopy(15) },
            new List<Weapon>() { weapons.getWeaponCopy(17), weapons.getWeaponCopy(17) }, null)));

        //Handle Vita Vehicles
        temp = new Dictionary<string, float>();
        temp.Add("Heals Transported Units", 0.1f);
        setUnitForFaction("Vita", "Vehicles", (new UnitTemplate(180, 7f, 4, 12, 180, 4, 12, 2, 27, "Ambulance", "Vehicle that can transport up to 4 infantry and heals 30% of their health at the start" +
            " of each turn.", "Green", "Medium", "Wheeled", new List<string>() { "Move", "Fortify", "Load Units", "Unload Units" }, null, null, null, null, null, temp)));
        setUnitForFaction("Vita", "Vehicles", (new UnitTemplate(180, 7f, 1, 11, 180, 1, 11, 2, 40, "Mortar Truck", "Long ranged vehicle that can devastate closely packed lightly armored units" +
            " and makes poison gas.",
            "Green", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(33), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(33) }, null, null)));
        setUnitForFaction("Vita", "Vehicles", (new UnitTemplate(180, 7f, 2, 11, 180, 2, 11, 2, 45, "Flak Truck", "Medium ranged vehicle that can pummel light armored units" +
            " and makes poison gas.",
            "Green", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(34), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(34) }, null, null)));
        setUnitForFaction("Vita", "Vehicles", (new UnitTemplate(500, 7f, 1, 7.5f, 500, 1, 7.5f, 2, 60, "Venom Tank", "Extremely tough tank that deals huge amounts of damage" +
            " and makes poison gas.",
            "Green", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(35), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(35) }, null, null)));

        //Handle Ignis Advanced Vechicles
        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(375, 6, 2, 9, 375, 2, 9, 3, 60, "Duality Tank", "Heavily armored vehicle that combines the features of a Tank and a Rocket Truck.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry"}, weapons.getWeaponCopy(41), weapons.getWeaponCopy(42), null,
            new List<Weapon>() { weapons.getWeaponCopy(41), weapons.getWeaponCopy(42) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(600, 6.5f, 1, 9, 600, 1, 9, 0, 60, "Automated Tank", "Extremely heavily armored vehicle that does an extreme amount of damage and is immune to poison.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(43), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(43) }, null, null)));
        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(600, 6.5f, 3, 9, 600, 3, 9, 0, 100, "Assault Automated Tank", "Extremely heavily armored vehicle that does an extreme amount of damage, is immune " +
            "to poison, and has two laser turrets.", "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1", "Fire Turret 2" }, weapons.getWeaponCopy(43), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(43) }, new List<Weapon>() {weapons.getWeaponCopy(44),weapons.getWeaponCopy(44) }, null)));

        Dictionary<string, List<Vector4>> deployDict = new Dictionary<string, List<Vector4>>();
        List<Vector4> deployAttributes = new List<Vector4>();
        deployAttributes.Add(new Vector4(0, 0, 0, 0));
        deployAttributes.Add(new Vector4(0, 0, 0, 0));
        deployAttributes.Add(new Vector4(2, 0, 1, 0));
        deployAttributes.Add(new Vector4(3, 0, 0, 0));
        deployDict.Add("Drone", deployAttributes);
        setUnitForFaction("Ignis", "Advanced Vehicles", new UnitTemplate(270f, 7f, 3, 13f, 270f, 3, 13f, 0, 58, "Drone Deployer Truck", "Mobile factory that produces drones and can support " +
            "adjacent tiles once and immune to poison.", "Red", "Medium", "Wheeled", true, true, "Custom", null, 0, 0, 0, 0, 0, 0, 0, 0, deployDict,
            new List<string>() { "Move", "Deploy Drones", "Fortify"}, null, null, null, new List<Weapon>(), null, null));

        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(322.5f, 7f, 13, 12, 322.5f, 13, 12, 0, 133, "Assault Auto Transporter", "Autonomous vehicle that can transport up to 6 " +
            "units across the map" + " and has 5 laser turrets to defend itself.", "Red", "Medium", "Wheeled", true, "Adjacent", "Adjacent", new List<string>() { "Infantry" }, null, 6, 0,
            new List<string>() { "Move", "Fortify", "Load Units", "Unload Units", "Fire Turret 1", "Fire Turret 2", "Fire Turret 3", "Fire Turret 4", "Fire Turret 5" }, null, null, null, null, 
            new List<Weapon>() { weapons.getWeaponCopy(44), weapons.getWeaponCopy(44), weapons.getWeaponCopy(44) , weapons.getWeaponCopy(44) , weapons.getWeaponCopy(44) }, null)));
        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(187.5f, 7f, 2, 14, 187.5f, 2, 14, 0, 65, "Assault Auto Artillery", "Long ranged vehicle that can devastate closely packed heavily armored units and " +
            "is immune to poison.",
            "Red", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1" }, weapons.getWeaponCopy(18), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(18) }, new List<Weapon>() { weapons.getWeaponCopy(44) }, null)));
        setUnitForFaction("Ignis", "Advanced Vehicles", (new UnitTemplate(750, 7, 3, 8, 750, 3, 8, 0, 115, "Assault Auto Duality Tank", "Heavily armored vehicle that combines the features of an Automated Tank and" +
            " an Assault Auto Artillery.",
            "Red", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Fire Turret 1" }, weapons.getWeaponCopy(45), weapons.getWeaponCopy(46), null,
            new List<Weapon>() { weapons.getWeaponCopy(45), weapons.getWeaponCopy(46) }, new List<Weapon>() { weapons.getWeaponCopy(44) }, null)));




        //handle Vita Advanced Vehicles
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(512.5f, 7f, 1, 7.5f, 512.5f, 1, 7.5f, 2, 50, "Mortar Tank", "Long ranged armored vehicle that is sealed and can devastate " +
            "closely packed lightly armored units and makes poison gas.",
            "Green", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(36), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(36) }, null, null)));
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(512.5f, 7f, 2, 7.5f, 512.5f, 2, 7.5f, 2, 55, "P Flak Tank", "Short ranged armored vehicle that is sealed and can easily immolate " +
            "lightly armored units and poisons them.",
            "Green", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(37), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(37) }, null, null)));
        //Displace DP Rocket Tank by y=0.75 
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(512.5f, 7f, 2, 7f, 512.5f, 2, 7f, 3, 80, "DP Rocket Tank", "Long ranged armored vehicle that is sealed and can easily take out " +
            "heavily armored units and makes poison gas.",
            "Green", "Heavy", "Tracked", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(38), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(38) }, null, null)));
        temp = new Dictionary<string, float>();
        temp.Add("Heals Transported Units", 0.1f);
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(184.5f, 7f, 6, 11, 184.5f, 6, 11, 2, 35, "Long Ambulance", "Vehicle that can transport up to 6 infantry and heals 30% of their " +
            "health at the startn of each turn.", "Green", "Medium", "Wheeled", new List<string>() { "Move", "Fortify", "Load Units", "Unload Units" }, null, null, null, null, null, temp)));
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(184.5f, 7f, 5, 11, 184.5f, 5, 11, 2, 65, "Brewer Truck", "Long ranged vehicle that is sealed and can send out poison gas very far.",
            "Green", "Medium", "Wheeled", new List<string>() { "Move", "Attack", "Fortify", "Sentry" }, weapons.getWeaponCopy(39), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(39) }, null, null)));

        

        deployDict = new Dictionary<string, List<Vector4>>();
        deployAttributes = new List<Vector4>();
        deployAttributes.Add(new Vector4(0, 0, 0, 0));
        deployAttributes.Add(new Vector4(0, 0, 0, 0));
        deployAttributes.Add(new Vector4(1, 0, 1, 0));
        deployAttributes.Add(new Vector4(1, 0, 0, 0));
        deployDict.Add("Small Slime", deployAttributes);
        setUnitForFaction("Vita", "Advanced Vehicles", (new UnitTemplate(184.5f, 7f, 1, 11, 184.5f, 1, 11, 6, 90, "Slime Launcher Truck", "Long ranged armored vehicle that launches slime balls that can" +
            " damage enemy units or make small slimes.", "Green", "Medium", "Wheeled", true, true, "Custom", null, 0, 0, 0, 0, 0, 0, 0, 0, deployDict,
            new List<string>() { "Move", "Attack", "Fortify", "Sentry", "Deploy Units"}, weapons.getWeaponCopy(40), null, null,
            new List<Weapon>() { weapons.getWeaponCopy(40) }, null, null)));

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
                            case "Commanders":
                                if (template.name == "Asher")
                                {
                                    return bM.unitPrefabs[46];
                                }
                                return null;
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
                            case "Advanced Vehicles":
                                if (template.name == "Duality Tank")
                                {
                                    return bM.unitPrefabs[40];
                                }
                                else if (template.name == "Automated Tank" || template.name == "Assault Automated Tank")
                                {
                                    return bM.unitPrefabs[41];
                                }
                                else if (template.name == "Drone Deployer Truck")
                                {
                                    return bM.unitPrefabs[42];
                                }
                                else if (template.name == "Assault Auto Transporter")
                                {
                                    return bM.unitPrefabs[43];
                                }
                                else if (template.name == "Assault Auto Artillery")
                                {
                                    return bM.unitPrefabs[44];
                                }
                                return bM.unitPrefabs[45];
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
                                else if (template.name == "Eyebat")
                                {
                                    return bM.unitPrefabs[22];
                                }
                                else if (template.name == "Rosebat")
                                {
                                    return bM.unitPrefabs[23];
                                }
                                else
                                {
                                    //Focibat
                                    return bM.unitPrefabs[24];
                                }
                            case "Token Advanced Infantry":
                                if (template.name == "Small Slime" || template.name == "Mini Slime")
                                {
                                    return bM.unitPrefabs[21];
                                }
                                //Weakened Foci
                                return bM.unitPrefabs[25];
                            case "Vehicles":
                                if (template.name == "Tank")
                                {
                                    return bM.unitPrefabs[26];
                                }
                                else if (template.name == "Heavy Tank")
                                {
                                    return bM.unitPrefabs[27];
                                }
                                else if (template.name == "Transporter")
                                {
                                    return bM.unitPrefabs[28];
                                }
                                else if (template.name == "Rocket Truck")
                                {
                                    return bM.unitPrefabs[29];
                                }
                                else if (template.name == "Ambulance")
                                {
                                    return bM.unitPrefabs[30];
                                }
                                else if (template.name == "Mortar Truck")
                                {
                                    return bM.unitPrefabs[31];
                                }
                                else if (template.name == "Flak Truck")
                                {
                                    return bM.unitPrefabs[32];
                                }
                                //Venom Tank
                                return bM.unitPrefabs[33];
                            case "Advanced Vehicles":
                                if (template.name == "Mortar Tank")
                                {
                                    return bM.unitPrefabs[34];
                                }
                                else if (template.name == "P Flak Tank")
                                {
                                    return bM.unitPrefabs[35];
                                }
                                else if (template.name == "DP Rocket Tank")
                                {
                                    return bM.unitPrefabs[36];
                                }
                                else if (template.name == "Long Ambulance")
                                {
                                    return bM.unitPrefabs[37];
                                }
                                else if (template.name == "Brewer Truck")
                                {
                                    return bM.unitPrefabs[38];
                                }
                                return bM.unitPrefabs[39];
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
