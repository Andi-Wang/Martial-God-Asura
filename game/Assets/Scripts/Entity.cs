using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Entity {
    public float health;
    public float maxHealth;
    public float energy;
    public float healthRegen;
    public float energyRegen;
    public float armor;
    public float jumpForce;
    public float maxSpeed;
    public float gravity;
    public float crouchSpeed;
    public Dictionary<string, int> skillList;

    public Entity() {
    }

    public Entity(float initialHealth, float initialEnergy, float initialHealthRegen, float initialEnergyRegen, float initialArmor, float initialJumpForce, float initialMaxSpeed, float initialGravity, float initialCrouchSpeed) {
        health = initialHealth;
        maxHealth = health;
        energy = initialEnergy;
        healthRegen = initialHealthRegen;
        energyRegen = initialEnergyRegen;
        armor = initialArmor;
        jumpForce = initialJumpForce;
        maxSpeed = initialMaxSpeed;
        gravity = initialGravity;
        crouchSpeed = initialCrouchSpeed;
        }
}
