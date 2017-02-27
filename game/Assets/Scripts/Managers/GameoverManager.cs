using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour {
    public UnityStandardAssets._2D.PlatformerCharacter2D playerHealth;
    public Text gameOverText;

    Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (playerHealth.getHealth() <= 0)
        {
            gameOverText.text = "You are dead";
            anim.SetTrigger("Gameover");
            
        }
    }
    public void BackToMain()
    {
        SceneManager.LoadScene(0);
    }
}
