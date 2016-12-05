using UnityEngine;
using System.Collections;

public class CameraAnimatorController : MonoBehaviour {

    public LadderTriggerManager ladderController;

    CameraFollow cam;
	// Use this for initialization
	void Awake () {
        cam = gameObject.GetComponentInParent<CameraFollow>();
	}
	
    void UnlockLadder(int ladderId)
    {
        ladderController.unlockLadder(ladderId);
    }

	void EndAnimating()
    {
        cam.isAnimating = false;
        GameManager.instance.playersTurn = true;
        Debug.Log("reseted");
    }

    void StartAnimating()
    {
        cam.isAnimating = true;
        GameManager.instance.playersTurn = false;
    }
}
