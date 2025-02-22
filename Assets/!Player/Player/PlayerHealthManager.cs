using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private int hasHealthChange;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (hasHealthChange != currentHealth)
        {
            //healthBar.SetHealth(currentHealth/maxHealth * 100);
        }
    }



}
