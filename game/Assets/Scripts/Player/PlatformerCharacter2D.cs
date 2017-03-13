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
        [SerializeField] private Entity playerEntity = new Entity(100f, 100f, 0f, 5f, 0f, 500f, 10f, 1f, 0.7f);
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
        float timeSinceLastHit = 1f;
        float stunned = 0;
        bool damaged = false;

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
        public void TakeDamage(float amount)
        {
            damaged = true;

            if(timeSinceLastHit >= invincibilityDurationWhenHit) {
                playerEntity.health -= amount;
                healthbar.fillAmount = playerEntity.health / playerEntity.maxHealth;

                timeSinceLastHit = 0;
                stunned = stunDurationWhenHit;

                skillStateManager = new Skill.SkillStateManager();

                if(m_Grounded) {
                    skillStateManager.backdashSpeed = skill.Backdash(m_Rigidbody2D, m_FacingRight, skillStateManager.backdashSpeed, true);
                    if (skillStateManager.backdashSpeed > 0) {
                        skillStateManager.backdashing = true;
                    }
                    else {
                        skillStateManager.backdashing = false;
                    }
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

            addEnergy(playerEntity.energyRegen * Time.deltaTime);
            addHealth(playerEntity.healthRegen * Time.deltaTime);
            timeSinceLastHit += Time.deltaTime;
        }


        public void Move(Controls input) {
            // If crouching, check to see if the character can stand up
            if (!input.vDown && m_Anim.GetBool("Crouch")) {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) {
                    input.vDown = true;
                }
            }

            //Stunlocked; inputs are ignored during this time
            if(stunned > 0) {
                stunned -= Time.deltaTime;
                input = new Controls();
            }

            //On the ground, so character can move
            if(m_Grounded) {
                m_Rigidbody2D.gravityScale = 1.0f;
                skillStateManager.secondJumpAvailable = true;

                //Slidekick
                if ((input.altMoveDown && m_Anim.GetBool("Crouch")) || skillStateManager.sliding) {
                    bool enhanced = true;          //replace with a check to see if we have the slide enhancement passive
                    bool slideCancel = false;

                    if (enhanced && skillStateManager.sliding && input.altMoveUp) {
                        slideCancel = true;
                        //cancel the slidekick animation here and start the animation for the shockwave
                    }

                    skillStateManager.slideSpeed = skill.Slide(m_Rigidbody2D, m_FacingRight, skillStateManager.slideSpeed, input.altMoveDown, enhanced, slideCancel);

                    if (skillStateManager.slideSpeed > 0) {
                        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Low Kick") && !m_Anim.GetBool("BasicPunch") && !skillStateManager.sliding) {
                            m_Anim.SetTrigger("LowKickT"); //Start kicking
                            m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                            attacking = true; //Set attacking to true because we are attacking
                        }
                        skillStateManager.sliding = true;
                    }
                    else {
                        skillStateManager.sliding = false;
                    }
                }
                //Backdash
                else if (input.altMoveDown || skillStateManager.backdashing) {
                    m_Anim.SetBool("Crouch", false);
                    skillStateManager.backdashSpeed = skill.Backdash(m_Rigidbody2D, m_FacingRight, skillStateManager.backdashSpeed, input.altMoveDown);
                    if (skillStateManager.backdashSpeed > 0) {
                        skillStateManager.backdashing = true;
                    }
                    else {
                        skillStateManager.backdashing = false;
                    }
                }

                else if (input.vUp) {
                    if(input.fire1Down) {
                        //Fireball nova; see Projectile function in Skill for a list of parameters
                        skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, 1, 0, 16, 2, 5, 20);
                    }
                    else if(input.fire2Hold) {
                        skillStateManager.counter = skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, skillStateManager.counter, 25, 20, 1, 1, 0);
                    }
                    else if(input.fire3Down) {
                        skill.Projectile(m_Rigidbody2D, m_FacingRight, m_fireball, 1, 0, 8, 2, 1, 0);
                    }
                }
                //Activates the punching hitbox and animation if we are not already punching;
                else if (input.fire1Down) {
                    addEnergy(-10);
                    if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Punch") && !m_Anim.GetBool("BasicPunch")) {
                        m_Anim.SetTrigger("BasicPunchT"); //Start punching
                        m_Anim.SetBool("BasicPunch", true); //Set BasicPunch to true because we are punching
                        attacking = true; //Set attacking to true because we are attacking
                    }

                    //m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = true;
                }
                else if (input.fire2Down) {
                    //Add kick stuff here
					if (!m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Flip Kick") && !m_Anim.GetBool ("BasicPunch")) {
						m_Anim.SetTrigger ("FlipKickT"); //Start punching
						m_Anim.SetBool ("BasicPunch", true); //Set BasicPunch to true because we are punching
						attacking = true; //Set attacking to true because we are attacking
					}
                }
                else if (input.fire3Down) {
                    //Add block stuff here
					if (!m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Block") && !m_Anim.GetBool ("BasicPunch")) {
						m_Anim.SetTrigger ("BlockT"); //Start punching
						m_Anim.SetBool ("BasicPunch", true); //Set BasicPunch to true because we are punching
						attacking = true; //Set attacking to true because we are attacking
					}
                }
                // If the player should jump...
                else if (m_Grounded && input.jumpDown && m_Anim.GetBool ("Ground")) {
				    // Add a vertical force to the player.
					m_Grounded = false;
					m_Anim.SetBool ("Ground", false);
					m_Rigidbody2D.AddForce (new Vector2 (0f, playerEntity.jumpForce));
				}
                //Perform movement commands if we are not currently attacking
                else if (!attacking) {
					// Reduce the speed if crouching by the crouchSpeed multiplier
					input.h = (input.vDown ? input.h * playerEntity.crouchSpeed : input.h);

					// Set whether or not the character is crouching in the animator
					m_Anim.SetBool ("Crouch", input.vDown);

					// The Speed animator parameter is set to the absolute value of the horizontal input.
					m_Anim.SetFloat ("Speed", Mathf.Abs (input.h));

					// Move the character
					m_Rigidbody2D.velocity = new Vector2 (input.h * playerEntity.maxSpeed, m_Rigidbody2D.velocity.y);

					// If the input is moving the player right and the player is facing left...
					if (input.h > 0 && !m_FacingRight) {
						// ... flip the player.
						Flip ();
					}
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (input.h < 0 && m_FacingRight) {
						// ... flip the player.
						Flip ();
					}
				}
            }
            //In the air
            else {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(input.h));

                if (input.vDown) {
                    skill.FastFall(m_Rigidbody2D, input.h * playerEntity.maxSpeed);
                }
                else if (skillStateManager.airdashing || (skillStateManager.secondJumpAvailable && input.jumpDown)) {
                    skillStateManager.airdashSpeed = skill.Airdash(m_Rigidbody2D, m_FacingRight, skillStateManager.airdashSpeed, skillStateManager.secondJumpAvailable && input.jumpDown);
                    skillStateManager.secondJumpAvailable = false;
                    if (skillStateManager.airdashSpeed > 0) {
                        skillStateManager.airdashing = true;
                    }
                    else {
                        skillStateManager.airdashing = false;
                    }
                }
                else if (input.altMoveDown) {
                    skill.Glide(m_Rigidbody2D, playerEntity.gravity, input.h * playerEntity.maxSpeed);
                }
                else {
                    // Move the character
                    if (input.h != 0) {
                        m_Rigidbody2D.velocity = new Vector2(input.h * playerEntity.maxSpeed, m_Rigidbody2D.velocity.y);
                    }
                    else {
                        m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x * airSpeedDecayTo, m_Rigidbody2D.velocity.y);
                    }


                    // If the input is moving the player right and the player is facing left...
                    if (input.h > 0 && !m_FacingRight) {
                        // ... flip the player.
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (input.h < 0 && m_FacingRight) {
                        // ... flip the player.
                        Flip();
                    }
                }
            }
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
