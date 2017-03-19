using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Entity {
    public float health;
    public float maxHealth;
    public float energy;
    public float maxEnergy;
    public float healthRegen;
    public float energyRegen;
    public float armor;
    public float might;
    public float jumpForce;
    public float maxSpeed;
    public float gravity;
    public float crouchSpeed;
    public float animationSpeed;
    public Dictionary<string, int> skillList;

    public Entity() {
        health = 0;
        maxHealth = 0;
        energy = 0;
        maxEnergy = 0;
        healthRegen = 0;
        energyRegen = 0;
        armor = 0;
        might = 0;
        jumpForce = 0;
        gravity = 0;
        crouchSpeed = 0;
        animationSpeed = 0;
        skillList = new Dictionary<string, int>();
    }

    public Entity(float initialHealth, float initialEnergy, float initialHealthRegen, float initialEnergyRegen, float initialArmor, float initialMight, float initialJumpForce, float initialMaxSpeed, float initialGravity, float initialCrouchSpeed) {
        health = initialHealth;
        maxHealth = health;
        energy = initialEnergy;
        maxEnergy = energy;
        healthRegen = initialHealthRegen;
        energyRegen = initialEnergyRegen;
        armor = initialArmor;
        might = initialMight;
        jumpForce = initialJumpForce;
        maxSpeed = initialMaxSpeed;
        gravity = initialGravity;
        crouchSpeed = initialCrouchSpeed;
        animationSpeed = 1;
        skillList = new Dictionary<string, int>();
    }
}
