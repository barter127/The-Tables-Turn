using UnityEngine;


[CreateAssetMenu(fileName = "AttackDetails", menuName = "AttackDetailsSO")]
public class AttackScriptableObject : ScriptableObject
{
    public string attackName;
    public string attackType;

    public AnimatorOverrideController animatorOV;

    [Header("Damage")]
    public int damage;

    [Header("Blowback")]
    public Vector3 enemyBlowbackHitDirection;
    public float enemyBlowbackForce;
    public bool canKnockdown;
    public float fallTimer;
}
