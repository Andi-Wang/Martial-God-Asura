using UnityEngine;
using System.Collections;

public class RangedAttackState : EnemyState
{
    private Enemy enemy;

    public void Execute()
    {
        Attack();
        enemy.changeState(enemy.chaseState);
    }

    public void Begin(Enemy enemy)
    {
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
        enemy.animator.SetBool("Moving", false);
    }
}
