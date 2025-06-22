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
    [Space(5)]
    [Header("Attributes")]
    public float maxHealth = 100f;

    public float wallJumpHorizontalForce = 5f;
    public float maxSkill = 100f;
    [SerializeField] private List<AttributeModifier> modifiers = new List<AttributeModifier>();

    public int PlayerSpeed => Mathf.RoundToInt(ApplyModifiers("playerSpeed", playerSpeed));
    public float JumpPower => ApplyModifiers("jumpPower", jumpPower);
    public float MaxHealth => ApplyModifiers("maxHealth", maxHealth);
    public float WallJumpHorizontalForce => ApplyModifiers("wallJumpHorizontalForce", wallJumpHorizontalForce);
    public float MaxSkill => ApplyModifiers("maxSkill", maxSkill);
    public void AddModifier(AttributeModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(AttributeModifier modifier)
    {
        modifiers.Remove(modifier);
    }

    public void RemoveModifiersBySource(string source)
    {
        modifiers.RemoveAll(m => m.source == source);
    }

    public void ClearModifiers()
    {
        modifiers.Clear();
    }

    // Bekomme alle Modifikatoren für ein Attribut
    public List<AttributeModifier> GetModifiersForAttribute(string attributeName)
    {
        return modifiers.FindAll(m => m.attributeName == attributeName);
    }

    // Prüfe ob ein Attribut Modifikatoren hat
    public bool HasModifiers(string attributeName)
    {
        return modifiers.Exists(m => m.attributeName == attributeName);
    }

    // Bekomme finale Werte mit Modifikationen
    private float ApplyModifiers(string attributeName, float baseValue)
    {
        float finalValue = baseValue;
        float additiveBonus = 0f;
        float multiplicativeBonus = 1f;

        foreach (var modifier in modifiers)
        {
            if (modifier.attributeName == attributeName)
            {
                switch (modifier.type)
                {
                    case ModifierType.Additive:
                        additiveBonus += modifier.value;
                        break;
                    case ModifierType.Multiplicative:
                        multiplicativeBonus *= (1f + modifier.value);
                        break;
                    case ModifierType.Override:
                        return modifier.value; // Überschreibt komplett
                }
            }
        }

        finalValue = (baseValue + additiveBonus) * multiplicativeBonus;
        return finalValue;
    }

    // Debug-Funktionen
    public void PrintAllModifiers()
    {
        Debug.Log($"=== Aktive Modifikatoren ({modifiers.Count}) ===");
        foreach (var mod in modifiers)
        {
            Debug.Log($"{mod.attributeName}: {mod.type} {mod.value} (von: {mod.source})");
        }
    }

    // Reset zu Grundwerten
    public void ResetToBaseValues()
    {
        ClearModifiers();
        Debug.Log("Alle Modifikatoren entfernt - Grundwerte wiederhergestellt");
    }
}

// Attribut-Modifikator Klasse
[System.Serializable]
public class AttributeModifier
{
    public string attributeName;
    public ModifierType type;
    public float value;
    public string source; // Woher kommt der Modifier
    public float duration; // Dauer in Sekunden (-1 = permanent)
    public float timeApplied; // Wann wurde er angewendet

    public AttributeModifier(string name, ModifierType modType, float modValue, string modSource = "", float modDuration = -1f)
    {
        attributeName = name;
        type = modType;
        value = modValue;
        source = modSource;
        duration = modDuration;
        timeApplied = Time.time;
    }
}

public enum ModifierType
{
    Additive,      // +5 Speed
    Multiplicative, // +50% Speed (0.5f = 50% Bonus)
    Override       // Setzt Wert auf exakten Wert
}

// Attribut-Manager für einfache Verwendung
public class AttributeManager : MonoBehaviour
{
    [Header("Player Data")]
    public PlayerData playerData;

    [Header("Debug")]
    public bool showDebugInfo = true;
    // Einfache Methoden zum Ändern von Attributen
    public void ChangeAttribute(string attributeName, ModifierType type, float value, string source = "Unknown", float duration = -1f)
    {
        var modifier = new AttributeModifier(attributeName, type, value, source, duration);
        playerData.AddModifier(modifier);

        if (showDebugInfo)
        {
            Debug.Log($"Attribut geändert: {attributeName} {type} {value} (Quelle: {source})");
        }
    }
}
