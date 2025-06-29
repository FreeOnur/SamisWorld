using System.Collections;
using UnityEngine;

public class FreezeProjectile : MonoBehaviour, IDamagable
{
    public float damage = 1f;
    public float lifetime = 5f;
    public float freezeDuration = 2f;
    public ParticleSystem freezeParticle;
    public float currentHealth = 1f;

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
        Destroy(gameObject);
    }

    void Start()
    {
        freezeParticle = GameObject.Find("FreezeParticleSystem")?.GetComponent<ParticleSystem>();
        if (freezeParticle == null)
        {
            Debug.LogWarning("FreezeParticleSystem nicht gefunden!");
        }
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

            Player player = other.GetComponent<Player>();
            if (player != null && player.isDead)
            {
                Destroy(gameObject);
                return;
            }

            StartCoroutine(FreezePlayer(other.gameObject));
        }
    }

    private IEnumerator FreezePlayer(GameObject player)
    {
        if (freezeParticle != null)
        {
            freezeParticle.Play();
        }

        PlayerMovement controller = player.GetComponent<PlayerMovement>();
        if (controller == null)
        {
            yield break;
        }
        controller.enabled = false;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        RigidbodyConstraints2D originalConstraints = RigidbodyConstraints2D.None;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }        
        yield return new WaitForSeconds(freezeDuration);
        if (freezeParticle != null)
        {
            freezeParticle.Stop();
        }

        if (controller != null)
        {
            controller.enabled = true;
        }

        Destroy(gameObject);
    }
}