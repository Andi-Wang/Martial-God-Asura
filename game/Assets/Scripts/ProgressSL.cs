using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class ProgressSL {
    private GameStatus gs;
    private string gsPath;

    public void prepareSaveData()
    {
        gs = new GameStatus();
        gs.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        //gs.ladderUnlocked = LadderManager.GetUnlockedLadder();
        gs.enemies = GameManager.instance.Enemies;
    }

    public void save()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream fsgs = File.Create(gsPath);
        bf.Serialize(fsgs, gs);
        fsgs.Close();
    }

    public void load()
    {
        gsPath = Application.dataPath + "/Resources/savedata.dat";
 
        if (File.Exists(gsPath))
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fsgs = File.Open(gsPath, FileMode.Open);

                gs = (GameStatus)bf.Deserialize(fsgs);
                fsgs.Close();
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


