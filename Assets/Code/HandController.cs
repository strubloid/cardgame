using NUnit.Framework;
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

        Vector3 distanceBetweenPoints = Vector3.zero;

        // Checking if we have more than one card
        if (heldCards.Count > 1) {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        // for loop that will be setting the card positions in the hand
        for (int i = 0; i < heldCards.Count; i++) 
        {
            Vector3 cardPos = minPos.position + (distanceBetweenPoints * i);
            cardPositions.Add(cardPos);

            // now we set the card position to the calculated position
            heldCards[i].transform.rotation = minPos.rotation;

            // console.log for the position
            //Debug.Log("Card " + i + " position set to: " + cardPositions[i]);

            // Moving the card to the position smoothly
            heldCards[i].MoveCardToPoint(cardPositions[i], minPos.rotation);
        }
    }
}
