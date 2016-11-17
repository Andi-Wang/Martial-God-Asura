using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorEnter : MonoBehaviour {

    public GameObject hintCanvas;

    private bool entered;

    void Awake()
    {
        hintCanvas.SetActive(false);
        entered = false;
    }

    void Update()
    {
        if (Input.GetButtonUp("Interact") && entered)
        {
            NextLevel(1);
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            hintCanvas.SetActive(true);
            entered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hintCanvas.SetActive(false);
        entered = false;
    }
    

    public void NextLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }
}
