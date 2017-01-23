using UnityEngine;
using System.Collections;

public class AttackState : EnemyState
{
    private Enemy enemy;
    private float attackTimer;
    private float attackCD = 2f;
    private bool canAttack = true;

    public void Execute()
    {
       // if (enemy.player != null)
      //  {
            enemy.Move();
            Attack();
     //   } else
        {
            enemy.changeState(new PatrolState());
        }
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
        attackTimer += Time.deltaTime;
        if(attackTimer >= attackCD)
        {
            canAttack = true;
            attackTimer = 0;
        }
        if(canAttack)
        {
            canAttack = false;
            enemy.animator.SetBool("enemyAttack", true);
        }
    }
}
