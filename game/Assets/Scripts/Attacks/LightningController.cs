using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class LightningController : ProjectileController {
        protected override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        protected override void setLifespan() {
            lifespan = 0.35f;
        }

        protected override void setDamage() {
            damage = 10f;
        }
    }
}
