using UnityEngine;
using System.Collections;

public class LoadProgressOnClick : MonoBehaviour {
    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

	public void LoadProgress()
    {
        gameManager.LoadProgress();
    }
}
