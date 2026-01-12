using System.Collections;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{

    // Singleton instance
    public static CardPointsController instance;

    // Arrays to hold player and enemy card points
    public CardPlacePoint[] PlayerCardPoints, EnemyCardPoints;

    // Time between actions in seconds
    public float timeBetweenActions = 0.5f;

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
     * Coroutine to handle player attack phase
     */
    IEnumerator PlayerAttackCo() {

        // Loop through each player card point
        yield return new WaitForSeconds(timeBetweenActions);

        // Loop through each player card point
        for (int  i = 0;  i < PlayerCardPoints.Length;  i++)
        {
            // Check if there is an active card at this point
            if (PlayerCardPoints[i].activeCard != null) {

                if (EnemyCardPoints[i].activeCard != null) {
                    // Attack the enemy card
                    EnemyCardPoints[i].activeCard.DamageCard(PlayerCardPoints[i].activeCard.attackPower);

                } else
                {
                    // Attack the enemy directly
                    Debug.Log("ELLLLSE");

                }

                // Wait between actions
                yield return new WaitForSeconds(timeBetweenActions);

            }
        }

        // After all attacks, advance the turn
        BattleController.instance.AdvanceTurn();
    }
}
