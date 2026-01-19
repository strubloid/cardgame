using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // Win and Lose game objects
    public GameObject win;
    public GameObject lose;

    // This will be the battle end title text
    public TMP_Text battleEndTitleText;

    // This will be the main menu scene name
    public string mainMenuSceneName = "MainMenu";

    // This will be the battle selection scene name
    public string battleSelecScene = "SelectBattle";

    // This will be the pause screen reference
    public GameObject pauseScreen;

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

        // This will handdle the pause/unpause input
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseUnpause();
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

        // calling the draw card for mana function
        DeckController.instance.DrawCardForMana();

        // playing the sound effect for drawing a card
        AudioManager.instance.PlaySoundEffect(0);
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

        // playing the sound effect for ending the turn
        BattleController.instance.EndPlayerTurn();

        // playing the sound effect for ending the turn
        AudioManager.instance.PlaySoundEffect(0);
    }

    /**
     * This will be handling the battle end screen buttons
     */
    public void MainMenu() { 

        // Loading the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);

        // Making sure that the time scale is set to 1
        Time.timeScale = 1f;

        // playing the sound effect for going to main menu
        AudioManager.instance.PlaySoundEffect(0);
    }

    /**
     * This will be restarting the level
     */
    public void RestartLevel()
    {

        // loading the current active scene
        Scene actualScene = SceneManager.GetActiveScene();

        // Afer we load it
        SceneManager.LoadScene(actualScene.name);

        // Making sure that the time scale is set to 1
        Time.timeScale = 1f;

        // playing the sound effect for restarting the level
        AudioManager.instance.PlaySoundEffect(0);
    }


    /**
     * This will be choosing a new battle
     */
    public void ChooseNewBattle()
    {
        // we load the battle selection scene
        SceneManager.LoadScene(battleSelecScene);

        // Making sure that the time scale is set to 1
        Time.timeScale = 1f;

        // playing the sound effect for going to battle selection
        AudioManager.instance.PlaySoundEffect(0);
    }

    /**
     * This will be pausing or unpausing the game
     * Here we learned that when is 0 is stopped, 1 is normal speed and
     * 1.2 we would get slow motion feeling.
     */
    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false) {

            // showing the pause screen
            pauseScreen.SetActive(true);

            // we set to the game to run at 0 speed, pausing the game
            Time.timeScale = 0f;

        } else
        {
            // remove the pause screen
            pauseScreen.SetActive(false);

            // here we set the game to run at 1, the normal speed game
            Time.timeScale = 1f;

        }

        // playing the sound effect for pausing/unpausing
        AudioManager.instance.PlaySoundEffect(0);
    }

}
