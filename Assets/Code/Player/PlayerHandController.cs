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
        if (handSize == 0) {
            return;
        }

        // Calculate the spacing between cards based on the max hand size
        float CardSpacing = 1.0f / MaxHandSize;

        // This is the center position of the hand span (0.5 means the middle)
        float CenterPosition = 0.5f;

        // Calculate the position of the first card so that the hand is centered around the middle
        float FirstCardPosition = CenterPosition - (handSize - 1) * CardSpacing /2 ;

        // Getting the spline from the spline container
        Spline Spline = SplineContainer.Spline;

        // loop that will be setting the card positions in the hand
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            // getting from the left to the right
            float position = GetPositionFromLeftToRight(FirstCardPosition, CardSpacing, i, handSize);

            // Evaluate the position on the spline for the current card
            Vector3 SplinePosition = Spline.EvaluatePosition(position);
            Vector3 Forward = Spline.EvaluateTangent(position);
            Vector3 Up = Spline.EvaluateUpVector(position);

            // Base rotation aligned to the spline
            Quaternion splineRotation = Quaternion.LookRotation(Up, Vector3.Cross(Up, Forward).normalized);

            // Extra tilt (15 degrees)
            Quaternion tiltRotation = Quaternion.Euler(0f, 0f, 15f);

            // Final rotation
            Quaternion Rotation = splineRotation * tiltRotation;

            // Add the calculated position to the card positions list
            cardPositions.Add(SplinePosition);

            // Add the calculated rotation to the card rotations list
            cardRotations.Add(Rotation);

            // Moving the card to the position smoothly
            cardsInHand[i].MoveCardToPoint(SplinePosition, Rotation);

            // hold the cart in the moment
            cardsInHand[i].inHand = true;
            cardsInHand[i].handPosition = i;
        }
    }

}
