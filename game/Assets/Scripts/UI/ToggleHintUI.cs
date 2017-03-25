using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ToggleHintUI : MonoBehaviour {

    public void toggleHint(float x = 0, float y = -999)
    {
        gameObject.transform.position = new Vector3(x, y);
    }

    public void toggleDialog(Canvas dialogBox, float x=0, float y = -999)
    {
        dialogBox.transform.position = new Vector3(x, y);
    }
}
