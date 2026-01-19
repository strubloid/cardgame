using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Name of the battle scene to load
    public string BattleSceneName = "MainMenu";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Play the menu music when the main menu is loaded
        AudioManager.instance.PlayMenuMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * This will start the game when the player clicks the "Start Game" button.
     */
    public void StartGame() 
    {
        // Loading the battle scene
        SceneManager.LoadScene(BattleSceneName);
    }

    /**
     * This will quit the game when the player clicks the "Quit Game" button.
     */
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // Log a message to the console (useful for testing in the editor)
        Debug.Log("Quit Game!");
    }
}
