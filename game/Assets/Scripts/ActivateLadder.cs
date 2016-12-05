using UnityEngine;
using System.Collections;

public class ActivateLadder : MonoBehaviour {

    public GameObject ladder;
    public int ladderId;

    bool entered;
    Animator anim;

    void Awake()
    {
        anim = GameObject.FindObjectOfType<Camera>().GetComponent<Animator>();

        entered = false;
    }

    void Update()
    {
        if (Input.GetButtonUp("Interact") && entered == true)
        {
            anim.SetTrigger("LadderUnlock" + ladderId);
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
