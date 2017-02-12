using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public GameObject[] tabContents;
	
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

    public void ToggleMenu(bool activate)
    {
        gameObject.SetActive(activate);
    }
}
