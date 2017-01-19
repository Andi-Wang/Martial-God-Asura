using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ProgressSL {
    private GameStatus gs;
    private Player playerData;
    private string gsPath;
    private string playerPath;

    public void save()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fsgs = File.Create(gsPath);
        bf.Serialize(fsgs, gs);
        fsgs.Close();

        FileStream fsplayer = File.Create(playerPath);
        bf.Serialize(fsplayer, playerPath);
        fsplayer.Close();
    }

    public void load()
    {
        gsPath = Application.dataPath + "/Resources/savedata.dat";
        playerPath = Application.dataPath + "/Resources/playerdata.dat";

        if (File.Exists(gsPath) && File.Exists(playerPath))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fsgs = File.Open(gsPath, FileMode.Open);

                gs = (GameStatus)bf.Deserialize(fsgs);
                fsgs.Close();

                FileStream fsplayer = File.Open(playerPath, FileMode.Open);

                playerData = (Player)bf.Deserialize(fsplayer);
                fsplayer.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
        {
            Debug.Log("Cannot find saved data file");
        }
    }
}

public class GameStatus {
    //current scene,activated ladder,enemies --snapshot,time if multiple saved game
    public int sceneNumber;
    public bool[] ladderActivated;

    public GameStatus()
    {

    }
}
public class Player
{
    public int playerLevel;
    public Vector2 playerPos;
    //player pos, player character status,
}
