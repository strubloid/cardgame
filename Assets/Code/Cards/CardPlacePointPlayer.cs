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

    // Interval for checking card effects
    [SerializeField] private float checkIntervalSeconds = 1.2f;

    // Timer to track time until next check
    private float timeUntilNextCheck;

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

        // updating the timer
        timeUntilNextCheck -= Time.deltaTime;

        // if the timer hasn't reached zero yet, we exit
        if (timeUntilNextCheck > 0f) return;

        // resetting the timer
        timeUntilNextCheck = checkIntervalSeconds;

        // we build the cards by type
        RebuildCardsByType();

        // Truns once every 2 seconds
        SetColorByMultipleType();
    }

    /**
     * This method sets the color of fire element cards placed
     **/
    public void SetFireElement()
    {
        List<CardPlacePoint> FireElements = GetAllFireElements();

        // We check if the minimum criteria is met
        if (FireElements.Count >= 2)
        {
            // we quickly loop all fire element cards placed
            FireElements.ForEach(point => point.ChangeToElementFireColor());
        } else {
            // if we have less means back to base color
            if (FireElements.ToArray().Length == 1)
            {
                FireElements[0].ChangeToFrameBaseColorColor();
            }
        }

    }

    /**
     * This method sets the color of wind element cards placed
     **/
    public void SetWindElement()
    {
        List<CardPlacePoint> WindElements = GetAllWindElements();
        // We check if the minimum criteria is met
        if (WindElements.Count >= 2)
        {
            // we loop all wind element cards placed
            WindElements.ForEach(point => point.ChangeToElementAirColor());
        } else {
            // specifically check if there's only one to change
            if (WindElements.ToArray().Length == 1)
            {
                WindElements[0].ChangeToFrameBaseColorColor();
            }
        }
    }

    /**
     * This method sets the color of water element cards placed
     **/
    public void SetWaterElement()
    {
        List<CardPlacePoint> WaterElements = GetAllWaterElements();
        // We check if the minimum criteria is met
        if (WaterElements.Count >= 2)
        {
            // we loop all water element cards placed
            WaterElements.ForEach(point => point.ChangeToElementWaterColor());
        } else {
            // if we have less means back to base color
            if (WaterElements.ToArray().Length == 1)
            {
                WaterElements[0].ChangeToFrameBaseColorColor();
            }
        }
    }

    /**
     * This method sets the color of earth element cards placed
     **/
    public void SetEarthElement()
    {
        List<CardPlacePoint> EarthElements = GetAllEarthElements();

        // We check if the minimum criteria is met
        if (EarthElements.Count >= 2)
        {
            // we loop all earth element cards placed
            EarthElements.ForEach(point => point.ChangeToElementEarthColor());
        } else {
            // if we have less means back to base color
            if (EarthElements.ToArray().Length == 1) { 
                EarthElements[0].ChangeToFrameBaseColorColor();
            }
        }
    }

    /**
     * This method sets the color of cards based on multiple types placed
     **/
    public void SetColorByMultipleType()
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
    public void RebuildCardsByType()
    {
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
