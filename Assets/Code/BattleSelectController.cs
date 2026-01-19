using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This will be responsible for controlling the battle selection screen.
 */
public class BattleSelectController : MonoBehaviour
{
    // This will the name to be loaded as scene, like "Battle1", "Battle2", etc.
    public string levelToLoad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Play the menu music when the main menu is loaded
        AudioManager.instance.PlayBattleSelectMusic();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * This will be called when the player selects a battle to play.
     */
    public void SelectBattle() {

        // this will load the scene for the selected battle
        SceneManager.LoadScene(levelToLoad);

        // Play a sound effect to indicate a battle has been selected
        AudioManager.instance.PlaySoundEffect(0);
    }
}
