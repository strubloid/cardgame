using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyHandController : HandController
{
    // Singleton instance
    public static EnemyHandController Instance { get; private set; } // Singleton instance

    /**
     * Awake is called when the script instance is being loaded
     */
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

  /**
   * This will be setting the card positions in the hand
   */
    public override void SetCardPositionsInHand()
    {
        // Clear previous card positions
        cardPositions.Clear();

        // Always calculate positions using a fixed hand size, not heldCards.Count
        int handSize = Mathf.Max(cardsInHand.Count, 1);

        Vector3 distanceBetweenPoints = Vector3.zero;

        // Checking if we have more than one position in the hand span
        if (handSize > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (handSize - 1);
        }

        // for loop that will be setting the card positions in the hand
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            // Clamp index into the span so cards always start at minPos and move right
            float t = (handSize == 1) ? 0f : (float)i / (handSize - 1);
            Vector3 position = Vector3.Lerp(minPos.position, maxPos.position, t);

            cardPositions.Add(position);

            // Moving the card to the position smoothly
            cardsInHand[i].MoveCardToPoint(cardPositions[i], minPos.rotation);

            // hold the cart in the moment
            cardsInHand[i].inHand = true;
            cardsInHand[i].handPosition = i;

            // keep enemy face-down
            cardsInHand[i].transform.rotation = Quaternion.Euler(
                cardsInHand[i].transform.rotation.eulerAngles.x,
                cardsInHand[i].transform.rotation.eulerAngles.y,
                -180f
            );
        }

    }

}
