using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBrain : MonoBehaviour
{
    private readonly static int[] animations =
    {
        //the names needs to match with the names from the animator or match with the animations
        Animator.StringToHash("Idle"),
        Animator.StringToHash("Run"),
        Animator.StringToHash("Hit"),
        Animator.StringToHash("Jump"),
        Animator.StringToHash("Landing"),
        Animator.StringToHash("Fall"),
        Animator.StringToHash("Dash"),
        Animator.StringToHash("Attack_1"),
        Animator.StringToHash("Attack_2"),
        Animator.StringToHash("Attack_3"),
        Animator.StringToHash("Death")
    };
    private Animator animator;
    private Animations[] currentAnimation;
    private bool[] layerLocked;
    private Action<int> DefaultAnimation;

    protected void Initialize(int layers, Animations startingAnimation, Animator animator, Action<int> DefaultAnimation)
    {
        layerLocked = new bool[layers];
        currentAnimation = new Animations[layers];
        this.animator = animator;
        this.DefaultAnimation = DefaultAnimation;

        for (int i = 0; i < layers; i++)
        {
            layerLocked[i] = false;
            currentAnimation[i] = startingAnimation;
        }
        
    }
    
    public Animations GetCurrenAnimation(int layer)
    {
        return currentAnimation[layer];
    }

    public void SetLocked(bool lockLayer, int layer)
    {
        layerLocked[layer] = lockLayer;
    }

    public void Play(Animations animation, int layer, bool lockLayer, bool bypassLock, float crossfade = 0.2f)
{
    // Prüfen, ob die notwendigen Felder initialisiert wurden
    if (layerLocked == null || currentAnimation == null || animator == null)
    {
        Debug.LogError("AnimatorBrain not initialized properly. Call Initialize before using Play.");
        return;
    }

    // Prüfen, ob der Layer-Index gültig ist
    if (layer < 0 || layer >= layerLocked.Length)
    {
        Debug.LogError($"Layer index {layer} out of range. Max index is {layerLocked.Length - 1}");
        return;
    }

    if (animation == Animations.NONE)
    {
        if (DefaultAnimation == null)
        {
            Debug.LogWarning("DefaultAnimation not set but animation is NONE");
            return;
        }
        DefaultAnimation(layer);
        return;
    }

    if (layerLocked[layer] && !bypassLock) return;
    layerLocked[layer] = lockLayer;
    
    if (currentAnimation[layer] == animation) return;
    currentAnimation[layer] = animation;
    
    // Überprüfen, ob die Animation im enum-Index-Bereich liegt
    int animIndex = (int)currentAnimation[layer];
    if (animIndex < 0 || animIndex >= animations.Length)
    {
        Debug.LogError($"Animation index {animIndex} is out of range. Max index is {animations.Length - 1}");
        return;
    }
    
    animator.CrossFade(animations[animIndex], crossfade, layer);
}


}



public enum Animations
{
    IDLE,
    RUN,
    HIT,
    JUMP,
    JUMPEND,
    FALL,
    DASH,
    DEATH,
    NONE
}