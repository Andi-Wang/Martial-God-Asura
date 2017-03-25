using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameStatus {

    //current scene,activated ladder,enemies --snapshot,time if multiple saved game
    public int sceneNumber;
    public bool[] ladderUnlocked;
    public int subLevel;
    
    public List<Enemy> enemies;
    public Player playerStat;
    public SkillTree skillTree;

    public GameStatus()
    {
        //playerStat = new Player();
    }
}

[System.Serializable]
public class Player
{
    public float playerPosX;
    public float playerPosY;
    public Entity playerEntity;
}