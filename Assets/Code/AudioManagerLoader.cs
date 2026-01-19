using UnityEngine;

public class AudioManagerLoader : MonoBehaviour
{
    // This will hold the AudioManager prefab to be instantiated
    public AudioManager audioManagerPrefab;

    // This will load before anything else
    private void Awake()
    {
        // Check if an AudioManager already exists in the scene
        if (Object.FindFirstObjectByType<AudioManager>() == null)
        {
            // Instantiate the AudioManager prefab
            Instantiate(audioManagerPrefab);
        }
    }
}
