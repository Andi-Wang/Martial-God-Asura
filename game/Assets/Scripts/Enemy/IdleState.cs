using UnityEngine;
using System.Collections;

public class IdleState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        //enemy.Move();
      /*  if(enemy.player != null)
        {
            enemy.changeState(new AttackState());
        }*/
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
       // if (other.tag == "Walls")
        //    enemy.Flip();
    }
}
