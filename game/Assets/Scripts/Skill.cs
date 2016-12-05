using UnityEngine;
using System.Collections;

public class Skill {
    public class SkillStateManager {
        public bool backdashing = false;
        public float backdashSpeed = 0f;
    }
    
    public static float Backdash(Rigidbody2D body, bool facingRight, float backdashSpeed, bool forceStart) {
        float minSpeed = 6f;
        float maxSpeed = 17f;
        float decay = 1f;

        if (forceStart || backdashSpeed > minSpeed) {
            if (backdashSpeed == 0) {
                backdashSpeed = maxSpeed;
            }
            else {
                backdashSpeed -= decay;
            }

            if (facingRight) {
                body.velocity = new Vector2(-backdashSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(backdashSpeed, body.velocity.y);
            }
        }
        else {
            backdashSpeed = 0;
        }

        return backdashSpeed;
    }
    


    public static void Glide(Rigidbody2D body, float defaultGravity, float x) {
        float velocityMultiplier = 0.25f;
        float gravityScaleMultiplier = 0.05f;

        //If the body's gravity is currently normal or greater than normal (by some other effect), start floating
        //Also checks if the body is moving down; don't want it to be able to increase max jump height
        if (body.gravityScale >= defaultGravity && body.velocity.y < 0f) {
            //Slows the body immediately upon cast if falling
            if (body.velocity.y < 0f) { 
                body.velocity = new Vector2(x, body.velocity.y * velocityMultiplier);
            }

            body.gravityScale *= gravityScaleMultiplier;
        }
        //If the body's gravity is less than normal but is greater than or equal to what Float can achieve, return to normal gravity
        //This basically means that if there's a stronger antigravity effect affecting the body, using Float will do nothing
        else if(body.gravityScale >= defaultGravity * gravityScaleMultiplier) {
            body.gravityScale = defaultGravity;
        }
    }

    public static void FastFall(Rigidbody2D body, float x) {
        float castThreshold = 4f;
        float initialFallSpeed = -8f;
        float gravity = 8f;
        
        if(body.velocity.y < castThreshold) {
            body.velocity = new Vector2(x, initialFallSpeed);
            body.gravityScale = gravity;
        }
    }

    //Damage the other entity
    public static void Punch(Enemy target) {
        target.enemyEntity.health -= 10f;
    }


    public static Enemy getEnemyScript(Collider2D other) {
        return other.attachedRigidbody.gameObject.GetComponent<Enemy>() as Enemy;
    }

    public static UnityStandardAssets._2D.PlatformerCharacter2D getPlayerScript(Collider2D other) {
        return other.attachedRigidbody.gameObject.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>() as UnityStandardAssets._2D.PlatformerCharacter2D;
    }
}
