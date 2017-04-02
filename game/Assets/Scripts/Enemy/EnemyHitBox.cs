using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class EnemyHitBox : MonoBehaviour {
        void OnTriggerEnter2D(Collider2D other) {
            GameObject enemy = transform.parent.gameObject;
            GameObject target = other.gameObject;
            if (target.tag == "Player" && enemy.tag == "Enemy") {
                Enemy enemyReal = enemy.GetComponent<Enemy>();
                GameObject.Find("MainCharacter").GetComponent<PlatformerCharacter2D>().TakeDamage(transform.GetComponentInParent<Rigidbody2D>(), enemyReal.playerDamage);
            }
        }
    }
}