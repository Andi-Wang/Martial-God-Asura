using UnityEngine;
using System.Collections;

public class SavePointTrigger : MonoBehaviour {
    public static string dialogkey;
    private ModalPanel modalPanel;
    ToggleHintUI hintUIController;
    private bool entered;

    void Awake()
    {
        hintUIController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();

        modalPanel = ModalPanel.Instance();
        entered = false;
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetButtonUp("Interact") && entered == true)
        {
            popupSaveConfirm();
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

    public void popupSaveConfirm()
    {
        ModalPanelDetails details = new ModalPanelDetails() { question = "Do you want to save the progress?" };
        details.buttondetails = new EventButtonDetails[2];
        details.buttondetails[0] = new EventButtonDetails() { title = "OK", action = OKCallback };
        details.buttondetails[1] = new EventButtonDetails() { title = "Cancel", action = CancelCallback };

        modalPanel.Choice(details);
    }

    void OKCallback()
    {
        GameManager.instance.SaveProgress();
    }
    void CancelCallback() { }
}
