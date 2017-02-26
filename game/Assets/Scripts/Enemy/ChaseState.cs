using UnityEngine;
using System.Collections;

public class ChaseState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        enemy.setPlayerPos();

        if (GameManager.instance.currentRoom != RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y))
            enemy.changeState(enemy.patrolState);
        else if (enemy.TargetInMeleeRange())
        {
            enemy.changeState(enemy.attackState);
        }
        else if (enemy.TargetInRange() && enemy.rangeCDCheck())
        {
            Vector3 raycastStartPoint = enemy.transform.position + new Vector3(enemy.getDirection().x, 0);
            RaycastHit2D rangeHit = Physics2D.Raycast(raycastStartPoint, enemy.getPlayerPos() - enemy.transform.position);
            if (rangeHit.collider.gameObject.tag == "Player")
            {
                enemy.changeState(enemy.rangedAttackState);
            }
        }

        //Debug.Log(enemy.getPlayerPos().y - enemy.transform.position.y);
        if (-0.2f < enemy.getPlayerPos().y - enemy.transform.position.y || enemy.getPlayerPos().y - enemy.transform.position.y < 0.2f)
        {
            if(true) //gap between player and enemy platforms
            {
                //Jump across platforms
            }
            enemy.LookAtTarget();
            enemy.Move();
        }
        else if (enemy.getPlayerPos().y - enemy.transform.position.y < -0.2f)
        {
            while (enemy.transform.position.y > enemy.getPlayerPos().y)
            {
                enemy.Move();
            }
        }
        else if (enemy.getPlayerPos().y - enemy.transform.position.y > 0.2f)
        {
            do
            {
                enemy.Move();
            } while (true); //while( Not on a jump point )
            //If on a jump point, face player and jump up to platform
        }
    }
    public void Begin(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.speed = 2f;
    }
    public void Leave()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {

    }
}

