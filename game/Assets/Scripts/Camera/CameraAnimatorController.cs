﻿using UnityEngine;
using System.Collections;

public class CameraAnimatorController : MonoBehaviour {

    public LadderManager ladderController;

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
    }

    void StartAnimating()
    {
        cam.isAnimating = true;
        GameManager.instance.playersTurn = false;
    }
}
