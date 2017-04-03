using UnityEngine;

public class AttackState : EnemyState
{
    private Enemy enemy;
    private bool attack;
    private bool hit;

    public void Execute()
    {
        Attack();
        if (!attack)
        {
            enemy.changeState(enemy.chaseState);
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
        attack = true;
        if (enemy.isBoss1)
        {
            float timer = 0f;
            //Animate hold punch
            while (true)
            {
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                    break;
            }
            //Animate punch + damage
            enemy.animator.SetBool("enemyAttack", true);
        }
        else if (enemy.isBoss2)
        {
            if (enemy.boss2Timer < Time.time)
            {
                hit = false;
                enemy.boss2Timer = Time.time + 6;
            }

            if (enemy.disToPlayer() >= 1.6f && Time.time < enemy.boss2Timer-4f && !hit)
            {
                enemy.animator.SetBool("enemyAttack", true);
                enemy.speed = 6f;
                enemy.ghostMove();
            }
            else if (enemy.disToPlayer() <= 1.6f && Time.time  < enemy.boss2Timer - 4f && !hit)
            {
                enemy.animator.SetBool("enemyAttack", false);
                enemy.speed = 100f;
                enemy.moveBackwards();
                hit = true;
                enemy.boss2Timer = enemy.boss2Timer - ((enemy.boss2Timer-4) - Time.time);
            }
            else if (Time.time < enemy.boss2Timer-2f)
            {
                enemy.resetRotation();
                enemy.animator.SetBool("enemyAttack", false);
                enemy.isImmune = false;
                enemy.rb2D.isKinematic = false;
            }
            else if (Time.time < enemy.boss2Timer)
            {
                enemy.rb2D.isKinematic = true;
                enemy.animator.SetBool("enemyAttack", false);
                enemy.isImmune = true;
                enemy.transform.Translate(Vector3.up * enemy.speed * Time.deltaTime, Space.World);
            }
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
        attack = false;
    }
}
