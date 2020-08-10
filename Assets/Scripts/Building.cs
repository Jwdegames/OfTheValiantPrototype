using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Controllable
{
    public string name, description;
    public Player player;
    public GameManager gM;
    public int captureStatus = 0;
    public float xSizeMulti, ySizeMulti;
    public List<Sprite> sprites;
    public bool enabled = true;
    public bool makesUnits = false;
    // Start is called before the first frame update
    void Start()
    {
        isBuilding = true;
        setSprite();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setSize()
    {
        transform.localScale = new Vector3(xSizeMulti, ySizeMulti, 1f);
    }

    public void setSide(Player player)
    {
        this.player = player;
        if (player == null)
        {
            side = "null";
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            side = player.side;
            GetComponent<SpriteRenderer>().color = player.color;
        }
        team = gM.getTeam(side);
    }

    public void beginTurn(bool starting)
    {
        enabled = true;
        if (starting) return;
        switch (name)
        {
            case "House":
                if (gM.day % 3 == 0)
                {
                    player.people++;
                }
                break;
            case "Apartment":
                if (gM.day % 3 == 0)
                {
                    player.people += 3;
                }
                break;
            case "Mine MK1":
                player.metal += 10;
                break;
            case "Mine MK2":
                player.metal += 25;
                break;
            case "Mine MK3":
                player.metal += 45;
                break;
        }
    }

    public void setSprite()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch (name)
        {
            case "House":
                renderer.sprite = sprites[0];
                description = "Makes a person every 3 days.";
                break;
            case "Apartment":
                renderer.sprite = sprites[1];
                description = "Makes 3 people every 3 days.";
                break;
            case "Mine MK1":
                description = "Makes 10 metal every day.";
                renderer.sprite = sprites[2];
                break;
            case "Mine MK2":
                description = "Makes 25 metal every day.";
                renderer.sprite = sprites[3];
                break;
            case "Mine MK3":
                description = "Makes 45 metal every day.";
                renderer.sprite = sprites[4];
                break;
            case "Barracks":
                description = "Can produce an infantry unit each turn.";
                renderer.sprite = sprites[5];
                makesUnits = true;
                break;
            case "Factory":
                description = "Can produce a land vehicle unit each turn.";
                renderer.sprite = sprites[6];
                makesUnits = true;
                break;
            case "Laboratory":
                description = "Allows production of advanced units.";
                renderer.sprite = sprites[7];
                break;

        }

        
    }

    public IEnumerator capture(Unit unit)
    {

        if (unit == null) yield return null;
        else
        {
            int flashes = 0;
            if (player == null)
            {
                player = gM.playerDictionary[unit.getSide()];
                while (flashes < 3)
                {
                    GetComponent<SpriteRenderer>().color = Color.white;

                    yield return new WaitForSeconds(0.25f);
                    GetComponent<SpriteRenderer>().color = player.color;
                    yield return new WaitForSeconds(0.25f);
                    flashes++;

                }
                side = player.side;
                gM.buildingDictionary[side].Add(this);
            }
            else
            {
                gM.buildingDictionary[side].Remove(this);
                while (flashes < 3)
                {
                    GetComponent<SpriteRenderer>().color = player.color;
                    yield return new WaitForSeconds(0.25f);
                    GetComponent<SpriteRenderer>().color = Color.white;
                    yield return new WaitForSeconds(0.25f);
                    flashes++;
                }
                //
                player = null;
                
            }
            
        }
        enabled = false;
        gM.uiScript.getBPEconStats();
        team = gM.getTeam(side);
    }
    public void setSizeAccordingly()
    {
        float width = GetComponent<SpriteRenderer>().sprite.rect.width;
        float height = GetComponent<SpriteRenderer>().sprite.rect.height;
        switch (name)
        {
            case "Apartment":
                transform.localScale = new Vector3(5f, 2.55f, 1f);
                break;
        }
    }

    override
    public string ToString()
    {
        return name;
    }
}
