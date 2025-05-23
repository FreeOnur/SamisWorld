using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
