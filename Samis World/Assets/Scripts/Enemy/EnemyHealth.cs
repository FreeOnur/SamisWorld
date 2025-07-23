using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 3f;
    public float MaxHealth => maxHealth;
    private SkillPointManager skillPointManager;

    private float currentHealth;
    public float CurrentHealth => currentHealth;


    public virtual void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        skillPointManager.OnEnemyKilled();
        Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        skillPointManager = GameObject.Find("GameManager").GetComponent<SkillPointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentHealth);
    }
}
