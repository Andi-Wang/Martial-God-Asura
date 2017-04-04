using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class SkillButtonMouseover : MonoBehaviour {
    public Text descriptionScript;
    private string description;
    private Text descriptionPanelScript;

	// Use this for initialization
	void Awake () {
        //deactivates the text object
        description = descriptionScript.text;
        descriptionScript.enabled = false;

        descriptionPanelScript = GameObject.Find("SkillTree").transform.GetChild(0).GetChild(0).GetChild(0).GetComponentInChildren<Text>();
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetDesc() {
        descriptionPanelScript.text = description;
        Debug.Log(descriptionPanelScript.text);
    }

    public void ResetDesc() {
        descriptionPanelScript.text = "";
    }
}
