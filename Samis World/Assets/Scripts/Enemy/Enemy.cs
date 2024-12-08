using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }

    //take damage function to use in other scripts
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Play Hurt Animation

        if(currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Enemy died!");
        //Die animation

        //Disable the enemy
    }
    
    void Update()
    {
        
    }
}
