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
        Animator.StringToHash("Fall")
    };
    [SerializeField] private Animator animator;
    [SerializeField] private Animations[] currentAnimation;
    [SerializeField] private bool[] layerLocked;
    [SerializeField] private Action<int> DefaultAnimation;
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
        if(animation == Animations.NONE)
        {
            DefaultAnimation(layer);
            return;
        }
        if (layerLocked[layer] && !bypassLock) return;
        layerLocked[layer] = lockLayer;
       
        if (currentAnimation[layer] == animation) return;

        currentAnimation[layer] = animation;
        animator.CrossFade(animations[(int)currentAnimation[layer]], crossfade, layer);

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
    NONE
}