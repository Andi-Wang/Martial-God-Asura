using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class EnemyHitBox : MonoBehaviour {
        void OnTriggerEnter2D(Collider2D other) { 
            GameObject target = other.gameObject;
            if (target.tag == "Player") {
                GameObject.Find("MainCharacter").GetComponent<PlatformerCharacter2D>().TakeDamage(10);
            }
        }
    }
}