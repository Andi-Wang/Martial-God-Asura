using UnityEngine;
using System.Collections;

public class PlayerHitbox : MonoBehaviour {
    public GameObject parent;
    public UnityStandardAssets._2D.PlatformerCharacter2D playerScript;

    void Awake() { 
        parent = this.transform.parent.gameObject;
        playerScript = parent.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>() as UnityStandardAssets._2D.PlatformerCharacter2D;
    }

    public void OnTriggerEnter(Collider2D other) {
        if (other.transform.parent.GetComponent<Enemy>() != null) {
            other.transform.parent.GetComponent<Enemy>().enemyEntity.health -= 10;
            //playerScript.enemiesHit.Add(other.transform.parent.gameObject.GetComponent<Enemy>() as Enemy);
        }
    }
}
