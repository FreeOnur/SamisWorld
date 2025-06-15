using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;
    public bool isEnemyProjectile = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemyProjectile && other.CompareTag("Enemy"))
        {
            IDamagable target = other.GetComponent<IDamagable>();
            if (target != null)
            {
                target.Damage(damage);
            }

            Destroy(gameObject);
        }
        // Wenn das Projektil vom Gegner ist, soll es nur den Spieler treffen
        else if (isEnemyProjectile && other.CompareTag("Player"))
        {
            IDamagable target = other.GetComponent<IDamagable>();
            if (target != null)
            {
                target.Damage(damage);
            }

            Destroy(gameObject);
        }
    }
}
