using UnityEngine;
using System.Collections;

public class ChaseState : EnemyState {

    private Enemy enemy;
    private bool canMelee = false;
    private bool canRanged = false;

    public void Execute()
    {
        //Get player position
        enemy.setPlayerPos();

        //Update melee and ranged cooldowns
        if (enemy.canMeleeAttack && !canMelee)
            canMelee = enemy.updateMeleeCD();
        if (enemy.canRangeAttack && !canRanged)
            canRanged = enemy.updateRangeCD();

        //Check if player left room
        if (GameManager.instance.currentRoom != RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y) && !enemy.isGhost)
        {
            enemy.changeState(enemy.patrolState);
        }
        //If can melee, melee
        else if ((enemy.TargetInMeleeRange() && enemy.canMeleeAttack && canMelee) || enemy.isBoss2)
        {
            canMelee = false;
            enemy.changeState(enemy.attackState);
        }
        //If can ranged, ranged
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
        //If melee on CD but in melee range
        else if (enemy.TargetInMeleeRange() && !canMelee)
        {
            if (enemy.isGhost)
                enemy.animator.SetBool("Moving", true);
            else
                enemy.animator.SetBool("Moving", false);
        }
        //If the unit is a ghost
        else if (enemy.isGhost)
        {
            if (enemy.disToPlayer() < 15f)
                enemy.ghostMove();
            else
                if(enemy.ghostMoveHome())
                    enemy.changeState(enemy.patrolState);
        }
        //Look at player and move
        else
        {
            enemy.LookAtTarget();
            enemy.Move();
        }
        //Check if player has gone up a floor
        if (Mathf.Abs(enemy.getPlayerPos().y - enemy.transform.position.y) > 8 && !enemy.isGhost)
        {
            enemy.changeState(enemy.patrolState);
        }
        //If can't move, just look at him all scary like
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

