using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour {
    public UnityStandardAssets._2D.PlatformerCharacter2D playerHealth;
    public Text gameOverText;

    Animator anim;
    bool finishing = false;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!finishing && playerHealth.getHealth() <= 0)
        {
            finishing = true;
            gameOverText.text = "You are dead";
            anim.SetTrigger("Gameover");
        }
    }

    public void BackToMain()
    {
        StartCoroutine(GameManager.instance.loadLvAsync(0));
    }
}
