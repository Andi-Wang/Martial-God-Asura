using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    public Entity enemyEntity;

    public Rigidbody2D m_fireball;

    public float speed = 1f;
    
    public float startingHealth = 10f;
    public float currentHealth;
    public float playerDamage;
    public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.
    public float detectionRange = 3f;
    public float stoppingDistance = 0.5f;

    protected EnemyState state;
    public PatrolState patrolState;
    public AttackState attackState;
    public ChaseState chaseState;
    public IdleState idleState;
    public PuzzlePatrolState puzzlePatrolState;
    public RangedAttackState rangedAttackState;

    private float AttackRange = 1.5f;
    private float RangeAttackRange = 15f;
    private float RangedAttackCD = 4f;
    private float LastRangeTime = 4f;
    private bool canRangeAttack = true;
    public bool isBoss = false;

    AudioSource enemyAudio;
    public Collider2D attackBox;
    private BoxCollider2D boxCollider;
    private GameObject player;
    private Vector3 playerPos;
    private Vector3 playerPrevPos;
    private Image healthbar;
    public Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    bool isDead;
    public Rigidbody2D rb2D;
    private bool e_FacingRight = true;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //currentHealth = startingHealth;
        enemyEntity = new Entity(startingHealth, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 0f);
        enemyAudio = GetComponent<AudioSource>();
    }

    //Start overrides the virtual Start function of the base class.
    void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);
        
        patrolState = new PatrolState();
        attackState = new AttackState();
        chaseState = new ChaseState();
        rangedAttackState = new RangedAttackState();

        animator = GetComponent<Animator>();
        changeState(patrolState);
        animator.SetBool("Moving", true);
        healthbar = transform.FindChild("EnemyCanvas").FindChild("Healthbar").FindChild("Health").GetComponent<Image>();
    }

    void Update()
    {
        if (isDead) return;

        if (enemyEntity.health > 0 /*&& playerHealth.currentHealth > 0*/)
        {
            state.Execute();
        }
        else if (enemyEntity.health <= 0)
        {
            Death();
        }
    }

    public void changeState(EnemyState newState)
    {
        if (state != null)
        {
            // check if new state is the same as current state
            if (state.GetType() == newState.GetType())
            {
                return;
            }
            state.Leave();
        }

        state = newState;
        state.Begin(this);
    }

    public void setPlayerPos()
    {
        playerPrevPos = playerPos;
        playerPos = player.transform.position;
    }

    public Vector3 getPlayerPrevPos()
    {
        return playerPrevPos;
    }

    public Vector3 getPlayerPos()
    {
        return playerPos;
    }

    public void LookAtTarget()
    {
        if (player != null)
        {
            float xDir = player.transform.position.x - transform.position.x;
            if(xDir < 0 && e_FacingRight || xDir > 0 && !e_FacingRight)
            {
                Flip();
            }
        }
    }

    public bool TargetInMeleeRange()
    {
        if (player != null)
        {
            float totalDis = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y, 2));
            if (Mathf.Abs(totalDis) <= AttackRange)
                return true;
            else
                return false;
        }
        return false;
    }

    public bool TargetInRange()
    {
        if (player != null)
        {
            float totalDis = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y,2));
            if (Mathf.Abs(totalDis) <= RangeAttackRange)
                return true;
            else
                return false;
        }
        return false;
    }

    public void updateRangeCD()
    {
        LastRangeTime += Time.deltaTime;
        if (LastRangeTime >= RangedAttackCD)
        {
            canRangeAttack = true;
            LastRangeTime = 0;
        }
        else
            canRangeAttack = false;
    }

    public bool rangeCDCheck()
    {
        updateRangeCD();
        return canRangeAttack;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead)
            return;

        
        enemyAudio.Play();//TODO:hurt audio
        animator.SetTrigger("Hit");

        enemyEntity.health -= amount;
        healthbar.fillAmount = enemyEntity.health / enemyEntity.maxHealth;

        //hitParticles.transform.position = hitPoint;
        //hitParticles.Play();

        if (enemyEntity.health <= 0)
        {
            Death();
        }

        changeState(attackState);
    }

    void Death()
    {
        isDead = true;

        foreach(Transform child in gameObject.transform) {
            if (child.name == "EnemyCanvas") child.gameObject.SetActive(false);
        }

        //boxCollider.isTrigger = true;

        animator.SetTrigger("Dead");

       // enemyAudio.clip = deathClip;
        //enemyAudio.Play();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        state.OnTriggerEnter2D(other);
    }

    public void Move()
    {
        animator.SetBool("Moving", true);
        transform.Translate(getDirection() * speed * Time.deltaTime, Space.World);
    }

    public Vector2 getDirection()
    {
        return e_FacingRight ? Vector2.right : Vector2.left;
    }

    public void Flip()
    {
        // Switch the way the player is labelled as facing.
        e_FacingRight = !e_FacingRight;

        // Multiply the player's x local scale by -1.
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }
}

