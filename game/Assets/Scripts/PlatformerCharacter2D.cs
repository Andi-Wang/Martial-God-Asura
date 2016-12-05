using System;
using UnityEngine;
using System.Collections.Generic;

namespace UnityStandardAssets._2D {
    public class PlatformerCharacter2D : MonoBehaviour {
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private Entity playerEntity = new Entity(100f, 100f, 0f, 5f, 0f, 500f, 10f, 1f, 0.7f);
        private Skill.SkillStateManager skillManager = new Skill.SkillStateManager();

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }


        private void FixedUpdate() {
            m_Grounded = false;
            m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);

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

			m_Anim.SetBool ("BasicPunch", false);

            /*
            //Disable hitbox when animation finishes; currently doesn't seem to work
            if(!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("BasicPunch")) {
                m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = false;
            }*/



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
                if(input.altMoveDown || skillManager.backdashing) {
                    m_Anim.SetBool("Crouch", false);
                    skillManager.backdashSpeed = Skill.Backdash(m_Rigidbody2D, m_FacingRight, skillManager.backdashSpeed, input.altMoveDown);
                    if(skillManager.backdashSpeed > 0) {
                        skillManager.backdashing = true;
                    }
                    else {
                        skillManager.backdashing = false;
                    }
                }
                //If the character punches; later, will make this just attack buttons in general in one else if
                else if(input.fire1Down) {
                    //Activates the hitbox and animation; hitbox activation doesn't seem to work consistently
                    m_Anim.SetBool("BasicPunch", true);
                    //m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = true;
                }
                // If the player should jump...
                else if (m_Grounded && input.jumpDown && m_Anim.GetBool("Ground")) {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.AddForce(new Vector2(0f, playerEntity.jumpForce));
                }
                else {
                    // Reduce the speed if crouching by the crouchSpeed multiplier
                    input.h = (input.vDown ? input.h * playerEntity.crouchSpeed : input.h);

                    // Set whether or not the character is crouching in the animator
                    m_Anim.SetBool("Crouch", input.vDown);

                    // The Speed animator parameter is set to the absolute value of the horizontal input.
                    m_Anim.SetFloat("Speed", Mathf.Abs(input.h));

                    // Move the character
                    m_Rigidbody2D.velocity = new Vector2(input.h * playerEntity.maxSpeed, m_Rigidbody2D.velocity.y);

                    // If the input is movting the player right and the player is facing left...
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
            //In the air
            else
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(input.h));

                if(input.vDown) {
                    Skill.FastFall(m_Rigidbody2D, input.h * playerEntity.maxSpeed);
                }
                else {
                    if(input.altMoveDown) {
                        Skill.Glide(m_Rigidbody2D, playerEntity.gravity, input.h * playerEntity.maxSpeed);
                    }
                }

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(input.h * playerEntity.maxSpeed, m_Rigidbody2D.velocity.y);


                // If the input is moving the player right and the player is facing left...
                if (input.h > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (input.h < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
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

        public void setHitboxEnabled(string hitboxName, bool value) {
            m_Rigidbody2D.gameObject.transform.Find("PunchHitbox").GetComponent<Collider2D>().enabled = value;
        }
    }
}
