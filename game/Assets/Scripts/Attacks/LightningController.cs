using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class LightningController : ProjectileController {
        public override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        public override void setLifespan() {
            lifespan = 0.35f;
        }

        public override void setDamage() {
            damage = 10f;
        }
    }
}
