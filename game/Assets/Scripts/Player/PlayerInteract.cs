using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class PlayerInteract : MonoBehaviour {

    public Vector3 levelExit;
    private Rigidbody2D playerRigidbody2D;

    float doorTrans = 3f;
    
    void Awake () {
	    playerRigidbody2D = GetComponent<Rigidbody2D>();
        levelExit = new Vector3(69, 4);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Tilemap otherObj = other.gameObject.GetComponentInParent<Tilemap>();

        if (otherObj == null)
        {
            return;
        }

        if (otherObj.tag == "DoorL" || otherObj.tag == "DoorR")
        {
            Vector3 destination = new Vector3();
            if (otherObj.tag == "DoorL")
            {
                destination.x += doorTrans;
            }
            else
            {
                destination.x -= doorTrans;
            }
            playerRigidbody2D.MovePosition(transform.position + destination);
        }
        else if(otherObj.name == "ExitLevelDoor" && GameManager.instance.SubLevelComplete)
        {
            playerRigidbody2D.position = levelExit;
        }
        else if (otherObj.name == "NextLevelDoor" && GameManager.instance.SubLevelComplete)
        {
            GameManager.instance.gotoNextLevel();
        }
    }
}
