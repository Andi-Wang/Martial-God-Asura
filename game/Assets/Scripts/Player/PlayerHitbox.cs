using UnityEngine;
using System.Collections;

public class PlayerHitbox : MonoBehaviour {
    
    GameObject parent;
    UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake() { 
        parent = this.transform.parent.gameObject;
        playerScript = parent.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>() as UnityStandardAssets._2D.PlatformerCharacter2D;
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
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
