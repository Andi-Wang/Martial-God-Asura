using UnityEngine;
using System.Collections;

public class TopFloorView : MonoBehaviour {

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
        if (!alreadyEntered && other.tag == "Player")
        {
            alreadyEntered = true;
            anim.SetTrigger("OnTopFloor");
        }
    }
}
