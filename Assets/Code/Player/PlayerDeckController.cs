using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

/**
 * Class that will be adding the rules of the deck
 */
public class PlayerDeckController : DeckController
{
    public static PlayerDeckController Instance { get; private set; } // Singleton instance

    // Starting the instance of this controller
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /**
     * This will be drawing a card to the player's hand
     */
    public override void DrawCardToHand()
    {
        // Checking if we have cards to draw
        if (activeCards.Count == 0)
        {
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
        PlayerHandController.Instance.AddCardToHand(newCard);

        // Playing the card draw sound effect
        AudioManager.instance.PlayCardDraw();
    }

}
