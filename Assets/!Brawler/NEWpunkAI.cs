using System.Collections;
using UnityEngine;

public class NEWpunkAI : MonoBehaviour
{
    private Rigidbody rb;
    private SpriteRenderer spr;

    [SerializeField] private GameObject player; //Set in Unity.
    private NEWComboSystem playerComboSystem;
    [SerializeField] private HealthBar healthManager; //Set in Unity.
    private GameObject manager;
    private LogicManager lm;
    private Animator animator;
    [SerializeField] private BoxCollider boxColl;


    [SerializeField] private float speed;

    //Check direction to face.
    public bool isFacingRight;

    //Distance to player
    private Vector3 distanceVector;
    private float distanceX;
    private float distanceZ;

    //private Vector3 moveDirection; //Direction of Movement.

    //Range to Player.
    [SerializeField] private float minRangeX;
    [SerializeField] private float maxRangeX;
    [SerializeField] private float minRangeZ;
    [SerializeField] private float maxRangeZ;

    //Attack Vars
    private float attackTimer;
    [SerializeField] private float minAttackTimerLength;
    [SerializeField] private float maxAttackTimerLength;
    [SerializeField] private float minAttackDistance; //How close AI gets before attacking.
    private bool animationFarted = false;

    [SerializeField] private AttackScriptableObject basicAttack;
    [SerializeField] private AttackScriptableObject aerialAttack;

    //Health & collision
    [SerializeField] int health = 0;
    [SerializeField] private int maxHealth; //Health enemy starts with.
    public AttackScriptableObject playerCurrentAttack; //Stores attack data.
    public bool hasBeenHit = false; //Used in DamageNumbersController
    private bool playGetHitOnce = false;
    private float fallTimer;
    public float getUpTimer;
    [SerializeField] private float getUpTimerLength;
    public bool onGround;
    private float hurtTimer;

    public bool spotPlayer;
    [SerializeField] private float playerSpotDistance;

    // Current state of the AI
    private State currentState;

    public enum State
    {
        Idle,
        Move,
        Attack,
        Hurt,
        Fall,
        OnGround,
        GetUp,
        Die,
    }

    void Start()
    {
        // Set initial state.
        currentState = State.Idle;

        // Set Components.
        rb = GetComponent<Rigidbody>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Sylvia");
        playerComboSystem = player.GetComponentInChildren<NEWComboSystem>();
        manager = GameObject.Find("LogicManager");
        lm = manager.GetComponent<LogicManager>();

        attackTimer = Random.Range(0, 2 + maxAttackTimerLength);
        health = maxHealth;
    }

    void Update()
    {
        //Calculate distance from player.
        distanceVector = transform.position - player.transform.position;
        distanceX = distanceVector.x;
        distanceZ = distanceVector.z;

        // Check current state and perform actions accordingly.
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Move:
                Move();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Hurt:
                Hurt();
                break;
            case State.Fall:
                Fall();
                break;
            case State.OnGround:
                OnGround();
                break;
        }

        if (health <= 0) //NEED TO IMPLEMENT FALL DOWN THEN FADE AWAY
        {
            LogicManager.alertEnemies--;
            Destroy(gameObject); //Destroy self if health = 0
        }

