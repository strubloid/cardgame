using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    // Singleton instance
    public static BattleController instance;

    // Starting mana variables
    public int startMana = 4;
    public int maxMana = 12;
    public int playerMana;
    public int manaPerTurn = 2;

    // cards variable
    public int DrawingCardsPerTurn = 1;

    // Quantity of starting cards
    public int startingCardsAmount = 5;

    // This is for the order of the turns in the game, if we add any new stage, we
    // should add it here
    public enum TurnOrder
    {
        PlayerTurn,
        PlayerCardAttack,
        EnemyTurn,
        EnemyCardAttack
    }

    // Current phase of the turn
    public TurnOrder currentPhrase;

    // Time before starting the turn
    public float timeBeforeStartingTurn = 1f;

    // Position that the card will move when discarded
    public Transform DiscardPoint;

    // default starting health
    private int startHealth = 20;

    // Player Health
    public int PlayerHealth;

    // Enemy Health
    public int EnemyHealth;

    // Enemy Mana
    public int enemyMana;

    // Enemy maximum mana
    public int enemyMaxMana = 12;

    // Flag to indicate if the battle has ended
    public bool battleEnded = false;

    // Delay time before showing the results screen
    public float resultsScreenDelayTime = 1.0f;

    // some default colors
    Color greenColor = new Color32(0x08, 0xA1, 0x06, 0xB1); // #08A106
    Color redColor = new Color32(0xA1, 0x31, 0x05, 0xB1); // #A13105

    // Chance for the enemy to start first
    [Range(0f, 1f)]
    public float playerFirstChance = .5f;

    // flag to indicate if it's the first turn
    public bool firstTurn = true;

    // counter for the first turn
    public int FirstTurnCount = 0;

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
        PlayerHealth = startHealth;
        EnemyHealth = startHealth;
        //EnemyHealth = 3;

        // updating the UI of the player health
        UiController.instance.SetPlayerHealthText(PlayerHealth);

        // updating the UI of the enemy health
        UiController.instance.SetEnemyHealthText(EnemyHealth);

        // Update the mana for the player at the start
        UiController.instance.SetPlayerManaText(playerMana);

        // Update the mana for the enemy at the start
        UiController.instance.SetEnemyManaText(playerMana);

        // This at the start of the battle we draw the starting cards
        DeckController.instance.DrawMultipleCards(startingCardsAmount);

        // Fill the mana at the start of the battle
        bool startMatch = true;
        FillPlayerMana(startMatch);
        FillEnemyMana(startMatch);

        // taking some seconds before starting the first turn
        StartingTheInitialPhrase(timeBeforeStartingTurn);

        // this will determine who starts first
        WhoStartsFirst();

        // Play the background music for the battle
        AudioManager.instance.PlayBackgroundMusic();

    }

    /**
     * This will be determining who starts first in the battle
     */
    public void WhoStartsFirst()
    {
        // Randomly decide who starts first
        if (Random.value > playerFirstChance)
        {
            // turn before the enemy starts
            currentPhrase = TurnOrder.PlayerCardAttack;

            // now it goes to the enemy turn
            AdvanceTurn();

        } else {
            // turn before the player starts
            currentPhrase = TurnOrder.EnemyCardAttack;

            // now it goes to the player turn
            AdvanceTurn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If we press T, we advance the turn
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            AdvanceTurn();
        }
    }

    /**
     * This will be starting the initial phrase after a waiting time
     */
    public void StartingTheInitialPhrase(float waitingTime) { 

        StartCoroutine(StartingTheInitialPhraseCo(waitingTime));

    }

    /**
     * Coroutine to advance the turn after a waiting time
     */
    IEnumerator StartingTheInitialPhraseCo(float waitingTime)
    {
        // will wait for the time between drawing cards
        yield return new WaitForSeconds(waitingTime);

        // will run the draw card to hand function
        UiController.instance.SetPlayerTurn();
    }

    /**
     * This will be changing the turn to the next phase
     * the main responsibility of this method is to handle the turn order
     */
    public void AdvanceTurn() {

        // we wont advance the turn if the battle has ended
        if (battleEnded)
        {
            return;
        }

        // advance to the next turn phase
        currentPhrase++;

        // if we exceed the max size, we reset to zero
        if ( (int) currentPhrase >= System.Enum.GetValues(typeof(TurnOrder)).Length){
            currentPhrase = 0;
        }

        // after 4 turns, we disable the first turn flag
        if (firstTurn && FirstTurnCount >= 4) {
            firstTurn = false;
        }

        // Show the turn change in the UI
        switch (currentPhrase)
        {
            // Player's turn to play cards
            case TurnOrder.PlayerTurn:

                // Update the UI to show it's the player's turn
                UiController.instance.SetPlayerTurn();

                // Those are the free action we can do at the start of the turn
                DeckController.instance.DrawMultipleCards(DrawingCardsPerTurn);

                // if isnt the first turn, we let the cards attack
                if (!firstTurn)
                {
                    // only on the second turn forward, we are able to increment the mana
                    IncrementMana();
                } else {
                    FirstTurnCount++;
                }

                break;

            // Player's cards attack
            case TurnOrder.PlayerCardAttack:

                // Update the UI to show it's the player's card attack phase
                UiController.instance.SetPlayerCardAttack();

                // if isnt the first turn, we let the cards attack
                if (!firstTurn) {
                    CardPointsController.instance.PlayerAttack(); // Let the cards attack
                } else {
                    FirstTurnCount++;

                    // we move the turn forward
                    AdvanceTurn();
                }

                break;

            // Enemy's turn to play cards
            case TurnOrder.EnemyTurn:

                // Update the UI to show it's the enemy's turn
                UiController.instance.SetEnemyTurn();

                // if isnt the first turn, we let the cards attack
                if (!firstTurn)
                {
                    // only on the second turn forward, we are able to increment the mana
                    IncrementEnemyMana();
                }
                else
                {
                    FirstTurnCount++;
                }

                // Let the enemy play their actions
                EnemyController.instance.StartAction();

                break;

            // Enemy's cards attack
            case TurnOrder.EnemyCardAttack:

                // Update the UI to show it's the enemy's card attack phase
                UiController.instance.SetEnemyCardAttack();

                // if isnt the first turn, we let the cards attack
                if (!firstTurn)
                {
                    // Let the enemy cards attack
                    CardPointsController.instance.EnemyAttack();
                } else {
                    FirstTurnCount++;

                    // we move the turn forward
                    AdvanceTurn();
                }

                break;
        }
    }

    /**
     * This will be spending the mana of the player, 
     * after check if is with a value bellow zero, if does, will set it to zero
     */
    public void SpendPlayerMana(int ammountToSpend)
    {

        // remove mana spent
        playerMana -= ammountToSpend;

        // double check if the value gets below 0
        if (playerMana < 0)
        {
            playerMana = 0;
        }

        // updating the UI if the instance exists
        if (UiController.instance != null)
        {
            UiController.instance.SetPlayerManaText(playerMana);
        }

    }

    /**
     * This will be increasing the player mana at the start of their turn,
     * we have a rule of not exceeding the maximum mana.
     */
    public void IncrementMana() {

        // increase the player mana by the defined amount
        playerMana += manaPerTurn;
        
        // rule 1: we cant exceed the maximum mana
        if (playerMana > maxMana) {
            playerMana = maxMana;
            maxMana++;
        }

        // Update the UI at the start
        UiController.instance.SetPlayerManaText(playerMana);
        
    }

    /**
    * This will be spending the mana of the enemy, 
    * after check if is with a value bellow zero, if does, will set it to zero
    */
    public void SpendEnemyMana(int ammountToSpend)
    {

        // remove mana spent
        enemyMana -= ammountToSpend;

        // double check if the value gets below 0
        if (enemyMana < 0)
        {
            enemyMana = 0;
        }

        // updating the UI
        UiController.instance.SetEnemyManaText(enemyMana);
    }

    /**
     * This will be filling the player mana to the maximum at the start of the battle
     */
    public void FillPlayerMana(bool startMatch)
    {
        // increase the wnwmy mana by the defined amount
        playerMana = startMatch ? startMana : maxMana;

        // Update the UI at the start
        UiController.instance.SetPlayerManaText(playerMana);
    }

    /**
     * This will be filling the enemy mana to the maximum at the start of the battle
     */
    public void FillEnemyMana(bool startMatch)
    {
        // increase the enemy mana by the defined amount
        enemyMana = startMatch ? startMana : enemyMaxMana;

        // Update the UI at the start
        UiController.instance.SetEnemyManaText(enemyMana);
    }

    /**
     * This will be increasing the enemy mana at the start of their turn,
     * we have a rule of not exceeding the maximum mana.
     */
    public void IncrementEnemyMana()
    {
        // increase the wnwmy mana by the defined amount
        enemyMana += manaPerTurn;

        // we cant exceed the maximum mana
        if (enemyMana > enemyMaxMana)
        {
            enemyMana = enemyMaxMana;
            enemyMaxMana++;
        }

        // Update the UI at the start
        UiController.instance.SetEnemyManaText(enemyMana);
    }

    /**
     * This will be called when the player ends their turn
     */
    public void EndPlayerTurn() 
    {
        // Disable the end turn button to prevent multiple clicks
        UiController.instance.endTurnButton.SetActive(false);

        // For now this is the same as advancing the turn
        AdvanceTurn();
    }

    /**
     * This will be showing the damage indicator for the player or enemy
     */
    private void ShowDamage(UiDamageIndicator playerDamagePrefab, int damageAmount, Transform parent)
    {
        // show the damage indicator
        UiDamageIndicator damageClone = Instantiate(playerDamagePrefab, parent);

        // update the text
        damageClone.damageText.text = damageAmount.ToString();

        // activate the damage indicator
        damageClone.gameObject.SetActive(true);
    }


    /**
     * This will be damaging the player with the given ammount
     */
    public void DamagePlayer(int damageAmmount) {

        // reduce the player health
        if (PlayerHealth > 0 || battleEnded == false) {
            PlayerHealth -= damageAmmount;
        }

        // check if the health goes below zero
        if (PlayerHealth <= 0) {
            PlayerHealth = 0;
            
            // end the battle
            EndBattle();
        }

        // updating the UI
        UiController.instance.SetPlayerHealthText(PlayerHealth);

        // show the damage indicator
        ShowDamage(UiController.instance.playerDamage, damageAmmount, UiController.instance.playerDamage.transform.parent);

        // Play direct attack sound effect
        AudioManager.instance.PlayHurtPlayer();
    }

    /**
     * This will be damaging the enemy with the given ammount
     */
    public void DamageEnemy(int damageAmmount)
    {
        // reduce the player health
        if (EnemyHealth > 0 || battleEnded == false)
        {
            EnemyHealth -= damageAmmount;
        }

        // check if the enemy health goes below zero
        if (EnemyHealth <= 0)
        {
            EnemyHealth = 0;

            // end the battle
            EndBattle();
        }

        // updating the UI
        UiController.instance.SetEnemyHealthText(EnemyHealth);

        // show the damage indicator
        ShowDamage(UiController.instance.enemyDamage, damageAmmount, UiController.instance.enemyDamage.transform.parent);

        // Play direct attack sound effect
        AudioManager.instance.PlayHurtEnemy();
    }

    /**
     * This will be ending the battle when one of the players reach 0 health
     */
    void EndBattle() {

        // Set the battle ended flag to true
        battleEnded = true;

        // clear the player's hand
        HandController.instance.EmptyHand();

        // Determine the battle outcome and update the UI
        if (EnemyHealth <= 0) {
            UiController.instance.battleEndTitleText.text = "You Won";

            // changing the color of the background
            Image background = UiController.instance.battleEndScreen.GetComponent<Image>();
            background.color = greenColor;

            // loading the correct background
            UiController.instance.lose.SetActive(false);
            UiController.instance.win.SetActive(true);

            // removing the enemy cards
            foreach (CardPlacePoint point in CardPointsController.instance.EnemyCardPoints)
            {
                // we will clean all cards that are active only
                if (point.activeCard != null) {
                    point.activeCard.MoveCardToPoint(DiscardPoint.position, point.activeCard.transform.rotation);
                }
            }

        } else {
            UiController.instance.battleEndTitleText.text = "You Lost";

            // changing the color of the background
            Image background = UiController.instance.battleEndScreen.GetComponent<Image>();
            background.color = redColor;

            // loading the correct background
            UiController.instance.win.SetActive(false);
            UiController.instance.lose.SetActive(true);

            // removing the enemy cards
            foreach (CardPlacePoint point in CardPointsController.instance.PlayerCardPoints)
            {
                // we will clean all cards that are active only
                if (point.activeCard != null)
                {
                    point.activeCard.MoveCardToPoint(DiscardPoint.position, point.activeCard.transform.rotation);
                }
            }

        }
        // Start the coroutine to show the results screen after a delay
        StartCoroutine(ShowResultCo());
    }

    /**
     * Coroutine to show the results screen after a delay
     */
    IEnumerator ShowResultCo() 
    {
        // wait for the defined delay time
        yield return new WaitForSeconds(resultsScreenDelayTime);

        // Show the battle end screen
        UiController.instance.battleEndScreen.SetActive(true);

    }
}
