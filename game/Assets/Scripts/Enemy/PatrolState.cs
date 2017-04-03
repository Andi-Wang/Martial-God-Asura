using UnityEngine;
using System.Collections;

public class PatrolState : EnemyState {

    private Enemy enemy;

    public void Execute()
    {
        enemy.setPlayerPos();
        float dis = Mathf.Sqrt(Mathf.Pow(enemy.getPlayerPos().x - enemy.transform.position.x, 2) + Mathf.Pow(enemy.getPlayerPos().y - enemy.transform.position.y, 2));

        Vector3 raycastStartPoint = enemy.transform.position + new Vector3(enemy.getDirection().x, 0);
        RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, enemy.getDirection());
        if ((hit.collider != null || dis < enemy.detectionRange) && !enemy.cannotChase)
        {
            if (hit.collider.gameObject.tag == "Player" && dis < enemy.detectionRange && GameManager.instance.currentRoom == RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y))
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
        if (GameManager.instance.currentRoom == RoomManager.Instance.findRoomId(enemy.transform.position.x, enemy.transform.position.y) && (!enemy.canMove || enemy.isBoss))
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
       // if (other.tag == "Walls")
        //    enemy.Flip();
    }
}
