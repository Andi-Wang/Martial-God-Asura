using UnityEngine;
using System.Collections;

public class ChaseState : EnemyState {

    private Enemy enemy;
    private bool canMelee;
    private bool canRanged;

    public void Execute()
    {
        enemy.setPlayerPos();

        if (enemy.canMeleeAttack && !canMelee)
            canMelee = enemy.updateMeleeCD();
        if (enemy.canRangeAttack && !canRanged)
            canRanged = enemy.updateRangeCD();

        if (GameManager.instance.currentRoom != RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y))
        {
            enemy.changeState(enemy.patrolState);
        }
        else if (enemy.TargetInMeleeRange() && enemy.canMeleeAttack && canMelee)
        {
            canMelee = false;
            enemy.changeState(enemy.attackState);
        }
        else if (enemy.TargetInRange() && enemy.canRangeAttack && canRanged)
        {
            canRanged = false;
            Vector3 raycastStartPoint = enemy.transform.position + new Vector3(enemy.getDirection().x, 0);
            RaycastHit2D rangeHit = Physics2D.Raycast(raycastStartPoint, enemy.getPlayerPos() - enemy.transform.position);
            if (rangeHit.collider.gameObject.tag == "Player")
            {
                enemy.changeState(enemy.rangedAttackState);
            }
        }

        else if (enemy.TargetInMeleeRange() && !canMelee)
        {
            enemy.animator.SetBool("Moving", false);
        }
        else
        {
            enemy.LookAtTarget();
            enemy.Move();
        }

        if (Mathf.Abs(enemy.getPlayerPos().y - enemy.transform.position.y) > 8)
        {
            enemy.changeState(enemy.patrolState);
        }

        if (!enemy.canMove)
            enemy.LookAtTarget();
    }
    public void Begin(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.speed = enemy.chaseSpeed;
    }
    public void Leave()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {

    }
}

