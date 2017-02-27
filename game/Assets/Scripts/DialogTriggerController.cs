using UnityEngine;
using System.Collections;

public class DialogTriggerController : MonoBehaviour {
    public DialogDisplayManager ddm;
    

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !ddm.displaying)
        {
            ddm.displayNext();
        }
    }
}
