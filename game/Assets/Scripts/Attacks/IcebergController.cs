using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class IcebergController : ProjectileController {
        private float elapsedLifespan = 0;

        void FixedUpdate() {
            
            if(elapsedLifespan > 0.6f) {
                projectile.transform.Rotate(new Vector3(0, 0, 5));


                //Summon animation location, movement, and facing direction are currently bugged
            }
            else {
                elapsedLifespan += Time.fixedDeltaTime;
            }
        }
        

        protected override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        protected override void setLifespan() {
            lifespan = 2f;
        }
    }
}

