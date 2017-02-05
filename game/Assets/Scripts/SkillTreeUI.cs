using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillTreeUI : MonoBehaviour {
    const int NUM_SKILL_TREE_BRANCHES = 9;
    const int NUM_SKILL_TREE_TIERS = 3;
    const int SKILLS_PER_TIER = 3;
    
    public Toggle martialArtsToggle;
    public Toggle cultivationToggle;
    public Toggle spiritualToggle;

    public GameObject martialArtsTree;
    public GameObject cultivationTree;
    public GameObject spiritualTree;

    public Image background;

    public Button[] buttons = new Button[NUM_SKILL_TREE_BRANCHES * NUM_SKILL_TREE_TIERS * SKILLS_PER_TIER];

    const int OFFSPE_BRANCH = 0;
    const int OFFDEF_BRANCH = 1;
    const int DEFSPE_BRANCH = 2;
    const int FIRE_BRANCH = 3;
    const int WATERICE_BRANCH = 4;
    const int STORM_BRANCH = 5;
    const int ENHANCEMENT_BRANCH = 6;
    const int FORMATION_BRANCH = 7;
    const int SUMMONING_BRANCH = 8;

    SkillTree tree;

    void clickListener(int i) {
        int branch = i / NUM_SKILL_TREE_BRANCHES;
        int tier = (i - branch * NUM_SKILL_TREE_BRANCHES) / NUM_SKILL_TREE_TIERS;
        int skill = i % SKILLS_PER_TIER;

        //Right now, no limitations on how you take skills; can add prerequisites here later
        //Currently, clicking toggles having the skill vs not having the skill; if the player has the skill, the button's text changes
        if(tree.getSkill(branch, tier, skill)) {
            tree.setSkillFalse(branch, tier, skill);
            buttons[i].GetComponentInChildren<Text>().text = buttons[i].GetComponentInChildren<Text>().text.Substring(0, buttons[i].GetComponentInChildren<Text>().text.Length - 7);
        }
        else {
            tree.setSkillTrue(branch, tier, skill);
            buttons[i].GetComponentInChildren<Text>().text += " (have)";
        }
    }


    // Use this for initialization
    void Start () {
        // normally, populate the array with previously-saved character skill data; still need to save it, too
        tree = new SkillTree();
        for(int i = 0; i < buttons.Length; i++) {
            int temp = i;
            buttons[temp].onClick.AddListener(() => clickListener(temp));
        }


        //If we want to set color in the script
        /*
        martialArtsToggle.GetComponentInChildren<Image>().color = Color.red;
        cultivationToggle.GetComponentInChildren<Image>().color = Color.blue;
        spiritualToggle.GetComponentInChildren<Image>().color = Color.green;
        */
    }
	
	// Update is called once per frame
	void Update () {
	    if(martialArtsToggle.isOn) {
            martialArtsTree.SetActive(true);
            cultivationTree.SetActive(false);
            spiritualTree.SetActive(false);
            background.color = martialArtsToggle.GetComponentInChildren<Image>().color;
        }
        else if(cultivationToggle.isOn) {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(true);
            spiritualTree.SetActive(false);
            background.color = cultivationToggle.GetComponentInChildren<Image>().color;
        }
        else if(spiritualToggle.isOn) {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(false);
            spiritualTree.SetActive(true);
            background.color = spiritualToggle.GetComponentInChildren<Image>().color;
        }
        else {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(false);
            spiritualTree.SetActive(false);
            background.color = Color.white;
        }
	}
}
