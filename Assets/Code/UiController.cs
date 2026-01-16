using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    // Singleton instance
    public static UiController instance;

    // Text element to display player's mana and mana warning UI
    public TMP_Text playerManaText, enemyManaText;
    public GameObject manaWarning;
    public float manaWarningTime = 2.0f;
    private float manaWarningCounter;

    // Text elements to display player and enemy health
    public TMP_Text PlayerHealthText;
    public TMP_Text EnemyHealthText;

    // reference of the draw card button
    public GameObject drawCardButton;
    public GameObject endTurnButton;

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

    // Damage indicator references
    public UiDamageIndicator playerDamage, enemyDamage;

    // This will be the reference of the battle end screen
    public GameObject battleEndScreen;

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
        playerManaText.text = "" + manaAmmount;
    }

    /**
     * This will be updating the enemy mana text in the UI
     */
    public void SetEnemyManaText(int manaAmmount)
    {
        enemyManaText.text = "" + manaAmmount;
    }

    /**
     * This will be updating the player health text in the UI
     */
    public void SetPlayerHealthText(int life)
    {
        PlayerHealthText.text = "" + life;
    }

    /**
     * This will be updating the enemy health text in the UI
     */
    public void SetEnemyHealthText(int life)
    {
        EnemyHealthText.text = "" + life;
    }


    /**
     * This will be setting the UI for the player turn
     */
    public void SetPlayerTurn() {

        // enabling the end turn button
        endTurnButton.SetActive(true);

        // enabling the draw card button
        drawCardButton.SetActive(true);

        // Setting up the player turn Text as visble and the other ones disabled
        PlayerTurnText.fontSizeMax = 30;
        PlayerTurnText.color = PlayerTurnColor;
        PlayerTurnText.fontStyle = FontStyles.Italic;

        PlayerCardAttackText.fontSizeMax = 26;
        PlayerCardAttackText.color= DisabledTurn;
        PlayerCardAttackText.fontStyle = FontStyles.Normal;

        EnemyTurnText.fontSizeMax = 26;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;

        EnemyCardAttackText.fontSizeMax = 26;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;

    }

    /**
     * This will be setting the UI for the player card attack phase
     */
    public void SetPlayerCardAttack() {

        // disabling the end turn button
        endTurnButton.SetActive(false);

        // disabling the draw card button
        drawCardButton.SetActive(false);

        // Setting up the player attack Text as visble and the other ones disabled
        PlayerTurnText.fontSizeMax = 26;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 30;
        PlayerCardAttackText.color = PlayerCardAttackColor;
        PlayerCardAttackText.fontStyle = FontStyles.Italic;

        EnemyTurnText.fontSizeMax = 26;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;

        EnemyCardAttackText.fontSizeMax = 26;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;
    }

    /**
     * This will be setting the UI for the enemy turn
     */
    public void SetEnemyTurn() {

        // disabling the end turn button
        endTurnButton.SetActive(false);

        // disabling the draw card button
        drawCardButton.SetActive(false);

        // Setting up the enemy turn Text as visble and the other ones disabled
        PlayerTurnText.fontSizeMax = 26;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 26;
        PlayerCardAttackText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        EnemyTurnText.fontSizeMax = 30;
        EnemyTurnText.color = EnemyTurnColor;
        EnemyTurnText.fontStyle = FontStyles.Italic;

        EnemyCardAttackText.fontSizeMax = 26;
        EnemyCardAttackText.color = DisabledTurn;
        EnemyCardAttackText.fontStyle = FontStyles.Normal;
    }

    /**
     * This will be setting the UI for the enemy card attack phase
     */
    public void SetEnemyCardAttack() {

        // disabling the end turn button
        endTurnButton.SetActive(false);

        // disabling the draw card button
        drawCardButton.SetActive(false);

        // Setting up the enemy attack Text as visble and the other ones disabled
        PlayerTurnText.fontSizeMax = 26;
        PlayerTurnText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        PlayerCardAttackText.fontSizeMax = 26;
        PlayerCardAttackText.color = DisabledTurn;
        PlayerTurnText.fontStyle = FontStyles.Normal;

        EnemyTurnText.fontSizeMax = 26;
        EnemyTurnText.color = DisabledTurn;
        EnemyTurnText.fontStyle = FontStyles.Normal;

        EnemyCardAttackText.fontSizeMax = 30;
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
        drawCardButtonText.text = $"Draw Card {drawCost} Mana :)";
    }

    /**
     * This will be ending the player turn
     */
    public void EndPlayerTurn() { 
        BattleController.instance.EndPlayerTurn();

    }

    /**
     * This will be handling the battle end screen buttons
     */
    public void MainMenu() { 
    
        Debug.Log("Returning to Main Menu...");
    }

    /**
     * This will be restarting the level
     */
    public void RestartLevel()
    {
        Debug.Log("Restarting Level...");

    }


    /**
     * This will be choosing a new battle
     */
    public void ChooseNewBattle()
    {
        Debug.Log("Choosing New Battle...");

    }

}
