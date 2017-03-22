using UnityEngine;
using System.Collections;

public class FireballPuzzleController : MonoBehaviour {
    public float m_maxDamage = 5f;
    public float cooldownDamage = 0.2f;

    float timeCounter = 0;
    bool damaging;
    GameObject player;
    UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();

    }

    void Update()
    {
        // so that we don't continuously hit the player
        if (damaging)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= cooldownDamage)
            {
                damaging = false;
                timeCounter = 0;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            playerScript.TakeDamage(m_maxDamage);
            damaging = true;
        }   
    }
}
