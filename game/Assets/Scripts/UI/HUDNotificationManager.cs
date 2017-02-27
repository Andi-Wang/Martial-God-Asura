using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDNotificationManager : MonoBehaviour {
    public float displayTime = 1f;
    public Color flashColour = new Color(1f, 1f, 1f, 1f);
    public float flashSpeed = 1f;

    bool displaying;
    Text msg;
    float timer;

	void Start () {
        msg = GetComponentInChildren<Text>();
        timer = 0;
        displaying = false;
	}
	
	void Update () {
	    if (displaying)
        {
            msg.color = flashColour;
            //timer += Time.deltaTime;
        }
        
        else //(timer >= displayTime)
        {
            //msg.color = Color.clear;
            msg.color = Color.Lerp(msg.color, Color.clear, flashSpeed * Time.deltaTime);
            //Debug.Log("test");
            //displaying = false;
            //timer = 0;
        }
        displaying = false;
    }

    public void Display(string txt)
    {
        msg.text = txt;
        msg.color = flashColour;
        displaying = true;
    }
}
