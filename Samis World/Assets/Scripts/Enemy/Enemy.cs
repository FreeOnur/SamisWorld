using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

<<<<<<< Updated upstream
    // Update is called once per frame
=======
    //take damage function to use in other scripts
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //Play Hurt Animation

        if (currentHealth <= 0)
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

>>>>>>> Stashed changes
    void Update()
    {

    }
}