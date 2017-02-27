using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogDisplayManager : MonoBehaviour {
    public string[] testDialog;
    public bool displaying = false;
    public static DialogDisplayManager instance = null;

    Text dialogText;
    public int currentDialog = -1;
    bool displayed = false;

	private string str = "";
    
    public static DialogDisplayManager Instance()
    {
        if (instance == null)
        {
            instance = new DialogDisplayManager();
        }
        return instance;
    }
	// Use this for initialization
	void Awake () {
        dialogText = GetComponentInChildren<Text>();
	}

	IEnumerator AnimateText(string strComplete){
		int i = 0;
		str = "";
		while( i < strComplete.Length ){
			str += strComplete[i++];
			dialogText.text = str;
			yield return new WaitForSeconds(0.05F);
		}
	}

    public bool displayNext()
    {
        if (displayed)
        {
            return false;
        }

        displaying = true;
        GameManager.instance.playersTurn = false;
        currentDialog++;
        if (currentDialog >= testDialog.Length)
        {
            gameObject.SetActive(false);
            displayed = true;
            displaying = false;
            GameManager.instance.playersTurn = true;
            return false;
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
        
		StartCoroutine( AnimateText(testDialog[currentDialog]) );

        return true;
    }
}
