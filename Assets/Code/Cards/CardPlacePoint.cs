using UnityEngine;
using UnityEngine.XR;

public class CardPlacePoint : MonoBehaviourWithMouseControls
{
    // The active card assigned to this place point
    public Card activeCard;

    // Boolean to check if this is for the player
    public bool isPlayerPoint;

    // getting the sprite renderer for being able to change the frame color
    private SpriteRenderer spriteRenderer;

    // Reference to the hand controller
    public HandController HandController;


    // Frame colors for selection indication
    public Color BaseColor = new Color32(0xAA, 0xAC, 0xFF, 0xFF); // #AAACFF
    public Color EnemyBaseColor = new Color32(0xC0, 0x58, 0x58, 0xFF); // #C05858

    public Color FrameBaseColor = new Color32(0xAA, 0xAC, 0xFF, 0xFF); // #AAACFF
    public Color FrameSelectedColor = new Color32(0xDE, 0xAD, 0x5D, 0xFF); // #DEAD5D
    public Color ErrorSelectionColor = new Color32(0xFF, 0x5A, 0x5A, 0xFF); // #FF5A5A

    // Element colors
    public Color AirElementColor = new Color32(0x9F, 0xE7, 0xFF, 0xFF); // #9FE7FF – light sky / wind
    public Color FireElementColor = new Color32(0xFF, 0x7A, 0x3C, 0xFF); // #FF7A3C – ember orange
    public Color EarthElementColor = new Color32(0x7A, 0x9B, 0x4E, 0xFF); // #7A9B4E – moss / soil
    public Color WaterElementColor = new Color32(0x3C, 0x8D, 0xFF, 0xFF); // #3C8DFF – deep water blue

    // Particle system for hover effect
    public ParticleSystem HoverEffectAnimator;

    // Reference to the father object
    public GameObject father;

    /**
     * Awake is called when the script instance is being loaded
     * so we can initialize the sprite renderer and set the base color
     **/
    private void Awake()
    {
        // Finding the hand controller in the scene
        HandController = Object.FindFirstObjectByType<HandController>();

        // Initialize the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the initial base color
        if (spriteRenderer != null)
        {
            // Set the initial color based on whether it's a player point or enemy point
            if (isPlayerPoint) {
                spriteRenderer.color = FrameBaseColor;
            } else {
                spriteRenderer.color = EnemyBaseColor;
            }
        }
    }

    /**
     * This will show if the player has a card in hand
     */
    private bool PlayerHasCardInHand()
    {
        return HandController != null && HandController.cardsInHand.Count > 0;
    }


    /**
     * This will show if there is already a card placed in this point 
     */
    private bool HasCardAlreadyPlaced()
    {
        return activeCard != null;
    }

    /**
     * This will show if the player has a card selected and is in hand
     */
    private bool PlayerHasACardInHands()
    {
        return Card.SelectedCard != null && Card.SelectedCard.inHand;
    }

    /**
     * This will change the frame color to the earth element color
     */
    public void ChangeToElementFireColor() {

        // we set the base color to fire element color
        BaseColor = FireElementColor;

        // we change the sprite renderer color to base color
        spriteRenderer.color = BaseColor;
    }

    /**
     * This will change the frame color to the earth element color
     */
    public void ChangeToElementWaterColor() {

        // we set the base color to water element color
        FrameBaseColor = WaterElementColor;

        // we change the sprite renderer color to base color
        spriteRenderer.color = FrameBaseColor;
    }

    /**
     * This will change the frame color to the earth element color
     */
    public void ChangeToElementEarthColor() {

        // we set the base color to earth element color
        FrameBaseColor = EarthElementColor;

        // we change the sprite renderer color to base color
        spriteRenderer.color = FrameBaseColor;
    }

    /**
     * This will change the frame color to the air element color
     */
    public void ChangeToElementAirColor() {

        // we set the base color to air element color
        FrameBaseColor = AirElementColor;

        // we change the sprite renderer color to base color
        spriteRenderer.color = FrameBaseColor;
    }

    /**
     * This will change the frame color to the fire base color
     */
    public void ChangeToFrameBaseColorColor()
    {
        // we set the base color to frame base color
        FrameBaseColor = BaseColor;

        // we set the base color to base color
        spriteRenderer.color = FrameBaseColor;
    }
    
    /**
     * This will be called when the mouse hover enters the card
     */
    protected override void OnHoverEnter()
    {

        // Action only for the player point
        if (isPlayerPoint && PlayerHasCardInHand() && PlayerHasACardInHands() && spriteRenderer != null)
        {
            OnHoverEnterPlayer();
        }
    }

    /**
     * This will be called when the mouse hover enters the card for the player point
     */
    private void OnHoverEnterPlayer() 
    {
        // Change to selected color when is hovering
        if (!HasCardAlreadyPlaced())
        {
            // We change the sprite renderer color to selected color
            spriteRenderer.color = FrameSelectedColor;

            // we play the hover effect
            HoverEffectAnimator.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            HoverEffectAnimator.Play(true);

        } else  {
            spriteRenderer.color = ErrorSelectionColor;
        }
    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        // just changed for the player point for now
        if (isPlayerPoint && spriteRenderer != null)
        {
            // we check if the current frame contain another color, if doesn't we reset to base color
            if (FrameBaseColor == BaseColor) {
                spriteRenderer.color = FrameBaseColor;
            }

            // we stop the hover effect
            HoverEffectAnimator.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

    }

}
