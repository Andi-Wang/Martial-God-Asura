using UnityEngine;
using System.Collections;

public class PickupTrigger : MonoBehaviour {
    GameObject hintCanvas;
    private bool entered;
    ToggleHintUI hintUIController;

    void Awake()
    {
        hintUIController = GameObject.Find("HintCanvas").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();

        entered = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonUp("Interact") && entered == true)
        {
            hintUIController.toggleHint();

            // we can go to next level now
            GameManager.instance.SubLevelComplete = true;

            //TODO: update repository
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            entered = true;
            hintUIController.toggleHint(transform.position.x, transform.position.y+3);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
        hintUIController.toggleHint();
    }
    
}
