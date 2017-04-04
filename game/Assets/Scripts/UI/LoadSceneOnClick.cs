using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void loadByIndex(int sceneIndex)
	{
        StartCoroutine(GameManager.instance.loadLvAsync(sceneIndex));
	}
}
