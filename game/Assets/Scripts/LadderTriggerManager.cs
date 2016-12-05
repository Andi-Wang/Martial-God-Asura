using UnityEngine;
using System.Collections;

public class LadderTriggerManager : MonoBehaviour {

    public GameObject[] ladders;

	// Use this for initialization
	void Awake () {
	    foreach(GameObject ladder in ladders)
        {
            if (ladder.activeInHierarchy)
            {
                ladder.SetActive(false);
            }
        }
	}

    public void unlockLadder(int ladderId)
    {
        ladders[ladderId].SetActive(true);
    }
}
