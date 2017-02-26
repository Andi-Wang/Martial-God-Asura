using UnityEngine;
using System.Collections;

public class EnemyHitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        GameObject target = other.gameObject;
        if (target.tag == "Player")
        {
           // PlatformerCharacter2D player = target.GetComponent<PlatformerCharacter2D>();
            
        }
    }
}
