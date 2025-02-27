using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    public void SetHealth(float health, float maxHealth) //Remember can't divide ints into a decimal
    {
        Debug.Log(health);

        healthFill.fillAmount = health / maxHealth; //Calculates health percentage.
    }
}
