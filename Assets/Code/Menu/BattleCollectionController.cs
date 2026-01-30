using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This will be responsible for controlling the battle selection screen.
 */
public class BattleCollectionControler: MonoBehaviour
{
    // Folder where battle scenes are located
    private const string BattleFolder = "/Scenes/Battles/";

    // we will be sotring all the battle scenes available
    public List<string> AvaiableBattleScenes = new List<string>();

    // This will start with some initial setup
    private void Awake()
    {
        // checking if we dont have items already
        if (AvaiableBattleScenes.Count <= 0) {
            // loading the available battle scenes
            AvaiableBattleScenes = GetBattleScenes();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * This will provide a list of battles scenes that are localized at:
     * /Assets/Scenes/Battles/
     * Each file inside of this folde will be considered a battle scene.
     */
    public List<string> GetBattleScenes()
    {
        // this will hold the list of scenes found
        List<string> scenes = new List<string>();

        // Getting a total count of scenes in build settings, some of them arent battle scenes
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        // Looping through all scenes in build settings
        for (int i = 0; i < sceneCount; i++)
        {
            // Getting the path of the scene at index i
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            // Ensure scene is inside Assets/Scenes/Battles
            if (!scenePath.Contains(BattleFolder))
                continue;

            // Extracting the scene name from the path
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            // adding the scene name to the list
            scenes.Add(sceneName);
        }

        // we will be getting all the battle scenes found
        return scenes;
    }
    

    /**
     * This will be called when the player selects a battle to play.
     */
    public void NextPage() {


        // Play a sound effect to indicate a battle has been selected
        //AudioManager.instance.PlayButtonPress();
    }
}
