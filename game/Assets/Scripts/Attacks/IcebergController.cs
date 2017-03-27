using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class IcebergController : ProjectileController {
        private float elapsedLifespan = 0;
        private bool facingRight;

        void FixedUpdate() {
            
            if(elapsedLifespan > 0.6f) {
                projectile.transform.Rotate(new Vector3(0, 0, 5));

                float movement = 15;
                if(!facingRight) { movement *= -1; }

                projectile.velocity = new Vector2(movement, 0);

                //Summon animation location, movement, and facing direction are currently bugged
            }
            else {
                elapsedLifespan += Time.fixedDeltaTime;
            }
        }
        

        public override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        public override void setLifespan() {
            lifespan = 2f;
        }

        public override void setMisc() {
            facingRight = projectile.transform.localScale.x > 0;
        }
    }
}

