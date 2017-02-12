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
	public Texture bgtxt;
	public GameObject bgpanel;
	public Sprite bg1;
	public Sprite bg2;
	public Sprite bg3;

	public Text pgtitle;

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

	RectTransform maRT;
	RectTransform cuRT;
	RectTransform spRT;

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

		maRT = martialArtsToggle.GetComponent<RectTransform>();
		cuRT = cultivationToggle.GetComponent<RectTransform>();
		spRT = spiritualToggle.GetComponent<RectTransform>();
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
			bgpanel.GetComponent<Image> ().sprite = bg1;
			if ((Vector2)maRT.localPosition != new Vector2 (maRT.localPosition.x, -40)) {
				maRT.localPosition = new Vector2 (maRT.localPosition.x, -40);
			}
			if ((Vector2)cuRT.localPosition != new Vector2 (cuRT.localPosition.x, -20)) {
				cuRT.localPosition = new Vector2 (cuRT.localPosition.x, -20);
			}
			if ((Vector2)spRT.localPosition != new Vector2 (spRT.localPosition.x, -22)) {
				spRT.localPosition = new Vector2 (spRT.localPosition.x, -22);
			}
			maRT.sizeDelta = new Vector2 (108, 125);
			maRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 120);
			cuRT.sizeDelta = new Vector2 (108, 62);
			cuRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			spRT.sizeDelta = new Vector2 (108, 62);
			spRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			pgtitle.text = "Martial Arts";
            //background.color = martialArtsToggle.GetComponentInChildren<Image>().color;
        }
        else if(cultivationToggle.isOn) {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(true);
            spiritualTree.SetActive(false);
			bgpanel.GetComponent<Image> ().sprite = bg2;
			if ((Vector2)maRT.localPosition != new Vector2 (maRT.localPosition.x, -20)) {
				maRT.localPosition = new Vector2 (maRT.localPosition.x, -20);
			}
			if ((Vector2)cuRT.localPosition != new Vector2 (cuRT.localPosition.x, -40)) {
				cuRT.localPosition = new Vector2 (cuRT.localPosition.x, -40);
			}
			if ((Vector2)spRT.localPosition != new Vector2 (spRT.localPosition.x, -22)) {
				spRT.localPosition = new Vector2 (spRT.localPosition.x, -22);
			}
			maRT.sizeDelta = new Vector2 (108, 62);
			maRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			cuRT.sizeDelta = new Vector2 (108, 125);
			cuRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 120);
			spRT.sizeDelta = new Vector2 (108, 62);
			spRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			pgtitle.text = "Cultivation";
			//maToggle.transform;
            //background.color = cultivationToggle.GetComponentInChildren<Image>().color;
        }
        else if(spiritualToggle.isOn) {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(false);
            spiritualTree.SetActive(true);
			bgpanel.GetComponent<Image> ().sprite = bg3;
			if ((Vector2)maRT.localPosition != new Vector2 (maRT.localPosition.x, -20)) {
				maRT.localPosition = new Vector2 (maRT.localPosition.x, -20);
			}
			if ((Vector2)cuRT.localPosition != new Vector2 (cuRT.localPosition.x, -20)) {
				cuRT.localPosition = new Vector2 (cuRT.localPosition.x, -20);
			}
			if ((Vector2)spRT.localPosition != new Vector2 (spRT.localPosition.x, -40)) {
				spRT.localPosition = new Vector2 (spRT.localPosition.x, -40);
			}
			maRT.sizeDelta = new Vector2 (108, 62);
			maRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			cuRT.sizeDelta = new Vector2 (108, 62);
			cuRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 62);
			spRT.sizeDelta = new Vector2 (108, 125);
			spRT.GetChild (0).GetComponent<RectTransform> ().sizeDelta = new Vector2(104, 120);
			pgtitle.text = "\t\tSpirit";
            //background.color = spiritualToggle.GetComponentInChildren<Image>().color;
        }
        else {
            martialArtsTree.SetActive(false);
            cultivationTree.SetActive(false);
            spiritualTree.SetActive(false);
            //background.color = Color.white;
        }
	}

	private float originalWidth = 640.0f;  // define here the original resolution
	private float originalHeight = 400.0f; // you used to create the GUI contents 
	private Vector3 scale;

	/*void OnGUI(){
		scale.x = Screen.width/originalWidth; // calculate hor scale
		scale.y = Screen.height/originalHeight; // calculate vert scale
		scale.z = 1;
		var svMat = GUI.matrix; // save current matrix
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		// draw your GUI controls here:
		//GUI.Box(new Rect(10,10,600,400), bgtxt);
		GUI.DrawTexture(new Rect(10, 10, 600, 400), bgtxt, ScaleMode.ScaleToFit, true, 0.0F);
		//GUI.Button(new Rect(300,100,50,50), "Button");
		//...
		// restore matrix before returning
		GUI.matrix = svMat; // restore matrix
	}*/
}
