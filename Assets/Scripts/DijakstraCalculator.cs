using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Configuration;

//A custom class that performs calculations based off of Dijakstra's Algorithm for C#. Made by JWDE.
public class DijakstraCalculator
{
    private SimplePriorityQueue<Tile> frontier;
    //private HashSet<SimpleNode<Tile, float>> explored;
    private HashSet<Tile> exploredT;
    private Tile start;
    private Tile dest;
    private SimpleNode<Tile, float> current;
    private Tile currentT;
    private bool foundGoal = false;
    GameManager gM;

    //Constructor
    public DijakstraCalculator(GameManager g, Tile s, Tile d)
    {
        start = s;
        dest = d;
        gM = g;
        reset();
    }

    /**
     * Simplified Find Path for Movement via Djakstra Algorithm. 
     * Less code because all adjacent nodes are handled via a list.
     */

    public List<Tile> findPath(bool careAboutUnitOnDest, bool careAboutNonDestUnits)
    {
        //if (careAboutUnitOnDest) Debug.Log("Finding path and caring about unit on dest!");
        foundGoal = false;
        List<Tile> path = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        Unit unit = start.getUnitScript();
        frontier.Enqueue(start, 0);
        start.predecessor = null;
        float unitMP = start.getUnitScript().getCurrentMP();
        if (start == dest) return new List<Tile>() { start};
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            //Debug.Log(currentTile.getPos());
            exploredT.Add(current.Data);
            //If we can move directly to the node then we should do so
            //Debug.Log(current.Data.canMoveToAdjacent(dest, true));
            if (careAboutUnitOnDest)
            {
                if (currentTile == dest && currentTile.predecessor.canMoveToAdjacent(dest,true))
                {
                    //dest.predecessor = current.Data;
                    currentT = dest;
                    foundGoal = true;
                    break;
                }
                /*if (current.Data.canMoveToAdjacent(dest, true) && current.Priority + currentTile.getMoveCost(unit, dest) <= unit.getCurrentMP())
                {
                    //Debug.Log("Found a path costing " + current.Priority + " MP!");
                    dest.predecessor = current.Data;
                    currentT = dest;
                    foundGoal = true;
                    break;
                }
                //We already started on the tile
                else if (dest == start)
                {
                    dest.predecessor = null;
                    currentT = dest;
                    foundGoal = true;
                    break;
                }
            }
            else
            {
                if (current.Data.canMoveToAdjacent(dest, false) && current.Priority + currentTile.getMoveCost(unit,dest)<= unit.getCurrentMP())
                {
                    dest.predecessor = current.Data;
                    currentT = dest;
                    foundGoal = true;
                    break;
                }
                //We already started on the tile
                else if (dest == start)
                {
                    dest.predecessor = null;
                    currentT = dest;
                    foundGoal = true;
                    break;
                }*/
            }
            //Add each adjacent node to the frontier
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                //Skip if t is null or if the mp to move to the tile exceeds our current MP
                if (t == null || Math.Round(current.Priority+currentTile.getMoveCost(unit, t), 3) > Math.Round(unitMP,3)) continue;
                if (start.canMoveTo(t, careAboutNonDestUnits))
                {
                    //Check first if the node is in the explored array, otherwise we don't do anything
                    temp = new SimpleNode<Tile, float>(t);
                    temp.Priority = current.Priority + currentTile.getMoveCost(unit, t);
                    if (temp.Priority > unit.getCurrentMP()) continue;
                    if (!exploredT.Contains(t) && !frontier.Contains(t))
                    {
                        //Add the node to the frontier if we didn't explore it already
                        frontier.Enqueue(t, current.Priority + currentTile.getMoveCost(unit, t));
                        t.predecessor = current.Data;
                    }
                    //If we have a move path that is shorter than what is in the frontier, replace it
                    else if (frontier.Contains(t))
                    {
                        temp2 = frontier.RemoveNode(t);
                        if (temp.Priority < temp2.Priority)
                        {
                            frontier.Enqueue(temp.Data, temp.Priority);
                            t.predecessor = currentTile;
                        }
                        else
                        {
                            frontier.Enqueue(temp2.Data, temp2.Priority);
                        }

                    }
                }
            }
        }
        //Generate the path if we managed to find out goal
        if (foundGoal)
        {
            while (currentT != null)
            {
                //Since we are adding backwards, add at the front rather than the back of the list
                //Debug.Log("Adding " + currentT + " for " + currentT.moveCost + " MP to path!");
                path.Insert(0, currentT);
                currentT = currentT.predecessor;

            }
            //Debug.Log("Unit had a MP of " + path[0].getUnitScript().getCurrentMP());
            return path;
        }
        else
        {
           // Debug.Log("We couldn't find a path!");
            return null;
        }
    }

    //Returns the path that the unit on the start can travel to such that it moves as far as possible to the destination tile
    //Start from destination tile and move outward and try to find the closest to the destination tile that has a path

    public List<Tile> getAsFarAsPossiblePath()
    {
        foundGoal = false;
        List<Tile> path = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        frontier.Enqueue(dest, 0);
        start.predecessor = null;
        Unit unit = start.getUnitScript();
        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, null,null);
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();

            exploredT.Add(current.Data);

            //Check to see if a path can be made
            extraCalc.reset();
            extraCalc.setValues(start, currentTile);
            
            path = extraCalc.findPath(true, false);
            if (path != null)
            {
                //Debug.Log("Found path from " + start + " to " + currentTile +" on way to "+dest);
                return path;
            }
            else
            {
                //Debug.Log("Couldn't find path from " + start + " to " + currentTile + " on way to " + dest);
            }

            //Add each adjacent node to the frontier
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                //Check first if the node is in the explored array, otherwise we don't do anything
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + currentTile.getMoveCost(unit, t);
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + currentTile.getMoveCost(unit, t));
                    t.predecessor = current.Data;
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                        t.predecessor = currentTile;
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }

            }

        }
        //We couldn't find a path
        return null;
     }


    //Returns the amount of mp away the destination tile is from the unit tile ignoring everything else given the unit is on the start
    public float getMovementDistance(Unit unit)
    {
        foundGoal = false;
        List<Tile> path = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        frontier.Enqueue(start, 0);
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();

            exploredT.Add(current.Data);

            if (currentTile == dest)
            {
                //Debug.Log("Found dest!");
                return current.Priority;
            }
            //Add each adjacent node to the frontier
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;

                //Check first if the node is in the explored array, otherwise we don't do anything
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + currentTile.getMoveCost(unit,t);
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + currentTile.getMoveCost(unit, t));
                    t.predecessor = current.Data;
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
                
            }

        }
        //Return a negative number to indicate we couldn't find the attack distance
        Debug.LogError("Couldn't find dest!");
        return -1;
     }

    //Method to get the aoe tiles around the target that can be attacked on
    public List<Tile> getPresetAOETiles(Weapon weapon, Tile target, bool includeTarget)
    { 
        if (target == null) return null;
        if (weapon.aoe <= 0) return null;
        if (weapon.aoeType == 0) return null;
        List<Tile> presetAOETiles = new List<Tile>();
        Tile temp, previous = target;
        List<string> dirs = new List<string>() { "upleft", "upright", "left", "right", "downleft", "downright" };
        switch(weapon.aoeType)
        {
            case 1:
                //Go in each dir from the dest and add tiles
                foreach (string dir in dirs)
                {
                    previous = target;
                    for (int i = 0; i < weapon.aoe; i++)
                    {
                        temp = previous.getAdjacentTileOnSide(dir);
                        if (temp == null)
                        {
                            break;
                        }
                        presetAOETiles.Add(temp);
                        previous = temp;
                    }
                }
                break;

        }
        if (includeTarget)
        {
            presetAOETiles.Add(target);
        }
        return presetAOETiles;
    }

    //Find all tiles that the unit can move to so that it can attack an enemy unit
    //Can also be used for healing
    public List<Tile> getInRangeTiles(List<Weapon> weapons)
    {
        if (dest == null || start == null)
        {
        Debug.LogError("Either dest is null(" + (dest == null) + ") and/or start is null(" + (start == null) + ").");
            return null;
        }
        List<Tile> inRange = new List<Tile>();
        foreach (Weapon weapon in weapons)
        {
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            DijakstraCalculator aoeCalc = new DijakstraCalculator(gM, null, null);
            List<Tile> presetAOETiles = new List<Tile>();
            if (weapon.aoe > 0)
            {
                if (weapon.aoeType != 0)
                {
                    presetAOETiles = getPresetAOETiles(weapon, dest, true);
                }
            }
            SimpleNode<Tile, float> temp, temp2;
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                inRange.Add(dest);
            }
            else
            {
                //Add the destination node, than add all nodes around it
                //We do this because we want to find all nodes that we can attack the enemy unit from
                frontier.Enqueue(dest, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;
                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (weapon.aoe <= 0)
                    {
                        if (current.Priority >= minRange && current.Priority <= maxRange)
                        {
                            if (currentTile.getUnit() == null)
                            {
                                inRange.Add(current.Data);
                            }

                        }
                    }
                    else
                    {
                        switch (weapon.aoeType)
                        {
                            case 0:
                            if (current.Priority >= minRange - weapon.aoe && current.Priority <= maxRange + weapon.aoe)
                            {
                                if (currentTile.getUnit() == null)
                                {
                                    inRange.Add(current.Data);

                                }
                            }
                            break;
                            //Attack tiles in same dir
                            case 1:
                            if (current.Priority >= minRange - weapon.aoe && current.Priority <= maxRange + weapon.aoe)
                            {
                                //Now must determine if current is in the same dir
                                //Better to get tiles before hand, rather than generating a whole bunch
                                if (currentTile.getUnit() == null && presetAOETiles.Contains(currentTile))
                                    {
                                        inRange.Add(current.Data);
                                    }
                            }
                            break;
                        }
                    }
                        
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (temp.Priority > maxRange + weapon.aoe) continue;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }
                }
            }
        }
        if (inRange == null || inRange.Count == 0) return null;
        return inRange;
    }



    //Gets the path to a tile that can attack an enemy unit or other thing
    //Returns null if no path available
    public List<Tile> findInRangePath(Unit unit, List<Weapon> weapons, bool absolute, bool hypothetical)
    {
        //First get the tiles we want to move to and then reset the calculator

        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, start, dest);
        List<Tile> inRangeTiles = extraCalc.getInRangeTiles(weapons);
        DijakstraCalculator extraCalc2 = new DijakstraCalculator(gM, start, dest);
        if (unit.getTile() == null && !hypothetical)
        {
            Debug.LogError(unit + " does not have a tile!");
        }
        if (!hypothetical)
        {
            //gM.printPath(inRangeTiles);
        }
            //Debug.Log(extraCalc2.getAbsoluteDist());

        if (inRangeTiles == null|| inRangeTiles.Count == 0)
        {
            Debug.Log("Couldn't find tile to attack " + dest);
        }

        //
        //Debug.Log("Printing in range tiles!");
        //Debug.Log(inRangeTiles.Count);
        if (!hypothetical)
        {
            if (inRangeTiles.Contains(start))
            {
                //Debug.Log("We can fire");
                return new List<Tile>() { start };
            }
            else
            {
                //NOTICE: apparently tile is null otherwise, debug statement would run
                if (unit.getTile() != null)
                {
                    //Debug.Log(unit + " is not in range of " + dest);
                }
            }
        }
        //Debug.Log(start);
        reset();

        foundGoal = false;
        List<Tile> path = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        frontier.Enqueue(start, start.getMoveCost(unit));
        start.predecessor = null;

        //printPath(inRangeTiles);

        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();

            exploredT.Add(current.Data);
            //Debug.Log(currentTile);
            //If we can move directly to the node then we should do so
            //Loop through all possible tiles we can move to
            foreach (Tile tile in inRangeTiles)
            {
                if (current.Data.canMoveToAdjacent(tile, true) || tile == start)
                {
                    tile.predecessor = current.Data;
                    currentT = tile;
                    foundGoal = true;
                    break;
                }
            }
            if (foundGoal) break;
            //Add each adjacent node to the frontier
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                //Check to make sure we can move to the tile as well as that we have the mp to move there
                if (!hypothetical && !start.canMoveTo(t, false))
                {
                    continue;
                }
                if (hypothetical && !start.canMoveToWithUnit(unit, t, false))
                {
                    continue;
                }
                if (!absolute && !((float)Math.Round(currentTile.getMoveCost(unit) + current.Priority, 3) <= (float)Math.Round(start.getUnitScript().getCurrentMP(), 3)))
                {
                    continue;
                }
                //Check first if the node is in the explored array, otherwise we don't do anything
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + t.getMoveCost(unit);
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + t.getMoveCost(unit));
                    t.predecessor = current.Data;
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                        t.predecessor = currentTile;
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
                
            }
        }
        //Generate the path if we managed to find out goal
        if (foundGoal)
        {
            while (currentT != null)
            {
                //Since we are adding backwards, add at the front rather than the back of the list
                path.Insert(0, currentT);
                currentT = currentT.predecessor;

            }
            return path;
        }
        else
        {
            Debug.Log("We couldn't find a path to get in range as "+unit+" to tile "+dest+"!");
            return null;
        }
    }

    //Finds the closest empty tile
    public Tile findClosestEmptyTile(bool includeStart)
    {
        SimpleNode<Tile, float> temp, temp2;
        if (includeStart)
        {
            frontier.Enqueue(start, 0);
        }
        else
        {
            start.setAdjacent();
            List<Tile> adjTiles = start.getAdjacent();
            foreach(Tile adjTile in adjTiles)
            {
                frontier.Enqueue(start, 0);
            }
            exploredT.Add(start);
        }
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            exploredT.Add(current.Data);
            if (currentTile.getUnit() == null)
            {
                return currentTile;
            }

            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + 1;

                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + 1);
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }
        }
        return null;
    }
   

    //Finds all tiles that the unit at the start tile can move to
    public List<Tile> findAllMoveables()
    {
        if (start.getUnit() == null) return null;
        Unit unit = start.getUnitScript();
        string side = unit.getSide();
        int team = unit.getTeam();
        List<Tile> moveable = new List<Tile>();


        SimpleNode<Tile, float> temp, temp2;
        //Debug.Log(weapon.name);
        
        frontier.Enqueue(start, 0);
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            exploredT.Add(current.Data);
            if (current.Priority <= unit.getCurrentMP())
            {
                moveable.Add(current.Data);

            }
            List<Tile> adjacentNodes = currentTile.getAdjacent();

            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + currentTile.getMoveCost(unit);
                if (temp.Priority > unit.getCurrentMP()) continue;
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + 1);
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }



        }

        if (moveable.Contains(start)) moveable.Remove(start);
        if (moveable == null || moveable.Count == 0) return new List<Tile>();
        return moveable;
    }

    //Find all tiles that can be attacked from the start if the unit was there
    public List<Tile> findAllAttacks(Unit unit)
    {
         return findAllAttacksWithWeapons(unit, unit.getAllDamageHandWeapons());
    }

    //Find all tiles that can be attacked from the start if the unit was there
    public List<Tile> findAllAttacksWithWeapons(Unit unit, List<Weapon> weapons)
    {
        string side = unit.getSide();
        int team = unit.getTeam();
        List<Tile> attackable = new List<Tile>();
        //Weapon weapon = unit.getCurrentWeapon();
        foreach (Weapon weapon in weapons)
        {
            if (weapon.currentAttacks == weapon.maxAttacksPerTurn) continue;
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            SimpleNode<Tile, float> temp, temp2;
            //Debug.Log(weapon.name);
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;
                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (current.Priority >= minRange && current.Priority <= maxRange)
                    {
                        //Debug.Log("Tile identified!");
                        if (weapon.mustTargetEnemies)
                        {
                            //Debug.Log("Found tile to attack! It was "+currentTile.getPos());
                            if (currentTile.getUnit() != null && currentTile.getUnitScript().getTeam() != team)
                                attackable.Add(current.Data);
                        }
                        else
                        {
                            if (currentTile.getUnit() != null)
                            {
                                //Prevent Friendly Fire
                                if (currentTile.getUnitScript().getTeam() != team)
                                {
                                    attackable.Add(current.Data);
                                }
                            }
                            else
                            {
                                attackable.Add(current.Data);
                            }
                        }
                    }
                    List<Tile> adjacentNodes = currentTile.getAdjacent();

                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }



                }
            }
        }
        if (attackable == null || attackable.Count == 0) return new List<Tile>();
        return attackable;
    }

    //Find all enemy units that can be attacked within range
    public List<Tile> findAllEnemyUnitsAttackable()
    {
        Unit unitScript = start.getUnitScript();
        return findAllEnemyUnitsAttackableWithWeapons(unitScript.getAllDamageHandWeapons());
    }

    public List<Tile> findAllEnemyUnitsAttackableWithWeapons(List<Weapon> weapons)
    {
        if (start.getUnit() == null) return null;
        List<Tile> attackableTiles = findAllTilesAttackableWithWeapons(weapons);
        int team = start.getUnitScript().getTeam();
        attackableTiles.RemoveAll(item => item.getUnit() == null || item.getUnitScript().getTeam() == team);
        return attackableTiles;
    }

    public List<Tile> findAllTilesAttackableWithWeapons(List<Weapon> weapons)
    {
        if (start.getUnit() == null) return null;
        Unit unitScript = start.getUnitScript();
        string side = start.getUnitSide();

        double minRange = unitScript.getCurrentWeapon().minRange;
        ////double maxRange = unitScript.getCurrentWeapon().maxRange;
        double unitMP = unitScript.getCurrentMP();

        //Extra calculator for getting attackble tiles
        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, null, null);
        DijakstraCalculator pathCalc = new DijakstraCalculator(gM, null, null);
        List<Tile> attackable = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        

        frontier.Enqueue(start, 0);
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            exploredT.Add(current.Data);

            if (current.Priority <= unitMP)
            {
                //Make sure we can actually get to the tile itself
                pathCalc.reset();
                pathCalc.setValues(start, currentTile);

                if (pathCalc.findPath(true, false) != null)
                {
                    extraCalc.reset();
                    extraCalc.setStart(currentTile);
                    //Only add the tile if we can attack the enemy unit
                    //if (currentTile.getUnit() != null && currentTile.getUnitScript().getSide() != side && currentTile.unitHP > currentTile.expectedDamage)
                    //Debug.Log(currentTile + " is the tile we are measuring from");
                    attackable.AddRange(extraCalc.findAllAttacksWithWeapons(unitScript,weapons));
                    //printPath(attackable);
                }
            }
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + t.getMoveCost(unitScript);
                if (temp.Priority > unitMP) continue;
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + t.getMoveCost(unitScript));
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }



        }
        
        
        
        return new HashSet<Tile>(attackable).ToList<Tile>();
    } 

    //Gets all tiles and assigns a number to them
    //Adds points if part of team, subtract if not
    //Use to determine areas to retreat to/attack
    //+1/-1 for unit
    //+1/-1 if unit can attack this tile without moving
    //+1/-1 if unit can attack tile at all this turn
    //+2/-2 if unit can heal this tile without moving
    //+1/-1 if unit can heal this tile without moving
    //+1/-1 for a building
    //+3/-3 for a building that heals
    public Dictionary<Tile,float> getInfluenceMapAsDict()
    {
        if (start.getUnit() == null) return null;
        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, null, null);
        frontier.Enqueue(start, 0);
        Unit unit = start.getUnitScript();
        int team = unit.getTeam();
        SimpleNode<Tile, float> temp, temp2;
        Dictionary<Tile, float> influenceDict = new Dictionary<Tile, float>();
        while (frontier.Count > 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            exploredT.Add(current.Data);

            int point = 0;
            //Handle if there's a building
            if (currentTile.getBuilding() != null)
            {
                Building building = currentTile.getBuilding();

                point++;
                if (building.team >= 0)
                {
                    if (unit.biological && !unit.flying && building.name == "Hospital")
                    {
                        point += 3;
                    }
                    else if (unit.mechanical)
                    {
                        if (!unit.flying && building.name == "Warehouse")
                        {
                            point += 3;
                        }
                        else if (unit.flying && building.name == "Airpad")
                        {
                            point += 3;
                        }
                    }
                    if (building.team != team)
                    {
                        point *= -1;
                    }
                }                               
            }

            //Handle giving bonuses to tiles
            if (currentTile.getUnit() != null)
            {
                Unit currentUnit = currentTile.getUnitScript();
                int mod = 0, score = 0;
                if (currentUnit.getTeam() == team)
                {
                    point++;
                    mod = 3;
                }
                else
                {
                    point--;
                    mod = -3;
                }

                extraCalc.reset();
                extraCalc.setValues(currentTile, null);
                List<Tile> attackTilesWithoutMoving = extraCalc.findAllAttacksWithWeapons(currentUnit, currentUnit.getAllDamageActiveWeapons());
                extraCalc.reset();
                extraCalc.setValues(currentTile, null);
                List<Tile> attackTiles = extraCalc.findAllTilesAttackableWithWeapons(currentUnit.getAllDamageActiveWeapons());
                extraCalc.reset();
                extraCalc.setValues(currentTile, null);
                List<Tile> healTilesWithoutMoving = extraCalc.findAllHealables(currentUnit, currentUnit.getAllHealRepairActiveWeapons());
                extraCalc.reset();
                extraCalc.setValues(currentTile, null);
                List<Tile> healTiles = extraCalc.findAllTilesHealableWithWeapons(currentUnit.getAllHealRepairActiveWeapons());
                //Go through each list and add a value
                foreach(Tile tile in attackTilesWithoutMoving)
                {
                    if (influenceDict.ContainsKey(tile))
                    {
                        influenceDict[tile] += mod;
                    }
                    else
                    {
                        influenceDict.Add(tile,mod);
                    }
                }
                foreach (Tile tile in attackTiles)
                {
                    if (influenceDict.ContainsKey(tile))
                    {
                        influenceDict[tile] += mod;
                    }
                    else
                    {
                        influenceDict.Add(tile, mod);
                    }
                }
                foreach (Tile tile in healTilesWithoutMoving)
                {
                    if (influenceDict.ContainsKey(tile))
                    {
                        influenceDict[tile] += mod * 2;
                    }
                    else
                    {
                        influenceDict.Add(tile, mod * 2);
                    }
                }
                foreach (Tile tile in healTiles)
                {
                    if (influenceDict.ContainsKey(tile))
                    {
                        influenceDict[tile] += mod;
                    }
                    else
                    {
                        influenceDict.Add(tile, mod);
                    }
                }

            }

            //Add the point to the current Tile
            if (influenceDict.ContainsKey(currentTile))
            {
                influenceDict[currentTile] += point;
            }
            else
            {
                influenceDict.Add(currentTile, point);
            }

            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + 1;
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + 1);
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }

        }
        return influenceDict;
    }
    
    //Determine if enemy unit can be attacked by the current unit with any of the active weapons
    public bool canAttackEnemyAtAll(Unit attacker, Unit defender)
    {
        List <Weapon> weapons = start.getUnitScript().getAllActiveWeapons();
        return canAttackEnemyWithWeapons(attacker, weapons, defender);
    }

    //Determine if enemy unit can be attacked by the current unit with any of the primary/secondary/tertiary weapons
    public bool canAttackEnemy(Unit attacker, Unit defender)
    {
        List<Weapon> weapons = start.getUnitScript().getAllHandWeapons();
        return canAttackEnemyWithWeapons(attacker, weapons, defender);
    }

    //Determines if we can attack an enemy with a weapon from the current tile
    public bool canAttackEnemyWithWeapon(Unit attacker, Weapon weapon, Unit defender)
    {
        List<Weapon> weapons = new List<Weapon>() { weapon };
        return canAttackEnemyWithWeapons(attacker, weapons, defender);
    }

    public int getAbsoluteDist()
    {
        SimpleNode<Tile, float> temp, temp2;
        //Debug.Log(start != null);
        //Log(dest != null);

            frontier.Enqueue(start, 0);
            while (frontier.Count > 0)
            {
                current = frontier.DequeueNode();
                Tile currentTile = current.Data;

                currentTile.setAdjacent();
                exploredT.Add(current.Data);

                    if (currentTile == dest)
                    {
                        return (int)current.Priority;
                }
                //else if (current.Priority >= maxRange) continue;
                List<Tile> adjacentNodes = currentTile.getAdjacent();
                foreach (Tile t in adjacentNodes)
                {
                    if (t == null) continue;
                    temp = new SimpleNode<Tile, float>(t);
                    temp.Priority = current.Priority + 1;
                    if (!exploredT.Contains(t) && !frontier.Contains(t))
                    {
                        //Add the node to the frontier if we didn't explore it already
                        frontier.Enqueue(t, current.Priority + 1);
                    }
                    //If we have a move path that is shorter than what is in the frontier, replace it
                    else if (frontier.Contains(t))
                    {
                        //Debug.Log(t);
                        temp2 = frontier.RemoveNode(t);
                        if (temp.Priority < temp2.Priority)
                        {
                            frontier.Enqueue(temp.Data, temp.Priority);
                        }
                        else
                        {
                            frontier.Enqueue(temp2.Data, temp2.Priority);
                        }

                    }
                }
            }
            //Debug.Log(start.getPos() + " is not within " + minRange + "-" + maxRange + " tiles of " + dest.getPos());
            return -1;
    }

    //Helper method to determine if a tile is within a certain distance of another tile
    public bool isInAbsoluteRange(int minRange, int maxRange)
    {
        SimpleNode<Tile, float> temp, temp2;
        //Debug.Log(start != null);
        //Log(dest != null);
        if (maxRange < minRange)
        {
            return false;
        }
        else if (minRange == 0 && start == dest)
        {
            return true;
        }
        else
        {
            frontier.Enqueue(start, 0);
            while (frontier.Count > 0)
            {
                current = frontier.DequeueNode();
                Tile currentTile = current.Data;

                currentTile.setAdjacent();
                exploredT.Add(current.Data);

                if (current.Priority >= minRange && current.Priority <= maxRange)
                {
                    if (currentTile == dest)
                    {
                        return true;
                    }
                }
                //else if (current.Priority >= maxRange) continue;
                List<Tile> adjacentNodes = currentTile.getAdjacent();
                foreach (Tile t in adjacentNodes)
                {
                    if (t == null) continue;
                    temp = new SimpleNode<Tile, float>(t);
                    temp.Priority = current.Priority + 1;
                    if (temp.Priority > maxRange) continue;
                    if (!exploredT.Contains(t) && !frontier.Contains(t))
                    {
                        //Add the node to the frontier if we didn't explore it already
                        frontier.Enqueue(t, current.Priority + 1);
                    }
                    //If we have a move path that is shorter than what is in the frontier, replace it
                    else if (frontier.Contains(t))
                    {
                        //Debug.Log(t);
                        temp2 = frontier.RemoveNode(t);
                        if (temp.Priority < temp2.Priority)
                        {
                            frontier.Enqueue(temp.Data, temp.Priority);
                        }
                        else
                        {
                            frontier.Enqueue(temp2.Data, temp2.Priority);
                        }

                    }
                }
            }
            //Debug.Log(start.getPos() + " is not within " + minRange + "-" + maxRange + " tiles of " + dest.getPos());
            return false;

        }
    }

    //Helper method to get tiles in absolute range
    public List<Tile> getInAbsoluteRange(int minRange, int maxRange)
    {
        SimpleNode<Tile, float> temp, temp2;
        List<Tile> tiles = new List<Tile>();
        //Debug.Log(start != null);
        //Log(dest != null);
        if (maxRange < minRange)
        {
            return null;
        }
        else
        {
            frontier.Enqueue(start, 0);
            while (frontier.Count > 0)
            {
                current = frontier.DequeueNode();
                Tile currentTile = current.Data;

                currentTile.setAdjacent();
                exploredT.Add(current.Data);

                if (current.Priority >= minRange && current.Priority <= maxRange)
                {
                        tiles.Add(currentTile);
                }
                //else if (current.Priority >= maxRange) continue;
                List<Tile> adjacentNodes = currentTile.getAdjacent();
                foreach (Tile t in adjacentNodes)
                {
                    if (t == null) continue;
                    temp = new SimpleNode<Tile, float>(t);
                    temp.Priority = current.Priority + 1;
                    if (temp.Priority > maxRange) continue;
                    if (!exploredT.Contains(t) && !frontier.Contains(t))
                    {
                        //Add the node to the frontier if we didn't explore it already
                        frontier.Enqueue(t, current.Priority + 1);
                    }
                    //If we have a move path that is shorter than what is in the frontier, replace it
                    else if (frontier.Contains(t))
                    {
                        //Debug.Log(t);
                        temp2 = frontier.RemoveNode(t);
                        if (temp.Priority < temp2.Priority)
                        {
                            frontier.Enqueue(temp.Data, temp.Priority);
                        }
                        else
                        {
                            frontier.Enqueue(temp2.Data, temp2.Priority);
                        }

                    }
                }
            }
            //Debug.Log(start.getPos() + " is not within " + minRange + "-" + maxRange + " tiles of " + dest.getPos());
            if (tiles == null || tiles.Count == 0) return null;
            return tiles;

        }
    }


    public bool canAttackEnemyExactlyWithWeapon(Unit attacker, Weapon weapon, Unit defender)
    {
        return canAttackEnemyExactlyWithWeapons(attacker, new List<Weapon>() { weapon }, defender);
    }

    //Ignores AOE and determines if the unit can attack the defender unit with the given weapons
    public bool canAttackEnemyExactlyWithWeapons(Unit attacker, List<Weapon> weapons, Unit defender)
    {
        if (defender != null)
        {
            defender = dest.getUnitScript();
        }
        foreach (Weapon weapon in weapons)
        {
            //Continue to next weapon if the weapon can't attack
            if (!weapon.doesSomething || !weapon.damages) continue;
            if (defender != null)
            {
                if (defender.flying && !weapon.canTargetAir) continue;
                if (defender.isSubmerged && !weapon.canTargetSub) continue;
            }
            string side = attacker.getSide();
            List<Tile> attackable = new List<Tile>();
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            DijakstraCalculator aoeCalc = new DijakstraCalculator(gM, null, null);
            SimpleNode<Tile, float> temp, temp2;
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                //attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;

                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (current.Priority >= minRange && current.Priority <= maxRange)
                    {
                        //if (currentTile.getUnit() != null && currentTile.getUnitScript().getSide() != side)
                        //attackable.Add(current.Data);

                        if (currentTile == dest)
                        {
                            return true;
                        }
                    }
                    //else if (current.Priority >= maxRange) continue;
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (temp.Priority > maxRange) continue;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }
                }
            }
        }
        return false;
    }

    //Determine if enemy unit can be attacked by the current unit with any of the active weapons
    public bool canAttackEnemyWithWeapons(Unit attacker, List<Weapon> weapons, Unit defender)
    {
        if (defender != null)
        {
            defender = dest.getUnitScript();
        }
        foreach (Weapon weapon in weapons)
        {
            //Continue to next weapon if the weapon can't attack
            if (!weapon.doesSomething || !weapon.damages) continue;
            if (defender != null)
            {
                if (defender.flying && !weapon.canTargetAir) continue;
                if (defender.isSubmerged && !weapon.canTargetSub) continue;
            }
            string side = attacker.getSide();
            List<Tile> attackable = new List<Tile>();
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            DijakstraCalculator aoeCalc = new DijakstraCalculator(gM, null, null);
            SimpleNode<Tile, float> temp, temp2;
            List<Tile> presetAOETiles = new List<Tile>();
            if (weapon.aoe > 0)
            {
                if (weapon.aoeType != 0)
                {
                    presetAOETiles = getPresetAOETiles(weapon, dest, true);
                }
            }
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                //attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;

                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (weapon.aoe <= 0)
                    {
                        if (current.Priority >= minRange && current.Priority <= maxRange)
                        {
                            if (currentTile == dest)
                            {
                                return true;
                            }

                        }
                    }
                    else
                    {
                        switch (weapon.aoeType)
                        {
                            case 0:
                                if (current.Priority >= minRange - weapon.aoe && current.Priority <= maxRange + weapon.aoe)
                                {
                                    if (currentTile == dest)
                                    {
                                        return true;

                                    }
                                }

                                break;
                            case 1:
                                if (current.Priority >= minRange - weapon.aoe && current.Priority <= maxRange + weapon.aoe)
                                {
                                    //Now must determine if current is in the same dir
                                    //Better to get tiles before hand, rather than generating a whole bunch
                                    if (presetAOETiles.Contains(currentTile))
                                    {
                                        return true;
                                    }
                                }
                                break;
                        }

                    }


                    //else if (current.Priority >= maxRange) continue;
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (temp.Priority > maxRange) continue;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }



                }
            }
        }
        return false;
    }


    //ALL HEALING/REPAIR CODE BELOW

    //HEALING

    //REPAIR

    //<Healing Section>
    //Find all tiles that can be healed from the start if the unit was there with the given weapon
    //Also works for repairing
    public List<Tile> findAllHealables(Unit unit , List<Weapon> weapons)
    {
        string side = unit.getSide();
        int team = unit.getTeam();
        List<Tile> attackable = new List<Tile>();
        //Weapon weapon = unit.getCurrentWeapon();
        foreach (Weapon weapon in weapons)
        {
            //Skip to the next weapon if this can't heal or repair
            if (!(weapon.heals || weapon.repairs)) continue;
            //Debug.Log("Finding Healables");
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            SimpleNode<Tile, float> temp, temp2;
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;
                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (current.Priority >= minRange && current.Priority <= maxRange)
                    {
                        //Treat mustTargetEnemies as mustTargetAllies with regards to healing and repairing
                        if (weapon.mustTargetEnemies)
                        {
                            if (currentTile.getUnit() != null && currentTile.getUnitScript().getTeam() == team)
                                //Check to make sure we can actually heal the unit
                                if ((currentTile.getUnitScript().biological && weapon.heals) || (currentTile.getUnitScript().mechanical && weapon.repairs))
                                {
                                    attackable.Add(current.Data);
                                }
                        }
                        else
                        {
                            if (currentTile.getUnit() != null)
                            {
                                //Prevent Healing Enemies
                                if (currentTile.getUnitScript().getTeam() == team)
                                {
                                    attackable.Add(current.Data);
                                }
                            }
                            else
                            {
                                attackable.Add(current.Data);
                            }
                        }
                    }
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }



                }
            }
        }
        if (attackable == null || attackable.Count == 0) return new List<Tile>();
        return attackable;
    }

    //Find all ally units that can be healed/repaired within range
    public List<Tile> findAllAllyUnitsHealable(List<Weapon> weapons)
    {
        Unit unitScript = start.getUnitScript();
        string side = start.getUnitSide();



        //Extra calculator for getting attackble tiles
        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, null, null);
        DijakstraCalculator pathCalc = new DijakstraCalculator(gM, null, null);
        List<Tile> attackable = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;

        foreach (Weapon weapon in weapons)
        {
            double minRange = weapon.minRange;
            double maxRange = weapon.maxRange;
            double unitMP = unitScript.getCurrentMP();
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;
                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);

                    if (current.Priority <= unitMP)
                    {
                        //Make sure we can actually get to the tile itself
                        pathCalc.reset();
                        pathCalc.setValues(start, currentTile);

                        if (pathCalc.findPath(true, false) != null)
                        {
                            extraCalc.reset();
                            extraCalc.setStart(currentTile);
                            //Only add the tile if we can attack the enemy unit
                            //if (currentTile.getUnit() != null && currentTile.getUnitScript().getSide() != side && currentTile.unitHP > currentTile.expectedDamage)
                            //Debug.Log(currentTile + " is the tile we are measuring from");
                            attackable.AddRange(extraCalc.findAllHealables(unitScript, weapons));
                            //printPath(attackable);
                        }
                    }
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + t.getMoveCost(unitScript);
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + t.getMoveCost(unitScript));
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }



                }
            }
        }
        if (attackable == null || attackable.Count == 0) return null;
        return new HashSet<Tile>(attackable).ToList<Tile>();
    }

    public List<Tile> findAllAllyUnitsHealableWithWeapons(List<Weapon> weapons)
    {
        if (start.getUnit() == null) return null;
        List<Tile> attackableTiles = findAllTilesHealableWithWeapons(weapons);
        int team = start.getUnitScript().getTeam();
        attackableTiles.RemoveAll(item => item.getUnit() == null || item.getUnitScript().getTeam() != team);
        return attackableTiles;
    }

    public List<Tile> findAllTilesHealableWithWeapons(List<Weapon> weapons)    {
        if (start.getUnit() == null) return null;
        Unit unitScript = start.getUnitScript();
        string side = start.getUnitSide();

        double minRange = unitScript.getCurrentWeapon().minRange;
        ////double maxRange = unitScript.getCurrentWeapon().maxRange;
        double unitMP = unitScript.getCurrentMP();

        //Extra calculator for getting attackble tiles
        DijakstraCalculator extraCalc = new DijakstraCalculator(gM, null, null);
        DijakstraCalculator pathCalc = new DijakstraCalculator(gM, null, null);
        List<Tile> attackable = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;


        frontier.Enqueue(start, 0);
        while (frontier.Count != 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;
            currentTile.setAdjacent();
            exploredT.Add(current.Data);

            if (current.Priority <= unitMP)
            {
                //Make sure we can actually get to the tile itself
                pathCalc.reset();
                pathCalc.setValues(start, currentTile);

                if (pathCalc.findPath(true, false) != null)
                {
                    extraCalc.reset();
                    extraCalc.setStart(currentTile);
                    //Only add the tile if we can attack the enemy unit
                    //if (currentTile.getUnit() != null && currentTile.getUnitScript().getSide() != side && currentTile.unitHP > currentTile.expectedDamage)
                    //Debug.Log(currentTile + " is the tile we are measuring from");
                    attackable.AddRange(extraCalc.findAllHealables(unitScript, weapons));
                    //printPath(attackable);
                }
            }
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                temp.Priority = current.Priority + t.getMoveCost(unitScript);
                if (temp.Priority > unitMP) continue;
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, current.Priority + t.getMoveCost(unitScript));
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }



        }



        return new HashSet<Tile>(attackable).ToList<Tile>();
    }


    //Determine if enemy unit can be attacked by the current unit with any of the primary/secondary/tertiary weapons
    public bool canHealRepairAlly()
    {
        List<Weapon> weapons = start.getUnitScript().getAllHandWeapons();
        return canHealRepairAllyWithWeapons(weapons);
    }

    //Determines if we can heal an ally with a weapon from the current tile
    public bool canHealRepairAllyWithWeapon(Weapon weapon)
    {
        return canHealRepairAllyWithWeapons(new List<Weapon>() { weapon });
    }

    public bool canHealRepairAllyWithWeapons(List<Weapon> weapons)
    {
        //Continue to next weapon if the weapon can't attack
        foreach (Weapon weapon in weapons)
        {
            if (!weapon.doesSomething || !(weapon.heals || weapon.repairs)) continue;

            string side = start.getUnitSide();
            List<Tile> attackable = new List<Tile>();
            int minRange = weapon.minRange;
            int maxRange = weapon.maxRange;
            SimpleNode<Tile, float> temp, temp2;
            //max range should never be less than min range
            if (maxRange < minRange)
            {
                continue;
            }
            else if (minRange == 0 && maxRange == 0)
            {
                //attackable.Add(start);
            }
            else
            {
                frontier.Enqueue(start, 0);
                while (frontier.Count != 0)
                {
                    current = frontier.DequeueNode();
                    Tile currentTile = current.Data;

                    currentTile.setAdjacent();
                    exploredT.Add(current.Data);
                    if (current.Priority >= minRange && current.Priority <= maxRange)
                    {
                        //if (currentTile.getUnit() != null && currentTile.getUnitScript().getSide() != side)
                        //attackable.Add(current.Data);

                        if (currentTile == dest)
                        {
                            return true;
                        }
                    }
                    else if (current.Priority >= maxRange) continue;
                    List<Tile> adjacentNodes = currentTile.getAdjacent();
                    foreach (Tile t in adjacentNodes)
                    {
                        if (t == null) continue;
                        temp = new SimpleNode<Tile, float>(t);
                        temp.Priority = current.Priority + 1;
                        if (!exploredT.Contains(t) && !frontier.Contains(t))
                        {
                            //Add the node to the frontier if we didn't explore it already
                            frontier.Enqueue(t, current.Priority + 1);
                        }
                        //If we have a move path that is shorter than what is in the frontier, replace it
                        else if (frontier.Contains(t))
                        {
                            //Debug.Log(t);
                            temp2 = frontier.RemoveNode(t);
                            if (temp.Priority < temp2.Priority)
                            {
                                frontier.Enqueue(temp.Data, temp.Priority);
                            }
                            else
                            {
                                frontier.Enqueue(temp2.Data, temp2.Priority);
                            }

                        }
                    }



                }
            }
        }
        //Debug.Log("We can't heal this ally!");
        return false;
    }


    //Gets a list of all tiles that can be moved to regardless if they have a unit on them
    public List<Tile> getTilesInMoveRange()
    {
        if (start.getUnit() == null) return null;
        Unit unit = start.getUnitScript();
        List<Tile> moveables = new List<Tile>();
        SimpleNode<Tile, float> temp, temp2;
        float maxRange = unit.getMP();
        frontier.Enqueue(start, 0);
        while (frontier.Count > 0)
        {
            current = frontier.DequeueNode();
            Tile currentTile = current.Data;

            currentTile.setAdjacent();
            exploredT.Add(current.Data);
            moveables.Add(currentTile);
            List<Tile> adjacentNodes = currentTile.getAdjacent();
            foreach (Tile t in adjacentNodes)
            {
                if (t == null) continue;
                temp = new SimpleNode<Tile, float>(t);
                
                float p = current.Priority + currentTile.getMoveCost(unit, temp.Data);
                temp.Priority = p;
                if (temp.Priority > maxRange) continue;
                if (!exploredT.Contains(t) && !frontier.Contains(t))
                {
                    //Add the node to the frontier if we didn't explore it already
                    frontier.Enqueue(t, p);
                }
                //If we have a move path that is shorter than what is in the frontier, replace it
                else if (frontier.Contains(t))
                {
                    //Debug.Log(t);
                    temp2 = frontier.RemoveNode(t);
                    if (temp.Priority < temp2.Priority)
                    {
                        frontier.Enqueue(temp.Data, temp.Priority);
                    }
                    else
                    {
                        frontier.Enqueue(temp2.Data, temp2.Priority);
                    }

                }
            }
        }
        if (moveables.Count == 0) return null;
        return moveables;
    }

    //Gets all buildings in the move range
    public List<Tile> getBuildingsInMR()
    {
        List<Tile> moveables = getTilesInMoveRange();
        if (moveables == null) return null;
        //Remove all tiles that don't have a building
        moveables.RemoveAll(item => item.getBuilding() == null);
        return moveables;
    }

    public List<Tile> getAllyBuildingsInMR()
    {
        List<Tile> buildings = getBuildingsInMR();
        if (buildings == null) return null;
        Unit unit = start.getUnitScript();
        int team = unit.getTeam();
        //Remove all tiles that don't have a building
        buildings.RemoveAll(item => item.getBuilding().team != team);
        return buildings;
    }

    public List<Tile> getNeutralBuildingsInMR()
    {
        List<Tile> buildings = getBuildingsInMR();
        if (buildings == null) return null;
        //Remove all tiles that don't have a building
        buildings.RemoveAll(item => item.getBuilding().team >= 0);
        return buildings;
    }

    public List<Tile> getEnemyBuildingsInMR()
    {
        List<Tile> buildings = getBuildingsInMR();
        if (buildings == null) return null;
        Unit unit = start.getUnitScript();
        int team = unit.getTeam();
        //Remove all tiles that don't have a building
        buildings.RemoveAll(item => item.getBuilding().team == team || item.getBuilding().team < 0);
        return buildings;
    }

    public List<Tile> getCapturableBuildingsInMR()
    {
        List<Tile> buildings = getBuildingsInMR();
        if (buildings == null) return null;
        Unit unit = start.getUnitScript();
        int team = unit.getTeam();
        //Remove all tiles that don't have a building
        buildings.RemoveAll(item => item.getBuilding().team == team);
        return buildings;
    }



    public void reset()
    {
        frontier = new SimplePriorityQueue<Tile>();
        exploredT = new HashSet<Tile>();
    }

    public void setStart(Tile s)
    {
        start = s;
    }

    public void setDest(Tile d)
    {
        dest = d;
    }

    public void setValues(Tile s, Tile d)
    {
        start = s;
        dest = d;
    }

    public Tile getStart()
    {
        return start;
    }

    public Tile getDest()
    {
        return dest;
    }

    public void printPath(List<Tile> path)
    {
        String toPrint = "Printing a path: \n";
        if (path == null)
        {
            Debug.Log("There are no tiles in the path!");
            return;
        }
        foreach (Tile tile in path)
        {
            //Unit unit = tile.getUnitScript();
            toPrint += tile + ",";
        }
        toPrint = toPrint.Substring(0, toPrint.Length - 1);
        toPrint += "\n";
        Debug.Log(toPrint);
    }
}
