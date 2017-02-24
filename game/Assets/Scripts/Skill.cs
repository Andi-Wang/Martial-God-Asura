﻿
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Skill {

    public class SkillStateManager {
        public bool backdashing = false;
        public float backdashSpeed = 0f;
        public bool sliding = false;
        public float slideSpeed = 0f;
        public bool secondJumpAvailable = true;
        public bool airdashing = false;
        public float airdashSpeed = 0f;

    }

    public void FireballNova(Rigidbody2D body, bool facingRight, Rigidbody2D fireball) {
        float speed = 8f;
        float startDistance = 2f;
        int numProjectiles = 3;
        float spread = 20f;         //Maximum angle of projectile trajectory (above and below the horizontal)

        for (int i = 0; i < numProjectiles; i++) {
            float angle = 0f;
            if (numProjectiles > 1) {
                angle = spread - 2f * spread / (numProjectiles - 1) * i;
            }
            if (!facingRight) { angle += 180f; }

            float xdeg = Mathf.Cos(angle * Mathf.Deg2Rad);
            float ydeg = Mathf.Sin(angle * Mathf.Deg2Rad);

            Rigidbody2D clone = Rigidbody2D.Instantiate(fireball, new Vector2(body.transform.position.x + xdeg * startDistance, body.transform.position.y + ydeg * startDistance), Quaternion.AngleAxis(angle, Vector3.forward)) as Rigidbody2D;
            clone.velocity = speed * new Vector2(xdeg, ydeg);
            // clone.AddForce(new Vector2(xdeg, ydeg) * fb_force);
        }
    }

    public void Iceball(Rigidbody2D body, bool facingRight) {
        float force = 450f;
        float startDistance = 2f;
        int numProjectiles = 1;
        float spread = 20f;         //Maximum angle of projectile trajectory (above and below the horizontal)

        for (int i = 0; i < numProjectiles; i++) {
            float angle = 0f;
            if (numProjectiles > 1) {
                angle = spread - 2f * spread / (numProjectiles - 1) * i;
            }
            if (!facingRight) { angle += 180f; }
            float xdeg = Mathf.Cos(angle * Mathf.Deg2Rad);
            float ydeg = Mathf.Sin(angle * Mathf.Deg2Rad);

            GameObject clone = GameObject.Instantiate(Resources.Load("Iceball"), new Vector2(body.transform.position.x + xdeg * startDistance, body.transform.position.y + ydeg * startDistance), Quaternion.AngleAxis(angle, Vector3.forward)) as GameObject;
            clone.GetComponent<Rigidbody2D>().AddForce(new Vector2(xdeg, ydeg) * force);
        }
    }

    public void Torrent(Rigidbody2D body, bool facingRight, bool skillButtonHeld) {
        float force = 450f;
        float startDistance = 2f;
        int numProjectiles = 1;
        float spread = 20f;         //Maximum angle of projectile trajectory (above and below the horizontal)

        for (int i = 0; i < numProjectiles; i++) {
            float angle = 0f;
            if (numProjectiles > 1) {
                angle = spread - 2f * spread / (numProjectiles - 1) * i;
            }
            if (!facingRight) { angle += 180f; }
            float xdeg = Mathf.Cos(angle * Mathf.Deg2Rad);
            float ydeg = Mathf.Sin(angle * Mathf.Deg2Rad);

            GameObject clone = GameObject.Instantiate(Resources.Load("Iceball"), new Vector2(body.transform.position.x + xdeg * startDistance, body.transform.position.y + ydeg * startDistance), Quaternion.AngleAxis(angle, Vector3.forward)) as GameObject;
            clone.GetComponent<Rigidbody2D>().AddForce(new Vector2(xdeg, ydeg) * force);
        }
    }

    public float Backdash(Rigidbody2D body, bool facingRight, float backdashSpeed, bool forceStart) {

        float minSpeed = 6f;
        float maxSpeed = 17f;
        float decay = 1f;

        if (forceStart || backdashSpeed > minSpeed) {
            if (backdashSpeed == 0) {
                backdashSpeed = maxSpeed;
            }
            else {
                backdashSpeed -= decay;
            }

            if (facingRight) {
                body.velocity = new Vector2(-backdashSpeed, body.velocity.y);
            }
            else {
                body.velocity = new Vector2(backdashSpeed, body.velocity.y);
            }
        }
        else {
            backdashSpeed = 0;
        }

        return backdashSpeed;
    }


    public float Slide(Rigidbody2D body, bool facingRight, float slideSpeed, bool forceStart, bool enhanced, bool enhancedCancel) {
        float minSpeed = 6f;
        float maxSpeed = 17f;
        float enhancementBoost = 7f;

        if (enhanced) {
            minSpeed += enhancementBoost;
            maxSpeed += enhancementBoost;
        }

        float decay = 1f;

        if (enhanced && enhancedCancel) {
            slideSpeed = 0;
        }
        else if (forceStart || slideSpeed > minSpeed) {
            if (slideSpeed == 0) {
                slideSpeed = maxSpeed;
            }
            else {
                slideSpeed -= decay;
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

        if (forceStart || airdashSpeed > minSpeed) {
            float verticalSpeedOffset = 0f;

            if (airdashSpeed == 0) {
                airdashSpeed = maxSpeed;
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
            }
            else {
                verticalSpeedOffset = Mathf.Sqrt((airdashSpeed - minSpeed) / (maxSpeed - minSpeed)) * verticalSpeedModifier;
                airdashSpeed -= decay;
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
