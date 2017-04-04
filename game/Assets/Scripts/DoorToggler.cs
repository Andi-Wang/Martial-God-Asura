using UnityEngine;
using System.Collections;

public class DoorToggler : MonoBehaviour {
    public Enemy boss;

    EdgeCollider2D col;
	// Use this for initialization
	void Start () {
        col = GetComponentInChildren<EdgeCollider2D>();
        col.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	    if (!col.enabled && boss.isDead)
        {
            col.enabled = true;
        }
	}
}
