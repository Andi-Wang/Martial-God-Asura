using UnityEngine;
using System.Collections;

[System.Serializable]
//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class Enemy : MonoBehaviour
{
    public Entity enemyEntity;

    public float speed = 1f;
    
    public float startingHealth = 10f;
    public float currentHealth;
    public float playerDamage;
    public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.
    public AudioClip attackSound2;                      //Second of two audio clips to play when attacking the player.
    public float detectionRange = 3f;
    public float stoppingDistance = 0.5f;

    private EnemyState state;
    private BoxCollider2D boxCollider;
    private GameObject player;
    public Animator animator;                          //Variable of type Animator to store a reference to the enemy's Animator component.
    bool isDead;
    private Rigidbody2D rb2D;
    [SerializeField]
    private float meleeRange = 2f;
    public bool inMeleeRange;

    private bool e_FacingRight = true;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        //currentHealth = startingHealth;
        enemyEntity = new Entity(10f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 0f);
    }

    //Start overrides the virtual Start function of the base class.
    void Start()
    {
        //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
        //This allows the GameManager to issue movement commands.
        GameManager.instance.AddEnemyToList(this);
        //Get and store a reference to the attached Animator component.
        animator = GetComponent<Animator>();
        animator.SetBool("Moving", true);
        changeState(new PatrolState());
    }

    void Update()
    {
     /*   if (player != null)
        {
            inMeleeRange = Vector2.Distance(transform.position, player.transform.position) <= meleeRange;
        } else inMeleeRange = false;
        */
        if (isDead) return;
        else if (enemyEntity.health > 0 /*&& playerHealth.currentHealth > 0*/)
        {
            state.Execute();
            //LookAtTarget();
            Vector3 raycastStartPoint = transform.position + new Vector3(getDirection().x*2, 0);
            RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, getDirection());
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag != "Player")
                {
                    //float distance = Mathf.Abs(hit.point.x - raycastStartPoint.x);
                    //Debug.Log(distance);
                    // change to opposite direction when enemy is close to the wall
                    if (hit.distance <= 2)
                    {
                        Flip();
                    }
                }
                
            }
        }
        else if (enemyEntity.health <= 0)
        {
            Death();
        }
    }

    public void changeState(EnemyState newState)
    {
        if (state != null)
            state.Leave();

        state = newState;
        state.Begin(this);
    }

    private void LookAtTarget()
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

    void Death()
    {
        isDead = true;
        
        //boxCollider.isTrigger = true;

        //animator.SetTrigger("Dead");

       // enemyAudio.clip = deathClip;
        //enemyAudio.Play();   
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        state.OnTriggerEnter2D(other);
    }

    public void Move()
    {
        //if (!inMeleeRange)
       // {
           // animator.SetBool("Moving", true);
            transform.Translate(getDirection() * speed * Time.deltaTime, Space.World);
       // }
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

