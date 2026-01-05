using UnityEngine;

public class HandController : MonoBehaviour
{
    // Cards array that the player is holding
    public Card[] heldCards;

    // Min and max position of the hand
    public Transform minPos, maxPos;

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
    
    }
}
