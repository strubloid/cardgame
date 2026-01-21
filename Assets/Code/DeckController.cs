using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

/**
 * Class that will be adding the rules of the deck
 */
public class DeckController : MonoBehaviour
{
    // Singleton instance
    public static DeckController instance;

    // Cost to draw a card from the deck
    public int drawCost = 1;

    // This is the time between drawing each card, so they dont all draw at once
    public float timeBetweenDrawingCards = .35f;

    // checking if is a deck for another player
    public bool isEnemy = false;

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

    // This will be the deck to use in the battle
    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();

    // This will be the active cards in the deck
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    // This will be the card to spawn from the deck
    public Card cardToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupDeck();

        // Update the UI text once at start
        UiController.instance.SetDrawCardButtonText(drawCost);
    }

    // Update is called once per frame
    void Update()
    {
        // Tab as key for drawing a card for mana
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            DrawCardForMana();
        }
    }

    /**
     * Rules about how to setup the deck at the start of the battle
     */
    public void SetupDeck() {

        // Clear the active cards list
        activeCards.Clear();

        // Create a temporary copy of the deck to use
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>(deckToUse);

        // Creating a safe check to avoid infinite loops
        int iterations = 0;
        int maxIterations = 500;

        // This will be shuffling the deck
        while (tempDeck.Count > 0 && iterations < maxIterations) {

            // getting a random element from the tempDeck
            int selected = Random.Range(0, tempDeck.Count);

            // adding the selected card to the active cards
            activeCards.Add(tempDeck[selected]);

            // removing the selected card from the tempDeck
            tempDeck.RemoveAt(selected);

            // incrementing the iterations counter
            iterations++;
        }
    }

    /**
     * This will be drawing a card from the deck to the hand
     */
    public void DrawCardToHand() 
    {
        // Checking if we have cards to draw
        if (activeCards.Count == 0) {
            SetupDeck();
        }

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        // This will create a copy of the card prefab at the deck position
        Card newCard = Instantiate(cardToSpawn, initialPosition, initialRotation);

        // Setting the card data to be the first card in the active cards
        newCard.cardData = activeCards[0];

        // Removing the drawn card from the active cards
        newCard.SetupCard();

        // removing the first card from the active cards list
        activeCards.RemoveAt(0);

        // TODO: Refactor here
        // Adding the new card to the player's hand
        HandController targetHand = isEnemy ? EnemyHandController.Instance : PlayerHandController.Instance;
        targetHand.AddCardToHand(newCard);

        //HandController.instance.AddCardToHand(newCard);

        // Playing the card draw sound effect
        AudioManager.instance.PlayCardDraw();
    }


    /**
     * 
     * This will be drawing a card for mana (if any special rules apply)
     */
    public void DrawCardForMana()
    {
        // 
        if (BattleController.instance.playerMana >= drawCost)
        {
            DrawCardToHand();
            BattleController.instance.SpendPlayerMana(drawCost);
        }
        else { 
            UiController.instance.ShowManaWarning();
            UiController.instance.drawCardButton.SetActive(false);
        }

    }

    /**
     * This will be drawing multiple cards from the deck to the hand
     */
    public void DrawMultipleCards(int amountToDraw) {

        // this is the way to start an IEnumerator function
        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    /**
     * This will be drawing multiple cards from the deck to the hand with a delay
     */
    IEnumerator DrawMultipleCo(int amountToDraw) 
    {
        // Main loop to draw multiple cards, in this case the ammount to draw will
        for (int i = 0; i < amountToDraw; i++)
        {
            // will run the draw card to hand function
            DrawCardToHand();

            // will wait for the time between drawing cards
            yield return new WaitForSeconds(timeBetweenDrawingCards);
        }
    }
    
}
