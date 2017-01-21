using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Ability
{
    public int level;
    public bool unlocked;
    public Ability(int lv, bool u)
    {
        level = lv;
        unlocked = u;
    }
}

public class Skills
{
    private Dictionary<string, Ability> abilities;

    public Dictionary<string, Ability> Abilities
    {
        get {  return abilities;}
    }

    public void AddSkill(string name, int level, bool unlocked)
    {
        abilities.Add(name, new Ability(level, unlocked));
    }

    public bool isUnlocked(string skillName)
    {
        if (abilities.ContainsKey(skillName))
        {
            return abilities[skillName].unlocked;
        }
        else
        {
            Debug.LogError("Cannot find skill name:" + skillName);
            return false;
        }
    }

    public List<string> getSkillNames()
    {
        List<string> skillNameOutput = new List<string>();
        Dictionary<string, Ability>.KeyCollection keyColl = abilities.Keys;
        foreach(string s in keyColl)
        {
            skillNameOutput.Add(s);
        }
        return skillNameOutput;
    }
}