using UnityEngine;
using System.Collections;
/** ladder unlock animator **/
public class UnlockLadder : MonoBehaviour {

    public GameObject ladder;
    public int ladderId;
    public bool unlockable;

    bool entered;
    bool animating;
    Animator anim;

    void Awake()
    {
        anim = GameObject.FindObjectOfType<Camera>().GetComponent<Animator>();

        entered = false;
        animating = false;
        unlockable = true;
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (unlockable && other.tag == "Player" && Input.GetButtonDown("Interact"))
        {
            animating = true;
            anim.SetTrigger("LadderUnlock" + ladderId);
        }
    }
}
