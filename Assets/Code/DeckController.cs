using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/**
 * Class that will be adding the rules of the deck
 */
public class DeckController : MonoBehaviour
{
    // Singleton instance
    public static DeckController instance;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            DrawCardToHand();

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
     * 2.92 0.56
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

        // Adding the new card to the player's hand
        HandController.instance.AddCardToHand(newCard);
    }
}
