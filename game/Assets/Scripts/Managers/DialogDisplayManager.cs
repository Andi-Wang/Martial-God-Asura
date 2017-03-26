using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogDisplayManager : MonoBehaviour {
    public string[] testDialog;
    public bool displaying = false;
	public bool playerTalking = false;
	public Image playerImg;

	public Sprite psprite1;
	public Sprite psprite2;
	public Sprite psprite3;

    public static DialogDisplayManager instance = null;

    Text dialogText;
    public int currentDialog = -1;
    bool displayed = false;
    private IEnumerator coroutine;

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
			playerTalking = true;
			str += strComplete[i++];
			dialogText.text = str;

			if (playerTalking) {
				if (playerImg.sprite == psprite1) {
					playerImg.sprite = psprite2;
				}
				else if (playerImg.sprite == psprite2) {
					playerImg.sprite = psprite3;
				}
				else {
					playerImg.sprite = psprite1;
				}
			}

			yield return new WaitForSeconds(0.05F);

		}
        playerTalking = false;
	}

    public bool displayNext()
    {
        if (displayed)
        {
            return false;
        }

        GameManager.instance.playersTurn = false;

        if (playerTalking)
        {
            StopCoroutine(coroutine);
            dialogText.text = testDialog[currentDialog];

            playerTalking = false;
            return true;
        }
        else
        {
            displaying = true;
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

            coroutine = AnimateText(testDialog[currentDialog]);

            StartCoroutine(coroutine);

            return true;
        }
    }
}
