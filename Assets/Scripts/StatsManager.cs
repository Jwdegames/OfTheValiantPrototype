using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class that contains a variety of stats that other classes should use
public static class StatsManager
{
    public static List<string> offensiveCommands = new List<string>() {"Anger", "Debuff", "Confuse", "Intimidate"};
    public static List<string> boostCommands = new List<string>() {"Rally", "Bolster", "Focus", "Reset Ally", "Pressure", "Caution", "Inspire", "Energize"};
    public static Dictionary<string, string> commandEffectDictionary = new Dictionary<string, string>() { { "Rally", "Rallied" }, {"Bolster", "Bolstered" }, { "Focus", "Focused"}, {"Pressure", "Pressured" },
        {"Caution", "Cautioned" }, {"Inspire", "Inspired"}, {"Anger", "Angered"}, {"Debuff", "Debuffed" }, {"Confuse", "Confused" }, {"Intimidate", "Intimidated" } };
    public static List<string> basicAICommands = addStringLists(getStringSubList(offensiveCommands, 0, 1),addStringLists(getStringSubList(boostCommands,1,2),(getStringSubList(boostCommands,4,7))));
    //public static List<string> basicAICommands = getStringSubList(boostCommands, 5, 5);
    public static List<string> executeCommandsLast = addStringLists(getStringSubList(boostCommands, 1, 1),getStringSubList(boostCommands,7,7));
    public static List<string> getStringSubList(List<string> strings, int first, int last)
    {
        List<string> subList = new List<string>();
        for (int i = first; i <= last; i++)
        {
            subList.Add(strings[i]);
        }
        return subList;
    }

    public static List<string> addStringLists(List<string> strings1, List<string> strings2) 
    {
        List<string> combinedList = new List<string>();
        combinedList.AddRange(strings1);
        combinedList.AddRange(strings2);
        return combinedList;
    }

    public static string getRandomString(List<string> strings)
    {
        return strings[Random.Range(0, strings.Count)];
    }
}
