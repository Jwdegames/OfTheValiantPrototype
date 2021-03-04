using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class defines Master Effects, effects that give off other effects, while this effect is active
 * 
 */
public class MasterEffect
{
    public string name;
    //The vector 4 are the subeffect properties
    //x is the effect duration
    //y is if the effect can override (0 if false, 1 if true)
    public Dictionary<string, Vector4> subEffects = new Dictionary<string, Vector4>();
 
    public MasterEffect()
    {

    }

    public MasterEffect(string n, Dictionary<string, Vector4> sE)
    {
        name = n;
        subEffects = sE;
    }
}

class MasterEffectList
{
    public List<MasterEffect> masterEffects = new List<MasterEffect>();

    public MasterEffectList()
    {
        Dictionary<string, Vector4> temp = new Dictionary<string, Vector4>();
        temp.Add("StrengthE-1", new Vector4(3, 1, 0, 0));
        temp.Add("MovementE-3", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Rallied", temp));
        masterEffects.Add(new MasterEffect("Bolstered", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("StrengthE-3", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Focused", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("MovementE-5", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Pressured", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("SlownessE-1", new Vector4(3, 1, 0, 0));
        temp.Add("DefenseE-1", new Vector4(3, 1, 0, 0));
        temp.Add("CautionE-1", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Cautioned", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("StrengthE-1", new Vector4(3, 1, 0, 0));
        temp.Add("MovementE-3", new Vector4(3, 1, 0, 0));
        temp.Add("DefenseE-1", new Vector4(3, 1, 0, 0));
        temp.Add("CautionE-1", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Inspired", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("StrengthE-1", new Vector4(3, 1, 0, 0));
        temp.Add("FragilityE-2", new Vector4(3, 1, 0, 0));
        temp.Add("ImprudenceE-2", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Angered", temp));

        temp = new Dictionary<string, Vector4>();
        temp.Add("WeaknessE-2", new Vector4(3, 1, 0, 0));
        temp.Add("SlownessE-2", new Vector4(3, 1, 0, 0));
        masterEffects.Add(new MasterEffect("Debuffed", temp));
    }

    public MasterEffect findMasterEffect(string n)
    {
        foreach (MasterEffect masterEffect in masterEffects)
        {
            if (masterEffect.name.Equals(n))
            {
                return masterEffect;
            }
        }
        return null;
    }
}
