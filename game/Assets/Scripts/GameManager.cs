using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.
	
public class GameManager : MonoBehaviour
{
	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;							//Delay between each Player turn.
	public int playerPoints = 100;						//Starting value for Player points.
	public static GameManager instance = null;				//Static instance of GameManager which allows it to be accessed by any other script.
	[HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    public int currentRoom;	
		
	private Text levelText;									//Text to display current level number.
	private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
	//private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
	public int level = 1;								
	private List<Enemy> enemies;							//List of all Enemy units, used to issue them move commands.
	private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
		
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
		
	//Initializes the game for each level.
	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;
        playersTurn = false;
			
		//Get a reference to our image LevelImage by finding it by name.
		levelImage = GameObject.Find("LevelImage");
			
		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
			
		//Set the text of levelText to the string "Day" and append the current level number.
		levelText.text = "Level " + level;
			
		//Set levelImage to active blocking player's view of the game board during setup.
		levelImage.SetActive(true);
			
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);
			
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
}


