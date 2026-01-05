using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // Cards array that the player is holding
    public Card[] heldCards;

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
        if (heldCards.Length > 1) {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Length - 1);
        }

        // for loop that will be setting the card positions in the hand
        for (int i = 0; i < heldCards.Length; i++) 
        {
            Vector3 cardPos = minPos.position + (distanceBetweenPoints * i);
            cardPositions.Add(cardPos);

            // now we set the card position to the calculated position
            heldCards[i].transform.position = cardPositions[i];
        }

        //for (int i = 0; i < heldCards.Length; i++) 
        //{
        //    float t = heldCards.Length == 1 ? 0.5f : (float)i / (heldCards.Length - 1);
        //    Vector3 cardPos = Vector3.Lerp(minPos.position, maxPos.position, t);
        //    cardPositions.Add(cardPos);
        //}
    }
}
