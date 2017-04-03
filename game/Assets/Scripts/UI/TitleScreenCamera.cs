using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenCamera : MonoBehaviour {

	//public Camera cm;
	public GameObject titleimg;
	public RectTransform timg;
	public Button newButton;
	public Button loadButton;
	public Button quitButton;
	public Image blackScreen;

	public Text newimgtxt;
	public Text loadimgtxt;
	public Text quitimgtxt;

	private int stage = 0;
	private Image newimg;
	private Image loadimg;
	private Image quitimg;

	private float na, la, qa;

	private bool faded;

	Vector3 startPosition = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
	Vector3 endPosition = new Vector3 (Screen.width / 2, Screen.height, 0);

	// Use this for initialization
	void Start () {
		//cm.transform.position = new Vector3 (0, 1, -2);
		//cm.fieldOfView = 20;
		timg.position = new Vector3 (Screen.width / 2, 2 * Screen.height / 3, 0);
		newimg = newButton.GetComponent<Image> ();
		//newimgtxt = newButton.GetComponent<Text> ();
		newimg.color = new Color(255,255,255,0);
		loadimg = loadButton.GetComponent<Image> ();
		//loadimgtxt = loadButton.GetComponent<Text> ();
		loadimg.color = new Color(255,255,255,0);
		quitimg = quitButton.GetComponent<Image> ();
		//quitimgtxt = quitButton.GetComponent<Text> ();
		quitimg.color = new Color(255,255,255,0);

		newimgtxt.color = new Color(0,0,0,0);
		loadimgtxt.color = new Color(0,0,0,0);
		quitimgtxt.color = new Color(0,0,0,0);

		newButton.interactable = false;
		loadButton.interactable = false;
		quitButton.interactable = false;

		faded = false;
	}

	// Update is called once per frame
	void Update () {
		if (!faded) {
			Color c = blackScreen.color;
			float alpha = c.a - 0.01f;
			blackScreen.color = new Color(c.r, c.g, c.b, alpha);
			if (blackScreen.color.a < 0.005f) {
				faded = true;
			}
		} else {
			//timg.position = Vector3.Lerp(startPosition, endPosition, speed * Time.deltaTime);
			if (timg.anchoredPosition.y < 1 / Screen.height) {
				timg.Translate (Vector3.up * Time.deltaTime * 80);
			} else {

				if ((na < 1.0f) && (la < 1.0f) && (qa < 1.0f)) {
					na = newimg.color.a + 0.02f;
					la = loadimg.color.a + 0.02f;
					qa = quitimg.color.a + 0.02f;

					newimg.color = new Color (255, 255, 255, na);
					newimgtxt.color = new Color (0, 0, 0, na);
					loadimg.color = new Color (255, 255, 255, la);
					loadimgtxt.color = new Color (0, 0, 0, la);
					quitimg.color = new Color (255, 255, 255, qa);
					quitimgtxt.color = new Color (0, 0, 0, qa);
				} else {
					newButton.interactable = true;
					loadButton.interactable = true;
					quitButton.interactable = true;
				}
			}
		}
	}
}
