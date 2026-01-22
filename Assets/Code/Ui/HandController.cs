using System.Collections.Generic;
using UnityEngine;

public abstract class HandController : MonoBehaviour
{
    // Cards array that the player is holding
    public List<Card> cardsInHand = new List<Card>();

    // Min and max position of the hand
    public Transform minPos, maxPos;

    // This is a list of vectors that will be holding the card positions in the hand
    public List<Vector3> cardPositions = new List<Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        SetCardPositionsInHand();
    }

    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
    }

    /**
     * This will be returning a list of cards that can be played
     * based on the available mana
     */
    public List<Card> CardsAvaiableToPlay(int maxMana) {

        // those are the cards that we can play
        List<Card> cardsAvaiableToPlay = new List<Card>();

        // looping through the cards in hand
        foreach (Card card in cardsInHand)
        {
            // checking if we have enough mana to play the card
            if (card.cardData.manaCost <= maxMana)
            {
                cardsAvaiableToPlay.Add(card);
            }
        }

        return cardsAvaiableToPlay;

    }

    // abstract method to be implemented in child classes
    public abstract void SetCardPositionsInHand();

    /**
     * This will be removing a card from the hand
     */
    public void RemoveCardFromHand(Card cardToRemove) {

        // Check if the hand position is valid
        if (cardToRemove.handPosition < 0 || cardToRemove.handPosition >= cardsInHand.Count)
        {
            return;
        }

        // Double check that the card is actually in the hand before removing
        if (cardsInHand[cardToRemove.handPosition] == cardToRemove)
        {
            cardsInHand.RemoveAt(cardToRemove.handPosition);
            SetCardPositionsInHand();
        }
    }

    /**
     * This will be adding a card to the hand
     */
    public void AddCardToHand(Card cardToAdd) {

        // Add the card to the held cards list
        cardsInHand.Add(cardToAdd);
        SetCardPositionsInHand();
    }

    /**
     * This will be emptying the player's hand
     * and discarding all held cards
     */
    public void EmptyHand() {

        // loop all cards in hand and move them to discard pile
        foreach (Card heldCard in cardsInHand) {

            // set the card as not in hand
            heldCard.inHand = false;

            // fliping 180 so the card back is facing up
            heldCard.transform.rotation = Quaternion.Euler(
                heldCard.transform.rotation.eulerAngles.x,
                heldCard.transform.rotation.eulerAngles.y,
                -180f
            );

            // move the card to discard pile
            heldCard.MoveCardToPoint(BattleController.instance.DiscardPoint.position, heldCard.transform.rotation);
        }

        // clearing the array of held cards
        cardsInHand.Clear();

    }
}
