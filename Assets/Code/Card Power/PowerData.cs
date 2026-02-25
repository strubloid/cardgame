using UnityEngine;

/**
 * ScriptableObject that stores power-specific data including particle effects and animation parameters
 * Each card type (Fire, Water, Wind, Earth) will have a corresponding PowerData asset
 */
[CreateAssetMenu(fileName = "New Power Data", menuName = "Objects/Power Data", order = 2)]
public class PowerData : ScriptableObject
{
    // The card type this power data is for
    public CardScriptableObject.CardType cardType;

    // The particle system prefab for this power type
    public GameObject particleSystemPrefab;

    // Animation parameters for this power
    [Header("Animation Parameters")]
    public float travelTime = 0.5f;
    public float lingerTime = 0.2f;

    // Optional: additional visual effect prefab (e.g., impact effect)
    public GameObject impactEffectPrefab;
}
