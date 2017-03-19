using UnityEngine;
using System.Collections;

public class PlayerHitbox : MonoBehaviour {
    
    public GameObject parent;
    public UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake() { 
        parent = this.transform.parent.gameObject;
        playerScript = parent.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>() as UnityStandardAssets._2D.PlatformerCharacter2D;
    }

    
    void OnTriggerEnter2D(Collider2D other) {
        GameObject target = other.gameObject;
        if (target.tag == "Enemy") {
            Enemy enemy = target.GetComponent<Enemy>();
            enemy.TakeDamage(playerScript.increaseDamageByMight(5));

            playerScript.strikeEnemy();
        }
    }
}
