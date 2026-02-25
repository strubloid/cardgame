using UnityEngine;
using System.Collections.Generic;

/**
 * Manager that handles power data lookup and initialization
 * Stores PowerData for each card type and provides easy access
 */
public class PowerManager : MonoBehaviour
{
    // Singleton instance
    public static PowerManager instance;

    // Dictionary to store PowerData by card type
    private Dictionary<CardScriptableObject.CardType, PowerData> powerDataMap = new Dictionary<CardScriptableObject.CardType, PowerData>();

    // Array of all power data (set in inspector)
    [SerializeField] private PowerData[] allPowerData;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the power data map
        InitializePowerDataMap();
    }

    /**
     * Initialize the power data map from the array
     */
    private void InitializePowerDataMap()
    {
        powerDataMap.Clear();

        if (allPowerData == null || allPowerData.Length == 0)
        {
            Debug.LogError("PowerManager: No power data assigned! Please add the PowerManager prefab (Assets/Code/Prefabs/Level Essentials/PowerManager.prefab) to your scene, or assign PowerData assets in the inspector.");
            return;
        }

        foreach (PowerData powerData in allPowerData)
        {
            if (powerData == null)
            {
                Debug.LogWarning("PowerManager: Found null PowerData in array!");
                continue;
            }

            powerDataMap[powerData.cardType] = powerData;
        }
    }

    /**
     * Lazy initialization - create PowerManager if it doesn't exist
     */
    private static void EnsureInitialized()
    {
        if (instance == null)
        {
            GameObject powerManagerGO = new GameObject("PowerManager");
            powerManagerGO.AddComponent<PowerManager>();
        }
    }

    /**
     * Get PowerData for a specific card type
     */
    public static PowerData GetPowerDataForType(CardScriptableObject.CardType cardType)
    {
        EnsureInitialized();

        if (instance.powerDataMap.ContainsKey(cardType))
        {
            return instance.powerDataMap[cardType];
        }

        Debug.LogWarning($"PowerManager: No PowerData found for card type {cardType}");
        return null;
    }
}
