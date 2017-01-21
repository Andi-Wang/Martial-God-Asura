using UnityEngine;
using System.Collections;
/** contain list of all ladders in scene
 * */
public class LadderManager : MonoBehaviour {

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

    public bool[] GetUnlockedLadder()
    {
        bool[] unlocked = new bool[ladders.Length];
        for (int i = 0; i < unlocked.Length; ++i)
        {
            unlocked[i] = (ladders[i].activeInHierarchy) ? true : false;
        }
        return unlocked;
    }
}
