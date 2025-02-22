using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetHealth(float health, float maxHealth) //Remember can't divide ints into a decimal
    {
        slider.value = (health / maxHealth) * 100; //Calculates health percentage.
    }
}
