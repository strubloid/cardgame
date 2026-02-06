using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{

    // Singleton instance
    public static CardPointsController instance;

    // Arrays to hold player and enemy card points
    public CardPlacePoint[] PlayerCardPoints;
    public CardPlacePoint[] EnemyCardPoints;

    // Time between actions in seconds
    public float timeBetweenActions = 1.5f;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /**
     * This will be starting the player attack phase
     */
    public void PlayerAttack() {

        // we start the player attack coroutine
        StartCoroutine(PlayerAttackCo());

    }

    /**
     * This will be returning all active cards from the given card points
     */
    private CardPlacePoint[] GetActiveCards(CardPlacePoint[] cardPoints)
    {
        List<CardPlacePoint> activeCards = new List<CardPlacePoint>();

        for (int i = 0; i < cardPoints.Length; i++)
        {
            if (cardPoints[i].activeCard != null)
            {
                activeCards.Add(cardPoints[i]);
            }
        }

        return activeCards.ToArray();
    }

    /**
     * This will be returning all active enemy cards
     */
    public CardPlacePoint[] GetActiveEnemyCards()
    {
        return GetActiveCards(EnemyCardPoints);
    }

    /**
     * This will be returning all active player cards
     */
    public CardPlacePoint[] GetActivePlayerCards()
    {
        return GetActiveCards(PlayerCardPoints);
    }

    /**
     * Coroutine to handle player attack phase
     */
    IEnumerator PlayerAttackCo() {

        // Get all active enemy cards
        CardPlacePoint[]  activeEnemyCards = GetActiveEnemyCards();

        // Get all active player cards
        CardPlacePoint[]  activePlayerCards = GetActivePlayerCards();

        // this will store what is the current player card index
        int currentEnemyCardIndex = 0;

        // If players has no attackers, just advance
        if (activePlayerCards.Length == 0)
        {
            BattleController.instance.AdvanceTurn();
            CheckAssignedCards();
            yield break;
        }

        // if exist card to defend the attack we should loop through them
        if (activeEnemyCards.Length > 0)
        {
            // looping through each player card point
            for (int currentPlayerCardIndex = 0; currentPlayerCardIndex < activePlayerCards.Length; currentPlayerCardIndex++)
            {
                Vector3 currentTargetPosition;

                // checkinf if we have enemy cards to attack
                if (currentEnemyCardIndex >= activeEnemyCards.Length)
                {
                    // loading the position to throw the attack animation to the enemy directly
                    currentTargetPosition = BattleController.instance.EnemyPosition.position;

                    // This will attacking the enemy directly
                    BattleController.instance.DamageEnemy(activePlayerCards[currentPlayerCardIndex].activeCard.attackPower);

                } else {

                    // Safety checks
                    if (activePlayerCards[currentPlayerCardIndex].activeCard == null)
                        continue;

                    // Enemy card already destroyed → move to next
                    if (activeEnemyCards[currentEnemyCardIndex].activeCard == null)
                    {
                        currentEnemyCardIndex++;
                        currentPlayerCardIndex--; // retry same attacker on next enemy
                        continue;
                    }

                    // this will be the position to throw the attack animation
                    currentTargetPosition = activeEnemyCards[currentEnemyCardIndex].activeCard.transform.position;

                    // attack from player card to enemy card
                    activeEnemyCards[currentEnemyCardIndex].activeCard.DamageCard(
                        activePlayerCards[currentPlayerCardIndex].activeCard.attackPower
                    );

                    // If enemy died, advance enemy index
                    if (activeEnemyCards[currentEnemyCardIndex].activeCard == null)
                    {
                        currentEnemyCardIndex++;
                    }

                }

                // This will trigger the animation of Attack
                //activePlayerCards[currentPlayerCardIndex].activeCard.animator.SetTrigger("Attack");
                //

                // we will turn the power and set the animation to damage the enemy card
                activePlayerCards[currentPlayerCardIndex].activeCard.PowerController.ActivatePowerAnimation(
                    currentTargetPosition
                );

                yield return new WaitForSeconds(timeBetweenActions);

                // ending the loop if battle ended, so wont be having any extra attacks after 0 health
                if (BattleController.instance.battleEnded == true) {
                    currentPlayerCardIndex = activePlayerCards.Length;
                }
            }

        } else {

            // looping through each player card point
            for (int currentPlayerCardIndex = 0; currentPlayerCardIndex < activePlayerCards.Length; currentPlayerCardIndex++)
            {
                // this will be the position to throw the attack animation
                Vector3 currentTargetPosition = BattleController.instance.EnemyPosition.position;

                // No defending cards → player attacks enemy directly
                BattleController.instance.DamageEnemy(activePlayerCards[currentPlayerCardIndex].activeCard.attackPower);

                // This will trigger the animation of Attack
                //activePlayerCards[currentPlayerCardIndex].activeCard.animator.SetTrigger("Attack");

                // we will turn the power and set the animation to damage the enemy card
                activePlayerCards[currentPlayerCardIndex].activeCard.PowerController.ActivatePowerAnimation(
                    currentTargetPosition
                );

                yield return new WaitForSeconds(timeBetweenActions);

                // ending the loop if battle ended, so wont be having any extra attacks after 0 health
                if (BattleController.instance.battleEnded == true)
                {
                    currentPlayerCardIndex = activePlayerCards.Length;
                }
            }
        }

        // After all attacks, advance the turn
        BattleController.instance.AdvanceTurn();

        // extra check for destroyed cards
        CheckAssignedCards();
    }

    /**
     * This will be starting the enemy attack phase
     */
    public void EnemyAttack() {
        // we start the player attack coroutine
        StartCoroutine(EnemyAttackCo());
    }

    /**
     * Coroutine to handle enemy attack phase
     */
    IEnumerator EnemyAttackCo()
    {
        // Get all active enemy cards (attackers)
        CardPlacePoint[] activeEnemyCards = GetActiveEnemyCards();

        // Get all active player cards (defenders)
        CardPlacePoint[] activePlayerCards = GetActivePlayerCards();

        // If enemy has no attackers, just advance
        if (activeEnemyCards.Length == 0)
        {
            BattleController.instance.AdvanceTurn();
            CheckAssignedCards();
            yield break;
        }

        // This will store what is the current player card index (defender)
        int currentPlayerCardIndex = 0;

        // If player has cards to defend, loop through them
        if (activePlayerCards.Length > 0)
        {
            // Loop through each enemy card (attacker)
            for (int currentEnemyCardIndex = 0; currentEnemyCardIndex < activeEnemyCards.Length; currentEnemyCardIndex++)
            {
                // this will be the position to throw the attack animation
                Vector3 currentTargetPosition;

                // If we ran out of player defenders, enemy attacks directly
                if (currentPlayerCardIndex >= activePlayerCards.Length)
                {
                    // loading the position to throw the attack animation to the player directly
                    currentTargetPosition = BattleController.instance.PlayerPosition.position;

                    BattleController.instance.DamagePlayer(activeEnemyCards[currentEnemyCardIndex].activeCard.attackPower);

                } else {

                    // Safety checks
                    if (activeEnemyCards[currentEnemyCardIndex].activeCard == null)
                        continue;

                    // Player card already destroyed -> move to next player defender
                    if (activePlayerCards[currentPlayerCardIndex].activeCard == null)
                    {
                        currentPlayerCardIndex++;
                        currentEnemyCardIndex--; // retry same attacker on next defender
                        continue;
                    }

                    // loading the position to throw the attack animation
                    currentTargetPosition = activePlayerCards[currentPlayerCardIndex].activeCard.transform.position;

                    // Attack from enemy card to player card
                    activePlayerCards[currentPlayerCardIndex].activeCard.DamageCard(
                        activeEnemyCards[currentEnemyCardIndex].activeCard.attackPower
                    );

                    // If player died, advance defender index
                    if (activePlayerCards[currentPlayerCardIndex].activeCard == null)
                    {
                        currentPlayerCardIndex++;
                    }

                }

                // Trigger attacker animation
                //activeEnemyCards[currentEnemyCardIndex].activeCard.animator.SetTrigger("Attack");

                // we will turn the power and set the animation to damage the enemy card
                activeEnemyCards[currentEnemyCardIndex].activeCard.PowerController.ActivatePowerAnimation(
                    currentTargetPosition
                );

                yield return new WaitForSeconds(timeBetweenActions);

                // ending the loop if battle ended, so wont be having any extra attacks after 0 health
                if (BattleController.instance.battleEnded == true)
                {
                    currentEnemyCardIndex = activeEnemyCards.Length;
                }
            }

        }
        else
        {
            // Looping through each enemy card (attacker)
            for (int currentEnemyCardIndex = 0; currentEnemyCardIndex < activeEnemyCards.Length; currentEnemyCardIndex++)
            {
                // this will be the position to throw the attack animation
                Vector3 currentTargetPosition = BattleController.instance.PlayerPosition.position;

                // No defending cards -> enemy attacks player directly
                BattleController.instance.DamagePlayer(activeEnemyCards[currentEnemyCardIndex].activeCard.attackPower);

                // Trigger enemy animation
                //activeEnemyCards[currentEnemyCardIndex].activeCard.animator.SetTrigger("Attack");

                // we will turn the power and set the animation to damage the enemy card
                activeEnemyCards[currentEnemyCardIndex].activeCard.PowerController.ActivatePowerAnimation(
                    currentTargetPosition
                );

                yield return new WaitForSeconds(timeBetweenActions);

                // ending the loop if battle ended, so wont be having any extra attacks after 0 health
                if (BattleController.instance.battleEnded == true)
                {
                    currentEnemyCardIndex = activeEnemyCards.Length;
                }
            }

        }

        // After all attacks, advance the turn
        BattleController.instance.AdvanceTurn();

        // Extra check for destroyed cards
        CheckAssignedCards();
    }



    /**
     * This will be checking assigned cards for both player and enemy card points
     */
    public void CheckAssignedCards() {

        // Check enemy card points for destroyed cards
        foreach (CardPlacePoint point in EnemyCardPoints) {

            // Check if there is an active card
            if (point.activeCard != null) {

                // Check if the card's health is zero or below
                if (point.activeCard.currentHealth <= 0) {
                    // Remove destroyed card
                    point.activeCard = null;
                }
            }
        }

        // Check player card points for destroyed cards
        foreach (CardPlacePoint point in PlayerCardPoints)
        {
            // Check if there is an active card
            if (point.activeCard != null)
            {
                // Check if the card's health is zero or below
                if (point.activeCard.currentHealth <= 0)
                {
                    // Remove destroyed card
                    point.activeCard = null;
                }
            }
        }
    }
}
