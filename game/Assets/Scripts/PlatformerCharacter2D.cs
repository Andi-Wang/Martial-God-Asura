using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 500f;                  // Amount of force added when the player jumps.
        [SerializeField] private float m_BackdashDecay = 1f;                // Speed decay during backdash
        [SerializeField] private float m_BackdashMaxSpeed = 17f;            // The fastest the player can travel during a backdash.
        [SerializeField] private float m_BackdashMinSpeed = 6f;             // The slowest the player can travel during a backdash.
        [SerializeField] private float m_GlideGravityScale = 0.05f;         // Gravity scale while gliding.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = 0.7f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private bool m_Floating = false;
        private float m_BackdashSpeed = 0;
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


        private void FixedUpdate()
        {
            m_Grounded = false;

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
        }


        public void Move(float move, bool vDown, bool jump, bool alt_move_down, bool alt_move_hold)
        {
            // If crouching, check to see if the character can stand up
            if (!vDown && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    vDown = true;
                }
            }

            //On the ground, so character can move
            if(m_Grounded)
            {
                m_Rigidbody2D.gravityScale = 1.0f;
                m_Floating = false;

                if(alt_move_down || m_BackdashSpeed > m_BackdashMinSpeed)
                {
                    m_Anim.SetBool("Crouch", false);
                    if (m_BackdashSpeed == 0)
                    {
                        m_BackdashSpeed = m_BackdashMaxSpeed;
                    }
                    else
                    {
                        m_BackdashSpeed -= m_BackdashDecay;
                    }

                    if (m_FacingRight)
                    {
                        m_Rigidbody2D.velocity = new Vector2(-m_BackdashSpeed, m_Rigidbody2D.velocity.y);
                    }
                    else
                    {
                        m_Rigidbody2D.velocity = new Vector2(m_BackdashSpeed, m_Rigidbody2D.velocity.y);
                    }
                }
                // If the player should jump...
                else if (m_Grounded && jump && m_Anim.GetBool("Ground"))
                {
                    // Add a vertical force to the player.
                    m_Grounded = false;
                    m_Anim.SetBool("Ground", false);
                    m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
                }
                else
                {
                    m_BackdashSpeed = 0;
                    // Reduce the speed if crouching by the crouchSpeed multiplier
                    move = (vDown ? move * m_CrouchSpeed : move);

                    // Set whether or not the character is crouching in the animator
                    m_Anim.SetBool("Crouch", vDown);

                    // The Speed animator parameter is set to the absolute value of the horizontal input.
                    m_Anim.SetFloat("Speed", Mathf.Abs(move));

                    // Move the character
                    m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                    // If the input is movting the player right and the player is facing left...
                    if (move > 0 && !m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                    // Otherwise if the input is moving the player left and the player is facing right...
                    else if (move < 0 && m_FacingRight)
                    {
                        // ... flip the player.
                        Flip();
                    }
                }
            }
            //In the air
            else
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                if(vDown && m_Rigidbody2D.velocity.y < 6.0f)
                {
                    if(m_Rigidbody2D.gravityScale <= 1.0f)
                    {
                        m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, -8.0f);
                    }

                    m_Rigidbody2D.gravityScale = 8.0f;
                }
                else {
                    if(alt_move_down)
                    {
                        m_Floating = !m_Floating;

                        if(m_Rigidbody2D.velocity.y < 0)
                        {
                            m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y * 0.25f);
                        }
                    }

                    if (m_Rigidbody2D.velocity.y < 0 && m_Floating)
                    {
                        m_Floating = true;
                        if(m_Rigidbody2D.velocity.y > -0.5f)
                        {
                            m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, -0.5f);
                        }
                        m_Rigidbody2D.gravityScale = m_GlideGravityScale;
                    }
                    else if(!m_Floating)
                    {
                        m_Rigidbody2D.gravityScale = 1.0f;
                    }
                }

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);


                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }


        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
