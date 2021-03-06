﻿using UnityEngine;
using System.Collections;

public class PatrolState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        enemy.setPlayerPos();
        float dis = enemy.disToPlayer();

        Vector3 raycastStartPoint = enemy.transform.position + new Vector3(enemy.getDirection().x, 0);
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, enemy.getDirection());
        if ((hit.collider != null || dis < enemy.detectionRange) && !enemy.cannotChase)
        {
            if (hit.collider.gameObject.tag == "Player" || (dis < enemy.detectionRange && GameManager.instance.currentRoom == enemy.roomId))
                enemy.changeState(enemy.chaseState); 
            else if (hit.distance <= 2 && hit.collider.gameObject.tag != "Enemy")
                enemy.Flip();
            else
                enemy.Move();
        }
        else if (enemy.cannotChase)
        {
            if (hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "Enemy" && hit.distance <= 1)
                enemy.Flip();
            else
                enemy.moveThough();
        }
        if (GameManager.instance.currentRoom == enemy.roomId && ((!enemy.canMove && !enemy.cannotChase) || enemy.isBoss))
            enemy.changeState(enemy.chaseState);
    }
    public void Begin(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.speed = enemy.baseSpeed;
    }
    public void Leave()
    {

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
    }
}
