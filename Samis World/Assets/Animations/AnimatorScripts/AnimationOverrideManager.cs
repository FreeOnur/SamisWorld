using UnityEngine;

// Dieses Script als zusätzliche Komponente auf deinen Player setzen
public class AnimationOverrideManager : MonoBehaviour
{
    private Animator animator;
    private Combos combosScript;
    private PlayerMovement playerMovement;

    [Header("Debug")]
    public bool debugMode = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        combosScript = GetComponent<Combos>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void LateUpdate()
    {
        // LateUpdate stellt sicher, dass dies NACH allen anderen Updates läuft
        HandleSwordRunOverride();
    }

    private void HandleSwordRunOverride()
    {
        if (combosScript.useSwordRun && !combosScript.isAttacking)
        {
            // Prüfe ob wir uns bewegen
            bool isMoving = Mathf.Abs(playerMovement.PlayerRb.velocity.x) > 0.1f &&
                           Mathf.Abs(playerMovement.HorizontalInput) > 0.1f &&
                           playerMovement.IsGrounded();

            if (isMoving)
            {
                // Forciere Run_Sword Animation
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run_Sword"))
                {
                    animator.SetBool("useSwordRun", true);
                    animator.Play("Run_Sword", 0, 0f);

                    if (debugMode)
                        Debug.Log("AnimationOverrideManager: Forcing Run_Sword");
                }
            }
        }
    }
}