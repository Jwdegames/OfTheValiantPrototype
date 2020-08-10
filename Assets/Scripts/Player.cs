using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to hold player info
public class Player
{
    public string side, faction;
    public int metal, people;
    public Color color;
    public List<Unit> units = new List<Unit>();
    public List<Building> buildings = new List<Building>();
    public GameManager gM;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getPPLPDay()
    {
        buildings = gM.buildingDictionary[side];
        int count = 0;
        foreach (Building building in buildings)
        {
            switch (building.name)
            {
                case "House":
                    count++;
                    break;
                case "Apartment":
                    count += 3;
                    break;
            }
        }
        return count;
    }

    //Returns true if this player has a lab so they can produce advanced units
    public bool hasLab()
    {
        foreach (Building building in buildings)
        {
            if (building.name == "Laboratory")
            {
                return true;
            }
        }
        return false;
    }

    public int getMTPDay()
    {
        buildings = gM.buildingDictionary[side];
        int count = 0;
        foreach (Building building in buildings)
        {
            switch (building.name)
            {
                case "Mine MK1":
                    count += 10;
                    break;
                case "Mine MK2":
                    count += 25;
                    break;
                case "Mine MK3":
                    count += 45;
                    break;
            }
        }
        return count;
    }
}
