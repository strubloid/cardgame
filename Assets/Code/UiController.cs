using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    // Singleton instance
    public static UiController instance;

    // Text element to display player's mana and mana warning UI
    public TMP_Text playerManaText;
    public GameObject manaWarning;
    public float manaWarningTime = 2.0f;
    private float manaWarningCounter;

    // reference of the draw card button
    public GameObject drawCardButton;

    // Text oject of the Draw Card Button
    public TMP_Text drawCardButtonText;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){

        // if we get a counter means we will process the deltaTime
        if (manaWarningCounter > 0) { 
            manaWarningCounter -= Time.deltaTime;

            // if this goes below zero, we disable the mana warning UI
            if (manaWarningCounter <= 0) { 
                manaWarning.SetActive(false);
            }
        }
    }

    /**
     * This will be updating the player mana text in the UI
     */
    public void SetPlayerManaText(int manaAmmount) {
        playerManaText.text = "Mana: " + manaAmmount;
    }

    /**
     * This will be showing the mana warning UI element
     */
    public void ShowManaWarning() 
    {
        // Activate the mana warning UI element
        manaWarning.SetActive(true);
        manaWarningCounter = manaWarningTime;
    }

    /**
     * This will be the function to draw a card from the deck
     */
    public void DrawCard() { 
        DeckController.instance.DrawCardForMana();
    }

    /**
     * This will be setting the draw card button text
     */
    public void SetDrawCardButtonText(int drawCost)
    {
        if (drawCardButtonText == null) return;
        drawCardButtonText.text = $"Draw Card\n-{drawCost} Mana :)";
    }
}
