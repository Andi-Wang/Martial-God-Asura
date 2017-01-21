using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

/** hint UI control and player teleport for ladder **/
public class LadderTrigger : MonoBehaviour
{
    string ladderTag;
    ToggleHintUI hintUIController;
    Transform playerPosition;
    Vector3 destination;

    private bool entered;
    int ladderTrans = 14;
    Vector3 ladderCentre;
   
    void Awake()
    {
        hintUIController = hintUIController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();
        
        ladderTag = gameObject.tag;
        ladderCentre = new Vector3(transform.localScale.x / 2, 3);
        
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
            
            Vector3 hintPos = transform.position + ladderCentre;
            hintUIController.toggleHint(hintPos.x, hintPos.y);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        entered = false;
        
        hintUIController.toggleHint();
    }
    
}
