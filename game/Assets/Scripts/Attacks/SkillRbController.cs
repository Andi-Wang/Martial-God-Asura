using UnityEngine;
using System.Collections.Generic;


namespace UnityStandardAssets._2D {
    public class SkillRbController : MonoBehaviour {
        //public float m_explosionradius;

        public LayerMask m_hittargetmask;
        public float m_maxlifetime;
        public float m_maxdamage;
        public HashSet<Collider2D> alreadyHit;

        public virtual void Awake() {
            //m_explosionradius = 1f;

            m_maxlifetime = 0.7f;
            m_maxdamage = 5f;
            m_hittargetmask = LayerMask.NameToLayer("Enemy");
            alreadyHit = new HashSet<Collider2D>();

            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_maxlifetime);
        }

        public virtual void OnTriggerStay2D(Collider2D other) {
            if (other.IsTouchingLayers(m_hittargetmask)) {
                Enemy enemyScript = other.GetComponentInParent<Enemy>();

                if(!alreadyHit.Contains(other) && enemyScript) {
                    enemyScript.TakeDamage(m_maxdamage);
                    alreadyHit.Add(other);
                }
            }
        }


        public virtual void OnTriggerEnter2D(Collider2D other) {
            if(other.IsTouchingLayers(m_hittargetmask)) {
                Enemy enemyScript = other.GetComponentInParent<Enemy>();
                if(enemyScript) {
                    other.GetComponentInParent<Enemy>().TakeDamage(m_maxdamage);
                    Destroy(gameObject);
                }
            }


            /*
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
            }*/
        }
    }
}
