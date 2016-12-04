using UnityEngine;
using System.Collections;

public class ActivateLadder : MonoBehaviour {

    public GameObject ladder;

    Animator anim;

    void Awake()
    {
        anim = GameObject.FindObjectOfType<Camera>().GetComponent<Animator>();
    }

	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {
	    if (other.tag == "Player" && !ladder.activeInHierarchy)
        {
            anim.SetTrigger("ladderUnlocked");
        }
	}
    
    public void unlockLadder()
    {
        ladder.SetActive(true);
    }
}
