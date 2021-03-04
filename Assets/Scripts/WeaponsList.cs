using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Public class with all weapons
public class WeaponsList
{
    //List to store weapons
    public List<Weapon> weaponList = new List<Weapon>();


    //Name, Description, Quote, Minimum Range, Maximum Range, AOE, AOE Type, Rounds Fired Per Attack, Damage, And Then The Weapon Type
    public WeaponsList()
    {
        weaponList.Add(new Weapon("Rifle", "An efficient anti-light armor gun that can target air units.", "No, it does not shoot grease.", 1, 1, 0, 0, 5, 15, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("RPG", "A simple rocket weapon that excels at taking down heavily armored and can target aerial units.", "BOOM!", 1, 2, 0, 0, 1, 85, 0, 0, 0,
            "Heavy", true, true, false, false, false, true, true, null));

        Dictionary<string, float> temp = new Dictionary<string, float>();
        temp.Add("Anti-Light Multi-Bonus", 2);
        weaponList.Add(new Weapon("Sniper Rifle", "A long range weapon that excels at taking down lightly armored units and can target aerial units.", "", 1, 5, 0, 0, 1, 60, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, temp));

        weaponList.Add(new Weapon("Mortar", "A long range anti-light weapon that can deal damage to multiple units at once", "", 2, 4, 1, 0, 1, 65, 0, 0, 0,
            "Light", false, true, false, false, false, false, true, null));

        //4
        weaponList.Add(new Weapon("Uzi", "A small anti-light armor gun that can target air units.", "", 1, 1, 0, 0, 3, 15, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        temp = new Dictionary<string, float>();
        //0 should be light armor, 1 is medium armor, 2 is heavy armor, 3 is slime
        temp.Add("Override To X Armor", 1);
        temp.Add("Guard Damage Reduction Percentage Override", 0.75f);
        weaponList.Add(new Weapon("Shield", "This changes the unit's armor to medium and offers a guard increase.", "", 0, 0, 0, 0, 0, 0, 0, 0, 0,
            "Light", false, false, false, false, false, true, false, temp));

        weaponList.Add(new Weapon("Minigun", "An anti-light armor gun that has a range of two tiles and can attack twice per day.", "", 1, 2, 0, 0, 7, 2, 0, 10, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Medkit", "Used for healing biological units.", "", 0, 1, 0, 0, 1, 0, 0, 0.30f, 0,
            "Light", false, false, true, false, false, true, true, null));

        weaponList.Add(new Weapon("Wrench", "Used for healing mechanical units.", "", 0, 1, 0, 0, 1, 0, 0, 0.30f, 0,
            "Light", false, false, false, true, false, true, true, null));

        //9
        weaponList.Add(new Weapon("Laser Rifle", "A super-efficient anti-light armor gun that can target air units.", "No, it does not shoot grease.", 1, 1, 0, 0, 5, 18, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Laser Uzi", "A good small anti-light armor gun that can target air units.", "", 1, 1, 0, 0, 3, 18, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Advanced RPG", "An advanced rocket weapon that excels at taking down heavily armored and can target aerial units.", "BOOM!", 1, 2, 0, 0, 1, 102, 
            0, 0, 0, "Heavy", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Laser Minigun", "A great anti-light armor gun that has a range of two tiles and can attack twice per day.", "", 1, 2, 0, 0, 7, 2, 0, 12, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Drone Gun", "A great small anti-light armor gun that can target air units.", "", 1, 1, 0, 0, 3, 22, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        //14
        weaponList.Add(new Weapon("Tank Cannon", "A powerful anti-medium armor gun.", "", 1, 1, 0, 0, 1, 130, 0, 0, 0,
            "Medium", false, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Heavy Tank Cannon", "A powerful anti-heavy armor gun.", "", 1, 1, 0, 0, 1, 185, 0, 0, 0,
            "Heavy", false, true, false, false, false, true, true, null));

        temp = new Dictionary<string, float>();
        temp.Add("HP Bonus", 10);
        weaponList.Add(new Weapon("Assault Turret", "A great anti-light armor gun that gives a small hp boost.", "", 1, 1, 0, 0, 5, 16, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, temp));

        weaponList.Add(new Weapon("Rocket Burster", "Mid-Range Rocket Launcher that excels at taking heavily armored units from afar.", "BOOM!", 2, 4, 0, 0, 5, 30, 0, 0, 0,
            "Heavy", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Artillery Cannon", "A long range anti-heavy weapon that can deal damage to multiple units at once", "", 4, 7, 1, 0, 1, 140, 0, 0, 0,
            "Heavy", false, true, false, false, false, false, true, null));

        //19
        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        weaponList.Add(new Weapon("Poison Rifle", "An efficient anti-light armor gun that also poisons.", "No, it does not shoot grease.", 1, 1, 0, 0, 5, 16, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Gas Grenade", "An anti-heavy projectile that releases a cloud of poison gas.", "", 2, 3, 0, 0, 1, 3, 0, 45, 0, 0, 0,
            "Heavy", false, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Gas Mortar", "A long range anti-light weapon that can deal damage to multiple units at once and creates poison gas.", "", 2, 4, 1, 0, 1, 1, 0,
            72, 0, 0, 0, "Light", false, true, false, false, false, false, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("StrengthE-3", 3);
        temp.Add("DefenseE-3", 3);
        weaponList.Add(new Weapon("Syringe", "Used for healing and inreases a unit's damage output. Additionally it gives a unit damage resistance.", "", 0, 1, 0, 0, 1, 0, 0, 0.40f,
            0, "Light", false, false, true, false, false, true, true, temp));

        weaponList.Add(new Weapon("Screamer Mouth", "Sends out sound waves in one direction that can hit multiple enemies", "", 1, 1, 2, 1, 1, 90, 0, 0, 0,
            "Light", true, true, false, false, false, false, true, null));

        //24
        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Brewer Head", "Sends out poison gas clouds. Additionally, this can attack five times per day." , "", 1, 1, 0, 0, 1, 5, 0 ,50, 0, 0, 0,
            "Light", true, true, false, false, false, false, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        weaponList.Add(new Weapon("Blast Eye x3", "3 scary eyes that shoots out poisonous blasts.", "No, it does not shoot grease.", 1, 1, 0, 0, 3, 45, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Slime Ball", "A slime ball is a dangerous ball of condensed poisonous gas.", "", 2, 5, 0, 0, 1, 2, 0, 112, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Small Slime Ball", "A slime ball is a dangerous ball of condensed poisonous gas.", "", 2, 5, 0, 0, 1, 2, 0, 84, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Mini Slime Ball", "A slime ball is a dangerous ball of condensed poisonous gas.", "", 2, 5, 0, 0, 1, 2, 0, 63, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        //29
        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        weaponList.Add(new Weapon("Blast Eye x5", "5 scary eyes that shoots out poisonous blasts.", "No, it does not shoot grease.", 1, 1, 0, 0, 5, 45, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));


        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Bio Rose", "Sends out poison gas clouds toward two tiles at a time. Additionally, this can attack four times per day.", "", 1, 1, 1, 1, 1, 4, 0, 70, 0, 0, 0,
            "Light", true, true, false, false, false, false, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Focus Eye", "A scary eye that deals a lot of damage and makes poison gas.", "No, it does not shoot grease.", 1, 3, 0, 0, 6, 45, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Weak Focus Eye", "A scary eye that deals quite a bit of damage and makes poison gas.", "No, it does not shoot grease.", 1, 3, 0, 0, 4, 45, 0, 0, 0,
            "Medium", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Gas Mortar MK 2", "A long range anti-light weapon that can deal damage to multiple units at once and creates poison gas.", "", 2, 4, 1, 0, 1, 1, 0,
            97.2f, 0, 0, 0, "Light", false, true, false, false, false, false, true, temp));

        //34
        weaponList.Add(new Weapon("Flak Gun", "A great anti-light armor gun that has takes apart light units from a medium distance.", "", 2, 3, 0, 0, 9, 2, 0, 10, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, null));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Venom Tank Cannon", "Deals a massive amount of anti-heavy damage and makes poison gas.", "", 1, 1, 0, 0, 1, 1, 0,
            250, 0, 0, 0, "Heavy", false, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Gas Mortar MK 3", "A long range anti-light weapon that can deal extreme damage to multiple units at once and creates poison gas.", "", 2, 4, 1, 0, 1, 1, 0,
            136.08f, 0, 0, 0, "Light", false, true, false, false, false, false, true, temp));

        
        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        weaponList.Add(new Weapon("P Flak Gun", "A great anti-light armor gun that has takes apart light units from a medium distance and also poisons.", "", 2, 3, 0, 0, 9, 2, 0, 10, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("DP Rocket Burster", "Long-Range Rocket Launcher that excels at taking heavily armored units from afar and makes poison gas. Additionally, this can attack twice per" +
            "day.", "BOOM!", 3, 5, 0, 0, 5, 2, 0, 31, 0, 0, 0,
            "Heavy", true, true, false, false, false, true, true, temp));

        //39
        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        weaponList.Add(new Weapon("Tall Brewer", "Sends out poison gas clouds toward four tiles at a time. Additionally, this can attack 5 times per day.", "", 1, 1, 3, 1, 1, 5, 0, 60, 0, 0, 0,
            "Light", true, true, false, false, false, false, true, temp));

        temp = new Dictionary<string, float>();
        temp.Add("Poisons", 3);
        temp.Add("Makes Poison Gas", 5);
        temp.Add("Launches Small Slime", 0);
        weaponList.Add(new Weapon("Slime Launcher", "The slime launcher launches multipurpose slime balls that can either make poison gas or small slimes.", "", 
            3, 6, 0, 0, 1, 1, 0, 130, 0, 0, 0, "Medium", true, true, false, false, false, true, true, temp));

        
        weaponList.Add(new Weapon("Duality Tank Cannon", "A powerful anti-medium armor gun. Specialized for the Duality Tank.", "", 1, 1, 0, 0, 1, 143, 0, 0, 0,
            "Medium", false, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Duality Rocket Burster", "Mid-Range Rocket Launcher that excels at taking heavily armored units from afar. Specialized for the Duality Tank.", "BOOM!", 2, 4, 0, 0, 5, 33, 0, 0, 0,
            "Heavy", true, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Laser Tank Cannon", "An extremely powerful anti-heavy armor gun.", "", 1, 1, 0, 0, 1, 222, 0, 0, 0,
            "Heavy", false, true, false, false, false, true, true, null));

        //44
        temp = new Dictionary<string, float>();
        temp.Add("HP Bonus", 20);
        weaponList.Add(new Weapon("Laser Assault Turret", "A powerful anti-light armor gun that gives a small hp boost.", "", 1, 1, 0, 0, 5, 19.2f, 0, 0, 0,
            "Light", true, true, false, false, false, true, true, temp));

        weaponList.Add(new Weapon("Duality Laser Tank Cannon", "An extremely powerful anti-heavy armor gun. Specialized for the Assault Auto Duality Tank", "", 1, 1, 0, 0, 1, 244.2f, 0, 0, 0,
            "Heavy", false, true, false, false, false, true, true, null));

        weaponList.Add(new Weapon("Duality Artillery Cannon", "A long range anti-heavy weapon that can deal damage to multiple units at once. Specialized for the Assault Auto Duality Tank", "", 4, 7, 1, 0, 1, 154, 0, 0, 0,
            "Heavy", false, true, false, false, false, false, true, null));

        temp = new Dictionary<string, float>();
        temp.Add("StrengthE-2",1);
        weaponList.Add(new Weapon("Pinpointer", "A device that greatly boosts the attack power for 1 turn of an allied unit.", "", 1, 7, 0, 0, 1, 0, 0, 0f, 0,
    "Light", false, false, true, true, false, true, true, temp));


    }

    public Weapon getWeaponCopy(int index)
    {
        return weaponList[index].copy();
    }
}

