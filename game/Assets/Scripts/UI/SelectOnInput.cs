using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectOnInput : MonoBehaviour {
    public EventSystem eventSystem;
    public GameObject selectedObject;

    private bool buttonSelected;
   
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetAxisRaw("Vertical") != 0 && !buttonSelected)
        {
            eventSystem.SetSelectedGameObject(selectedObject);
            buttonSelected = true;
        }
	}

    void OnDisabled()
    {
        buttonSelected = false;
    }
}
