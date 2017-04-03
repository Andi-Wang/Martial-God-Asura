using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    public Entity enemyEntity;

    public GameObject m_fireball;

    public float speed = 3f;
    public float chaseSpeed;
    public float baseSpeed;
    
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
    private float RangeAttackRange = 12f;

    public float rangedAttackCD = 4f;
    private float nextRangeTime;
    public bool canRangeAttack = true;

    public float meleeAttackCD = 1f;
    public float nextMeleeTime;
    public bool canMeleeAttack = true;

    public bool isGhost = false;

    public bool isBoss = false;
    public bool isBoss1 = false;
    public bool isBoss2 = false;
    public bool isBoss3 = false;

    [HideInInspector]
    public float boss2Timer = 0; 

    public bool canMove = true;

    public bool canBeInterrupted;

    public bool isImmune = false;

    public bool cannotChase;

    private bool hasMeleeAttacked = true;
    private bool hasRangedAttacked = true;

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

    public bool dashing;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        enemyEntity = new Entity(startingHealth, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 0f);
        enemyAudio = GetComponent<AudioSource>();
    }

    //Start overrides the virtual Start function of the base class.
    void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);

        currentHealth = startingHealth;

        nextRangeTime = rangedAttackCD;
        nextMeleeTime = meleeAttackCD;

        if (!canMove)
            AttackRange = 3f;

        baseSpeed = speed;
        if (isBoss)
        {
            if(chaseSpeed == 0)
                chaseSpeed = speed + 2f;
        }
        else
        {
            if(chaseSpeed == 0)
                chaseSpeed = speed + 1f;
        }

        patrolState = new PatrolState();
        attackState = new AttackState();
        chaseState = new ChaseState();
        rangedAttackState = new RangedAttackState();
        animator = GetComponent<Animator>();
        if (canMove)
        {
            changeState(patrolState);
        }
        else
            changeState(chaseState);

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

    public float disToPlayer()
    {
        return Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.y - transform.position.y, 2));
    }

    public bool TargetInMeleeRange()
    {
        if (player != null)
        {
            float totalDis = disToPlayer();
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
            float totalDis = disToPlayer();
            if (Mathf.Abs(totalDis) <= RangeAttackRange)
                return true;
            else
                return false;
        }
        return false;
    }

    public bool updateRangeCD()
    {
        if (Time.time > nextRangeTime)
        {
            hasRangedAttacked = false;
            nextRangeTime = Time.time + rangedAttackCD;
            return true;
        }
        else
            return false;
    }

    public void rangeCDCheck(bool tf)
    {
        hasRangedAttacked = tf;
    }

    public bool updateMeleeCD()
    {
        if (Time.time > nextMeleeTime)
        {
            hasMeleeAttacked = false;
            nextMeleeTime = Time.time + meleeAttackCD;
            return true;
        }
        else
            return false;
    }

    public void meleeCDCheck(bool tf)
    {
        hasMeleeAttacked = tf;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead)
            return;

        if (canBeInterrupted)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            rb2D.velocity = Vector2.zero;
        }

        if (!isImmune)
        {
            enemyAudio.Play();
            enemyEntity.health -= amount;
            healthbar.fillAmount = enemyEntity.health / enemyEntity.maxHealth;
        }

        if (enemyEntity.health <= 0)
        {
            Death();
        }

        changeState(chaseState);
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
        if (canMove)
        {
            animator.SetBool("Moving", true);
            transform.Translate(getDirection() * speed * Time.deltaTime, Space.World);
        }
    }

    public void moveBackwards()
    {
        if (canMove)
        {
            animator.SetBool("Moving", true);
            e_FacingRight = !e_FacingRight;
            transform.Translate(getDirection() * speed * Time.deltaTime, Space.World);
            e_FacingRight = !e_FacingRight;
        }
    }

    public void ghostMove()
    {
        LookAtTarget();
        animator.SetBool("Moving", true);
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * 2 * Time.deltaTime);

        //Flipping sprite
        Vector3 vToPlay = player.transform.position - transform.position;
        float angle = Mathf.Atan2(vToPlay.y, vToPlay.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        if(Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            GetComponent<SpriteRenderer>().flipY = true;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipY = false;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void resetRotation()
    {
        transform.rotation = Quaternion.identity;
        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            GetComponent<SpriteRenderer>().flipY = true;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipY = false;
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public void moveThough()
    {
        float xMT;
        if (e_FacingRight)
            xMT = transform.position.x + 1;
        else
            xMT = transform.position.x - 1;
        Vector3 v3 = new Vector3(xMT,transform.position.y,transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, v3, speed * Time.deltaTime);
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