using System.Collections;
using UnityEngine;

public class FreezeProjectile : MonoBehaviour
{
    public float damage = 1f;
    public float lifetime = 5f;
    public float freezeDuration = 2f;
    public ParticleSystem freezeParticle;

    void Start()
    {
        freezeParticle = GameObject.Find("FreezeParticleSystem").GetComponent<ParticleSystem>();
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Schaden zufügen
            IDamagable target = other.GetComponent<IDamagable>();
            if (target != null)
            {
                target.Damage(damage);
            }

            StartCoroutine(FreezePlayer(other.gameObject));

            Destroy(gameObject);
        }
    }

    private IEnumerator FreezePlayer(GameObject player)
    {
        freezeParticle.Play();
        // Steuerung deaktivieren
        MonoBehaviour controller = player.GetComponent<MonoBehaviour>(); // z. B. dein PlayerController
        if (controller != null)
            controller.enabled = false;

        // Rigidbody einfrieren
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        yield return new WaitForSeconds(freezeDuration);

        freezeParticle.Stop();
        // Steuerung wieder aktivieren
        if (controller != null)
            controller.enabled = true;

        // Physik zurücksetzen (Rotation bleibt eingefroren)
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}
