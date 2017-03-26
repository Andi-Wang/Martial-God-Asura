using UnityEngine;
using System.Collections;
using System;

public class PickupTrigger : MonoBehaviour {
    GameObject hintCanvas;
    private bool entered;
    ToggleHintUI hintUIController;
    public SkillTreeUI skillTreeUI;
    public int categoryId, branchId, tierId, skillId;

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
            entered = false;

            hintUIController.toggleHint();

            // let the player know he/she got a skill
            GameManager.instance.displayNotification("You got a skill scroll!");

            Invoke("ActivateSkill", 1);

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
    
    void ActivateSkill()
    {
        GameManager.instance.ToggleSkillMenu();

        skillTreeUI.ActivateCategory(categoryId);

        Invoke("activateSkillDelay", 1);
    }

    void activateSkillDelay()
    {
        GameManager.instance.skillTree.setSkillTrue(branchId, tierId, skillId);
    }
}
