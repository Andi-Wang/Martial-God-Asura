using UnityEngine;
using System.Collections;

public class AttackState : EnemyState
{
    private Enemy enemy;

    private float attackTimer;
    private float attackCD = 1f;
    private bool canAttack = true;

    public void Execute()
    {
        attackTimer += Time.deltaTime;
        if (!enemy.TargetInMeleeRange())
        {
            enemy.animator.SetBool("Moving", true);
            enemy.changeState(enemy.chaseState);
        }
        enemy.animator.SetBool("Moving", false);
        Attack();
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
        if(attackTimer >= attackCD)
        {
            canAttack = true;
            
        }
        if(canAttack)
        {
            if (enemy.isBoss)
            {
                //Animate hold punch
                //yield return new WaitForSeconds(0.5f);
                //Animate punch + damage
                canAttack = false;
                attackTimer = 0;
            }
            else
            {
                enemy.animator.SetBool("enemyAttack", true);
                canAttack = false;
                attackTimer = 0;
            }
        }
    }
}
