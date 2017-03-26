using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PickupTrigger : MonoBehaviour {
    GameObject hintCanvas;
    private bool entered;
    bool pickedup = false;
    ToggleHintUI hintUIController;
    public SkillTreeUI skillTreeUI;
    public int categoryId, buttonId;

    void Awake()
    {
        hintUIController = GameObject.Find("HintCanvas").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();

        entered = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonUp("Interact") && entered && !pickedup)
        {
            pickedup = true;

            hintUIController.toggleHint();

            GameManager.instance.playersTurn = false;

            // let the player know he/she got a skill
            GameManager.instance.displayNotification("You got a skill scroll!");

            Invoke("ActivateSkill", 0.8f);

            // we can go to next level now
            GameManager.instance.SubLevelComplete = true;

            //TODO: update repository
            gameObject.SetActive(false);

            GameManager.instance.playersTurn = true;
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
    
    void ActivateSkill()
    {
        GameManager.instance.ToggleSkillMenu();

        skillTreeUI.ActivateCategory(categoryId);

        Button skillbutton = skillTreeUI.GetButton(buttonId);
        if (skillbutton == null)
            Debug.Log("button is null");
        else
            skillbutton.onClick.Invoke();
    }
}
