using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    [SerializeField] private float gravityScale = 6f;
    [SerializeField] private float gravityMultiplier = 0.2f;
    [SerializeField] private float fastFallGravityMult = 3f;
    public float jumpHangGravityMultiplier = 0.3f;
    public float GravityMultiplier => gravityMultiplier;
    public float GravityScale => gravityScale;
    public float MaxFallSpeed => maxFallSpeed;
    public float FastFallGravityMult => fastFallGravityMult;
    [Space(5)]
    [Header("Speed")]
    [SerializeField] private float maxFallSpeed = 50f;
    [SerializeField] private int playerSpeed = 5;
    [SerializeField] private float jumpPower = 12f;
    public int PlayerSpeed => playerSpeed;
    public float JumpPower => jumpPower;
    [Space(5)]
    [Header("Attributes")]
    public float health = 100f;
    public float currentHealth = 0;
}
