using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PlayerMovement
{
    [SerializeField] private float currentHealth;
    public float currentSkill;
    public HealthBar healthBar;
    public SkillBar skillBar;
    public Animator animator;
    public bool isDead = false;
    void Start()
    {
        Initialize(GetComponent<Animator>().layerCount, Animations.IDLE, GetComponent<Animator>(), DefaultAnimation);
        currentHealth = playerData.maxHealth;
        healthBar.SetMaxHealth(playerData.maxHealth);

        currentSkill = playerData.maxSkill;
        skillBar.SetMaxSkill(playerData.maxSkill);

    }
    void Update()
    {
        if (isDead) return;
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill(10);
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

    public void UseSkill(float amount)
    {
        currentSkill -= amount;
        if (currentSkill < 0) currentSkill = 0;
        skillBar.SetSkill(currentSkill);
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple calls
        isDead = true;
    }

}
