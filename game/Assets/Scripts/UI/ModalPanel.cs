using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class ModalPanel : MonoBehaviour {
    
    public Text question;
    public Button[] buttons;
    public Text[] buttonTexts;

    public GameObject modalPanelObj;
    public GameObject buttonPanelObj;

    private static ModalPanel modalPanel;

    public static ModalPanel Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }

        return modalPanel;
    }

    public void Choice(ModalPanelDetails details)
    {
        modalPanelObj.SetActive(true);
        GameManager.Pause();

        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(false);
        }

        question.text = details.question;

        for(int i = 0; i<details.buttondetails.Length; ++i)
        {
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(details.buttondetails[i].action);
            buttons[i].onClick.AddListener(ClosePanel);
            buttonTexts[i].text = details.buttondetails[i].title;
            buttons[i].gameObject.SetActive(true);
        }
    }

    void ClosePanel()
    {
        modalPanelObj.SetActive(false);
        GameManager.Resume();
    }
}

public class ModalPanelDetails
{
    public string question;
    public EventButtonDetails[] buttondetails;
}
public class EventButtonDetails
{
    public string title;
    public UnityAction action;
}