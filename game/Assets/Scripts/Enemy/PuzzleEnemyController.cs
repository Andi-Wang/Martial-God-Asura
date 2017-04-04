using UnityEngine;
using System.Collections;

public class PuzzleEnemyController : Enemy {
    
    //private bool e_FacingRight = true;

    private bool caughtPlayer;
    

    void Awake()
    {
        roomId = 9;  // room ID for top floor
        enemyEntity = new Entity(startingHealth, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 0f);
        caughtPlayer = false;
        isDead = false;
        animator = GetComponent<Animator>();
        idleState = new IdleState();
        puzzlePatrolState = new PuzzlePatrolState();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
        
        changeState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        state.Execute();
        if (GameManager.instance.currentRoom == roomId && !caughtPlayer)
        {
            changeState(puzzlePatrolState);
            
            animator.SetBool("Moving", true);

            Vector3 raycastStartPoint = transform.position + new Vector3(getDirection().x, 0);
            RaycastHit2D hit = Physics2D.Raycast(raycastStartPoint, getDirection());
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    caughtPlayer = true;
                }
                else
                {
                    if (hit.distance <= 1)
                    {
                        Flip();
                    }
                }
            }
        }

        if (caughtPlayer)
        {
            changeState(idleState);
            animator.SetBool("Moving", false);

            GameManager.instance.ResetPuzzle(roomId);

            caughtPlayer = false;
        }
    }

    public override void TakeDamage(float amount)
    {
        if (enemyEntity.health != startingHealth)
        {
            enemyEntity.health = startingHealth;
        }
    }
}
