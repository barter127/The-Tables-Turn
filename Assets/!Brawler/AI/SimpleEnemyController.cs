using UnityEngine;
using System.Collections;

public enum EnemyControllerState
{
    Idle,
    Walk,
    Run,
    BackAway,
    Jump,
    Hurt,
    Dodge,
    Block,
    Grab,
    IsGrabbed,
    Death,
    Attack
}

public class SimpleEnemyController : MonoBehaviour
{
    // Reference to the enum state machine
    private EnemyControllerState state;

    // Player References
    private GameObject player;
    private NEWComboSystem playerCombo;
    private PlayerController playerContr;

    // Component References
    private Animator animator;
    private Rigidbody rb;

    private bool isFacingRight;

    private float playerDistance;

    // Movement Vars
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float backAwaySpeed;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float playerRunDistance; // Distance before the AI starts to run. 
    [SerializeField] private float playerWalkDistance; // Distance at AI walks (so that the animation states don't spontaneously switch).
    [SerializeField] private float attackRange; // Range where AI can attack

    //Hurt Vars
    [SerializeField] private bool gotHit; //Changed in Unity Animatitor not Scripting.
    public bool addForceOnce;

    private int maxHealth;
    private int totalHealth;

    private void Start()
    {
        //Declaring Components
        player = GameObject.Find("Sylvia");
        playerCombo = player.GetComponentInChildren<NEWComboSystem>();
        playerContr = player.GetComponent<PlayerController>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        //Declaring Variables
        isFacingRight = false;
        gotHit = false;
    }

    private void Update()
    {
        GetPlayerAction();
        Flip(); // May have some problems just sitting in the update but oh well
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyControllerState.Hurt:
                Hurt();
                break;
            case EnemyControllerState.Walk:
                Walk();
                break;
            case EnemyControllerState.Run:
                Run();
                break;
            case EnemyControllerState.BackAway:
                BackAway();
                break;
        }

    }

    private void GetPlayerAction()
    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (gotHit)
        {
            state = EnemyControllerState.Hurt;
        }
        else if (playerDistance >= maxDistance)
        {
            state = EnemyControllerState.Idle;
        }
        else if (playerDistance >= playerRunDistance)
        {
            state = EnemyControllerState.Run;
        }
        else if (playerDistance > playerWalkDistance)
        {
            state = EnemyControllerState.Walk;
        }
        else if (playerDistance < minDistance && !gotHit)
        {
            state = EnemyControllerState.BackAway;
        }

        if (playerCombo.isAttacking == false)
        {
            addForceOnce = true;
        }
    }

    private void Hurt()
    {
        rb.velocity = Vector3.zero;
        addForceOnce = true; 

        state = EnemyControllerState.BackAway;
    }


    #region Movement/Follow States

    private void Walk()
    {
        if (playerDistance > playerWalkDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, walkSpeed * Time.deltaTime);
        }
        else
        {
            state = EnemyControllerState.Idle;
        }
    }

    private void Run()
    {
        transform.position = Vector3.MoveTowards(transform.position , player.transform.position, runSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        // jump logic
    }

    private void BackAway() // If player is too close start backing away
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, -0.5f * walkSpeed * Time.deltaTime);
    }

    #endregion

    #region Attack States

    private void AttackLogic() // Determines when to attack and how it should
    {
        if (playerDistance < attackRange)
        {
            // Attack logic
        }
    }

    private void Attack()
    {
        // Attack logic
    }

    #endregion

    #region Flip Character

    private void Flip()
    {
        if (player.transform.position.x > transform.position.x && !isFacingRight || player.transform.position.x < transform.position.x && isFacingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = !isFacingRight;
        }
    }

    #endregion

    #region Damage Collision
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "PlayerHitbox")
    //    {
    //        animator.SetTrigger("GetHit");

    //        //if (playercombo.comboblowbackforce > 0)
    //        //{
    //        //    addforceonce = false;
    //        //    gothit = true;
    //        //    rb.addforce(playercombo.comboblowbackvector * playercombo.comboblowbackforce, forcemode.impulse);
    //        //}
    //    }
    //}
    #endregion
}
