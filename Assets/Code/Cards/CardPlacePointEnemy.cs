using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static CardScriptableObject;

public class CardPlacePointEnemy : CardPlacePointBase
{

    /**
     * Update is called once per frame
     **/
    protected override void Update()
    {
        base.Update();
    }

    /**
     * This method sets the color of cards based on multiple types placed
     **/
    public override void SetColorByMultipleType()
    {
        // we first set fire element
        SetFireElement();

        // second is wind element
        SetWindElement();

        // third is water element
        SetWaterElement();

        // forth is the earth element
        SetEarthElement();

    }

    /**
     * This method rebuilds the CardsByType dictionary
     * to ensure it reflects the current state of placed cards.
     **/
    public override void RebuildCardsByType()
    {
        // Clearing the previous entries
        foreach (var list in CardsByType.Values)
            list.Clear();

        // Populating the dictionary with current placed cards
        foreach (GameObject cardFrame in CardFrames)
        {
            // getting the CardPlacePoint component
            CardPlacePoint CardFramePoint = cardFrame.GetComponent<CardPlacePoint>();

            // if we have an active card, we proceed
            if (CardFramePoint != null && CardFramePoint.activeCard != null)
            {
                // getting the type of the card
                CardType type = CardFramePoint.activeCard.cardData.cardType;

                // adding the card to the corresponding list
                CardsByType[type].Add(CardFramePoint);
            } else {
                // we add to frame base color
                CardFramePoint.ChangeToFrameBaseColorColor();
            }
        }
    }

}
