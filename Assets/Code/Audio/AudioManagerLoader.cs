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
            // loading the instance of AudioManager
            AudioManager.instance = Instantiate(audioManagerPrefab);

            // making sure that the AudioManager persists across scenes
            DontDestroyOnLoad(AudioManager.instance.gameObject);
        }
    }
}
