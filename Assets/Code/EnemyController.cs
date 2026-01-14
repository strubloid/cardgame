using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Singleton instance
    public static EnemyController instance;

    // This is the deck to use in the battle
    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();

    // This will be the active cards in the deck
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    // This is the time between drawing each card, so they dont all draw at once
    public float timeBetweenDrawingCards = .5f;

    // This will be the card to spawn from the deck
    public Card cardToSpawn;

    // This is the point where the cards will spawn from the deck
    public Transform cardSpawnPoint;

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
        // Setting up the deck at the start of the battle
        SetupDeck();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * Rules about how to setup the deck at the start of the battle
     */
    public void SetupDeck()
    {

        // Clear the active cards list
        activeCards.Clear();

        // Create a temporary copy of the deck to use
        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>(deckToUse);

        // Creating a safe check to avoid infinite loops
        int iterations = 0;
        int maxIterations = 500;

        // This will be shuffling the deck
        while (tempDeck.Count > 0 && iterations < maxIterations)
        {

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
     * Enemy starts their action
     */
    public void StartAction() {

        // amount of cards to draw
        StartCoroutine(EnemyActionCo());
    }


    /**
     * This will be drawing multiple cards from the deck to the hand with a delay
     */
    IEnumerator EnemyActionCo()
    {
        // Checking if we have cards to draw
        if (activeCards.Count == 0) { 
            SetupDeck();
        }

        // will run the draw card to hand function
        yield return new WaitForSeconds(timeBetweenDrawingCards);

        // temporary list to hold the card points
        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();

        // adding the player card points to the list
        cardPoints.AddRange(CardPointsController.instance.EnemyCardPoints);

        // getting a random point index
        int randomPointIndex = Random.Range(0, cardPoints.Count);

        // getting the selected point
        CardPlacePoint selectedPoint = cardPoints[randomPointIndex];

        // ensuring the selected point is free
        while (selectedPoint.activeCard != null && cardPoints.Count > 0)
        {
            // getting a new random point index
            randomPointIndex = Random.Range(0, cardPoints.Count);

            // getting the selected point
            selectedPoint = cardPoints[randomPointIndex];

            // removing the point from the list
            cardPoints.RemoveAt(randomPointIndex);
        }

        // checking if the selected point is free
        if (selectedPoint.activeCard == null) {

            // Spawning the card at the spawn point
            Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);

            // Setting the card as enemy card
            newCard.cardData = activeCards[0];

            // Setting up the card
            activeCards.RemoveAt(0);

            // Because of the setup of the card prefab, we need to apply an extra rotation to make it face the correct way
            Quaternion finalRotation = Quaternion.Euler(0f, 180f, 0f);

            // Moving the card to the selected point
            newCard.MoveCardToPoint(selectedPoint.transform.position, finalRotation);

            // Assigning the card to the selected point
            selectedPoint.activeCard = newCard;

            // Assigning the selected point to the card
            newCard.assignedPlace = selectedPoint;

        }

        // will run the draw card to hand function
        yield return new WaitForSeconds(timeBetweenDrawingCards);

        // Advancing the turn to the player
        BattleController.instance.AdvanceTurn();
    }


}
