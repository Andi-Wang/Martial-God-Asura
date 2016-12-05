using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class LadderTrigger : MonoBehaviour
{
    GameObject hintCanvas;

    string ladderTag;

    Transform playerPosition;
    Vector3 destination;
    Collider2D doorCollider;
    private bool entered;
    int ladderTrans = 14;
    float ladderCentre;

    void Awake()
    {
        hintCanvas = GameObject.Find("HintCanvas");
        ladderTag = gameObject.tag;
        ladderCentre = transform.localScale.x / 2;
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");

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
