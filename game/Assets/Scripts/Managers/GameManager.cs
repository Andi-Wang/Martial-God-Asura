using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;                   //Allows us to use UI.
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 1.2f;                      //Time to wait before starting level, in seconds.
//    public float turnDelay = 0.1f;                          //Delay between each Player turn.
    //public int playerPoints = 100;                      //Starting value for Player points.
    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    [HideInInspector] public bool playersTurn = true;       //Boolean to check if it's players turn, hidden in inspector but public.
    public int level;
    public int currentRoom;
    public HUDNotificationManager notiManager;

    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.

    private Player playerStat;                                                 //private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
    private Skills skills;
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    Transform player;
    UnityStandardAssets._2D.PlatformerCharacter2D playerScript;
    LadderManager ladderManager;
    static bool startFromLoad = false;
    static GameStatus gs;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //these should changed if load from data
        enemies = new List<Enemy>();
        playerStat = new Player();
        skills = new Skills();

        //DontDestroyOnLoad(gameObject);
        //Call the InitGame function to initialize the first level 
        if (level >= 0)
        {
            InitGame();
        }
    }

    void Update()
    {
        // update the room that player is in
        if(level > 0 && !RoomManager.Instance.inRoom(currentRoom, player.position.x, player.position.y))
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
        
        // find ladderManager
        if (level > 0)
        {
            ladderManager = GameObject.Find("TrapDoorTriggers").GetComponent<LadderManager>();
        }
        
        // assign player position
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.transform;
        playerScript = p.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();

        if (startFromLoad)
        {
            SetupFromLoad();
            startFromLoad = false;
        }

        if (level > 0)
        {
            // Set up room manager
            currentRoom = RoomManager.Instance.findRoomId(player.position.x, player.position.y); 

            levelImage = GameObject.Find("LevelImage");

            if (levelImage != null)
            {
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

    void SetupFromLoad()
    {
        playerScript.PlayerEntity = gs.playerStat.playerEntity;
        player.position = new Vector3(gs.playerStat.playerPosX, gs.playerStat.playerPosY);

        //setup skills gs.playerStat.skills;
        
        //setup enemies gs.enemies = enemies;// null enemies cause serializable exception
        for(int i = 0; i < gs.ladderUnlocked.Length; ++i)
        {
            if (gs.ladderUnlocked[i])
            {
                ladderManager.unlockLadder(i);
            }
        }

        // reset gamestatus data
        gs = null;
    }

    public void SaveProgress()
    {
        gs = new GameStatus();

        playerStat.playerEntity = playerScript.PlayerEntity;
        playerStat.playerPosX = player.position.x;
        playerStat.playerPosY = player.position.y;

        // dummy skills for save game
        skills.AddSkill("Back Dash", 1, true);
        skills.AddSkill("Glide", 1, true);
        skills.AddSkill("Fast Fall", 1, true);
        skills.AddSkill("Punch", 1, true);
        playerStat.skills = skills;

        gs.playerStat = playerStat;
        //gs.enemies = enemies;// null enemies cause serializable exception
        gs.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        gs.ladderUnlocked = ladderManager.GetUnlockedLadder();
        
        ProgressSL.save(gs);
    }

    public void LoadProgress()
    {
        gs = ProgressSL.load();

        if (gs != null)
        {
            startFromLoad = true;
            if (gs.sceneNumber != SceneManager.GetActiveScene().buildIndex)
            {
                SceneManager.LoadScene(gs.sceneNumber);
            }
            else
            {
                Debug.Log("in the right scene");
                InitGame();
            }
        }
        else
        {
            startFromLoad = false;
        }
    }

    public void ResetPuzzle(int roomNum)
    {
        playersTurn = false;

        notiManager.Display("The enemy has seen you");

        float puzzleStartX = RoomManager.Instance.GetXMax(roomNum) - 2f;

        player.position = new Vector3(puzzleStartX, player.position.y);
        
        playersTurn = true;
    }

    void transformPlayer()
    {
        
    }

    public List<Enemy> Enemies
    {
        get{ return enemies;}
    }

    public static void Pause()
    {
        Time.timeScale = 0;
    }

    public static void Resume()
    {
        Time.timeScale = 1;
    }
    
}


