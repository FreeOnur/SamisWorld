using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimatorBrain : MonoBehaviour
{
    private readonly static int[] animations =
    {
        //the names needs to match with the names from the animator or match with the animations
        Animator.StringToHash("Idle"),        // IDLE (0)
        Animator.StringToHash("Run"),         // RUN (1)
        Animator.StringToHash("Hit"),         // HIT (2)
        Animator.StringToHash("Jump"),        // JUMP (3)
        Animator.StringToHash("Landing"),     // JUMPEND (4)
        Animator.StringToHash("Fall"),        // FALL (5)
        Animator.StringToHash("Dash"),        // DASH (6)
        Animator.StringToHash("Attack_1"),    // ATTACK_1 (7)
        Animator.StringToHash("Attack_2"),    // ATTACK_2 (8)
        Animator.StringToHash("Attack_3"),    // ATTACK_3 (9) 
        Animator.StringToHash("Death"),       // DEATH (10)
        Animator.StringToHash("Climb")        // CLIMB (11)
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

        // Die Animation-Index-Zuordnung korrigieren
        int animHash;
        switch (animation)
        {
            case Animations.IDLE: animHash = animations[0]; break;
            case Animations.RUN: animHash = animations[1]; break;
            case Animations.HIT: animHash = animations[2]; break;
            case Animations.JUMP: animHash = animations[3]; break;
            case Animations.JUMPEND: animHash = animations[4]; break;
            case Animations.FALL: animHash = animations[5]; break;
            case Animations.DASH: animHash = animations[6]; break;
            case Animations.ATTACK_1: animHash = animations[7]; break;
            case Animations.ATTACK_2: animHash = animations[8]; break;
            case Animations.ATTACK_3: animHash = animations[9]; break;
            case Animations.DEATH: animHash = animations[10]; break;
            case Animations.CLIMB: animHash = animations[11]; break;
            default: animHash = animations[0]; break;
        }

        animator.CrossFade(animHash, crossfade, layer);
    }
}

// Korrekte Enum-Definition
public enum Animations
{
    IDLE,
    RUN,
    HIT,
    JUMP,
    JUMPEND,
    FALL,
    DASH,
    ATTACK_1,
    ATTACK_2,
    ATTACK_3,
    DEATH,
    CLIMB,
    NONE
}