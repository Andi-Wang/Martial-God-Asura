using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D
{
    public class FileballController : MonoBehaviour
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

                //TODO health deduction here
                // Find the playerheath script associated with the rigidbody.
                Enemy targetHealth = targetRigidbody.GetComponent<Enemy>();
                PlatformerCharacter2D tempTargetHealth = targetRigidbody.GetComponent<PlatformerCharacter2D>(); ;

                // If there is no Enemy script attached to the gameobject, go on to the next collider.
                if (!targetHealth || !tempTargetHealth)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the fireball.
                float damage = CalculateDamage(targetRigidbody.position);

                // Deal this damage to the enemy.
                if (!tempTargetHealth)
                {
                    targetHealth.TakeDamage(damage);
                }
                else if (!targetHealth)
                {
                    tempTargetHealth.TakeDamage(damage);
                }
            }
            /*
            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
            */
            // Destroy the fireball.
            // Destroy(gameObject);
        }

        private float CalculateDamage(Vector3 targetPosition)
        {
            // Create a vector from the fireball to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the fireball to the target.
            float explosionDistance = explosionToTarget.magnitude;

            // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
            float relativeDistance = (m_explosionradius - explosionDistance) / m_explosionradius;

            // Calculate damage as this proportion of the maximum possible damage.
            float damage = relativeDistance * m_maxdamage;

            // Make sure that the minimum damage is always 0.
            damage = Mathf.Max(0f, damage);

            return damage;
        }
    }
}
