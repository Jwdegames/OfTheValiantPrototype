using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityList
{
    public static List<AbilityStats> presetAbilities = new List<AbilityStats>();

    public static void initDefaultAbilites()
    {
        presetAbilities = new List<AbilityStats>();

        //Index 0
        presetAbilities.Add(new AbilityStats("Rally", "Rally", "Increase the movement speed and damage output of all friendly units for 3 turns. Must cooldown for 5 turns.", 3, 1, new List<Vector4> {new Vector4(5,1,0,0)}));
    }
}
