using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        private bool m_Alt_Move_Down;
        private bool m_Alt_Move_Hold;

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
        }


        private void Update()
        {
            if (!m_Alt_Move_Down)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Alt_Move_Down = CrossPlatformInputManager.GetButtonDown("AltMove");
            }

            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool vDown = CrossPlatformInputManager.GetAxis("Vertical") < 0;
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            m_Alt_Move_Hold = CrossPlatformInputManager.GetButton("AltMove");
            // Pass all parameters to the character control script.
            m_Character.Move(h, vDown, m_Jump, m_Alt_Move_Down, m_Alt_Move_Hold);
            m_Jump = false;
            m_Alt_Move_Down = false;
        }
    }
}
