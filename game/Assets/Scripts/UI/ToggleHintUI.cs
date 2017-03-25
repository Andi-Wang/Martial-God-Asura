using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToggleHintUI : MonoBehaviour {
    GameObject hintCanvas;

    void Start()
    {
        hintCanvas = GameObject.Find("HintCanvas");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void toggleHint(float x = 0, float y = -999)
    {
        if (findHintUI())
        {
            hintCanvas.transform.position = new Vector3(x, y);
        }
    }

    public void toggleDialog(Canvas dialogBox, float x=0, float y = -999)
    {
        dialogBox.transform.position = new Vector3(x, y);
    }
    
    bool findHintUI()
    {
        if (hintCanvas == null)
        {
            hintCanvas = GameObject.Find("HintCanvas");
            return (hintCanvas == null) ? false : true;
        }
        else
            return true;
    }
}
