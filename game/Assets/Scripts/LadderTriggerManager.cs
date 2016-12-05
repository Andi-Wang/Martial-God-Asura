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
        if (ladders[ladderId] != null)
        {
            ladders[ladderId].SetActive(true);
        }
    }
}
