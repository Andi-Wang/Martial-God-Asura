using UnityEngine;
using System.Collections;

public class PuzzlePatrolState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        enemy.Move();
    }
    public void Begin(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.speed = 1f;
    }

    public void Leave()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
    }
}
