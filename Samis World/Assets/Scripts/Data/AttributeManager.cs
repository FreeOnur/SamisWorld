using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
