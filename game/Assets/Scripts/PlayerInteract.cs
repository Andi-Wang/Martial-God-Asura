using UnityEngine;
using System.Collections;
using CreativeSpore.SuperTilemapEditor;

public class PlayerInteract : MonoBehaviour {

    private Rigidbody2D playerRigidbody2D;
    
    void Awake () {
	    playerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Tilemap otherObj = other.gameObject.GetComponentInParent<Tilemap>();

        if (otherObj != null && (otherObj.tag == "DoorL" || otherObj.tag == "DoorR"))
        {
            Vector3 destination = new Vector3();
            if (otherObj.tag == "DoorL")
            {
                destination.x += 4;
            }
            else
            {
                destination.x -= 4;
            }
            playerRigidbody2D.MovePosition(transform.position + destination);
        }
    }
}
