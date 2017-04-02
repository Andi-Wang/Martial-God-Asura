using UnityEngine;

public class AttackState : EnemyState
{
    private Enemy enemy;

    public void Execute()
    {
        Attack();
        enemy.setDashSpeed(20f);
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
        if (enemy.isBoss)
        {
            float holdTimer = 0f;
            //Animate hold punch
            while (true)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= 1.5f)
                    break;
            }
            //Animate punch + damage
            enemy.animator.SetBool("enemyAttack", true);
        }
        else if (!enemy.canMove)
        {
            enemy.animator.SetBool("enemyAttack", true);
            //Plant should do damage here
        }
        else
        {
            enemy.animator.SetBool("enemyAttack", true);
        }
    }
}
