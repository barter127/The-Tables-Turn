using UnityEngine;

public class NEWComboSystem : MonoBehaviour //Could use some optimisation but deadlines :(
{
    //Arrays of combos the order of attacks in the array is the order of the combo (except for grab and aerial).
    public AttackScriptableObject[] lightAttacks;
    public AttackScriptableObject[] heavyAttacks;
    public AttackScriptableObject[] aerialAttacks;
    //[SerializeField] private AttackScriptableObject[] runningAttacks;
    //[SerializeField] private AttackScriptableObject[] grabAttacks;
    public AttackScriptableObject currentAttack;

    //Component References
    public PlayerController pc;
    private Rigidbody rb;
    private Animator animator;
    [SerializeField] private BoxCollider attackHB;

    //Bool to be referenced to stop certain actions while attacking.
    public bool isAttacking;

    private bool buttonPressed;

    //Combo Vars
    public int comboCounter;
    public int comboAnimCounter;
    private float comboTimer;
    [SerializeField] private float comboTimerLength;
    private bool startLightAttack;

    private float heavyTimer;
    private bool startHeavyAttack;
    [SerializeField] private float heavyTimerLength;

    private bool addForceOnce = false;

    void Start()
    {
        pc = GetComponentInParent<PlayerController>();
        rb = GetComponentInParent<Rigidbody>();
        animator = GetComponent<Animator>();

        isAttacking = false;
    }

    void Update()
    {
        #region Attacks

        if (Input.GetKeyDown(KeyCode.H) && !pc.hasBeenHit && pc.IsGrounded()) //Light Attack input check.
        {
            if (!buttonPressed) //Limits comboCounter increase to once per press.
            {
                comboCounter++;
                buttonPressed = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.H))
            buttonPressed = false;

        if (comboCounter > 0 && !isAttacking && comboAnimCounter < lightAttacks.Length)
        {
            isAttacking = true;

            comboTimer += comboTimerLength;

            currentAttack = lightAttacks[comboAnimCounter];

            //Plays animation attack
            animator.runtimeAnimatorController = lightAttacks[comboAnimCounter].animatorOV;
            animator.Play("Attack State", 0, 0);

            comboCounter--;
            comboAnimCounter++;
        }

        if (comboTimer >= 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else if (comboTimer < 0)
        {
            isAttacking = false;
            EndCombo();
        }

        if (Input.GetKeyDown(KeyCode.J) && !pc.hasBeenHit && pc.IsGrounded()) //Heavy Attack input check.
        {
            startHeavyAttack = true;
        }

        if (heavyTimer >= 0)
            heavyTimer -= Time.deltaTime;

        if (heavyTimer <= 0 && startHeavyAttack)
        {
            currentAttack = heavyAttacks[comboAnimCounter];

            comboTimer += 0.6f;

            animator.runtimeAnimatorController = heavyAttacks[comboAnimCounter].animatorOV;
            animator.Play("Attack State", 0, 0);

            isAttacking = true;
            heavyTimer = heavyTimerLength;

            EndCombo();
        }

        #endregion

        if (isAttacking)
        {
            attackHB.enabled = true;

            animator.SetBool("IsMoving", false);
            rb.velocity = Vector3.zero;
            if (!addForceOnce && currentAttack.name != "Jumpkick")
            {
                addForceOnce = true;
                float forceToAdd = pc.isFacingRight ? 350 : -350;
                rb.AddForce(forceToAdd, transform.position.y, transform.position.z);
            }
            else if (!addForceOnce && currentAttack.name == "Jumpkick")
            {
                addForceOnce = true;
                float forceToAdd = pc.isFacingRight ? 100 : -100;
                rb.AddForce(forceToAdd, transform.position.y, transform.position.z);
            }
        }
        else
        {
            attackHB.enabled = false;
        }
    }

    public void EndCombo()
    {
        comboCounter = 0;
        comboAnimCounter = 0;
    }

    public void OnAttackAnimationEnd() //Called at the end of an attack animation.
    {
        isAttacking = false;
        addForceOnce = false;
        startHeavyAttack = false;
    }

    public void PlayerHurtAnimationHasEnded()
    {
        pc.hasBeenHit = false;
        isAttacking = false; //Fixes issue where stays in combo if hit while attacking.

        EndCombo();
    }

    #region Running Attacks Code
    //else if (pc.isRunning)
    //{
    //    animator.runtimeAnimatorController = runningAttacks[0].animatorOV;
    //    animator.Play("Attack State", 0, 0);
    //}

    //            else if (pc.isRunning && !isAttacking)
    //        {
    //            animator.runtimeAnimatorController = runningAttacks[1].animatorOV;
    //            animator.Play("Attack State", 0, 0);

    //animator.SetBool("StartRoll", true);
    //isAttacking = true;
    //            pc.isRolling = true;
    //            heavyTimer = heavyTimerLength;
    //        }
    #endregion
}
