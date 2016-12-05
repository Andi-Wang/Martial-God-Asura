using UnityEngine;
using System.Collections;

public class PickupTrigger : MonoBehaviour {
    GameObject hintCanvas;
    private bool entered;
    Vector3 hidePosition = new Vector3(0, -999);
    Vector3 appearPosition;

    void Awake()
    {
        hintCanvas = GameObject.Find("HintCanvas");

        appearPosition = new Vector3(0, 3);

        entered = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonUp("Interact") && entered == true)
        {
            //TODO: update repository
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            entered = true;
            toggleInteractHint(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
        toggleInteractHint(false);
    }

    void toggleInteractHint(bool status)
    {
        if (status)
        {
            hintCanvas.transform.position = transform.position + appearPosition;
        }
        else
        {
            hintCanvas.transform.position = hidePosition;
        }
    }
}
