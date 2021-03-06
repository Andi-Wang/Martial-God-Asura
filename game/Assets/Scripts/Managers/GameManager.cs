﻿using UnityEngine;
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
    public int subLevel;
    public int currentRoom;

    public SkillTree skillTree;

    HUDNotificationManager notiManager;
    private Text levelText;                                 //Text to display current level number.
    private GameObject levelImage;                          //Image to block out level as levels are being set up, background for levelText.

    private Player playerStat;                                                 //private BoardManager boardScript;						//Store a reference to our BoardManager which will set up the level.
   // private Skills skills;
    private List<Enemy> enemies;                            //List of all Enemy units, used to issue them move commands.
    private bool doingSetup = true;                         //Boolean to check if we're setting up board, prevent Player from moving during setup.
    Transform player;
    UnityStandardAssets._2D.PlatformerCharacter2D playerScript;
    UnityStandardAssets._2D.Platformer2DUserControl userCtrl;
    LadderManager ladderManager;
    static bool startFromLoad = false;
    static GameStatus gs;
    bool subLevelComplete = false;
    

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
        skillTree = new SkillTree();

        subLevel = 0;

        findNotificationManager();

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // update the room that player is in
        if(level > 0 && !RoomManager.Instance.inRoom(currentRoom, player.position.x, player.position.y))
        {
            currentRoom = RoomManager.Instance.findRoomId(player.position.x, player.position.y);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

	//Initializes the game for each level.
	void InitGame()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
		doingSetup = true;
        playersTurn = false;
 
        subLevelComplete = false;

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
                levelText.text = "Level " + level + " - Part " + subLevel;

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
		
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // scene 0 ~ 2 level number is -1 0 1, later unchange
        // scene 2~4 sublevel 1 2 3
        if (scene.buildIndex>=2)
        {
            level = 1;
            subLevel = scene.buildIndex - 1;

            // assign player position
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            player = p.transform;
            playerScript = p.GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>();
            userCtrl = p.GetComponent<UnityStandardAssets._2D.Platformer2DUserControl>();

            findNotificationManager();
            ladderManager = GameObject.Find("TrapDoorTriggers").GetComponent<LadderManager>();

            if (startFromLoad)
            {
                SetupFromLoad();
                startFromLoad = false;
            }

            InitGame();
        }
        else
        {
            level = -1;
        }
    }

    bool findNotificationManager()
    {
        if (notiManager != null)
            return true;

        GameObject obj = GameObject.Find("NotificationPanel");
        if (obj != null)
        {
            notiManager = obj.GetComponent<HUDNotificationManager>();
            return true;
        }

        return false;
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
        playerScript.PlayerEntity = playerStat.playerEntity;
        player.position = new Vector3(playerStat.playerPosX, playerStat.playerPosY);

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

    public IEnumerator SaveProgress()
    {
        gs = new GameStatus();

        playerStat.playerEntity = playerScript.PlayerEntity;
        playerStat.playerPosX = player.position.x;
        playerStat.playerPosY = player.position.y;
        gs.playerStat = playerStat;
        //gs.enemies = enemies;// null enemies cause serializable exception
        gs.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        gs.subLevel = subLevel;
        gs.ladderUnlocked = ladderManager.GetUnlockedLadder();
        gs.skillTree = skillTree;

        yield return null;

        ProgressSL.save(gs);

        yield return null;

        displayNotification("Progress Saved");
    }

    public void LoadProgress()
    {
        gs = ProgressSL.load();

        if (gs != null)
        {
            skillTree = gs.skillTree;
            playerStat = gs.playerStat;
            subLevel = gs.subLevel;

            startFromLoad = true;
            if (gs.sceneNumber != SceneManager.GetActiveScene().buildIndex)
            {
                StartCoroutine(loadLvAsync(gs.sceneNumber));
            }
            else
            {
                Debug.Log("in the correct scene");
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

        displayNotification("The enemy has seen you");

        float puzzleStartX = RoomManager.Instance.GetXMax(roomNum) - 2f;

        player.position = new Vector3(puzzleStartX, player.position.y);
        
        playersTurn = true;
    }

    public void displayNotification(string str)
    {
        if (notiManager != null)
        {
            notiManager.Display(str);
        }
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

    public void gotoNextLevel()
    {
        StartCoroutine(loadLvAsync(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public IEnumerator loadLvAsync(int idx)
    {
        displayNotification("Loading...");

        yield return null;

        AsyncOperation async = SceneManager.LoadSceneAsync(idx);

        if (!async.isDone)
        {
            yield return null;
        }
    }

    public bool SubLevelComplete
    {
        get
        {
            return subLevelComplete;
        }
        set
        {
            subLevelComplete = value;
        }
    }

    public void ToggleSkillMenu()
    {
        userCtrl.ToggleSkillMenu();
    }
}


