
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Skill {

    public class SkillStateManager {
        public bool backdashing = false;
        public float backdashSpeed = 0f;
        public bool sliding = false;
        public float slideSpeed = 0f;
        public bool secondJumpAvailable = false;
        public bool airdashing = false;
        public float airdashSpeed = 0f;
        public float counter = 0f;
    }

    //All-in-one method for different projectile types
    public float Projectile(Rigidbody2D body, bool facingRight, Rigidbody2D projectile, float counter, float frequencyMultiplier, float speed, float startDistance, int numProjectiles, float spread) {
        //Counter and frequency multiplier are used for held-down skills (determines the number of times per second a new projectile is made while the key is held down)
        //Speed is projectile travel speed, startDistance is where the projectile starts in relation to the player (horizontal), numProjectiles is the number of projectiles fired by one call (affected by spread)
        //Spread is the maximum angle of projectile trajectory (above and below the horizontal)

        //Some examples of possible function calls for different projectile behaviours:
        //Fireball Nova call:     skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, 1, 0, 16, 2, 5, 20);
        //Torrent call:           skillStateManager.counter = skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, skillStateManager.counter, 25, 20, 1, 1, 0);
        //Iceball call:           skill.Projectile(m_Rigidbody2D, m_FacingRight, m_iceball, 1, 0, 8, 2, 1, 0);
        //Flame Lance call:       skill.Projectile(m_Rigidbody2D, m_FacingRight, m_flamelance, 1, 0, 0, 1, 1, 0);

        //m_flamelance and m_iceball prefabs not created yet

        counter += Time.deltaTime * frequencyMultiplier;

        if (counter >= 1) {
            counter -= 1f;

            for (int i = 0; i < numProjectiles; i++) {
                float angle = 0f;
                if (numProjectiles > 1) {
                    angle = spread - 2f * spread / (numProjectiles - 1) * i;
                }
                if (!facingRight) { angle += 180f; }
                float xdeg = Mathf.Cos(angle * Mathf.Deg2Rad);
                float ydeg = Mathf.Sin(angle * Mathf.Deg2Rad);

                Rigidbody2D clone = Rigidbody2D.Instantiate(projectile, new Vector2(body.transform.position.x + xdeg * startDistance, body.transform.position.y + ydeg * startDistance), Quaternion.AngleAxis(angle, Vector3.forward)) as Rigidbody2D;
                clone.velocity = speed * new Vector2(xdeg, ydeg);
            }
        }

        return counter;
    }

    public float Backdash(Rigidbody2D body, bool facingRight, float backdashSpeed, bool forceStart) {

        float minSpeed = 6f;
        float maxSpeed = 17f;
        float decay = 1f;
        float assumedFPS = 60f;

        if (forceStart || backdashSpeed > minSpeed) {
            if (backdashSpeed == 0) {
                backdashSpeed = maxSpeed;
            }
            else {
                backdashSpeed -= decay * Time.deltaTime * assumedFPS;
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


    public float Slide(Rigidbody2D body, bool facingRight, float slideSpeed, bool forceStart, bool enhanced, bool slideCancel) {
        float minSpeed = 6f;
        float maxSpeed = 17f;
        float enhancementBoost = 7f;
        float assumedFPS = 60f;

        if (enhanced) {
            minSpeed += enhancementBoost;
            maxSpeed += enhancementBoost;
        }

        float decay = 1f;

        if (enhanced && slideCancel) {
            slideSpeed = 0;
        }
        else if (forceStart || slideSpeed > minSpeed) {
            if (slideSpeed == 0) {
                slideSpeed = maxSpeed;
            }
            else {
                slideSpeed -= decay * Time.deltaTime * assumedFPS;
            }

            if (facingRight) {
                body.velocity = new Vector2(slideSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(-slideSpeed, body.velocity.y);
            }
        }
        else {
            slideSpeed = 0;
        }
        return slideSpeed;
    }


    public float Airdash(Rigidbody2D body, bool facingRight, float airdashSpeed, bool forceStart) {
        float minSpeed = 12f;
        float maxSpeed = 32f;
        float decay = 1.2f;
        float verticalSpeedModifier = 12f;
        float assumedFPS = 60f;

        if (forceStart || airdashSpeed > minSpeed) {
            float verticalSpeedOffset = 0f;

            if (airdashSpeed == 0) {
                airdashSpeed = maxSpeed;
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
            }
            else {
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
                airdashSpeed -= decay * Time.deltaTime * assumedFPS;
            }

            if (facingRight) {
                body.velocity = new Vector2(airdashSpeed, verticalSpeedOffset);
            }
            else {
                body.velocity = new Vector2(-airdashSpeed, verticalSpeedOffset);
            }
        }
        else {
            airdashSpeed = 0f;
        }
        return airdashSpeed;
    }

    public void Glide(Rigidbody2D body, float defaultGravity, float x) {
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
        else if (body.gravityScale >= defaultGravity * gravityScaleMultiplier) {
            body.gravityScale = defaultGravity;
        }
    }

    public void FastFall(Rigidbody2D body, float x) {
        float castThreshold = 4f;
        float initialFallSpeed = -8f;
        float gravity = 8f;

        if (body.velocity.y < castThreshold) {
            body.velocity = new Vector2(x, initialFallSpeed);
            body.gravityScale = gravity;
        }
    }
}
