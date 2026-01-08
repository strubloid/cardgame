using TMPro;
using UnityEngine;

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

    // 4 Objects for the different turn texts
    public TMP_Text PlayerTurnText;
    public TMP_Text PlayerCardAttackText;
    public TMP_Text EnemyTurnText;
    public TMP_Text EnemyCardAttackText;

    // Colors for the player
    private Color PlayerTurnColor = new Color32(0x10, 0x4D, 0x00, 0xB1); // #104D00, A=177
    private Color PlayerCardAttackColor = new Color32(0xD9, 0x4D, 0x29, 0xB1); // #D90429 (red - attack)

    // Colors for the enemy
    private Color EnemyTurnColor = new Color32(0x8B, 0x00, 0x00, 0xB1); // #8B0000 (red - enemy turn)
    private Color EnemyCardAttackColor = new Color32(0xD9, 0x04, 0x29, 0xB1); // #D90429 (red - attack)

    // Disabled turn color
    private Color DisabledTurn = new Color32(0x55, 0x55, 0x55, 0xB1); // #555555

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
     * This will be setting the UI for the player turn
     */
    public void SetPlayerTurn() {

        PlayerTurnText.fontSizeMax = 50;
        PlayerTurnText.color = PlayerTurnColor;
        PlayerTurnText.fontStyle = FontStyles.Italic;

        PlayerCardAttackText.fontSizeMax = 40;
        PlayerCardAttackText.color= DisabledTurn;
        PlayerCardAttackText.fontStyle = FontStyles.Normal;

        EnemyTurnText.fontSizeMax = 40;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;

        EnemyCardAttackText.fontSizeMax = 40;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;

    }

    /**
     * This will be setting the UI for the player card attack phase
     */
    public void SetPlayerCardAttack() {

        PlayerTurnText.fontSizeMax = 40;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 50;
        PlayerCardAttackText.color = PlayerCardAttackColor;
        PlayerCardAttackText.fontStyle = FontStyles.Italic;

        EnemyTurnText.fontSizeMax = 40;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;

        EnemyCardAttackText.fontSizeMax = 40;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;
    }

    /**
     * This will be setting the UI for the enemy turn
     */
    public void SetEnemyTurn() {
        PlayerTurnText.fontSizeMax = 40;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 40;
        PlayerCardAttackText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        EnemyTurnText.fontSizeMax = 50;
        EnemyTurnText.color = EnemyTurnColor;
        EnemyTurnText.fontStyle = FontStyles.Italic;

        EnemyCardAttackText.fontSizeMax = 40;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;
    }

    /**
     * This will be setting the UI for the enemy card attack phase
     */
    public void SetEnemyCardAttack() {
        PlayerTurnText.fontSizeMax = 40;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 40;
        PlayerCardAttackText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;


        EnemyTurnText.fontSizeMax = 40;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;


        EnemyCardAttackText.fontSizeMax = 50;
        EnemyCardAttackText.color = EnemyCardAttackColor;
        EnemyCardAttackText.fontStyle = FontStyles.Italic;
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
