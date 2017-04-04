using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

namespace UnityStandardAssets._2D {
    public class ProjectileController : MonoBehaviour {
        protected Rigidbody2D projectile;
        protected LayerMask targetMask;
        protected float lifespan;
        protected float damage;

        void Awake() {
            projectile = gameObject.GetComponent<Rigidbody2D>();
            setLifespan();
            setDamage();
            setTargetMask();
            setMisc();

            //Detects OnTriggerEnter2D if the projectile is spawned inside of another collider
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = true;

            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, lifespan);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.IsTouchingLayers(targetMask)) {
                Enemy enemyScript = other.GetComponentInParent<Enemy>();
                if (enemyScript) {
                    effects(enemyScript);
                }
            }
            if (other.GetComponent<TilemapChunk>() != null && other.GetComponentInParent<Tilemap>().tag == "Wall")
            {
                Destroy(gameObject);
            }
        }

        protected virtual void setLifespan() {
            lifespan = 0.7f;
        }

        protected virtual void setDamage() {
            damage = 5f;
        }

        protected virtual void setMisc() {
        }

        protected virtual void setTargetMask() {
            targetMask = LayerMask.NameToLayer("Enemy");
        }

        protected virtual void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
