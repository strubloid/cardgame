using UnityEngine;
using UnityEngine.InputSystem;

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
        PlayerActive,
        PlayerCardAttack,
        EnemyActive,
        EnemyCardAttack
    }

    // Current phase of the turn
    public TurnOrder currentPhrase;

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

        
        // we c/*heck the current phase to see what to do
        switch (currentPhrase++)
        {
            // Player's turn to play cards
            case TurnOrder.PlayerActive:
                UiController.instance.SetPlayerTurn();
                break;

            // Player's cards attack
            case TurnOrder.PlayerCardAttack:

                UiController.instance.SetPlayerCardAttack();
                break;

            // Enemy's turn to play cards
            case TurnOrder.EnemyActive:

                UiController.instance.SetEnemyTurn();
                break;

            // Enemy's cards attack
            case TurnOrder.EnemyCardAttack:

                UiController.instance.SetEnemyCardAttack();
                currentPhrase = TurnOrder.PlayerActive;
                break;
        }

    }

}
