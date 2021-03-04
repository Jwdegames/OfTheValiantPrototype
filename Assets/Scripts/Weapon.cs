using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int minRange, maxRange, aoe, aoeType, roundsFired, maxAttacksPerTurn = 1, currentAttacks = 0;
    public float damage, healRepairAmountBefore, healRepairPercent, healRepairAmountAfter, xScale = 1, yScale = 1;
    public string weaponType, name, description, quote;
    public bool canTargetAir, damages, heals, repairs, canTargetSub, mustTargetEnemies,  doesSomething;
    public Sprite uiSprite;
    public Dictionary<string, float> extraAttributes;
    //For unit
    public bool isPrimary, isSecondary, isTertiary, isTurret;

    public Weapon(string name, string description, string quote, int miR, int maR, int a, int aT, int rF, float d, float hRAB, float hRP, float hRAA, string wT, bool cTA, bool dmgs, bool h, bool r, bool cTS, bool mTE, bool dS, Dictionary<string,float> eA)
    {
        this.name = name;
        this.description = description;
        this.quote = quote;
        minRange = miR;
        maxRange = maR;
        aoe = a;
        aoeType = aT;
        roundsFired = rF;
        damage = d;
        healRepairAmountBefore = hRAB;
        healRepairPercent = hRP;
        healRepairAmountAfter = hRAA;
        weaponType = wT;
        canTargetAir = cTA;
        canTargetSub = cTS;
        heals = h;
        //if (name == "Medkit")
        //Debug.Log(h);
        repairs = r;
        damages = dmgs;
        mustTargetEnemies = mTE;
        doesSomething = dS;
        if (heals || repairs || damages) doesSomething = true;
        extraAttributes = eA;


    }

    public Weapon(string name, string description, string quote, int miR, int maR, int a, int aT, int rF, int mAPT, int cA, float d, float hRAB, float hRP, float hRAA, string wT, bool cTA, bool dmgs,  bool h, bool r, bool cTS, bool mTE, bool dS, Dictionary<string, float> eA)
    {
        this.name = name;
        this.description = description;
        this.quote = quote;

        minRange = miR;
        maxRange = maR;
        aoe = a;
        aoeType = aT;
        roundsFired = rF;
        //Debug.Log(mAPT);
        maxAttacksPerTurn = mAPT;
        currentAttacks = cA;
        damage = d;
        healRepairAmountBefore = hRAB;
        healRepairPercent = hRP;
        healRepairAmountAfter = hRAA;
        weaponType = wT;
        canTargetAir = cTA;
        canTargetSub = cTS;
        damages = dmgs;
        heals = h;
        //if (name == "Medkit")
        //Debug.Log("MAPT: "+maxAttacksPerTurn);
        repairs = r;
        
        mustTargetEnemies = mTE;
        doesSomething = dS;
        if (heals || repairs || damages) doesSomething = true;
        extraAttributes = eA;


    }

    public float getDamagePerSalvo()
    {
        return (float)Math.Round(damage * roundsFired,3);
    }

    //Applies attributes
    public float getActualDamagePerAttack(Unit attacker, Unit defender)
    {
        //Return 0 if we can't actually attack
        if (!attacker.gM.canAttackEnemyWithWeapon(attacker, attacker.getTile(),defender, defender.getTile(),this))
        {
            return 0;
        }
        //Debug.Log(attacker.name + " with attack type " + weaponType);
        //Debug.Log(defender.name + " with defense type " + defender.getArmor());
        return getHypotheticalDamagePerAttack(attacker, defender);
    }

    public float getHypotheticalDamagePerAttack(Unit attacker, Unit defender)
    {

        //Debug.Log(attacker.name + " with attack type " + weaponType);
        //Debug.Log(defender.name + " with defense type " + defender.getArmor());
        float multiplier = 1f;
        if (weaponType == "Light" || weaponType == "Heavy")
        {
            if (weaponType == defender.getArmor()) multiplier = 2f;
            else if (defender.getArmor() != "Medium") multiplier = 0.5f;
        }
        if (defender.getArmor() == "Slime") multiplier = 0.8f;
        /* Debug.Log(multiplier);
         Debug.Log(damage);
         Debug.Log(roundsFired);
         Debug.Log(damage * roundsFired);
         Debug.Log("Non-HP Ratio Dmg:" + (damage * roundsFired * multiplier));*/
        float baseDamage = ((damage * roundsFired * multiplier * attacker.getHPRatio() * (1 + attacker.attackBonus.x / 100.0f)) + attacker.attackBonus.y) * (1 + attacker.attackBonus.z / 100.0f);
        //Debug.Log("Base damage is " + baseDamage);
        //Reduce the amount of damage due to the guard bonus
        if (defender.getGuard())
        {
            baseDamage = (baseDamage * (1 - defender.guardCover) * (1- defender.cautionBonus.x / 100.0f) - defender.cautionBonus.y) * (1 - defender.cautionBonus.z / 100.0f);
        }
        baseDamage = (baseDamage * (1 - defender.defenseBonus.x / 100.0f) - defender.defenseBonus.y) * (1 - defender.defenseBonus.z/100.0f); 
        //Debug.Log(baseDamage);
        if (extraAttributes != null)
        {
            if (extraAttributes.ContainsKey("Anti-Light Multi-Bonus") && defender.getArmor() == "Light")
            {
                baseDamage *= extraAttributes["Anti-Light Multi-Bonus"];
            }
            if (extraAttributes.ContainsKey("Anti-Medium Multi-Bonus") && defender.getArmor() == "Medium")
            {
                baseDamage *= extraAttributes["Anti-Medium Multi-Bonus"];
            }
            if (extraAttributes.ContainsKey("Anti-Heavy Multi-Bonus") && defender.getArmor() == "Heavy")
            {
                baseDamage *= extraAttributes["Anti-Heavy Multi-Bonus"];
            }
            if (extraAttributes.ContainsKey("Anti-Slime Multi-Bonus") && defender.getArmor() == "Slime")
            {
                baseDamage *= extraAttributes["Anti-Slime Multi-Bonus"];
            }
        }
        return (float)Math.Round(baseDamage, 3);
    } 

    public float getActualDamagePerAttackWithGuard(Unit attacker, Unit defender, bool guard)
    {
        bool lastState = defender.getGuard();
        defender.setGuard(guard);
        float dmg = getActualDamagePerAttack(attacker, defender);
        defender.setGuard(lastState);
        return dmg;
    }

    public float getActualHPPerHeal(Unit healer, Unit healee)
    {
        //Return 0 if we can't actually heal
        //Debug.Log("Trying to heal!");
        if (!healer.getTile().gM.canHealAllyWithWeapon(healer.getTile(), healee.getTile(), this))
        {
            //Debug.Log("We can't heal!");
            return 0;
        }
        //Make sure the we can repair mechanical/heal biological, otherwise return 0
        if (!((healee.mechanical && repairs) || (healee.biological && heals)))
        {
            //Debug.Log("Non-matching heal type!");
            return 0;
        }
        float healed = healRepairAmountBefore;
        healed += healee.getHP()*healRepairPercent;
        healed += healRepairAmountAfter;
        //Debug.Log(healed);
        return (float)Math.Round(healed,3);
    }

    public Weapon copy()
    {
        return new Weapon(name, description, quote, minRange, maxRange, aoe, aoeType, roundsFired, maxAttacksPerTurn, currentAttacks, damage, healRepairAmountBefore, healRepairPercent, healRepairAmountAfter,
            weaponType, canTargetAir, damages, heals, repairs, canTargetSub, mustTargetEnemies, doesSomething, extraAttributes);
    }

    override
    public string ToString()
    {
        return name + " that has used " + currentAttacks + "/" + maxAttacksPerTurn + " attacks";
    }
}
