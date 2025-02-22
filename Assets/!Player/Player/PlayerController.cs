using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Declaring Variables
    //Components
    [SerializeField] private LayerMask groundLayer;
    public Rigidbody rb;
    public Animator animator;
    private NEWComboSystem combo;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private LogicManager lm;

    //Walk and Run Variables
    private float x;
    private float z;
    private Vector3 movementDirX;
    private Vector3 movementDirZ;

    //Adjust to get/set once finished experimenting with variables for less animation bugs.
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    public bool isRunning { get; private set; }
    public bool isMoving { get; private set; }

    [SerializeField] private float drag;
    private Vector3 dragMovementX;
    private Vector3 dragMovementZ;

    private const float DoubleClickTime = 0.2f;
    private float lastTapTime;

    public bool isFacingRight;
    //private bool isRolling;
    public bool addForceOnce = false;

    //Jump
    [SerializeField] private Transform groundCheck;

    private bool startJump;
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplyer = 2f;

    public bool hasBeenHit = false;
    
    private int health;
    [SerializeField] private int maxHealth;
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        combo = GetComponentInChildren<NEWComboSystem>();

        isFacingRight = true;

        health = maxHealth;
    }

    private void Update()
    {
        #region Input and Force Vectors
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");


            isMoving = true;
        }
        else if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
        {
            isMoving = false;
        }

        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        movementDirX = new Vector3(x, 0, 0);
        movementDirZ = new Vector3(0, 0, z * 0.75f);

        dragMovementX = new Vector3(-rb.velocity.x * drag, 0, 0);
        dragMovementZ = new Vector3(0, 0, -rb.velocity.z * drag);

        #endregion

        #region CheckDirectionToFace()

        if (x != 0 && !combo.isAttacking)
        {
            CheckDirectionToFace(x > 0);
        }

        #endregion

        #region Double Tap To Run

        if (Input.GetButtonDown("Horizontal"))
        {
            float timeSinceLastClick = Time.time - lastTapTime;

            if (timeSinceLastClick <= DoubleClickTime)
            {
                moveSpeed = runSpeed;
                isRunning = true;

            }
            else
            {
                moveSpeed = walkSpeed;
                isRunning = false;

            }

            lastTapTime = Time.time;
        }
        else if (Input.GetButtonUp("Horizontal"))
        {
            isRunning = false;
        }

        #endregion

        #region Jump

        if (Input.GetButtonDown("Jump") && IsGrounded() && !combo.isAttacking)
        {
            startJump = true;
        }

        #endregion
    }

    private void FixedUpdate()
    {
        #region Movement

        ///Adds force to the rb. 
        ///I used a force based movement system as using Vectors reduced the forward momentum when travelling diagonally and gave the player less control.
        ///This method might be a bit less optimal but it makes it feel closer to games like scott Pilgrim and River City Girls.
        ///Both movementDir Vectors are essentially just the x and z variables
        ///dragMovement vectors decrease the slippiness of the movement and gives the player more control

        if (!combo.isAttacking)
        {
            rb.AddForce(movementDirX * moveSpeed + dragMovementX);
            rb.AddForce(movementDirZ * moveSpeed + dragMovementZ);

            if (isMoving)
            {
                animator.SetBool("IsMoving", true);
                animator.SetBool("IsRunning", isRunning);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }

        #endregion

        #region Jump

        if (startJump == true)
        {
            startJump = false;
            rb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("StartJump");
        }
        else if (!IsGrounded())
        {
            animator.SetBool("InAir", true);
        }
        else if (IsGrounded())
        {
            animator.SetBool("InAir", false);
        }


        #endregion

        #region Jump Multipliers

        if (rb.velocity.y < 0 || rb.velocity.y == 0 && !IsGrounded())
        {
            rb.velocity += Vector3.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector3.up * Physics2D.gravity.y * (lowJumpMultiplyer - 1) * Time.deltaTime;
        }

        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemies" && !hasBeenHit)
        {
            Debug.Log("Take Damage");

            hasBeenHit = true;
            health -= 15; //Doesn't check for attackSO cause only 1 attack and I'm lazy.
            healthBar.SetHealth(health, maxHealth);

            if (health <= 0)
            {
                lm.LoseGame(); //I'd like this to be more smooth and like knock the player down then pause the game and show the text but I don't have the assets :p.
            }
            else
            {
                rb.velocity = Vector3.zero; //Freeze player after hit.
                float blowbackForce = isFacingRight ? -20 : 20;
                rb.AddForce(blowbackForce, 0, 0);

                animator.SetTrigger("GetHit");

                combo.EndCombo();
                combo.isAttacking = false;
            }
        }
    }

    public bool IsGrounded() //Checks if the player is on the ground.
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer);
        return colliders.Length > 0;
    }

    private void Flip() //Flips the player (It's easier to transform the local scale as I plan to use a plethora of hitboxes/hurtboxes later on).
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    private void CheckDirectionToFace(bool isMovingRight) //Checks condtions to see if the player should face left or right.
    {
        if (isMovingRight != isFacingRight)
            Flip(); 
    }

        #region Slide

    //public void Slide()
    //{
    //    if (isFacingRight)
    //    {
    //        rb.AddForce(Vector3.right * 7500 * Time.deltaTime);
    //    }
    //    else
    //    {
    //        rb.AddForce(Vector3.left * 7500 * Time.deltaTime);
    //    }
    //}

    #endregion
    //Created by Heather Carter <3
}
