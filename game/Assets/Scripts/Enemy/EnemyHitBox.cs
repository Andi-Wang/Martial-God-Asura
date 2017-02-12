using UnityEngine;
using System.Collections;

public class EnemyHitbox : MonoBehaviour
{
    /*
    public GameObject parent;
    public UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake() { 
        parent = this.transform.parent.gameObject;
        playerScript = parent.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>() as UnityStandardAssets._2D.PlatformerCharacter2D;
    }*/


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<>().enemyEntity.health -= 5;
        }
    }
}
