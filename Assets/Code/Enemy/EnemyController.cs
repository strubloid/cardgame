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

    // This is the type of AI the enemy will use
    public enum AIType
    {
        placeFromDeck,
        handRandomPlace,
        handDefensive,
        handAttacking
    }

    // This is the type of AI the enemy will use
    public AIType enemyAIType;

    // This will be the cards in the enemy hand
    private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();

    // This is the starting hand size for the enemy
    public int startHandSize = 5;

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

        // Setting up the starting hand based on the AI type
        if (enemyAIType != AIType.placeFromDeck) {
            SetupHand();
        }
        
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

        // we are ignoring the place from deck AI type
        if (enemyAIType != AIType.placeFromDeck) {

            // Drawing the cards to hand
            for (int i = 0; i < BattleController.instance.DrawingCardsPerTurn; i++) 
            {
                // adding the top card to the hand
                cardsInHand.Add(activeCards[0]);

                // removing the card from the active deck
                activeCards.RemoveAt(0);

                // Checking if we have cards to draw, and we will do fi we have it
                if (activeCards.Count == 0) { 
                    SetupDeck();
                }
            }
        }

        // temporary list to hold the card points
        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();

        // adding the player card points to the list
        cardPoints.AddRange(CardPointsController.instance.EnemyCardPoints);

        // getting a random point index
        int randomPointIndex = Random.Range(0, cardPoints.Count);

        // getting the selected point
        CardPlacePoint selectedPoint = cardPoints[randomPointIndex];

        // if the AI type is place from deck or hand random place, we need to ensure the selected point is free
        if (enemyAIType == AIType.placeFromDeck || enemyAIType == AIType.handRandomPlace)
        {
            // removing the point from the list
            cardPoints.Remove(selectedPoint);

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
        }

        // selected card to play
        CardScriptableObject selectedCard = null;

        // safety iterations
        int iterations = 0;

        // preferred and secondary points
        List<CardPlacePoint> preferredPoints = new List<CardPlacePoint>();
        List<CardPlacePoint> secondaryPoints = new List<CardPlacePoint>();

        // executing the AI type
        switch (enemyAIType) {

            // AI that places a card from the deck to the field
            case AIType.placeFromDeck:

                // checking if the selected point is free
                if (selectedPoint.activeCard == null)
                {

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
                break;

            // AI that places a random card from hand to the field
            case AIType.handRandomPlace:

                // getting a random card index from hand
                selectedCard = SelectedCardToPlay();

                // safety iterations
                iterations = 20;

                // double checking if we have a card to play
                while (selectedCard != null && iterations > 0 && selectedPoint.activeCard == null)
                {
                    // playing the selected card
                    PlayCard(selectedCard, selectedPoint);

                    // getting another random card index from hand
                    selectedCard = SelectedCardToPlay();

                    // decrementing iterations
                    iterations--;

                    // having a small delay so we can feel the card being played
                    yield return new WaitForSeconds(timeBetweenDrawingCards);

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

                }

                break;

            // AI that places a defensive card from hand to the field
            case AIType.handDefensive:

                // getting a random card index from hand
                selectedCard = SelectedCardToPlay();

                // safety iterations
                preferredPoints.Clear();
                secondaryPoints.Clear();

                // categorizing the card points
                for (int i = 0; i < cardPoints.Count; i++) 
                {
                    // checking if the point is at the back line
                    if (cardPoints[i].activeCard == null) 
                    {
                        // when we want to be defensive we put at the same line as the player
                        if (CardPointsController.instance.PlayerCardPoints[i].activeCard != null) {
                            preferredPoints.Add(cardPoints[i]);
                        } else {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 20;

                // we check if we have a card to play, the safe net of iterations and if we have a prefered or secondary point
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {

                    // checking if we use preferred or secondary points
                    if (preferredPoints.Count > 0)
                    {
                        // getting a random point index
                        int selectPoint = Random.Range(0, preferredPoints.Count);

                        // playing the selected card and removing the point from the list
                        selectedPoint = preferredPoints[selectPoint];
                        preferredPoints.RemoveAt(selectPoint);

                    } else {
                        // getting a random point index
                        int selectPoint = Random.Range(0, secondaryPoints.Count);

                        // playing the selected card and removing the point from the list
                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    // playing the selected card
                    PlayCard(selectedCard, selectedPoint);

                    // check if we should play another
                    selectedCard = SelectedCardToPlay();

                    // decrementing iterations
                    iterations--;

                    // will run the draw card to hand function
                    yield return new WaitForSeconds(timeBetweenDrawingCards);

                }
                

                break;

            // AI that places an attacking card from hand to the field
            case AIType.handAttacking:

                // getting a random card index from hand
                selectedCard = SelectedCardToPlay();

                // safety iterations
                preferredPoints.Clear();
                secondaryPoints.Clear();

                // categorizing the card points
                for (int i = 0; i < cardPoints.Count; i++)
                {
                    // checking if the point is at the back line
                    if (cardPoints[i].activeCard == null)
                    {
                        // when we want to be defensive we put at the same line as the player
                        if (CardPointsController.instance.PlayerCardPoints[i].activeCard == null)
                        {
                            preferredPoints.Add(cardPoints[i]);
                        }
                        else
                        {
                            secondaryPoints.Add(cardPoints[i]);
                        }
                    }
                }

                iterations = 20;

                // we check if we have a card to play, the safe net of iterations and if we have a prefered or secondary point
                while (selectedCard != null && iterations > 0 && preferredPoints.Count + secondaryPoints.Count > 0)
                {

                    // checking if we use preferred or secondary points
                    if (preferredPoints.Count > 0)
                    {
                        // getting a random point index
                        int selectPoint = Random.Range(0, preferredPoints.Count);

                        // playing the selected card and removing the point from the list
                        selectedPoint = preferredPoints[selectPoint];
                        preferredPoints.RemoveAt(selectPoint);

                    }
                    else
                    {
                        // getting a random point index
                        int selectPoint = Random.Range(0, secondaryPoints.Count);

                        // playing the selected card and removing the point from the list
                        selectedPoint = secondaryPoints[selectPoint];
                        secondaryPoints.RemoveAt(selectPoint);
                    }

                    // playing the selected card
                    PlayCard(selectedCard, selectedPoint);

                    // check if we should play another
                    selectedCard = SelectedCardToPlay();

                    // decrementing iterations
                    iterations--;

                    // will run the draw card to hand function
                    yield return new WaitForSeconds(timeBetweenDrawingCards);

                }

                break;

        }

        // will run the draw card to hand function
        yield return new WaitForSeconds(timeBetweenDrawingCards);

        // Advancing the turn to the player
        BattleController.instance.AdvanceTurn();
    }

    /**
     * Setting up the enemy starting hand
     */
    void SetupHand() 
    {
        // drawing the starting hand size
        for (int i = 0; i < startHandSize; i++) 
        {
            // checking if we have cards to draw
            if (activeCards.Count == 0) {
                SetupDeck();
            }
            // adding the top card to the hand
            cardsInHand.Add(activeCards[0]);

            // removing the card from the active deck
            activeCards.RemoveAt(0);
        }
    }

    /**
     * Playing a card from the deck to the selected point
     */
    public void PlayCard(CardScriptableObject cardToPlay, CardPlacePoint placePoint) 
    {
        // Spawning the card at the spawn point
        Card newCard = Instantiate(cardToSpawn, cardSpawnPoint.position, cardSpawnPoint.rotation);

        // Setting the card as enemy card
        newCard.cardData = cardToPlay;

        // Because of the setup of the card prefab, we need to apply an extra rotation to make it face the correct way
        Quaternion finalRotation = Quaternion.Euler(0f, 180f, 0f);

        // setting up the default deck position and rotation if not set
        if (!newCard.hasDefaultDeckPosition)
        {
            newCard.defaultDeckPosition = placePoint.transform.position;
            newCard.defaultDeckRotation = finalRotation;
            newCard.hasDefaultDeckPosition = true;
        }

        // Moving the card to the selected point
        newCard.MoveCardToPoint(placePoint.transform.position, finalRotation);

        // Assigning the card to the selected point
        placePoint.activeCard = newCard;

        // Assigning the selected point to the card
        newCard.assignedPlace = placePoint;

        // Removing the card from the hand
        cardsInHand.Remove(cardToPlay);

        // Spending the enemy mana
        BattleController.instance.SpendEnemyMana(cardToPlay.manaCost);

        // Playing the sound effect
        AudioManager.instance.PlayCardPlace();
    }

    /**
     * Selecting a card to play to the player
     */
    public CardScriptableObject SelectedCardToPlay() 
    {
        // instance of the card to play
        CardScriptableObject cardToPlay = null;

        // this is a list that will store all the cards that can be played
        List<CardScriptableObject> cardsToPlay = new List<CardScriptableObject>();

        // check all the cards that has less or lower than the current mana
        foreach (CardScriptableObject card in cardsInHand) 
        {

            // checking if the card mana cost is less than or equal to the current mana
            if (card.manaCost <= BattleController.instance.enemyMana) 
            {
                cardsToPlay.Add(card);
            }
        }

        // if we have a card to play we will do
        if (cardsToPlay.Count > 0) 
        {
            // this will select a card based on the AI type
            int selectedIndex = Random.Range(0, cardsToPlay.Count);

            // selecting the card to play
            cardToPlay = cardsToPlay[selectedIndex];
        }

        return cardToPlay;

    }

}
