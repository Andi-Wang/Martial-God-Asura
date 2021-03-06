using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        public DialogDisplayManager ddm;
        bool gamePaused;
        public MenuManager pauseMenu;
        public GameObject skillMenu;
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;
        private bool m_Alt_Move_Down;
        private bool m_Alt_Move_Hold;
        private Controls input;

        private void Awake() {
            m_Character = GetComponent<PlatformerCharacter2D>();
            input = new Controls();
            gamePaused = false;
        }

        private void Update() {
            if (!GameManager.instance.playersTurn)
            {
                if (ddm.displaying && Input.GetButtonUp("Jump"))
                {
                    ddm.displayNext();
                }
                else
                {
                    input.resetButtonDown();
                    return;
                }
                
            }

            if (Input.GetButtonUp("Cancel"))
            {
                ToggleSkillMenu();
            }

            // Read button down inputs in Update so button presses aren't missed.
            if (!input.altMoveDown) {
                input.altMoveDown = CrossPlatformInputManager.GetButtonDown("AltMove");
            }
            if(!input.altMoveUp) {
                input.altMoveUp = CrossPlatformInputManager.GetButtonUp("AltMove");
            }

            if(!input.interactDown) {
                input.interactDown = CrossPlatformInputManager.GetButtonDown("Interact");
            }
            if(!input.jumpDown) {
                input.jumpDown = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if(!input.fire1Up) {
                input.fire1Up = CrossPlatformInputManager.GetButtonUp("Fire1");
            }
            if(!input.fire2Up) {
                input.fire2Up = CrossPlatformInputManager.GetButtonUp("Fire2");
            }
            if (!input.fire3Up) {
                input.fire3Up = CrossPlatformInputManager.GetButtonUp("Fire3");
            }
            if (!input.fire4Up) {
                input.fire4Up = CrossPlatformInputManager.GetButtonUp("Fire4");
            }

            if (!input.fire1Down) {
                input.fire1Down = CrossPlatformInputManager.GetButtonDown("Fire1");
            }
            if (!input.fire2Down) {
                input.fire2Down = CrossPlatformInputManager.GetButtonDown("Fire2");
            }
            if (!input.fire3Down) {
                input.fire3Down = CrossPlatformInputManager.GetButtonDown("Fire3");
            }
            if (!input.fire4Down) {
                input.fire4Down = CrossPlatformInputManager.GetButtonDown("Fire4");
            }
        }


        private void FixedUpdate() {
            if (!GameManager.instance.playersTurn) {
                input.resetButtonDown();
                return;
            }


            input.h = CrossPlatformInputManager.GetAxis("Horizontal");
            input.v = CrossPlatformInputManager.GetAxis("Vertical");
            input.vDown = input.v < 0;
            input.vUp = input.v > 0;
            input.altMoveHold = CrossPlatformInputManager.GetButton("AltMove");
            input.jumpHold = CrossPlatformInputManager.GetButton("Jump");
            input.interactHold = CrossPlatformInputManager.GetButton("Interact");
            input.fire1Hold = CrossPlatformInputManager.GetButton("Fire1");
            input.fire2Hold = CrossPlatformInputManager.GetButton("Fire2");
            input.fire3Hold = CrossPlatformInputManager.GetButton("Fire3");
            input.fire4Hold = CrossPlatformInputManager.GetButton("Fire4");

            m_Character.Move(input);
            input.resetButtonDown();
        }

     /*   void Pause()
        {
            Time.timeScale = 0;
        }

        void Resume()
        {
            Time.timeScale = 1;
        }*/

        public void ToggleSkillMenu()
        {
            if (gamePaused)
            {
                gamePaused = false;
                skillMenu.SetActive(false);
                GameManager.Resume();
            }
            else
            {
                gamePaused = true;
                skillMenu.SetActive(true);
                GameManager.Pause();
            }
        }
    }
}
