using UnityEngine;
using System.Collections;

public class NPCInteract : MonoBehaviour {

    Canvas dialogBox;
    bool showing;
    Vector3 origPos;
    Vector3 hidePosition = new Vector3(0, -999);

    // Use this for initialization
    void Awake () {
        showing = false;
        dialogBox = GetComponentInChildren<Canvas>();
        origPos = dialogBox.transform.position;
        toggleInteractHint(false);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!showing && other.tag == "Player")
        {
            toggleInteractHint(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        toggleInteractHint(false);
        
    }

    void toggleInteractHint(bool status)
    {
        if (status)
        {
            dialogBox.transform.position = origPos;
        }
        else
        {
            dialogBox.transform.position = hidePosition;
        }
        showing = status;
    }
}
