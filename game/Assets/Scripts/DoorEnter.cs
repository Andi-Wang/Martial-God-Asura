using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorEnter : MonoBehaviour {

    public GameObject hintCanvas;

    ToggleHintUI hintUIController;
    private bool entered;

    void Awake()
    {
        hintUIController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ToggleHintUI>();
        hintUIController.toggleHint();
        entered = false;
    }

    void Update()
    {
        if (entered && Input.GetButtonUp("Interact"))
        {
            NextLevel(2);
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            
            hintUIController.toggleHint(transform.position.x, transform.position.y+2);
            entered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        hintUIController.toggleHint();
        entered = false;
    }
    

    public void NextLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }

}
