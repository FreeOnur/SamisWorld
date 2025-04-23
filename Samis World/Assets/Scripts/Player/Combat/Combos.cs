using UnityEngine;
using UnityEngine.Timeline;

public class Combos : MonoBehaviour
{
    public Animator animator;
    public int combo;
    public bool isAttacking;
    public Player playerScript;
    private RaycastHit2D[] hits;

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
            
            Debug.Log("Setting trigger: " + triggerName);
        }
    }

    private void Attack()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackLayer);
        for(int i = 0; i < hits.Length; i++)
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
        if (combo < 2) // Ändern auf 2 für max 3 Attacken
        {
            combo++;
            isAttacking = false;
            Debug.Log("Combo increased to: " + combo);
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
        
        // Alle Trigger zurücksetzen beim Beenden
        animator.ResetTrigger("1");
        animator.ResetTrigger("2");
        animator.ResetTrigger("3");
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}