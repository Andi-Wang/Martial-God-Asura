﻿using UnityEngine;
using System.Collections;
/** ladder unlock animator **/
public class UnlockLadder : MonoBehaviour {

    public GameObject ladder;
    public int ladderId;

    bool entered;
    bool animating;
    Animator anim;

    void Awake()
    {
        anim = GameObject.FindObjectOfType<Camera>().GetComponent<Animator>();

        entered = false;
        animating = false;
    }

    void Update()
    {
        if (Input.GetButtonUp("Interact") && entered && !animating)
        {
            anim.SetTrigger("LadderUnlock" + ladderId);
            animating = true;
        }
    }

    // Update is called once per frame
    void OnTriggerEnter2D (Collider2D other) {
	    if (other.tag == "Player" && !ladder.activeInHierarchy)
        {
            entered = true;
        }
	}
    
    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
    }
}
