using UnityEngine;
using System.Collections;

public class FileballPuzzleController : MonoBehaviour {
    public float m_maxTravelTime = 0.2f;
    public float m_maxDamage = 5f;
    public float m_speed = 1f;

    float timeCounter = 0;
    GameObject player;
    UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
    }

    void Update()
    {
        transform.position += transform.right * m_speed * Time.deltaTime;
        timeCounter += Time.deltaTime;
        if (timeCounter>= m_maxTravelTime)
        {
            ChangeDirection();
            timeCounter = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            playerScript.TakeDamage(m_maxDamage);
        }   
    }
    
    void ChangeDirection()
    {
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;
        //transform.localScale = theScale;
        transform.right *= -1;
    }
}
