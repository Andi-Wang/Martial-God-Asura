using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class IcebergController : ProjectileController {
        private float elapsedLifespan = 0;
        private bool facingRight;
        private bool facingSet = false;

        void FixedUpdate() {
            if(elapsedLifespan > 0.6f) {
                projectile.transform.Rotate(new Vector3(0, 0, 5));

                if(facingRight) {
                    projectile.velocity = new Vector2(8f, 0f);
                }
                else {
                    projectile.velocity = new Vector2(-8f, 0f);
                }
            }
            else {
                elapsedLifespan += Time.fixedDeltaTime;
                if(!facingSet) {
                    facingSet = true;
                    facingRight = projectile.velocity.x > 0;
                }
            }
        }
        

        protected override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
            if(!enemyScript.isDead) {
                enemyScript.Knockback(facingRight, 0.3f, 8f);
            }
        }

        protected override void setLifespan() {
            lifespan = 2.4f;
        }
    }
}

