using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;                      //Time to wait before starting level, in seconds.
    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    public int playerPoints = 100;                      //Starting value for Player points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    
    public int currentRoom;

    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.
                                                            //private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
    int level = -1;
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    Transform player;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Assign enemies to a new List of Enemy objects.
        enemies = new List<Enemy>();
        

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    void Update()
    {
        // update the room that player is in
        // Debug: remove = for release
        if(level >= 0 && !RoomManager.Instance.inRoom(currentRoom, player.position.x, player.position.y))
        {
            currentRoom = RoomManager.Instance.findRoomId(player.position.x, player.position.y);
        }
    }

	//Initializes the game for each level.
	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;
        playersTurn = false;

        // assign player position
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (level >= 0) //Debug: remove = for release version
        {
            // Set up room manager
            currentRoom = RoomManager.Instance.findRoomId(player.position.x, player.position.y); 

            levelImage = GameObject.Find("LevelImage");

            //Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
            levelText = GameObject.Find("LevelText").GetComponent<Text>();

            //Set the text of levelText to the string "Day" and append the current level number.
            levelText.text = "Level " + level;

            //Set levelImage to active blocking player's view of the game board during setup.
            levelImage.SetActive(true);

            //Call the HideLevelImage function with a delay in seconds of levelStartDelay.
            Invoke("HideLevelImage", levelStartDelay);
        }
        else
        {
            doingSetup = false;
            playersTurn = true;
        }
		//Clear any Enemy objects in our List to prepare for next level.
		enemies.Clear();
    }
		
		
	//Hides black image used between levels
	void HideLevelImage()
	{
		//Disable the levelImage gameObject.
		levelImage.SetActive(false);
			
		//Set doingSetup to false allowing player to move again.
		doingSetup = false;
        playersTurn = true;
	}
		
		
	//Call this to add the passed in Enemy to the List of Enemy objects.
	public void AddEnemyToList(Enemy script)
	{
		//Add Enemy to List enemies.
		enemies.Add(script);
	}
		
	public void switchRoom()
    {

    }

	//GameOver is called when the player reaches 0 food points
	public void GameOver()
	{
		//Set levelText to display number of levels passed and game over message
		levelText.text = "Game Over";
			
		//Enable black background image gameObject.
		levelImage.SetActive(true);
			
		//Disable this GameManager.
		enabled = false;
	}

    //This is called each time a scene is loaded.
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }

    void OnEnable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
        //Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public int Level
    {
        get
        {
            return level;
        }
    }
}


