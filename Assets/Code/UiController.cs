using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    // Singleton instance
    public static UiController instance;

    /**
     * Awake is called when the script instance is being loaded
     */
    private void Awake()
    {
        // Ensure only one instance of BattleController exists
        if (instance == null)
        {
            instance = this;
        }
    }

    // Text element to display player's mana
    public TMP_Text playerManaText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){   
    }

    /**
     * This will be updating the player mana text in the UI
     */
    public void SetPlayerManaText(int manaAmmount) {
        playerManaText.text = "Mana: " + manaAmmount;
    }
}
