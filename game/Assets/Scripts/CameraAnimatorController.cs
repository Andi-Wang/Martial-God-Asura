using UnityEngine;
using System.Collections;

public class CameraAnimatorController : MonoBehaviour {

    public ActivateLadder ladderController;

    CameraFollow cam;
	// Use this for initialization
	void Awake () {
        cam = gameObject.GetComponentInParent<CameraFollow>();
	}
	
    void UnlockLadder()
    {
        ladderController.unlockLadder();
    }

	void EndAnimating()
    {
        cam.isAnimating = false;
        GameManager.instance.playersTurn = true;
    }

    void StartAnimating()
    {
        cam.isAnimating = true;
        GameManager.instance.playersTurn = false;
    }
}
