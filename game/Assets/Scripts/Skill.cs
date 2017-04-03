
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Skill {
    const int assumedFPS = 60;

    public class CooldownManager {
        public const float slideKickCooldown = 0.55f;
        public float slideKickTimer = 0f;

        public const float cycloneKickCooldown = 0.55f;
        public float cycloneKickTimer = 0f;

        public const float flipKickCooldown = 0.55f;
        public float flipKickTimer = 0f;


        public const float fireballCooldown = 2f;
        public const float fireballReducedCooldown = 0.85f;
        public float fireballTimer = 0f;

        public const float waterDragonCooldown = 0.85f;
        public float waterDragonTimer = 0f;

        public const float icebergCooldown = 2f;
        public float icebergTimer = 0f;

        public const float callLightningCooldown = 2f;
        public float callLightningTimer = 0f;

        public const float meteorCooldown = 2f;
        public float meteorTimer = 0f;

        public const float barrierSigilCooldown = 5f;
        public float barrierSigilTimer = 0f;

        public const float drainingSigilCooldown = 2f;
        public float drainingSigilTimer = 0f;

        public const float teleportSigilCooldown = 5f;
        public float teleportSigilTimer = 0f;

        public const float lesserSpiritboldCooldown = 1f;
        public float lesserSpiritboltTimer = 0f;

        public const float onslaughtToggleCooldown = 0.85f;
        public float onslaughtToggleTimer = 0f;

        public const float airSharkCooldown = 0.8f;
        public float airSharkTimer = 0f;

        public void Tick(float amount) {
            slideKickTimer -= amount;
            cycloneKickTimer -= amount;
            flipKickTimer -= amount;

            fireballTimer -= amount;
            waterDragonTimer -= amount;
            icebergTimer -= amount;
            callLightningTimer -= amount;
            meteorTimer -= amount;
            barrierSigilTimer -= amount;
            drainingSigilTimer -= amount;
            teleportSigilTimer -= amount;
            lesserSpiritboltTimer -= amount;
            onslaughtToggleTimer -= amount;
            airSharkTimer -= amount;
        }
    }

    public class SkillStateManager {
        
        public float dashSpeed = 0;
        public float backdashSpeed = 0;
        public float slideSpeed = 0;
        public bool secondJumpAvailable = false;
        
        public float airdashSpeed = 0;
        public float channelTimer = 0;
        public int fireballCounter = 0;
        public int punchCounter = 0;
        public int ironStrikesStacks = 0;
        public float swoopingStrikeDuration = 0;

        //currently set to true by default since there's no keybind to toggle this manually
        public bool onslaughtToggle = false;
        public bool fastFallToggle = false;

        //Checks to see if anything (that needs to take place over multiple frames) is happening
        public bool gliding = false;
        public bool dashing = false;
        public bool backdashing = false;
        public bool sliding = false;
        public bool airdashing = false;
        public bool holdCasting = false;
        public bool swoopingStrike = false;

        //Holds the teleport sigil
        public bool teleportSigilExists = false;
        public GameObject teleportSigil;
        public bool teleportFacingRight;

        public void getHit() {
            dashing = false;
            dashSpeed = 0;
            backdashing = false;
            backdashSpeed = 0;
            sliding = false;
            slideSpeed = 0;
            airdashing = false;
            airdashSpeed = 0;
            gliding = false;
            fastFallToggle = false;
            swoopingStrike = false;
        }

        public void land() {
            airdashing = false;
            airdashSpeed = 0;
            gliding = false;
            fastFallToggle = false;
            secondJumpAvailable = true;
            swoopingStrike = false;
        }
    }

    public const float dashCost = 10f;
    public const float airdashCost = 10f;
    public const float glideCostPerSecond = 5f;

    public const float fireballCost = 15f;
    public const float waterDragonCost = 20f;
    public const float icebergCost = 35f;
    public const float callLightningCost = 40f;
    public const float meteorCost = 40f;
    public const float barrierSigilCost = 25f;
    public const float drainingSigilCost = 20f;
    public const float teleportSigilCost = 35f;
    public const float lesserSpiritboltCost = 15f;
    public const float cycloneKickTornadoCost = 0f;
    public const float airSharkCost = 20f;

    //Grants bonus armor (percentage damage reduction) while dashing
    //Call this regularly to update armor values
    public int EvasiveManeuvers_Passive(bool dashing) {
        int bonusArmor = 0;
        if (dashing) {
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
        float bonusEnergyRegenPerMissingEnergy = 0.1f;
        float bonusEnergyRegen = (maxEnergy - currentEnergy) * bonusEnergyRegenPerMissingEnergy;
        return bonusEnergyRegen;
    }


    //Speeds up animations (basically only affects attacks), but disables energy regeneration while toggled on
    //Automatically untoggles when below a certain Energy threshold (% of max mana)
    //Call this regularly to update toggle status
    public bool OnslaughtToggle(bool onslaughtToggle, float currentEnergy) {
        float automaticDisableThreshold = 10f;

        if (currentEnergy < automaticDisableThreshold) {
            onslaughtToggle = false;
        }

        return onslaughtToggle;
    }
    public float OnslaughtEffect(bool onslaughtToggle) {
        float bonusAnimationSpeed = 0;
        if (onslaughtToggle) bonusAnimationSpeed = 0.4f;
        return bonusAnimationSpeed;
    }

    

    
    //Method for projectiles with spread; mostly deprecated
    public float SpreadProjectile(Rigidbody2D body, bool facingRight, GameObject projectilePrefab, float counter, float frequencyMultiplier, float speed, float startDistance, int numProjectiles, float spread) {
        //Counter and frequency multiplier are used for held-down skills (determines the number of times per second a new projectile is made while the key is held down)
        //Speed is projectile travel speed, startDistance is where the projectile starts in relation to the player (horizontal), numProjectiles is the number of projectiles fired by one call (affected by spread)
        //Spread is the maximum angle of projectile trajectory (above and below the horizontal)

        //Some examples of possible function calls for different projectile behaviours:
        //Fireball Nova call:     skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, 1, 0, 16, 2, 5, 20);
        //Torrent call:           skillStateManager.counter = skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, skillStateManager.counter, 25, 20, 1, 1, 0);
        //Iceball call:           skill.Projectile(m_Rigidbody2D, m_FacingRight, m_iceball, 1, 0, 8, 2, 1, 0);
        //Flame Lance call:       skill.Projectile(m_Rigidbody2D, m_FacingRight, m_flamelance, 1, 0, 0, 1, 1, 0);

        //m_flamelance and m_iceball prefabs not created yet

        Rigidbody2D projectile = projectilePrefab.GetComponent<Rigidbody2D>();

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

                if(!facingRight) {
                    clone.transform.localScale = new Vector2(clone.transform.localScale.x, clone.transform.localScale.y * -1);
                }

                clone.velocity = speed * new Vector2(xdeg, ydeg);
            }
        }

        return counter;
    }

    //All-in-one method for different projectile types
    public void Projectile(Rigidbody2D body, bool facingRight, GameObject projectilePrefab, float startX, float startY, float xSpeed, float ySpeed) {
        Rigidbody2D projectile = projectilePrefab.GetComponent<Rigidbody2D>();
        if(!facingRight) {
            startX *= -1;
            xSpeed *= -1;
        }

        Rigidbody2D clone = Rigidbody2D.Instantiate(projectile, new Vector2(body.transform.position.x + startX, body.transform.position.y + startY), new Quaternion()) as Rigidbody2D;
        if (!facingRight) {
            clone.transform.localScale = new Vector2(clone.transform.localScale.x * -1, clone.transform.localScale.y);
        }

        clone.velocity = new Vector2(xSpeed, ySpeed);
    }

    public GameObject TeleportSigil(Rigidbody2D body, bool facingRight, GameObject projectilePrefab) {
        Rigidbody2D projectile = projectilePrefab.GetComponent<Rigidbody2D>();
        Rigidbody2D clone = Rigidbody2D.Instantiate(projectile, body.transform.position, new Quaternion()) as Rigidbody2D;
        return clone.gameObject;
    }
    public void TeleportToSigil(Rigidbody2D body, GameObject sigil) {
        body.position = sigil.GetComponent<Rigidbody2D>().position;
        Object.Destroy(sigil);
    }

    public float SwoopingStrike (Rigidbody2D body, float duration, float countdown, bool facingRight, bool justActivated) {
        float maxDuration = 1.5f;
        float delay = 0f;

        float xSpeed = 0f;
        float ySpeed = 0f;


        if (justActivated) {
            duration = maxDuration;
        }
        if(duration > maxDuration - delay) {
            xSpeed = 2f;
            ySpeed = 2f;
        }
        else if(duration > 0) {
            xSpeed = -15f;
            ySpeed = -5f;
        }

        if(facingRight) {
            xSpeed *= -1f;
        }

        body.velocity = new Vector2(xSpeed, ySpeed);

        return duration - countdown;
    }

    public float Dash(Rigidbody2D body, bool dashing, float dashSpeed, bool facingRight) {
        float minSpeed = 12f;
        float maxSpeed = 34f;
        float decay = 2f;

        if(dashing) {
            if(dashSpeed == 0) {
                dashSpeed = maxSpeed;
            }
            else if(dashSpeed > minSpeed) {
                dashSpeed -= decay * Time.deltaTime * assumedFPS;
            }
            else {
                dashSpeed = 0;
            }

            if(facingRight) {
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
    public float Backdash(Rigidbody2D body, bool backdashing, float dashSpeed, bool facingRight) {
        return Dash(body, backdashing, dashSpeed, !facingRight);
    }


    public float Slide(Rigidbody2D body, bool sliding, float slideSpeed, bool facingRight) {
        float minSpeed = 12f;
        float maxSpeed = 22f;
        float decay = 1.5f;

        if (sliding) {
            if(slideSpeed == 0) {
                slideSpeed = maxSpeed;
            }
            else if(slideSpeed > minSpeed) {
                slideSpeed -= decay * Time.deltaTime * assumedFPS;
            }
            else {
                slideSpeed = 0;
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

    

    public float Airdash(Rigidbody2D body, bool airdashing, float airdashSpeed, bool facingRight) {
        float maxSpeed = 20f;
        float minSpeed = 16f;
        float decay = 0.4f;
        float verticalSpeedModifier = 12f;
        float verticalSpeedOffset = 0f;

        if (airdashing) {          
            if (airdashSpeed == 0) {
                airdashSpeed = maxSpeed;
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
            }
            else if(airdashSpeed > minSpeed) {
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
                airdashSpeed -= decay * Time.deltaTime * assumedFPS;
            }
            else {
                airdashSpeed = 0;
            }

            if (facingRight) {
                body.velocity = new Vector2(airdashSpeed, 0);
                //body.velocity = new Vector2(airdashSpeed, verticalSpeedOffset);
            }
            else {
                body.velocity = new Vector2(-airdashSpeed, 0);
                //body.velocity = new Vector2(-airdashSpeed, verticalSpeedOffset);
            }
        }
        else {
            airdashSpeed = 0f;
        }
        return airdashSpeed;
    }

    /*
    public bool GlideToggle(Rigidbody2D body, bool gliding, bool grounded, bool justActivated) {
        float castThreshold = 4f;
        float velocityMultiplier = 0f;

        if(justActivated) {
            if(!gliding && body.velocity.y < castThreshold) {
                gliding = true;
                float newY = Mathf.Min(0, velocityMultiplier * body.velocity.y);
                body.velocity = new Vector2(body.velocity.x, newY);
            }
            else {
                gliding = false;
            }
        }
        if(grounded) {
            gliding = false;
        }

        return gliding;
    }
    public void GlideActivated(Rigidbody2D body) {
        float castThreshold = 4f;
        float velocityMultiplier = 0f;
        
        if (body.velocity.y < castThreshold) {
            float newY = Mathf.Min(0, velocityMultiplier * body.velocity.y);
            body.velocity = new Vector2(body.velocity.x, newY);
        }
    }*/
    public float GlideEffect() {
        float maxFallSpeed = -0.12f;
        return maxFallSpeed;
    }


    public bool FastFallToggle(Rigidbody2D body, bool fastFallToggle, bool grounded, bool justActivated) {
        float castThreshold = 8f;
        float initialFallSpeed = -8f;

        if(grounded) {
            fastFallToggle = false;
        }
        else if (justActivated && body.velocity.y < castThreshold) {
            fastFallToggle = true;
            body.velocity = new Vector2(body.velocity.x, initialFallSpeed);
        }

        return fastFallToggle;
    }
    public float FastFallEffect(bool fastFallToggle) {
        float bonusGravity = 0;
        if (fastFallToggle) bonusGravity = 5f;
        return bonusGravity;
    }
}
