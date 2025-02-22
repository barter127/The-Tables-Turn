//using UnityEngine;

//public class PlayerControllerAnimator : MonoBehaviour
//{
//    private Animator animator;
//    private PlayerController pc;

//    private void Start()
//    {
//        animator = GetComponent<Animator>();
//        pc = GetComponentInParent<PlayerController>();
//    }

//    private void Update()
//    {
//        #region Idle, Walk, and Run

//       if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
//       {
//            animator.SetBool("IsRunning", pc.isRunning);
//            animator.SetBool("IsMoving", true);
//       }

//       else if (Input.GetButtonUp("Horizontal") || Input.GetButtonUp("Vertical"))
//            animator.SetBool("IsMoving", false);

//        #endregion

//        #region Jump (Not Perfect)
//        if (Input.GetButtonDown("Jump"))
//            animator.SetTrigger("StartJump");
//        else if (!pc.IsGrounded())
//            animator.SetBool("IsJumping", true);
//        else if (pc.IsGrounded())
//            animator.SetBool("IsJumping", false);
//        #endregion
//    }
//}