        if (animationFarted) //Disables HB when not attacking to prevent unessecary collisions.
        {
            boxColl.enabled = true;
        }
        else if (!animationFarted && boxColl.enabled == true)
        {
            boxColl.enabled = false;
        }
    }

    void Idle() //State before the player gets in range of the enemy.
    {

        //Stand still till player in range then play spot animation.
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (distance < playerSpotDistance)
        {
            spotPlayer = true;
            LogicManager.alertEnemies++;
            ChangeState(State.Move);
        }
    }

    void Move() // Moves towards or away from the player.
    {
        Vector3 moveDirection = Vector3.zero;

        // Check if the AI is on the right or left side of the player.
        bool isOnRightSide = transform.position.x > player.transform.position.x;

        // If AI is on the right side, move towards the player on the X-axis if too far, else move away.
        if (isOnRightSide)
        {
            if (distanceX > maxRangeX)
            {
                moveDirection.x = -1f;   //Move left.
                animator.SetBool("IsWalking", true);
            }
            else if (distanceX < minRangeX)
            {
                moveDirection.x = 0.4f;   //Move right.
                animator.SetBool("IsWalking", true);
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                animator.SetBool("IsWalking", false);
            }
        }
        // If AI is on the left side, move away from the player on the X-axis if too close, else move towards.
        else if (!isOnRightSide)
        {
            if (distanceX < -maxRangeX)
            {
                moveDirection.x = 1f;   //Move right.
                animator.SetBool("IsWalking", true);
            }
                
                
            else if (distanceX > -minRangeX)
            {
                moveDirection.x = -0.4f;   //Move left.
                animator.SetBool("IsWalking", true); 
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
                animator.SetBool("IsWalking", false);
            }
        }

        if ((!isOnRightSide && !isFacingRight) || (isOnRightSide && isFacingRight))
        {
            FlipSprite();
        }

        // Check Z-axis movement.
        if (distanceZ > maxRangeZ)
            moveDirection.z = -0.4f; // Move backward (away from player).
        else if (distanceZ < minRangeZ)
            moveDirection.z = 0.4f; // Move forward (towards player).
        else
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0f);

        // Move the AI
        rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);

        #region Attack Timer
        if (lm.enemyIsAttacking)
            attackTimer = Random.Range(0, maxAttackTimerLength);

        else if (attackTimer < 0 && !lm.enemyIsAttacking)
        {
            ChangeState(State.Attack);
            lm.enemyIsAttacking = true;
        }
        else
            attackTimer -= Time.deltaTime;
        #endregion
    }

    void Attack() //Attacks the player.
    {
        Vector3 aiTargetPoint = new Vector3(player.transform.position.x + minAttackDistance, transform.position.y, player.transform.position.z);

        float distanceX = Mathf.Abs(transform.position.x - player.transform.position.x);

        if (distanceX > minAttackDistance) //If outside attack range
        {
            //Move the AI towards the aiTargetPoint.
            Vector3 moveDirection = (aiTargetPoint - transform.position).normalized;
            rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
            animator.SetBool("IsWalking", true);
        }

        else if (distanceX < minAttackDistance) //If in attack range.
        {
            rb.velocity = Vector3.zero;

            //Peform Attack.
            if (!animationFarted)
            {
                animationFarted = true;

                animator.runtimeAnimatorController = basicAttack.animatorOV;
                animator.Play("Attack State", 0, 0);
            }
        }
    }

    void Hurt()
    {

        playerCurrentAttack = playerComboSystem.currentAttack; //Update current attack for damage calculation.

        if (!playGetHitOnce) //Allow attack code to trigger once per collision.
        {
            //Calculate health
            health = health - playerCurrentAttack.damage;
            healthManager.SetHealth(health, maxHealth);

            if (playerCurrentAttack.canKnockdown) //If AttackScriptableObject marked as can knockdown.
            { 
                playGetHitOnce = true;
                animationFarted = false; //Fixes HB being on when onGround.
                animator.SetTrigger("StartFall"); //Start fall anim.
                fallTimer = playerCurrentAttack.fallTimer; //Start timer to determine how long the fall lasts.
                ChangeState(State.Fall);

                Debug.Log("switch to knockdown");
            }
            else
            {
                playGetHitOnce = true;
                animator.SetTrigger("GetHit");

                StartCoroutine(ResetHIt());

                rb.velocity = Vector3.zero; //Freeze enemy after hit.
                float blowbackForce = isFacingRight ? -playerCurrentAttack.enemyBlowbackForce : playerCurrentAttack.enemyBlowbackForce; //Changes blowback force depending on enemy direction.
                rb.AddForce(blowbackForce, 0, 0);

                Debug.Log("switch to move");
            }
        }

        if (hurtTimer <= 0)
        {
            animator.SetTrigger("ExitKnockdown");
            animator.SetBool("IsWalking", true);
            playGetHitOnce = false;
            hasBeenHit = false;
            ChangeState(State.Move);
        }
        else { hurtTimer -= Time.deltaTime; }


        }

    void Fall()
    {
        Debug.Log("Fall");

        if (fallTimer <= 0) //RB stops moving.
        {
            animator.SetTrigger("EndFall");

            getUpTimer = getUpTimerLength;
            onGround = true;

            ChangeState(State.OnGround);
        }
        else if (fallTimer > 0)
        {
            fallTimer -= Time.deltaTime;
        }     
    }

    void OnGround()
    {
        Debug.Log("Onground");

        rb.velocity = Vector3.zero;

        if (getUpTimer <= 0) 
        {   
            animator.SetTrigger("WAKEUP!!!");

            onGround = false;
            hasBeenHit = false;
            playGetHitOnce = false;

            ChangeState(State.Move);
        }
        else if (getUpTimer > 0)
        {
            getUpTimer -= Time.deltaTime;
        }
    }

    // Method to flip the sprite.
    private void FlipSprite()
    {
        isFacingRight = !isFacingRight; //Change to face correct direction.
        spr.flipX = isFacingRight; //Flip sprite to face player.
    }

    // Method to change states
    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    public void OnAttackAnimationEnd() //Called at the end of an attack animation.
    {
        ChangeState(State.Move);
        animationFarted = false;
        lm.enemyIsAttacking = false;
        attackTimer = Random.Range(minAttackTimerLength, maxAttackTimerLength);
        animator.SetBool("IsWalking", false);
    }

    public void OnHurtEnd() //Called at the end of hurt animation.
    {
        ChangeState(State.Move);

        hasBeenHit = false;

        //Set attack vars to incase hurt while in attack state.
        animationFarted = false;
        lm.enemyIsAttacking = false;

        Debug.Log("Hurt End");
    }

    private IEnumerator ResetHIt()
    {
        yield return new WaitForSeconds(0.35f);

        playGetHitOnce = false;
    }

    public void OnPlayerSpotEnd()
    {
        ChangeState(State.Move);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerHitbox" && !hasBeenHit)
        {
            hasBeenHit = true;

            hurtTimer = 3;
            ChangeState(State.Hurt);
        }
    }
}