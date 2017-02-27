using UnityEngine;
using System.Collections;

public class RangedAttackState : EnemyState
{
    private Enemy enemy;
    private Skill skill;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.

    public void Execute()
    {
        Attack();
        enemy.changeState(enemy.chaseState);
    }

    public void Begin(Enemy enemy)
    {
        skill = new Skill();
        this.enemy = enemy;
    }

    public void Leave()
    {
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void Attack()
    {
        Vector2 right = enemy.getDirection();
        if (right.x == -1)
        {
            m_FacingRight = false;
        }
        else
            m_FacingRight = true;
        enemy.animator.SetBool("Moving", false);
        skill.Projectile(enemy.rb2D, m_FacingRight, enemy.m_fireball, 1, 0, 20, 2, 5, 20);
    }
}
