using UnityEngine;
using System.Collections;

public class SkillTree {
    const int NUM_SKILL_TREE_BRANCHES = 9;
    const int NUM_SKILL_TREE_TIERS = 3;
    const int SKILLS_PER_TIER = 3;

    bool[,,] skills;

    public SkillTree() {
        skills = new bool[NUM_SKILL_TREE_BRANCHES, NUM_SKILL_TREE_TIERS, SKILLS_PER_TIER];
    }

    public SkillTree(bool[,,] input) {
        skills = input;
    }

    public void setSkillTrue(int branch, int tier, int skill) {
        skills[branch, tier, skill] = true;
    }
    public void setSkillFalse(int branch, int tier, int skill) {
        skills[branch, tier, skill] = false;
    }
    public bool getSkill(int branch, int tier, int skill) {
        return skills[branch, tier, skill];
    }
}
