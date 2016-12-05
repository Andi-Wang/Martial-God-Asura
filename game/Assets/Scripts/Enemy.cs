using UnityEngine;
using System.Collections;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    
    public float startingHealth = 10f;
    public float currentHealth;
    public float playerDamage;
    public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.
    public float detectionRange = 3f;
    public float stoppingDistance = 0.5f;

    private BoxCollider2D boxCollider;
    private Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    private Transform player;                           //Transform to attempt to move toward each turn.
    bool isDead;
    private Rigidbody2D rb2D;
    Vector3 movement;
    float origX;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        origX = transform.position.x;
        currentHealth = startingHealth;
        movement.Set(-1, 0, 0);
    }

    //Start overrides the virtual Start function of the base class.
    void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);

        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentHealth > 0 /*&& playerHealth.currentHealth > 0*/)
        {
            MoveEnemy();
        }
    }

    public void TakeDamage(float amount, Vector3 hitPoint)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        //PlayerScoreManager.score += amount;

        
        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        isDead = true;
        
        boxCollider.isTrigger = true;

        //animator.SetTrigger("Dead");

       // enemyAudio.clip = deathClip;
        //enemyAudio.Play();   
    }
		
	//MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
	public void MoveEnemy ()
	{
		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		int xDir = 0;
		int yDir = 0;

        float yOffset = player.position.y - transform.position.y;
        //If the difference in positions is approximately zero (Epsilon) do the following:
        if (Mathf.Abs(yOffset) < float.Epsilon && Mathf.Abs(player.position.x - transform.position.x) <= detectionRange)
        {
            //If the y coordinate of the player's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
            yDir = player.position.y > transform.position.y ? 1 : -1;
            xDir = player.position.x > transform.position.x ? 1 : -1;
  
            movement.Set(xDir, yDir, 0);
            movement = movement * speed * Time.deltaTime;
            rb2D.MovePosition(transform.position + movement);
        }
        else
        {
            Patrol();
        }
	}

    void Attack()
    {
        animator.SetTrigger("enemyAttack");

        //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
        SoundManager.instance.RandomizeSfx(attackSound1, attackSound2);

        //TODO: player take damage
    }

    void Patrol()
    {
        float currentXOffset = transform.position.x - origX;

        // out of positive range
        if (currentXOffset > detectionRange)
        {
            movement.Set(-1, 0, 0);
        }
        else if (currentXOffset < -detectionRange)
        {
            movement.Set(1, 0, 0);
        }

        movement = movement * speed * Time.deltaTime;

        rb2D.MovePosition(transform.position + movement);
    }
}

