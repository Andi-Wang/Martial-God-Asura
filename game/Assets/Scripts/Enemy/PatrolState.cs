using UnityEngine;
using System.Collections;

public class PatrolState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        Vector3 raycastStartPoint = enemy.transform.position + new Vector3(enemy.getDirection().x, 0);
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, enemy.getDirection());
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Player")
                enemy.changeState(enemy.chaseState); 
            else if (hit.distance <= 2)
                enemy.Flip();
        }

        enemy.Move();
    }
    public void Begin(Enemy enemy)
    {
        this.enemy = enemy;
        if (enemy.isBoss)
            enemy.speed = 3f;
        else
            enemy.speed = 1f;
    }
    public void Leave()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
       // if (other.tag == "Walls")
        //    enemy.Flip();
    }
}
