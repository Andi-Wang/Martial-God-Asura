using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RunDialog : MonoBehaviour {

	private string str;

	public Text txt;

	void Start(){
		StartCoroutine( AnimateText("This is an example of the dialog box. As you can see, the text will appear one character at a time. If it hits the end of the box, it loops to the next line, which I think is pretty neat.") );
	}


	IEnumerator AnimateText(string strComplete){
		int i = 0;
		str = "";
		while( i < strComplete.Length ){
			str += strComplete[i++];
			yield return new WaitForSeconds(0.05F);
		}
	}

	void Update(){
		txt.text = str;
	}
}