using UnityEngine;
using System.Collections;

public class NPCInteract : MonoBehaviour {
    ToggleHintUI hintUIController;
    Canvas dialogBox;
    bool showing;
    Vector3 origPos;

    // Use this for initialization
    void Awake () {
        hintUIController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();

        showing = false;
        dialogBox = GetComponentInChildren<Canvas>();
        origPos = dialogBox.transform.position;
        hintUIController.toggleDialog(dialogBox);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!showing && other.tag == "Player")
        {
            hintUIController.toggleDialog(dialogBox, origPos.x, origPos.y);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hintUIController.toggleDialog(dialogBox);
    }
    
}
