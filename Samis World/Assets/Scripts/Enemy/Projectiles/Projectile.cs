using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;
    public bool isEnemyProjectile = false;
    public bool isKnockbackUnlocked = false;
    public float knockbackForce = 15f;


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
        //else if (isEnemyProjectile && other.CompareTag("Player") && isKnockbackUnlocked)
        //{
        //    IDamagable target = other.GetComponent<IDamagable>();
        //    Rigidbody2D targetRB = GameObject.Find("Player").GetComponent<Rigidbody2D>();

        //    if (target != null)
        //    {
        //        targetRB.velocity = Vector2.zero;
        //        Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
        //        Debug.Log("Knockback starten");

        //        target.Damage(damage);
        //        targetRB.velocity = knockbackDirection * knockbackForce;
        //        Debug.Log("Knockback ausgeführt und beendet");

        //    }
        //    Destroy(gameObject);
        //}
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
