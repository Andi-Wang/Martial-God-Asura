
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Skill {

    public class SkillStateManager {
        public bool dashing = false;
        public float dashSpeed = 0f;
        public bool sliding = false;
        public float slideSpeed = 0f;
        public bool secondJumpAvailable = false;
        public bool airdashing = false;
        public float airdashSpeed = 0f;
        public float counter = 0f;
        public int punchCounter = 0;
        public int ironStrikesStacks = 0;

        //currently set to true by default since there's no keybind to toggle this manually
        public bool onslaughtToggle = true;
        public Entity bonusStats = new Entity();
    }

    //Grants bonus armor (percentage damage reduction) while dashing
    //Call this regularly to update armor values
    public int EvasiveManeuvers_Passive(bool isDashing) {
        int bonusArmor = 0;
        if (isDashing) {
            bonusArmor = 50;
        }
        return bonusArmor;
    }

    //Build stacks by hitting enemies (probably melee attacks only)
    //Grants bonus might (percentage damage bonus, currently for melee attacks only) and armor (percentage damage reduction) per stack
    //Stacks fall off after not hitting any enemy for a certain period of time
    //Call this regularly to update the number of stacks/might/armor
    //Call this from PlayerHitbox with stacks+1 to increase stacks after a hit
    public float[] IronStrikes_Passive(int stacks, float timeSinceLastStrike) {
        int maxStacks = 6;
        float bonusMightPerStack = 5;
        float bonusArmorPerStack = 4;
        float stackFalloffTime = 4f;

        if (stacks > maxStacks) {
            stacks = maxStacks;
        }
        else if(timeSinceLastStrike > stackFalloffTime) {
            stacks = 0;
        }

        return new float[3] { (float)stacks, bonusMightPerStack * stacks, bonusArmorPerStack * stacks };
    }

    //After a brief period of not being struck, starts building up stacks that grant bonus movement speed (stacks fall off when struck)
    //Call this regularly to update movespeed
    public float Momentum_Passive(float timeSinceLastStruck) {
        float bonusMovespeedPerStack = 0.5f;
        int maxStacks = 5;
        float timePerStack = 1f;
        float minTime = 2f;

        int stacks = 0;
        if (timeSinceLastStruck > minTime) {
            stacks = (int)(1 + (timeSinceLastStruck - minTime) / timePerStack);
        }        
        if(stacks > maxStacks) {
            stacks = maxStacks;
        }
        
        return stacks * bonusMovespeedPerStack;
    }

    //After a brief period of not being struck, regenerates health equal to the last source of damage over a duration
    //Call this regularly to update health regeneration
    public float FragileRegrowth_Passive(float timeSinceLastStruck, float lastDamageTaken) {
        float minTime = 1f;
        float duration = 5f;
        float bonusHealthRegeneration = 0;

        if(timeSinceLastStruck > minTime && timeSinceLastStruck < (minTime + duration)) {
            bonusHealthRegeneration = lastDamageTaken / duration;
        }

        return bonusHealthRegeneration;
    }


    //Grants energy regen based on amount of missing energy (more regen when more energy is missing)
    //Call this regularly to update energy regeneration
    public float Renewal_Passive(float currentEnergy, float maxEnergy) {
        float bonusEnergyRegenPerMissingEnergy = 0.01f;
        float bonusEnergyRegen = (maxEnergy - currentEnergy) * bonusEnergyRegenPerMissingEnergy;
        return bonusEnergyRegen;
    }


    //Speeds up animations (basically only affects attacks), but disables energy regeneration while toggled on
    //Automatically untoggles when below a certain Energy threshold (% of max mana)
    //Call this regularly to update toggle status
    public object[] Onslaught(bool onslaughtToggle, float currentEnergy, float maxEnergy) {
        float automaticDisableThreshold = 0.1f;
        float bonusAnimationSpeed = 0;

        if (currentEnergy / maxEnergy < automaticDisableThreshold) {
            onslaughtToggle = false;
        }

        if(onslaughtToggle) {
            bonusAnimationSpeed = 0.4f;
        }

        return new object[2] { (object)onslaughtToggle, (object)bonusAnimationSpeed };
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

    public float Dash(Rigidbody2D body, bool facingRight, float dashSpeed, bool forceStart) {

        float minSpeed = 12f;
        float maxSpeed = 34f;
        float decay = 2f;
        float assumedFPS = 60f;

        if (forceStart || dashSpeed > minSpeed) {
            if (dashSpeed == 0) {
                dashSpeed = maxSpeed;
            }
            else {
                dashSpeed -= decay * Time.deltaTime * assumedFPS;
            }

            if (facingRight) {
                body.velocity = new Vector2(dashSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(-dashSpeed, body.velocity.y);
            }
        }
        else {
            dashSpeed = 0;
        }

        return dashSpeed;
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
