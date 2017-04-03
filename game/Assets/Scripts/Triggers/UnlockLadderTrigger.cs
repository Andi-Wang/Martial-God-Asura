using UnityEngine;
using System.Collections;

public class UnlockLadderTrigger : MonoBehaviour {
    public Enemy bossObj;
    
    UnlockLadder ladder;
	// Use this for initialization
	void Start () {
        ladder.unlockable = false;
        ladder = GetComponent<UnlockLadder>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (!ladder.unlockable && bossObj.isDead)
        {
            ladder.unlockable = true;
        }
	}
}
