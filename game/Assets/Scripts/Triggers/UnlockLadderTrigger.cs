using UnityEngine;
using System.Collections;

public class UnlockLadderTrigger : MonoBehaviour {
    public Enemy bossObj;
    
    UnlockLadder ladder;
	// Use this for initialization
	void Start () {
        ladder = GetComponent<UnlockLadder>();
        ladder.unlockable = false;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!ladder.unlockable && bossObj.isDead)
        {
            ladder.unlockable = true;
        }
	}
}
