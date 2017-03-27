using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class WaterDragonController : ProjectileController{
        public override void effects(Enemy enemyScript) {
            enemyScript.TakeDamage(damage);
        }

        public override void setLifespan() {
            lifespan = 0.85f;
        }
    }
}
