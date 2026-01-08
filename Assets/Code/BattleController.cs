using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BattleController : MonoBehaviour
{
    // Singleton instance
    public static BattleController instance;

    // Starting mana variables
    public int startMana = 4;
    public int maxMana = 12;
    public int playerMana;

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
        playerMana = startMana;

        // Update the UI at the start
        UiController.instance.SetPlayerManaText(playerMana);

        // This at the start of the battle we draw the starting cards
        DeckController.instance.DrawMultipleCards(startingCardsAmount);

        // taking some seconds before starting the first turn
        StartingTheInitialPhrase(timeBeforeStartingTurn);

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
        ShowTurn();
    }

    /**
     * This will be spending the mana of the player, 
     * after check if is with a value bellow zero, if does, will set it to zero
     */
    public void SpendPlayerMana(int ammountToSpend) {

        // remove mana spent
        playerMana -= ammountToSpend;

        // double check if the value gets below 0
        if (playerMana < 0) {
            playerMana = 0;
        }

        // updating the UI if the instance exists
        if (UiController.instance != null){
            UiController.instance.SetPlayerManaText(playerMana);
        }

    }

    /**
     * This will be changing the turn to the next phase
     * the main responsibility of this method is to handle the turn order
     */
    public void AdvanceTurn() {
        
        // advance to the next turn phase
        currentPhrase++;

        // if we exceed the max size, we reset to zero
        if ( (int) currentPhrase >= System.Enum.GetValues(typeof(TurnOrder)).Length){
            currentPhrase = 0;
        }

        // Show the turn change in the UI
        ShowTurn();
    }

    /**
     * This will be showing the current turn based on the parameter
     */
    public void ShowTurn() {

        switch (currentPhrase)
        {
            // Player's turn to play cards
            case TurnOrder.PlayerTurn:
                UiController.instance.SetPlayerTurn();
                break;

            // Player's cards attack
            case TurnOrder.PlayerCardAttack:

                UiController.instance.SetPlayerCardAttack();
                break;

            // Enemy's turn to play cards
            case TurnOrder.EnemyTurn:
                UiController.instance.SetEnemyTurn();
                break;

            // Enemy's cards attack
            case TurnOrder.EnemyCardAttack:
                UiController.instance.SetEnemyCardAttack();
                break;
        }
    }

}
