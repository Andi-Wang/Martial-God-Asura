using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class SharkAirController : ProjectileController {
        protected override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        protected override void setLifespan() {
            lifespan = 0.85f;
        }
    }
}
