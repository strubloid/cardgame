using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
