using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityStandardAssets._2D {
    public class PlatformerCharacter2D : MonoBehaviour {
        public Slider healthSlider;

        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private Entity playerEntity = new Entity(100f, 100f, 0f, 5f, 0f, 500f, 10f, 1f, 0.7f);
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

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            isDead = false;
        }

        void TakeDamage(float amount)
        {
            playerEntity.health -= amount;
            healthSlider.value = playerEntity.health;

            //TODO: player hurt sound,animation
            if (playerEntity.health <= 0 && !isDead)
            {
                Death();
            }
        }
        void Death()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;
            
            // Tell the animator that the player is dead.
            //anim.SetTrigger("Die");

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
			if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Basic Punch")) {
				attacking = false;
			}
			// Set BasicPunch to false in Idle to ensure no interrupt during Basic Punch animation
			else if (m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) {
				m_Anim.SetBool ("BasicPunch", false);
			}

            //Force punch animation to play without override from more punches
            //if(!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Punch")) {
			//	m_Anim.SetBool ("BasicPunch", false);
                //m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = false;
            //}



        }


        public void Move(Controls input) {
            // If crouching, check to see if the character can stand up
            if (!input.vDown && m_Anim.GetBool("Crouch")) {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround)) {
                    input.vDown = true;
                }
            }

            //On the ground, so character can move
            if(m_Grounded) {
                m_Rigidbody2D.gravityScale = 1.0f;
                skillStateManager.secondJumpAvailable = true;
                if (!attacking) { //Perform movement commands if we are not currently attacking
					if (input.altMoveDown || skillStateManager.backdashing) {
                        if(m_Anim.GetBool("Crouch")) {
                            skillStateManager.slideSpeed = Skill.Slide(m_Rigidbody2D, m_FacingRight, skillStateManager.slideSpeed, input.altMoveDown);
                            if(skillStateManager.slideSpeed > 0) {
                                skillStateManager.sliding = true;
                            }
                            else {
                                skillStateManager.sliding = false;
                            }
                        }
                        else {
                            m_Anim.SetBool("Crouch", false);
                            skillStateManager.backdashSpeed = Skill.Backdash(m_Rigidbody2D, m_FacingRight, skillStateManager.backdashSpeed, input.altMoveDown);
                            if (skillStateManager.backdashSpeed > 0) {
                                skillStateManager.backdashing = true;
                            }
                            else {
                                skillStateManager.backdashing = false;
                            }
                        }
					}
                    //If the character punches; later, will make this just attack buttons in general in one else if
                    else if (input.fire1Down) {
					    //Activates the hitbox and animation if we are not already punching;
					    if (!m_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Basic Punch") && !m_Anim.GetBool ("BasicPunch")) {
						    m_Anim.SetTrigger ("BasicPunchT"); //Start punching
						    m_Anim.SetBool ("BasicPunch", true); //Set BasicPunch to true because we are punching
						    attacking = true; //Set attacking to true because we are attacking
					    }
					    //m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = true;
				    }
                    // If the player should jump...
                    else if (m_Grounded && input.jumpDown && m_Anim.GetBool ("Ground")) {
				        // Add a vertical force to the player.
					    m_Grounded = false;
					    m_Anim.SetBool ("Ground", false);
					    m_Rigidbody2D.AddForce (new Vector2 (0f, playerEntity.jumpForce));
				    }
                    else {
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
            }
            //In the air
            else {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(input.h));

                if (input.vDown) {
                    Skill.FastFall(m_Rigidbody2D, input.h * playerEntity.maxSpeed);
                }
                else if (skillStateManager.airdashing || (skillStateManager.secondJumpAvailable && input.jumpDown)) {
                    skillStateManager.airdashSpeed = Skill.Airdash(m_Rigidbody2D, m_FacingRight, skillStateManager.airdashSpeed, skillStateManager.secondJumpAvailable && input.jumpDown);
                    skillStateManager.secondJumpAvailable = false;
                    if (skillStateManager.airdashSpeed > 0) {
                        skillStateManager.airdashing = true;
                    }
                    else {
                        skillStateManager.airdashing = false;
                    }
                }
                else if (input.altMoveDown) {
                    Skill.Glide(m_Rigidbody2D, playerEntity.gravity, input.h * playerEntity.maxSpeed);
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
        public Entity PlayerEntity { get { return playerEntity; }
                                     set { PlayerEntity = value; }
        }
    }
}
