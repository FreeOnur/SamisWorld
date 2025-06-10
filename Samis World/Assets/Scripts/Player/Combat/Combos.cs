using UnityEngine;

public class Combos : MonoBehaviour
{
    public Animator animator;
    public int combo;
    public bool isAttacking;
    public Player playerScript;
    private RaycastHit2D[] hits;

    private float swordRunTimer = 0f;
    public bool useSwordRun = false;
    [SerializeField] private float swordRunDuration = 20f;

    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float damageAmount = 1f;

    public void Comboss()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            isAttacking = true;
            Attack();

            // Alle vorherigen Trigger zurücksetzen
            animator.ResetTrigger("1");
            animator.ResetTrigger("2");
            animator.ResetTrigger("3");

            // Neuen Trigger setzen
            string triggerName = "" + (combo + 1);
            animator.SetTrigger(triggerName);

            Debug.Log("Setting trigger: " + triggerName + ", isAttacking: " + isAttacking);
        }
    }

    private void Attack()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            IDamagable iDamagable = hits[i].collider.gameObject.GetComponent<IDamagable>();
            if (iDamagable != null)
            {
                iDamagable.Damage(damageAmount);
            }
        }
    }

    public void StartCombo()
    {
        if (combo < 2)
        {
            combo++;
            isAttacking = false; // Wichtig: isAttacking zurücksetzen für nächste Attacke
            Debug.Log("Combo increased to: " + combo + ", isAttacking reset to false");
        }
        else
        {
            FinishAnimation();
        }
    }

    public void FinishAnimation()
    {
        Debug.Log("FinishAnimation called");
        isAttacking = false;
        combo = 0;

        // Alle Trigger zurücksetzen
        animator.ResetTrigger("1");
        animator.ResetTrigger("2");
        animator.ResetTrigger("3");

        // Sword Run aktivieren
        useSwordRun = true;
        swordRunTimer = swordRunDuration;
        animator.SetBool("useSwordRun", true);

        Debug.Log("Sword Run aktiviert für " + swordRunDuration + " Sekunden");
    }

    void Start()
    {
        combo = 0;
        isAttacking = false;
    }

    void Update()
    {
        if (playerScript.isDead)
        {
            this.enabled = false;
        }

        Comboss();

        // Sword Run Timer
        if (useSwordRun)
        {
            swordRunTimer -= Time.deltaTime;
            if (swordRunTimer <= 0f)
            {
                useSwordRun = false;
                swordRunTimer = 0f;
                animator.SetBool("useSwordRun", false);
                Debug.Log("Sword Run deaktiviert");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}