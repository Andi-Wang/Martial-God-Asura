using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void loadByIndex(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex);
	}
}
