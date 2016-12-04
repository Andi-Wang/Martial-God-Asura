using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class LadderTrigger : MonoBehaviour
{
    public GameObject hintCanvas;

    string ladderTag;
    Rigidbody2D playerRigidBody;
    Transform playerPosition;
    Vector3 destination;
    Collider2D doorCollider;
    private bool entered;
    int ladderTrans = 14;
    float ladderCentre;

    void Awake()
    {
        ladderTag = gameObject.tag;
        ladderCentre = transform.localScale.x / 2;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerRigidBody = player.GetComponent<Rigidbody2D>();
        playerPosition = player.transform;

        entered = false;
    }
    
    void Update()
    {
        if (Input.GetButtonUp("Interact") && entered == true)
        {
            if (ladderTag == "LadderB")
            {
                destination.Set(0, ladderTrans, 0);
            }
            else if (ladderTag == "LadderT")
            {
                destination.Set(0, -ladderTrans, 0);
            }
            playerPosition.position += destination;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            entered = true;
            toggleInteractHint(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
        toggleInteractHint(false);
    }

    void toggleInteractHint(bool status)
    {
        if (status)
        {
            hintCanvas.transform.position = transform.position + new Vector3(ladderCentre, 3);
        }
        else
        {
            hintCanvas.transform.position = new Vector3(0, -999);
        }
    }
}
