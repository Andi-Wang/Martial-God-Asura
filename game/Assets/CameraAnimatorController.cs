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

	void SetAnimating()
    {
        cam.isAnimating = false;
    }
}
