using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerMovement
{
    [SerializeField] private float currentHealth;
    public HealthBar healthBar;
    public Animator animator;
    public bool isDead = false;
    void Start()
    {
        Initialize(GetComponent<Animator>().layerCount, Animations.IDLE, GetComponent<Animator>(), DefaultAnimation);
        currentHealth = playerData.maxHealth;
        healthBar.SetMaxHealth(playerData.maxHealth);
    }
    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple calls
        isDead = true;
    }

}
