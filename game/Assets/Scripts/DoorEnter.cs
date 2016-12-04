using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorEnter : MonoBehaviour {

    public GameObject hintCanvas;

    private bool entered;

    void Awake()
    {
        toggleInteractHint(false);
        entered = false;
    }

    void Update()
    {
        if (entered && Input.GetButtonUp("Interact"))
        {
            NextLevel(1);
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            toggleInteractHint(true);
            entered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        toggleInteractHint(false);
        entered = false;
    }
    

    public void NextLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

    void toggleInteractHint(bool status)
    {
        if (status)
        {
            hintCanvas.transform.position = transform.position + new Vector3(0, 2);
        }
        else
        {
            hintCanvas.transform.position = new Vector3(0, -999);
        }
    }
}
