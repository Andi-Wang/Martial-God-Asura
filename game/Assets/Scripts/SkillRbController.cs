using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D
{
    public class SkillRbController : MonoBehaviour
    {
        public LayerMask m_hittargetmask;
        public float m_maxlifetime = 0.7f;
        public float m_explosionradius = 1f;
        public float m_maxdamage = 5f;

        void Start()
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_maxlifetime);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_explosionradius, m_hittargetmask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody2D targetRigidbody = colliders[i].GetComponent<Rigidbody2D>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;
                
                // Find the playerheath script associated with the rigidbody.
                Enemy targetHealth = targetRigidbody.GetComponent<Enemy>();
                //PlatformerCharacter2D tempTargetHealth = targetRigidbody.GetComponent<PlatformerCharacter2D>(); ;
                

                if(targetHealth) {
                    targetHealth.TakeDamage(m_maxdamage);
                }
               // if(!tempTargetHealth) {
                   // Destroy(gameObject);
               // }
            }
        }
        
    }
}
