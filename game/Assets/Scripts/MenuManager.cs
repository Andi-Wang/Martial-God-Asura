using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject[] tabContents;

	// Use this for initialization
	void Start () {
	
	}
	
	public void SwitchTab(int tabId)
    {
        foreach(GameObject tabContent in tabContents)
        {
            if (tabContent.activeSelf)
            {
                tabContent.SetActive(false);
            }
        }

        tabContents[tabId].SetActive(true);
    }
}
