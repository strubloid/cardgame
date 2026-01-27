using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using static CardScriptableObject;

public class CardPlacePointPlayer : MonoBehaviourWithMouseControls
{
    // List with all player card frames
    public List<GameObject> CardFrames = new List<GameObject>();

    // Dictionary to hold cards by their type
    private Dictionary<CardType, List<CardPlacePoint>> CardsByType;

    /**
     * Awake is called when the script instance is being loaded
     */
    private void Awake()
    {
        // initializing the dictionary
        CardsByType = new Dictionary<CardType, List<CardPlacePoint>>();

        // loop through all card types and initialize the lists
        foreach (CardType type in System.Enum.GetValues(typeof(CardType)))
        {
            CardsByType[type] = new List<CardPlacePoint>();
        }

    }

    /**
     * Update is called once per frame
     **/
    protected override void Update()
    {
        base.Update();

        // if we press L key, 
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame)
        {

            SetColorByMultipleType();

        }
    }

    /**
     * This method sets the color of cards based on multiple types placed
     **/
    public void SetColorByMultipleType()
    {
        List<CardPlacePoint> FireElements = GetAllFireElements();

        // we will only play if we have more than 1 card placed
        if (FireElements.Count >= 2)
        {

            // we loop all fire element cards placed
            foreach (CardPlacePoint point in FireElements)
            {
                // we change to the color of fire
                point.ChangeToElementFireColor();
            }
        }

        List<CardPlacePoint> WaterElements = GetAllWaterElements();

        // we will only play if we have more than 1 card placed
        if (WaterElements.Count >= 2)
        {
            // we loop all water element cards placed
            foreach (CardPlacePoint point in WaterElements)
            {
                // we change to the color of water
                point.ChangeToElementWaterColor();
            }
        }

        List<CardPlacePoint> EarthElements = GetAllEarthElements();

        // we will only play if we have more than 1 card placed
        if (EarthElements.Count >= 2)
        {
            // we loop all earth element cards placed
            foreach (CardPlacePoint point in EarthElements)
            {
                // we change to the color of earth
                point.ChangeToElementEarthColor();
            }
        }

        List<CardPlacePoint> WindElements = GetAllWindElements();

        // we will only play if we have more than 1 card placed
        if (WindElements.Count >= 2)
        {
            // we loop all wind element cards placed
            foreach (CardPlacePoint point in WindElements)
            {
                // we change to the color of wind
                point.ChangeToElementAirColor();
            }
        }

    }


    /**
     * This method rebuilds the CardsByType dictionary
     * to ensure it reflects the current state of placed cards.
     **/
    public void RebuildCardsByType()
    {

        Debug.Log("Rebuilding CardsByType dictionary...");

        // Clearing the previous entries
        foreach (var list in CardsByType.Values)
            list.Clear();

        // Populating the dictionary with current placed cards
        foreach (GameObject cardFrame in CardFrames)
        {
            // getting the CardPlacePoint component
            CardPlacePoint point = cardFrame.GetComponent<CardPlacePoint>();

            // checking if we have an active card
            if (point == null || point.activeCard == null)
                continue;

            // getting the type of the card
            CardType type = point.activeCard.cardData.cardType;

            // adding the card to the corresponding list
            CardsByType[type].Add(point);
            Debug.Log("ADD");
        }
    }


    /**
     *  We get all fire element cards placed 
     **/
    public List<CardPlacePoint> GetAllFireElements()
    {
        return CardsByType[CardType.fire];
    }

    /**
     *  We get all water element cards placed 
     **/
    public List<CardPlacePoint> GetAllWaterElements()
    {
        return CardsByType[CardType.water];
    }

    /**
     *  We get all earth element cards placed 
     **/
    public List<CardPlacePoint> GetAllEarthElements()
    {
        return CardsByType[CardType.earth];
    }

    /**
     *  We get all wind element cards placed 
     **/
    public List<CardPlacePoint> GetAllWindElements()
    {
        return CardsByType[CardType.wind];
    }

    /**
     * This will be called when the mouse hover enters the card
     */
    protected override void OnHoverEnter()
    {

        
    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        

    }

}
