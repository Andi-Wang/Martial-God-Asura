using UnityEngine;
using System.Collections;

public class ToggleHintUI : MonoBehaviour {
    public GameObject hintCanvas;

    public void toggleHint(float x = 0, float y = -999)
    {
        hintCanvas.transform.position = new Vector3(x, y);
    }

    public void toggleDialog(Canvas dialogBox, float x=0, float y = -999)
    {
        dialogBox.transform.position = new Vector3(x, y);
    }
    
}
