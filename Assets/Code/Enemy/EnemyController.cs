using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Singleton instance
    public static EnemyController instance;

    // This is the time between drawing each card, so they dont all draw at once
    public float timeBetweenDrawingCards = .5f;

    // This will be the card to spawn from the deck
    public Card cardToSpawn;

    // This is the point where the cards will spawn from the deck
    public Transform cardSpawnPoint;

    // This is the type of AI the enemy will use
    public enum AIType
    {
        handDefensive,
        handAttacking
    }

    // This is the type of AI the enemy will use
    public AIType enemyAIType;

    // list of preferred and secondary points
    List<CardPlacePoint> PreferredPoints = new List<CardPlacePoint>();
    List<CardPlacePoint> SecondaryPoints = new List<CardPlacePoint>();

    // This will be the cards in the enemy hand
    //private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();

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
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {

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
        if (EnemyDeckController.Instance.activeCards.Count == 0) {
            EnemyDeckController.Instance.SetupDeck();
        }

        // Drawing the cards to hand + 1 active card
        EnemyDeckController.Instance.DrawCardToHand();

        // will run the draw card to hand function
        float playingCardsDelay = timeBetweenDrawingCards + 1.0f;

        // temporary list to hold the card points
        List<CardPlacePoint> cardPoints = new List<CardPlacePoint>();

        // adding the enemy card points to the list
        cardPoints.AddRange(CardPointsController.instance.EnemyCardPoints);

        // Selected point to play and card
        CardPlacePoint selectedPoint = null;
        Card selectedCard = null;

        // safety iterations
        int maxIterations = 20;

        // loading a card to play
        selectedCard = GetCardToPlay();

        // TODO: REMOVE THIS LIMITATION AFTER TESTING
        int cardPlayMax = 1;

        // executing the AI type
        switch (enemyAIType) {

            // AI that places a defensive card from hand to the field
            case AIType.handDefensive:

                // this will load the PreferredPoints and SecondaryPoints lists
                LoadPreferedSecondaryPoints(cardPoints, enemyAIType);

                // we check if we have a card to play, the safe net of iterations and if we have a prefered or secondary point
                while (selectedCard != null && maxIterations > 0 && PreferredPoints.Count + SecondaryPoints.Count > 0)
                {
                    // limiting to play only one card for attacking AI
                    if (cardPlayMax > 1) {
                        break;
                    }

                    // getting the selected point
                    selectedPoint = GetSelectedPoint();

                    // playing the selected card
                    PlayCard(selectedCard, selectedPoint);

                    // check if we should play another
                    selectedCard = GetCardToPlay();

                    // decrementing iterations
                    maxIterations--;

                    // will run the draw card to hand function
                    yield return new WaitForSeconds(playingCardsDelay);

                }

                break;

            // AI that places an attacking card from hand to the field
            case AIType.handAttacking:

                // this will load the PreferredPoints and SecondaryPoints lists
                LoadPreferedSecondaryPoints(cardPoints, enemyAIType);

                // we check if we have a card to play, the safe net of iterations and if we have a prefered or secondary point
                while (selectedCard != null && maxIterations > 0 && PreferredPoints.Count + SecondaryPoints.Count > 0)
                {

                    // limiting to play only one card for attacking AI
                    if (cardPlayMax > 1) {
                        break;
                    }

                    // getting the selected point
                    selectedPoint = GetSelectedPoint();

                    // playing the selected card
                    PlayCard(selectedCard, selectedPoint);

                    // check if we should play another
                    selectedCard = GetCardToPlay();

                    // decrementing iterations
                    maxIterations--;

                    cardPlayMax++;

                    // will run the draw card to hand function
                    yield return new WaitForSeconds(playingCardsDelay);

                }

                break;

        }

        // will run the draw card to hand function
        yield return new WaitForSeconds(timeBetweenDrawingCards);

        // Advancing the turn to the player
        BattleController.instance.AdvanceTurn();
    }

    /**
     * This will load the prefered points based on the AI type
     */
    private void LoadPreferedSecondaryPoints(List<CardPlacePoint> cardPoints, AIType enemyAIType) 
    {
        // basic cleaning before using it
        PreferredPoints.Clear();
        SecondaryPoints.Clear();

        // categorizing the card points
        for (int i = 0; i < cardPoints.Count; i++)
        {
            // checking if the point is at the back line
            if (cardPoints[i].activeCard == null)
            {
                // depend on the AI type we will categorize the points
                switch (enemyAIType)
                {
                    case AIType.handDefensive:

                        // when we want to be defensive we put at the same line as the player
                        if (CardPointsController.instance.PlayerCardPoints[i].activeCard != null)
                        {
                            PreferredPoints.Add(cardPoints[i]);
                        } else {
                            SecondaryPoints.Add(cardPoints[i]);
                        }

                        break;


                    case AIType.handAttacking:

                        // when we want to be defensive we put at the same line as the player
                        if (CardPointsController.instance.PlayerCardPoints[i].activeCard == null)
                        {
                            PreferredPoints.Add(cardPoints[i]);
                        } else {
                            SecondaryPoints.Add(cardPoints[i]);
                        }

                        break;
                }
            }
        }

    }

    /**
     * This will get the selected point to play
     */
    private CardPlacePoint GetSelectedPoint() {

        // Selected point to play
        CardPlacePoint selectedPoint = null;

        // checking if we use preferred or secondary points
        if (PreferredPoints.Count > 0)
        {
            // getting a random point index
            int selectPoint = Random.Range(0, PreferredPoints.Count);

            // playing the selected card and removing the point from the list
            selectedPoint = PreferredPoints[selectPoint];
            PreferredPoints.RemoveAt(selectPoint);

        } else {
            // getting a random point index
            int selectPoint = Random.Range(0, SecondaryPoints.Count);

            // playing the selected card and removing the point from the list
            selectedPoint = SecondaryPoints[selectPoint];
            SecondaryPoints.RemoveAt(selectPoint);
        }

        return selectedPoint;
    }


    /**
     * Playing a card from the deck to the selected point
     */
    public void PlayCard(Card cardToPlay, CardPlacePoint placePoint) 
    {
        // Because of the setup of the card prefab, we need to apply an extra rotation to make it face the correct way
        Quaternion finalRotation = Quaternion.Euler(0f, 180f, 0f);

        // setting up the default deck position and rotation if not set
        if (!cardToPlay.hasDefaultDeckPosition)
        {
            cardToPlay.defaultDeckPosition = placePoint.transform.position;
            cardToPlay.defaultDeckRotation = finalRotation;
            cardToPlay.hasDefaultDeckPosition = true;
        }

        // Moving the card to the selected point
        cardToPlay.MoveCardToPoint(placePoint.transform.position, finalRotation);

        // Assigning the card to the selected point
        placePoint.activeCard = cardToPlay;

        // Assigning the selected point to the card
        cardToPlay.assignedPlace = placePoint;

        // removing from the enemy hand controller
        EnemyHandController.Instance.RemoveCardFromHand(cardToPlay);

        // Spending the enemy mana
        BattleController.instance.SpendEnemyMana(cardToPlay.manaCost);

        // Playing the sound effect
        AudioManager.instance.PlayCardPlace();
    }

    /**
     * Selecting a card to play to the enemy
     */
    public Card GetCardToPlay() 
    {
        // instance of the card to play
        Card cardToPlay = null;

        // those are the cards that we can play
        List<Card> cardsAvaiableToPlay = EnemyHandController.Instance.CardsAvaiableToPlay(BattleController.instance.enemyMana);

        // if we have a card to play we will do
        if (cardsAvaiableToPlay.Count > 0)
        {
            // this will select a card based on the AI type
            int selectedIndex = Random.Range(0, cardsAvaiableToPlay.Count);

            // selecting the card to play
            cardToPlay = cardsAvaiableToPlay[selectedIndex];
        }

        return cardToPlay;

    }

}
