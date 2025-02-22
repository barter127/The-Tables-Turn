using UnityEngine;
using TMPro;

public class DamageNumbesController : MonoBehaviour
{
    [SerializeField] private TMP_Text dmgText;
    [SerializeField] private NEWpunkAI ai;
    [SerializeField] private Animator animator;

    private bool triggerOnce;
    public bool triggerHurtOnce;

    public bool displayDamageNumber;

    void Update()
    {
        if (ai.hasBeenHit && !ai.onGround)
        {
            if (!triggerHurtOnce)
            {
                triggerHurtOnce = true;
                dmgText.enabled = true;

                dmgText.text = ai.playerCurrentAttack.damage.ToString();
                animator.SetTrigger("DamageNumbers"); //Plays animation to enlarge numbers.
            }
        }
        else if (ai.spotPlayer == true)
        {
            if (!triggerOnce)
            {
                triggerOnce = true;

                dmgText.text = ("!");
                dmgText.enabled = true;
                animator.SetTrigger("Spot"); //Plays animation to enlarge exclamation mark.
            }
        }
        else
        {
            dmgText.enabled = false;
            triggerHurtOnce = false;
        }
    }

    public void SpotPlayerFalse()
    {
        ai.spotPlayer = false;
    }

    public void OnGroundAnimation()
    {
        ai.hasBeenHit = false;
    }
}
