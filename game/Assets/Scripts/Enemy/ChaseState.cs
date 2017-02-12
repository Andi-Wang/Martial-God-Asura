using UnityEngine;
using System.Collections;

public class ChaseState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        enemy.LookAtTarget();
        enemy.Move();
        if (GameManager.instance.currentRoom != RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y))
            enemy.changeState(enemy.patrolState);
        else if (enemy.TargetInRange())
        {
            enemy.changeState(enemy.attackState);
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

