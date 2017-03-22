using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityStandardAssets._2D {
    public class PlatformerCharacter2D : MonoBehaviour {
        public Image healthbar;
        public Image energybar;
        public Rigidbody2D m_fireball;
        public Image damageImage;
        public Color flashColour = new Color(1f, 0, 0, 0.3f);
        public float flashSpeed = 2f;

        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private Entity playerEntity = new Entity(100f, 100f, 0f, 5f, 0f, 0f, 700f, 10f, 1f, 0.00001f); //very small crouch speed so the player can turn around when crouched
        public Entity buffEntity = new Entity();
        private Skill skill;
        private Skill.SkillStateManager skillStateManager = new Skill.SkillStateManager();

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        const float airSpeedDecayTo = 0.993f;

        bool isDead;
		bool attacking = false; // used to detect if we are beginning an attack, in order to prevent input buffering

        const float invincibilityDurationWhenHit = 1f;
        const float stunDurationWhenHit = 0.5f;
        const float knockbackForceWhenHit = 700f;
        const float airKnockdownVelocity = 6f;
        float timeSinceLastStruck = 1f;
        float lastDamageTaken = 0;
        float timeSinceLastStrike = 1f;
        float stunned = 0;
        bool damaged = false;


        public void strikeEnemy() {
            timeSinceLastStrike = 0;
            skillStateManager.ironStrikesStacks++;
        }


        void Awake()
        {
            skill = new Skill();
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            isDead = false;
        }
        void Update()
        {
            if (damaged)
            {
                damageImage.color = flashColour;
            }
            else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            damaged = false;
        }

        public float reduceDamageByArmor(float damage) {
            float totalArmor = playerEntity.armor + buffEntity.armor;
            return damage * 100 / (100 + totalArmor);
        }

        public float increaseDamageByMight(float damage) {
            float totalMight = playerEntity.might + buffEntity.might;
            return damage / 100 * (100 + totalMight);
        }

        public void TakeDamage(float amount)
        {
            amount = reduceDamageByArmor(amount);

            damaged = true;

            if(timeSinceLastStruck >= invincibilityDurationWhenHit) {
                playerEntity.health -= amount;
                lastDamageTaken = amount;
                healthbar.fillAmount = playerEntity.health / playerEntity.maxHealth;

                timeSinceLastStruck = 0;
                stunned = stunDurationWhenHit;

                //Interrupt certain actions like sliding
                skillStateManager.getHit();

                if(m_Grounded) {
                    skillStateManager.backdashSpeed = skill.Backdash(m_Rigidbody2D, m_FacingRight, 0, true);
                    skillStateManager.backdashing = true;
                }
                else {
                    m_Rigidbody2D.velocity = new Vector2(0, -airKnockdownVelocity);

                    if (m_FacingRight) {
                        m_Rigidbody2D.AddForce(new Vector2(-knockbackForceWhenHit, 0));
                    }
                    else {
                        m_Rigidbody2D.AddForce(new Vector2(knockbackForceWhenHit, 0));
                    }
                }
                
            }

            //TODO: player hurt sound,animation
            if (playerEntity.health <= 0 && !isDead)
            {
                Death();
            }
        }

        public void addEnergy(float amount) {
            playerEntity.energy = Math.Min(playerEntity.energy + amount, playerEntity.maxEnergy);
            energybar.fillAmount = playerEntity.energy / playerEntity.maxEnergy;
        }
        public void addHealth(float amount) {
            playerEntity.health = Math.Min(playerEntity.health + amount, playerEntity.maxHealth);
            healthbar.fillAmount = playerEntity.health / playerEntity.maxHealth;
        }
        public bool energyCost(float cost) {
            if (playerEntity.energy > cost) {
                addEnergy(-cost);
                return true;
            }
            else return false;
        }

        public float getHealth() {
            return playerEntity.health;
        }

        void Death()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Tell the animator that the player is dead.
            m_Anim.SetTrigger("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            //playerAudio.clip = deathClip;
            //playerAudio.Play();
        }

        private void FixedUpdate() {
            m_Grounded = false;

            if (!GameManager.instance.playersTurn)
            {
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            }

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            // Set attacking to false in Basic Punch to prevent buffered input from overriding command
			if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Basic Punch") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Flip Kick") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Block")){
				attacking = false;
			}
			else if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Low Kick")) {
				attacking = false;
			}
			// Set BasicPunch to false in Idle to ensure no interrupt during Basic Punch animation
			else if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Crouch")) {
				m_Anim.SetBool ("BasicPunch", false);
			}

            //Add kick and block anim stuff here



            //Force punch animation to play without override from more punches
            //if(!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Punch")) {
            //	m_Anim.SetBool ("BasicPunch", false);
            //m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = false;
            //}


            //Calculate bonus stats from buffs
            float[] ironStrikesOutput = skill.IronStrikes_Passive(skillStateManager.ironStrikesStacks, timeSinceLastStrike);
            skillStateManager.ironStrikesStacks = (int)ironStrikesOutput[0];
            skillStateManager.onslaughtToggle = skill.OnslaughtToggle(skillStateManager.onslaughtToggle, playerEntity.energy);
            skillStateManager.fastFallToggle = skill.FastFallToggle(m_Rigidbody2D, skillStateManager.fastFallToggle, m_Grounded, false);


            //Bonus might
            buffEntity.might = ironStrikesOutput[1];

            //Bonus armor
            buffEntity.armor = ironStrikesOutput[2];
            if(stunned <= 0) {
                buffEntity.armor += skill.EvasiveManeuvers_Passive(skillStateManager.dashing);
            }

            //Bonus health regeneration
            buffEntity.healthRegen = skill.FragileRegrowth_Passive(timeSinceLastStruck, lastDamageTaken);

            //Bonus energy regeneration (and animation speed)
            buffEntity.energyRegen = skill.Renewal_Passive(playerEntity.energy, playerEntity.maxEnergy);
            if(skillStateManager.onslaughtToggle) {
                buffEntity.energyRegen = -playerEntity.energyRegen;
            }

            //Bonus gravity
            buffEntity.gravity = skill.FastFallEffect(skillStateManager.fastFallToggle);

            //Bonus movement speed
            buffEntity.maxSpeed = skill.Momentum_Passive(timeSinceLastStruck);

            //Bonus animation speed
            buffEntity.animationSpeed = skill.OnslaughtEffect(skillStateManager.onslaughtToggle);

            m_Rigidbody2D.gravityScale = playerEntity.gravity + buffEntity.gravity;
            m_Anim.speed = playerEntity.animationSpeed + buffEntity.animationSpeed;
            addEnergy((playerEntity.energyRegen + buffEntity.energyRegen) * Time.deltaTime);
            addHealth((playerEntity.healthRegen + buffEntity.healthRegen) * Time.deltaTime);
            timeSinceLastStruck += Time.deltaTime;
            timeSinceLastStrike += Time.deltaTime;
            skillStateManager.fireballCounter += Time.deltaTime;
        }


        public void Move(Controls input) {
            bool doNothing = false;

            //Update crouching status; if there the player can't stand up because of a ceiling, the player continues crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) input.vDown = true;
            m_Anim.SetBool("Crouch", input.vDown);

            //Stunlocked; inputs are ignored during this time
            if (stunned > 0) {
                stunned -= Time.deltaTime;
                input = new Controls();
            }

            if ((skillStateManager.slideSpeed = skill.Slide(m_Rigidbody2D, skillStateManager.sliding, skillStateManager.slideSpeed, m_FacingRight)) == 0) {
                skillStateManager.sliding = false;
            }
            else doNothing = true;

            if ((skillStateManager.dashSpeed = skill.Dash(m_Rigidbody2D, skillStateManager.dashing, skillStateManager.dashSpeed, m_FacingRight)) == 0) {
                skillStateManager.dashing = false;
            }
            else doNothing = true;

            if ((skillStateManager.backdashSpeed = skill.Backdash(m_Rigidbody2D, skillStateManager.backdashing, skillStateManager.backdashSpeed, m_FacingRight)) == 0) {
                skillStateManager.backdashing = false;
            }
            else doNothing = true;

            if ((skillStateManager.airdashSpeed = skill.Airdash(m_Rigidbody2D, skillStateManager.airdashing, skillStateManager.airdashSpeed, m_FacingRight)) == 0) {
                skillStateManager.airdashing = false;
            }
            else doNothing = true;


            if (doNothing) {
            }
            //On the ground, so character can move
            else if (m_Grounded) {
                skillStateManager.land();

                if(input.altMoveDown) {
                    if (input.vDown) {
                    }
                    else if(input.vUp) {
                    }
                    else {
                        skillStateManager.dashing = true;
                    }
                }
                else if (input.altMoveHold) {
                    if (input.vDown) {
                    }
                    else if (input.vUp) {
                    }
                    else {
                    }
                }
                else if (input.fire1Down) {
                    if (input.vDown) {
                        //Slidekick
                        skillStateManager.sliding = true;
                        m_Anim.SetTrigger("LowKickT"); //Start kicking
                        m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                        attacking = true;
                    }
                    else if (input.vUp) {
                        //Crescent Kick
                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Flip Kick") && !m_Anim.GetBool("BasicPunch")) {
                            m_Anim.SetTrigger("FlipKickT"); //Start punching
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true; //Set attacking to true because we are attacking
                        }
                    }
                    else {
                        //Activates the punching hitbox and animation if we are not already punching;
                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Punch") && !m_Anim.GetBool("BasicPunch")) {
                            m_Anim.SetTrigger("BasicPunchT"); //Start punching
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true; //Set attacking to true because we are attacking
                        }
                    }
                }
                else if (input.fire1Hold) {
                    if (input.vDown) {
                    }
                    else if (input.vUp) {
                    }
                    else {
                    }
                }
                //Note that fire2Up is used instead of fire2Down
                else if (input.fire2Up) {
                    if(skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                        skillStateManager.channelCounter = 0;
                    }
                    else if (input.vDown) {
                        //Water Dragon
                        //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 1, 1, 0);
                    }
                    else if (input.vUp) {
                    }
                    else {
                        bool combo = false;
                        if(skillStateManager.fireballCounter < 1) {
                            m_Anim.speed += 1;
                            combo = true;
                        }

                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Punch") && !m_Anim.GetBool("BasicPunch")) {
                            m_Anim.SetTrigger("BasicPunchT"); //Start punching
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true; //Set attacking to true because we are attacking


                            //Fireball
                            skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, 1, 0, 12, 2, 1, 0);
                            skillStateManager.fireballCounter = 0;
                        }

                        if(combo) {
                            m_Anim.speed -= 1;
                        }
                    }
                }
                else if (input.fire2Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;
                    skillStateManager.channelCounter += Time.deltaTime;
                    if(skillStateManager.channelCounter > threshold) {
                        skillStateManager.holdCasting = true;


                        if (input.vDown) {
                            //Iceberg
                            //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 7, 2, 1, 0);
                        }
                        else if (input.vUp) {
                            //Call Lightning
                            //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 3, 1, 0);
                        }
                        else {
                            //Meteor
                            //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 4, 1, 0);
                        }
                    }
                }
                //Note that fire3Up is used instead of fire3Down
                else if (input.fire3Up) {
                    if (skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                        skillStateManager.channelCounter = 0;
                    }
                    else if (input.vDown) {
                        //Barrier Sigil
                        //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 0, 1, 0);
                    }
                    else if (input.vUp) {
                        //Draining Sigil
                        //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 0, 1, 0);
                    }
                    else {
                        //Lesser Spirit Bolt
                        //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 20, 2, 1, 0);
                    }
                }
                else if (input.fire3Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;
                    skillStateManager.channelCounter += Time.deltaTime;
                    if (skillStateManager.channelCounter > threshold) {
                        skillStateManager.holdCasting = true;
                        
                        if (input.vDown) {
                        }
                        else if (input.vUp) {
                            //Teleport Sigil
                            
                        }
                        else {
                            //Greater Spirit Bolt
                            //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 20, 2, 1, 0);
                        }
                    }
                }
                else if (input.fire4Up) {
                    if (skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                        skillStateManager.channelCounter = 0;
                    }
                    else if (input.vDown) {
                    }
                    else if (input.vUp) {
                        //Onslaught
                        skillStateManager.onslaughtToggle = !skillStateManager.onslaughtToggle;
                    }
                    else {
                    }
                }
                else if (input.fire4Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;
                    skillStateManager.channelCounter += Time.deltaTime;
                    if (skillStateManager.channelCounter > threshold) {
                        skillStateManager.holdCasting = true;
                        
                        if (input.vDown) {
                            //Summon Skeletal Dragon
                        }
                        else if (input.vUp) {
                            //Summon Shadow Bat/Ghoul
                        }
                        else {
                            //Summon Spirit Wolf
                        }
                    }
                }
                // If the player should jump...
                else if (input.jumpDown) {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.AddForce(new Vector2(0f, playerEntity.jumpForce));
                }
                else if (input.jumpHold) {
                }
                //Perform movement commands if we are not currently attacking
                else if (!attacking) {
					// Reduce the speed if crouching by the crouchSpeed multiplier
					input.h = (input.vDown ? input.h * playerEntity.crouchSpeed : input.h);

					// The Speed animator parameter is set to the absolute value of the horizontal input.
					m_Anim.SetFloat ("Speed", Mathf.Abs (input.h));

					// Move the character
					m_Rigidbody2D.velocity = new Vector2 (input.h * (playerEntity.maxSpeed + buffEntity.maxSpeed), m_Rigidbody2D.velocity.y);

					// If the input is moving the player right and the player is facing left, flip the player
					if (input.h > 0 && !m_FacingRight) {
						Flip ();
					}
                    // Otherwise if the input is moving the player left and the player is facing right, flip the player
                    else if (input.h < 0 && m_FacingRight) {
						Flip ();
					}
				}
            }
            //In the air
            else {
                if (input.altMoveDown) {
                    if(skillStateManager.secondJumpAvailable) {
                        skillStateManager.airdashing = true;
                        skillStateManager.secondJumpAvailable = false;
                        skillStateManager.fastFallToggle = false;
                    }
                }
                else if (input.altMoveHold) {
                }
                else if (input.fire1Down) {
                    if (skillStateManager.fastFallToggle) {
                        //Swooping Strike
                    }
                    if(input.vUp) {
                        //Crescent Kick; currently an animation bug or something preventing it from being used
                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Flip Kick") && !m_Anim.GetBool("BasicPunch")) {
                            m_Anim.SetTrigger("FlipKickT"); //Start punching
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true; //Set attacking to true because we are attacking
                        }
                    }
                    else {
                        //Cyclone Kick
                    }
                }
                else if (input.fire1Hold) {
                }
                else if (input.fire2Down) {
                    //Shark Crescent
                    //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, 1, 0, 0, 0, 1, 0);
                }
                else if (input.fire2Hold) {
                }
                else if (input.fire3Down) {
                }
                else if (input.fire3Hold) {
                }
                else if (input.jumpDown) {
                }
                else if (input.jumpHold) {
                    if (m_Rigidbody2D.velocity.y < 0) {
                        skillStateManager.gliding = true;
                        skillStateManager.fastFallToggle = false;
                    }
                }
                else if(input.vDown) {
                    if (skillStateManager.fastFallToggle = skill.FastFallToggle(m_Rigidbody2D, skillStateManager.fastFallToggle, m_Grounded, true)) {
                        skillStateManager.gliding = false;
                    }
                }
                else {
                    skillStateManager.gliding = false;
                }

                if(true) {
                    //If gliding, cap fall speed
                    if(skillStateManager.gliding && m_Rigidbody2D.velocity.y < skill.GlideEffect()) {
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, skill.GlideEffect());
                    }                    
                    
                    // The Speed animator parameter is set to the absolute value of the horizontal input.
                    m_Anim.SetFloat("Speed", Mathf.Abs(input.h));

                    // Move the character
                    if (input.h != 0) {
                        m_Rigidbody2D.velocity = new Vector2(input.h * playerEntity.maxSpeed, m_Rigidbody2D.velocity.y);
                    }
                    else {
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x * airSpeedDecayTo, m_Rigidbody2D.velocity.y);
                    }


                    // If the input is moving the player right and the player is facing left, flip the player
                    if (input.h > 0 && !m_FacingRight) {
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right, flip the player
                    else if (input.h < 0 && m_FacingRight) {
                        Flip();
                    }
                }
            }
        }

        //Stop horizontal movement and play an animation when channeling a skill
        private void Channel() {
            m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
        }

        private void Flip() {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        //public void setHitboxEnabled(string hitboxName, bool value) {
        //    m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = value;
        //}
        public Entity PlayerEntity { get; set;}
    }
}
