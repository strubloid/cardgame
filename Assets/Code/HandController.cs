using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // Cards array that the player is holding
    public List<Card> heldCards = new List<Card>();

    // Min and max position of the hand
    public Transform minPos, maxPos;

    // This is a list of vectors that will be holding the card positions in the hand
    public List<Vector3> cardPositions = new List<Vector3>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetCardPositionsInHand();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This will be setting the card positions in the hand
     */
    public void SetCardPositionsInHand()
    {
        cardPositions.Clear();

        // Always calculate positions using a fixed hand size, not heldCards.Count
        int handSize = Mathf.Max(heldCards.Count, 1);

        Vector3 distanceBetweenPoints = Vector3.zero;

        // Checking if we have more than one position in the hand span
        if (handSize > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (handSize - 1);
        }

        // for loop that will be setting the card positions in the hand
        for (int i = 0; i < heldCards.Count; i++)
        {
            // Clamp index into the span so cards always start at minPos and move right
            float t = (handSize == 1) ? 0f : (float)i / (handSize - 1);
            Vector3 position = Vector3.Lerp(minPos.position, maxPos.position, t);

            cardPositions.Add(position);

            // Moving the card to the position smoothly
            heldCards[i].MoveCardToPoint(cardPositions[i], minPos.rotation);

            // hold the cart in the moment
            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }
}
