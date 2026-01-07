using UnityEngine;
using UnityEngine.XR;

public class CardPlacePoint : MonoBehaviourWithMouseControls
{
    public Card activeCard;
    public bool isPlayerPoint;

    // getting the sprite renderer for being able to change the frame color
    private SpriteRenderer spriteRenderer;

    // Reference to the hand controller
    private HandController handController;

    // Frame colors for selection indication
    public Color frameBaseColor = new Color32(0xAA, 0xAC, 0xFF, 0xFF); // #AAACFF
    public Color frameSelectedColor = new Color32(0xDE, 0xAD, 0x5D, 0xFF); // #DEAD5D
    public Color errorSelectionColor = new Color32(0xFF, 0x5A, 0x5A, 0xFF); // #FF5A5A

    /**
     * Awake is called when the script instance is being loaded
     * so we can initialize the sprite renderer and set the base color
     **/
    private void Awake()
    {
        // Finding the hand controller in the scene
        handController = Object.FindFirstObjectByType<HandController>();

        // just changed for the player point for now
        if (isPlayerPoint && PlayerHasCardInHand()) {
            // Initialize the sprite renderer
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set the initial frame color to base color
            if (spriteRenderer != null)
            {
                spriteRenderer.color = frameBaseColor;
            }
        }       
    }

    /**
     * This will show if the player has a card in hand
     */
    private bool PlayerHasCardInHand()
    {
        return handController != null && handController.heldCards.Count > 0;
    }

    /**
     * This will be called when the mouse hover enters the card
     */
    protected override void OnHoverEnter()
    {
        // just changed for the player point for now
        if (isPlayerPoint && PlayerHasCardInHand())
        {
            // Change to selected color when is hovering
            if (spriteRenderer != null)
            {
                spriteRenderer.color = frameSelectedColor;
            }
        }
    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        // just changed for the player point for now
        if (isPlayerPoint && PlayerHasCardInHand())
        {
            // Reset to base color when not hovering
            if (spriteRenderer != null)
            {
                spriteRenderer.color = frameBaseColor;
            }
        }
        
    }

}
