using UnityEngine;

public class Combos : MonoBehaviour
{
    public Animator animator;
    public int combo;
    public bool isAttacking;

    public void Comboss()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
        {
            isAttacking = true;
            
            // Alle vorherigen Trigger zur�cksetzen
            animator.ResetTrigger("1");
            animator.ResetTrigger("2");
            animator.ResetTrigger("3");
            
            // Neuen Trigger setzen
            string triggerName = "" + (combo + 1);
            animator.SetTrigger(triggerName);
            
            Debug.Log("Setting trigger: " + triggerName);
        }
    }

    public void StartCombo()
    {
        if (combo < 2) // �ndern auf 2 f�r max 3 Attacken
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
        
        // Alle Trigger zur�cksetzen beim Beenden
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
        Comboss();
    }
}