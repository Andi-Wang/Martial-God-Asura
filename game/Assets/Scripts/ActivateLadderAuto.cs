﻿using UnityEngine;
using System.Collections;

public class ActivateLadderAuto : MonoBehaviour {
    public GameObject ladder;
    public int ladderId;

    bool alreadyEntered;
    Animator anim;

    void Awake()
    {
        anim = GameObject.FindObjectOfType<Camera>().GetComponent<Animator>();
        alreadyEntered = false;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!alreadyEntered && other.tag == "Player" && !ladder.activeInHierarchy)
        {
            alreadyEntered = true;
            
        }
    }
}
