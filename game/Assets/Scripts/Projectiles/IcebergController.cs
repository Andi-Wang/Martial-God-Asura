using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class IcebergController : MonoBehaviour {
        public LayerMask m_hittargetmask;
        public float m_maxlifetime = 0.7f;

        void Start() {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_maxlifetime);
        }

        void OnTriggerEnter2D(Collider2D other) {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_hittargetmask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++) {
                // ... and find their rigidbody.
                Rigidbody2D targetRigidbody = colliders[i].GetComponent<Rigidbody2D>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;

                // Find the enemy script associated with the rigidbody.
                Enemy enemyScript = targetRigidbody.GetComponent<Enemy>();
                PlatformerCharacter2D playerScript = targetRigidbody.GetComponent<PlatformerCharacter2D>(); ;

                if (enemyScript) {
                    enemyScript.TakeDamage(CalculateDamage(targetRigidbody));
                }
                if (!playerScript) {
                    Destroy(gameObject);
                }
            }
        }

        private float CalculateDamage(Rigidbody2D target) {
            float damage = Skill.FireballEffect();
            return damage;
        }
    }
}
