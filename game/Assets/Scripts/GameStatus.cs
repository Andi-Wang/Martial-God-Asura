using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameStatus {

    //current scene,activated ladder,enemies --snapshot,time if multiple saved game
    public int sceneNumber;
    public bool[] ladderUnlocked;
    
    public List<Enemy> enemies;
    public Player playerStat;

    public GameStatus()
    {
        playerStat = new Player();
    }

    
}
public class Player
{
    public Skills skills;
    public Vector2 playerPos;
    public int playerHealth;
}