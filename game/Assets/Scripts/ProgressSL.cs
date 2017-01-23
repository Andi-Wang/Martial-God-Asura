using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class ProgressSL {
    //private GameStatus gs;
    private static string gsPath = Application.dataPath + "/Resources/savedata.dat";

   /* public void prepareSaveData(GameStatus gs)
    {
        gs = new GameStatus();
        gs.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        //gs.ladderUnlocked = LadderManager.GetUnlockedLadder();
        gs.enemies = GameManager.instance.Enemies;

    }*/

    public static void save(GameStatus gs)
    {
        //prepareSaveData(gs);
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fsgs = File.Create(gsPath);
        bf.Serialize(fsgs, gs);
        fsgs.Close();
    }

    public static GameStatus load()
    {
        GameStatus gs = new GameStatus();
       // gsPath = Application.dataPath + "/Resources/savedata.dat";
 
        if (File.Exists(gsPath))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fsgs = File.Open(gsPath, FileMode.Open);

                gs = (GameStatus)bf.Deserialize(fsgs);
                fsgs.Close();

                return gs;
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                return null;
            }
        }
        else
        {
            Debug.Log("Cannot find saved data file");
            //PopupAlertMsg();
            return null;
        }
    }
}


