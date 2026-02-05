using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerHandController : HandController
{
    public static PlayerHandController Instance { get; private set; } // Singleton instance

    // Starting the instance of this controller
    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /**
      * This will be setting the card positions in the hand
      */
    public override void SetCardPositionsInHand()
    {
        // Clear previous card positions
        cardPositions.Clear();

        // Clear previous card rotations
        cardRotations.Clear();

        // Always calculate positions using a fixed hand size, not heldCards.Count
        int handSize = Mathf.Max(cardsInHand.Count, 1);

        // checking if we dont have any cards in hand, if we dont have any cards we will just return
        if (handSize == 0)
        {
            return;
        }

        // Calculate the spacing between cards based on the max hand size
        // FIX: use a constant visible gap instead of compressing by hand size
        float CardSpacing = 0.12f; // guarantees side-by-side visibility

        // This is the center position of the hand span (0.5 means the middle)
        float CenterPosition = 0.5f;

        // Calculate the position of the first card so that the hand is centered around the middle
        float FirstCardPosition = CenterPosition - (handSize - 1) * CardSpacing / 2f;

        // Getting the spline from the spline container
        Spline Spline = SplineContainer.Spline;

        float splineStart = 0.15f;
        float splineEnd = 0.85f;
        float usableRange = splineEnd - splineStart;

        // loop that will be setting the card positions in the hand
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            // getting from the left to the right
            //float position = FirstCardPosition + i * CardSpacing;

            float t = handSize == 1 ? 0.5f : (float)i / (handSize - 1);

            float position = splineStart + t * usableRange;

            // Evaluate the position on the spline for the current card
            Vector3 splinePosition = Spline.EvaluatePosition(position);
            Vector3 Forward = Spline.EvaluateTangent(position);

            // ---------- FAN CALC ----------
            float centerIndex = (handSize - 1) * 0.5f;
            float normalizedIndex = centerIndex == 0 ? 0 : (i - centerIndex) / centerIndex;

            // FIX: small, readable fan
            float maxFanAngle = 15f;
            float fanAngle = normalizedIndex * maxFanAngle;

            // ---------- ROTATION ----------

            // Use spline direction ONLY for yaw (horizontal)
            Vector3 flatForward = new Vector3(Forward.x, 0f, Forward.z).normalized;
            if (flatForward.sqrMagnitude < 0.001f)
                flatForward = Vector3.forward;

            // Base rotation (yaw only)
            Quaternion yawRotation = Quaternion.LookRotation(flatForward, Vector3.up);

            // Mesh correction (card faces +X)
            Quaternion meshCorrection = Quaternion.Euler(0f, -90f, 0f);

            // Fan roll
            Quaternion fanRotation = Quaternion.Euler(0f, 0f, fanAngle);

            // Final rotation
            Quaternion rotation = yawRotation * meshCorrection * fanRotation;

            // Add the calculated position to the card positions list
            cardPositions.Add(splinePosition);

            // Add the calculated rotation to the card rotations list
            cardRotations.Add(rotation);

            // Moving the card to the position smoothly
            cardsInHand[i].MoveCardToPoint(splinePosition, rotation);

            // hold the cart in the moment
            cardsInHand[i].inHand = true;
            cardsInHand[i].handPosition = i;
        }
    }


    /**
     * This will be disabling the colliders of all the cards in hand except the active card
     */
    public void DisableAllCardCollidersExcept(Card activeCard)
    {
        foreach (var card in cardsInHand)
        {
            if (card != activeCard)
                card.SetColliderEnabled(false);
        }
    }

    /**
     * This will be enabling the colliders of all the cards in hand
     */
    public void EnableAllCardColliders()
    {
        foreach (var card in cardsInHand)
        {
            card.SetColliderEnabled(true);
        }
    }




}
