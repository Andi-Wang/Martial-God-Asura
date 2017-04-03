using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets._2D {
    public class PlatformerCharacter2D : MonoBehaviour {
        public Image healthbar;
        public Image energybar;
        public GameObject m_Fireball2;
        public GameObject m_WaterDragon;
        public GameObject m_Iceberg;
        public GameObject m_Lightning;
        public GameObject m_Meteor;
        public GameObject m_SharkAir;
        public GameObject m_BarrierSigil;
        public GameObject m_DrainingSigil;
        public GameObject m_TeleportSigil;
        public GameObject m_LesserSpiritbolt;
        public GameObject m_Tornado;
        public GameObject m_Flamewheel;
        public GameObject m_EtherealFist;

        public Image damageImage;
        public Color flashColour = new Color(1f, 0, 0, 0.3f);
        public float flashSpeed = 2f;

        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private Entity playerEntity = new Entity(100f, 100f, 0f, 25f, 0f, 0f, 700f, 10f, 2f, 0.00001f); //very small crouch speed so the player can turn around when crouched
        public Entity buffEntity = new Entity();
        private Skill skill;
        private Skill.SkillStateManager skillStateManager = new Skill.SkillStateManager();
        private Skill.CooldownManager cooldownManager = new Skill.CooldownManager();

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.
        const float airSpeedDecayTo = 0.993f;
        const float maxFallSpeed = -25f;

        bool isDead;
		bool attacking = false; // used to detect if we are beginning an attack, in order to prevent input buffering
        float attackingTimer = 0f;

        const float invincibilityDurationWhenHit = 1f;
        const float stunDurationWhenHit = 0.5f;
        const float knockbackForceWhenHit = 700f;
        const float airKnockdownVelocity = 6f;
        const float energyRegenCooldown = 1.5f;
        float timeSinceLastStruck = 1f;
        float lastDamageTaken = 0;
        float timeSinceLastStrike = 1f;
        float timeSinceLastEnergyUse = 1f;
        float stunned = 0;
        bool damaged = false;


        public void strikeEnemy() {
            timeSinceLastStrike = 0;
            skillStateManager.ironStrikesStacks++;
        }


        void Awake() {
            skill = new Skill();
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            isDead = false;
        }
        void Update() {
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

        public void TakeDamage(Rigidbody2D source, float amount) {
            amount = reduceDamageByArmor(amount);

            if(timeSinceLastStruck >= invincibilityDurationWhenHit) {
                damaged = true;

                playerEntity.health -= amount;
                lastDamageTaken = amount;
                healthbar.fillAmount = playerEntity.health / playerEntity.maxHealth;

                timeSinceLastStruck = 0;
                stunned = stunDurationWhenHit;

                if(source) {
                    bool fromRight = source.position.x > m_Rigidbody2D.position.x;

                    //Interrupt certain actions like sliding
                    skillStateManager.getHit();

                    if (m_Grounded) {
                        skillStateManager.backdashSpeed = skill.Backdash(m_Rigidbody2D, true, 0, fromRight);
                        skillStateManager.backdashing = true;
                    }
                    else {
                        m_Rigidbody2D.velocity = new Vector2(0, -airKnockdownVelocity);

                        if (fromRight) {
                            m_Rigidbody2D.AddForce(new Vector2(-knockbackForceWhenHit, 0));
                        }
                        else {
                            m_Rigidbody2D.AddForce(new Vector2(knockbackForceWhenHit, 0));
                        }
                    }
                }
            }

            //TODO: player hurt sound,animation
            if (playerEntity.health <= 0 && !isDead) {
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
                timeSinceLastEnergyUse = 0f;
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

		IEnumerator Cooldown() {
			yield return new WaitForSeconds (0.5f);
			m_Anim.SetBool ("BasicPunch", false);
		}

        private void FixedUpdate() {
            m_Grounded = false;

            if (!GameManager.instance.playersTurn)
            {
                m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
                m_Anim.SetFloat("Speed", 0f);
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
			if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Basic Punch") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Flip Kick") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Block")) {
				attacking = false;
			} else if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Low Kick")) {
				attacking = false;
			}
			// Set BasicPunch to false in Idle to ensure no interrupt during Basic Punch animation
			else if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Jumping") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle") || m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Crouch")) {
				m_Anim.SetBool ("BasicPunch", false);
                attacking = false;
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
            if (timeSinceLastEnergyUse > energyRegenCooldown) { addEnergy((playerEntity.energyRegen + buffEntity.energyRegen) * Time.fixedDeltaTime); }
            addHealth((playerEntity.healthRegen + buffEntity.healthRegen) * Time.fixedDeltaTime);
            timeSinceLastStruck += Time.fixedDeltaTime;
            timeSinceLastStrike += Time.fixedDeltaTime;
            timeSinceLastEnergyUse += Time.fixedDeltaTime;
            cooldownManager.Tick(Time.fixedDeltaTime);
            attackingTimer -= Time.fixedDeltaTime;
        }


        public void Move(Controls input) {
            bool doNothing = false;

            //Update crouching status; if there the player can't stand up because of a ceiling, the player continues crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) input.vDown = true;
            m_Anim.SetBool("Crouch", input.vDown);

            //Stunlocked; inputs are ignored during this time
            if (stunned > 0) {
                stunned -= Time.fixedDeltaTime;
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
                        if(energyCost(Skill.dashCost)) {
                            skillStateManager.dashing = true;
                        }
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
                        if(cooldownManager.slideKickTimer < 0) {
                            cooldownManager.slideKickTimer = Skill.CooldownManager.slideKickCooldown;
                            skillStateManager.sliding = true;
                            m_Anim.SetTrigger("LowKickT"); //Start kicking
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true;
                        }
                    }
                    else if (input.vUp) {
                        //Crescent Kick
                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Flip Kick") && !m_Anim.GetBool("BasicPunch")) {
                            if (cooldownManager.flipKickTimer < 0) {
                                cooldownManager.flipKickTimer = Skill.CooldownManager.flipKickCooldown;
                                m_Anim.SetTrigger("FlipKickT"); //Start punching
                                m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                                attacking = true; //Set attacking to true because we are attacking
                            }
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
                    skillStateManager.channelTimer = 0;
                    if(m_Anim.GetBool("ShoutChannel")) {
                        m_Anim.SetBool("ShoutChannel", false);
                    }
                    if (skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                    }
                    else if (input.vDown) {
                        //Water Dragon
                        if(cooldownManager.waterDragonTimer < 0) {
                            if(energyCost(Skill.waterDragonCost)) {
                                Cast();
                                cooldownManager.waterDragonTimer = Skill.CooldownManager.waterDragonCooldown;
                                skill.Projectile(m_Rigidbody2D, m_FacingRight, m_WaterDragon, 2f, 0f, 2f, 0f);
                            }
                        }
                    }
                    else if (input.vUp) {
                    }
                    else {
                        //Fireball
                        if (cooldownManager.fireballTimer < Skill.CooldownManager.fireballReducedCooldown - Skill.CooldownManager.fireballCooldown) {
                            skillStateManager.fireballCounter = 0;
                        }
                        if(cooldownManager.fireballTimer < 0) {
                            if(energyCost(Skill.fireballCost)) {
                                Cast();
                                if (skillStateManager.fireballCounter++ > 1) {
                                    skillStateManager.fireballCounter = 0;
                                    cooldownManager.fireballTimer = Skill.CooldownManager.fireballCooldown;
                                }
                                else {
                                    cooldownManager.fireballTimer = Skill.CooldownManager.fireballReducedCooldown;
                                }
                                skill.Projectile(m_Rigidbody2D, m_FacingRight, m_Fireball2, 2f, 0f, 12f, 0f);
                            }
                        }
                    }
                }
                else if (input.fire2Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;

                    skillStateManager.channelTimer += Time.fixedDeltaTime;
                    if(skillStateManager.channelTimer > threshold) {
                        if (input.vDown) {
                            //Iceberg
                            if(cooldownManager.icebergTimer < 0) {
                                if(energyCost(Skill.icebergCost)) {
                                    ChannelCast();
                                    cooldownManager.icebergTimer = Skill.CooldownManager.icebergCooldown;
                                    skill.Projectile(m_Rigidbody2D, m_FacingRight, m_Iceberg, 1.5f, -0.8f, 0.01f, 3f);
                                }
                            }
                        }
                        else if (input.vUp) {
                            //Call Lightning
                            if(cooldownManager.callLightningTimer < 0) {
                                if(energyCost(Skill.callLightningCost)) {
                                    ChannelCast();
                                    cooldownManager.callLightningTimer = Skill.CooldownManager.callLightningCooldown;
                                    skill.Projectile(m_Rigidbody2D, m_FacingRight, m_Lightning, 3f, 2f, 0f, 0f);
                                }
                            }
                        }
                        else {
                            //Meteor
                            if(cooldownManager.meteorTimer < 0) {
                                if(energyCost(Skill.meteorCost)) {
                                    ChannelCast();
                                    cooldownManager.meteorTimer = Skill.CooldownManager.meteorCooldown;
                                    skill.Projectile(m_Rigidbody2D, m_FacingRight, m_Meteor, -2f, 3f, 8f,-3.8f);
                                }
                            }
                        }
                    }
                }
                //Note that fire3Up is used instead of fire3Down
                else if (input.fire3Up) {
                    skillStateManager.channelTimer = 0;
                    if (m_Anim.GetBool("ShoutChannel")) {
                        m_Anim.SetBool("ShoutChannel", false);
                    }
                    if (skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                    }
                    else if (input.vDown) {
                        //Barrier Sigil
                        if(cooldownManager.barrierSigilTimer < 0) {
                            if(energyCost(Skill.barrierSigilCost)) {
                                Cast();
                                cooldownManager.barrierSigilTimer = Skill.CooldownManager.barrierSigilCooldown;
                                skill.Projectile(m_Rigidbody2D, m_FacingRight, m_BarrierSigil, 0f, 0f, 0f, 0f);
                            }
                        }
                    }
                    else if (input.vUp) {
                        //Draining Sigil
                        if(cooldownManager.drainingSigilTimer < 0) {
                            if(energyCost(Skill.drainingSigilCost)) {
                                Cast();
                                cooldownManager.drainingSigilTimer = Skill.CooldownManager.drainingSigilCooldown;
                                skill.Projectile(m_Rigidbody2D, m_FacingRight, m_DrainingSigil, 0f, 0f, 0f, 0f);
                            }
                        }
                    }
                    else {
                        //Lesser Spirit Bolt
                        if(cooldownManager.lesserSpiritboltTimer < 0) {
                            if(energyCost(Skill.lesserSpiritboltCost)) {
                                Cast();
                                cooldownManager.lesserSpiritboltTimer = Skill.CooldownManager.lesserSpiritboldCooldown;
                                skill.Projectile(m_Rigidbody2D, m_FacingRight, m_LesserSpiritbolt, -3f, 0f, 20f, 0f);
                            }
                        }
                    }
                }
                else if (input.fire3Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;
                    skillStateManager.channelTimer += Time.fixedDeltaTime;
                    if (skillStateManager.channelTimer > threshold) {                        
                        if (input.vDown) {
                        }
                        else if (input.vUp) {
                            //Teleport Sigil (creation and reactivation)
                            if(skillStateManager.teleportSigilExists) {
                                ChannelCast();
                                if (m_FacingRight != skillStateManager.teleportFacingRight) { Flip(); }
                                skill.TeleportToSigil(m_Rigidbody2D, skillStateManager.teleportSigil);
                                skillStateManager.teleportSigilExists = false;
                            }
                            else if(cooldownManager.teleportSigilTimer < 0) {
                                if(energyCost(Skill.teleportSigilCost)) {
                                    ChannelCast();
                                    cooldownManager.teleportSigilTimer = Skill.CooldownManager.teleportSigilCooldown;
                                    skillStateManager.teleportSigil = skill.TeleportSigil(m_Rigidbody2D, m_FacingRight, m_TeleportSigil);
                                    skillStateManager.teleportFacingRight = m_FacingRight;
                                    skillStateManager.teleportSigilExists = true;
                                }
                            }
                        }
                        else {
                            //Greater Spirit Bolt
                            //skill.Projectile(m_Rigidbody2D, m_FacingRight, Rigidbody2D projectile, -3f, 0f, 20f, 0f);
                        }
                    }
                }
                else if (input.fire4Up) {
                    skillStateManager.channelTimer = 0;
                    if (m_Anim.GetBool("ShoutChannel")) {
                        m_Anim.SetBool("ShoutChannel", false);
                    }
                    if (skillStateManager.holdCasting) {
                        //Don't also cast this skill if a hold-cast skill was used
                        skillStateManager.holdCasting = false;
                    }
                    else if (input.vDown) {
                    }
                    else if (input.vUp) {
                        //Onslaught
                        if(cooldownManager.onslaughtToggleTimer < 0) {
                            Cast();
                            cooldownManager.onslaughtToggleTimer = Skill.CooldownManager.onslaughtToggleCooldown;
                            skillStateManager.onslaughtToggle = !skillStateManager.onslaughtToggle;
                        }
                    }
                    else {
                    }
                }
                else if (input.fire4Hold && !skillStateManager.holdCasting) {
                    //Play channel animation here
                    Channel();

                    float threshold = 0.8f;
                    skillStateManager.channelTimer += Time.fixedDeltaTime;
                    if (skillStateManager.channelTimer > threshold) {
                        if (input.vDown) {
                            //Summon Skeletal Dragon
                            ChannelCast();
                        }
                        else if (input.vUp) {
                            //Summon Shadow Bat/Ghoul
                            ChannelCast();
                        }
                        else {
                            //Summon Spirit Wolf
                            ChannelCast();
                        }
                    }
                }
                // If the player should jump...
                else if (input.jumpDown) {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
					m_Anim.SetTrigger("JumpT");
                    m_Rigidbody2D.AddForce(new Vector2(0f, playerEntity.jumpForce));
                }
                else if (input.jumpHold) {
                }
                //Perform movement commands if we are not currently attacking
                else if (!attacking && attackingTimer < 0) {
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
                if(skillStateManager.swoopingStrike) {
                    if (skillStateManager.swoopingStrikeDuration < 0) {
                        skillStateManager.swoopingStrike = false;
                    }
                    skillStateManager.swoopingStrikeDuration = skill.SwoopingStrike(m_Rigidbody2D, skillStateManager.swoopingStrikeDuration, Time.fixedDeltaTime, m_FacingRight, false);
                }
                if (input.altMoveDown) {
                    if(skillStateManager.secondJumpAvailable) {
                        if(energyCost(Skill.airdashCost)) {
                            skillStateManager.airdashing = true;
                            skillStateManager.secondJumpAvailable = false;
                            skillStateManager.fastFallToggle = false;
                        }
                    }
                }
                else if (input.altMoveHold) {
                }
                else if (input.fire1Down) {
                    if (skillStateManager.fastFallToggle) {
                        //Swooping Strike
						if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Soaring Kick") && !m_Anim.GetBool("BasicPunch")) {
							m_Anim.SetTrigger("SoarKickT"); //Start punching
							m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
							attacking = true; //Set attacking to true because we are attacking
                            skillStateManager.swoopingStrikeDuration = skill.SwoopingStrike(m_Rigidbody2D, 0, Time.fixedDeltaTime, m_FacingRight, true);
                            skillStateManager.swoopingStrike = true;
						}
                    }
					if (input.vUp) {
						//Crescent Kick; currently an animation bug or something preventing it from being used
						if (!m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Flip Kick") && !m_Anim.GetBool ("BasicPunch")) {
                            if(cooldownManager.flipKickTimer < 0) {
                                cooldownManager.flipKickTimer = Skill.CooldownManager.flipKickCooldown;
                                m_Anim.SetTrigger("FlipKickT"); //Start punching
                                m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                                attacking = true; //Set attacking to true because we are attacking
                            }
						}
					} else {
						//Cyclone Kick
						if (!m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Spin Kick") && !m_Anim.GetBool ("BasicPunch")) {
                            if (cooldownManager.cycloneKickTimer < 0) {
                                m_Anim.SetTrigger ("SpinKickT"); //Start punching
							    m_Anim.SetBool ("BasicPunch", true); //Set BasicPunch to true because we are punching
                                attacking = true; //Set attacking to true because we are attacking

                                if (energyCost(Skill.cycloneKickTornadoCost)) {
                                    cooldownManager.cycloneKickTimer = Skill.CooldownManager.cycloneKickCooldown;
                                    skill.Projectile(m_Rigidbody2D, m_FacingRight, m_Tornado, 1f, 0f, 6f, 0f);
                                }
                            }                            
                        }
                    }
                }
                else if (input.fire1Hold) {
                }
                else if (input.fire2Down) {
                    //Shark Crescent
                    if(cooldownManager.airSharkTimer < 0) {
                        if(energyCost(Skill.airSharkCost)) {
                            cooldownManager.airSharkTimer = Skill.CooldownManager.airSharkCooldown;
                            skill.Projectile(m_Rigidbody2D, m_FacingRight, m_SharkAir, 0f, -1f, 0f, 0f);
                        }
                    }                    
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
                        if(energyCost(Skill.glideCostPerSecond * Time.fixedDeltaTime)) {
                            skillStateManager.gliding = true;
                            skillStateManager.fastFallToggle = false;
                        }
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

                if(!skillStateManager.swoopingStrike) {
                    //If gliding, cap fall speed
                    if(skillStateManager.gliding && m_Rigidbody2D.velocity.y < skill.GlideEffect()) {
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, skill.GlideEffect());
                    }
                    else if(m_Rigidbody2D.velocity.y < maxFallSpeed) {
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, maxFallSpeed);
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
            m_Anim.SetFloat("Speed", 0f);

            if (skillStateManager.channelTimer > 0.4f) {
                if (!m_Anim.GetBool("ShoutChannel")) {
                    if (!m_Anim.GetBool("Crouch")) {
                        m_Anim.SetTrigger("ShoutT");
                        m_Anim.SetBool("ShoutChannel", true);
                    }
                }
            }
        }
        private void ChannelCast() {
            skillStateManager.holdCasting = true;
            if (!m_Anim.GetBool("Crouch")) {
                m_Anim.SetBool("ShoutChannel", false);
            }
            attackingTimer = 0.4f;
        }
    
        private void Cast() {
            m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);
            m_Anim.SetFloat("Speed", 0f);

            if (!m_Anim.GetBool("Crouch")) {
                m_Anim.SetTrigger("ShoutT");
                m_Anim.SetBool("ShoutChannel", false);
            }

            attackingTimer = 0.9f;
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
