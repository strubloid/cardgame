using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/**
 * Class that will be adding the rules of a card
 */
public class Card : MonoBehaviourWithMouseControls
{
    // Singleton-ish reference to the currently selected card (the one being dragged/played)
    public static Card SelectedCard { get; private set; }

    // Boolean to check if the card belongs to the player or the enemy
    public bool isPlayer;

    // This will be the reference to the scriptable object of the card
    public CardScriptableObject cardData;

    // Basic Integer values of a card
    public int currentHealth, attackPower, manaCost;

    // Reference to the text components that we will be using it
    public TMP_Text healthText, attackText, manaText;
    public TMP_Text nameText, actionDescriptionText, loreText;

    // Reference to the images that we will be using it
    public Image characterImage, backgroundImage;

    // Target point for the card movement
    private Vector3 targetPoint;

    // Speed of the card movement
    public float moveSpeed = 5f, rotateSpeed = 540f;

    // Rotation of the card
    public Quaternion targetRotation;

    // Boolean to check if the card is in hand
    public bool inHand;

    // Position of the card in hand
    public int handPosition;

    // Reference to the hand controller
    private HandController handController;

    // Boolean to check if the card is selected
    private bool isSelected;

    // Reference to the collider of the card
    private Collider theCollider;

    // Layer mask to identify the desktop area
    public LayerMask whatIsDesktop, whatIsPlacement;

    // Boolean to check if the card was just pressed
    private bool justPressed;

    // this will store the asigned place point
    public CardPlacePoint assignedPlace;

    // Default deck position
    public Vector3 defaultDeckPosition;

    // Default deck rotation
    private Quaternion defaultDeckRotation;
    private bool hasDefaultDeckPosition;

    // To know when we are hovering an enemy card
    private bool enemyHoverActive;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Failsafe in case the main target point is not set
        if (targetPoint == Vector3.zero) {
            targetPoint = transform.position;
            targetRotation = transform.rotation;
        } 

        SetupCard();

        // Finding the hand controller in the scene
        handController = Object.FindFirstObjectByType<HandController>();

        // Getting the collider component of the card
        theCollider = GetComponent<Collider>();

        // If no deck position assigned, use current as default
        if (!hasDefaultDeckPosition)
        {
            defaultDeckPosition = transform.position;
            defaultDeckRotation = transform.rotation;
            hasDefaultDeckPosition = true;
        }
    }

    /**
     * Function that will be setting up the card values
     */
    public void SetupCard()
    {
        // Setting up the basic values of the card
        currentHealth = cardData.currentHealth;
        attackPower = cardData.attackPower;
        manaCost = cardData.manaCost;

        // Loading the health of the card to be the current health
        healthText.text = currentHealth.ToString();

        // Loading the attack power configured at the card to be the current attack
        attackText.text = attackPower.ToString();

        // Loading the current mana cost to be at the mana text 
        manaText.text = manaCost.ToString();

        // Loading the text values from the scriptable object to the card
        nameText.text = cardData.cardName;
        actionDescriptionText.text = cardData.actionDescription;
        loreText.text = cardData.cardLore;

        // Getting the images from the scriptable object to the card
        characterImage.sprite = cardData.characterSprite;

        // Getting the background image from the scriptable object to the card
        backgroundImage.sprite = cardData.backgroundSprite;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // Moving the card to the target point
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Rotating the card to the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // Checking if the card is selected
        if (isSelected)
        {
            WithCardOnTheHandAction();
        }
        else
        {
            // When not selected and not in hand, keep it at the deck default position
            if (!inHand && assignedPlace == null && hasDefaultDeckPosition && isPlayer)
            {
                MoveCardToPoint(defaultDeckPosition, defaultDeckRotation);                
            }
        }

        // Resetting the just pressed boolean
        justPressed = false;
    }

    /**
     * This will be the action when the card is selected
     */
    private void WithCardOnTheHandAction()
    {
        // Creating a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        // Variable to store the raycast hit information
        RaycastHit hit;

        // Moving a card if we hit the desktop area
        if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
        {
            MoveCardToPoint(hit.point + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
        }

        // Returning the card to hand if we right click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            ReturnToHand();
            return;
        }

        // This will allow if we press the left mouse button and isnt the just pressed
        if (Mouse.current.leftButton.wasPressedThisFrame && justPressed == false ) {

            // if we click and iteract with what is placement, we hit one of those card place points
            if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement) && BattleController.instance.currentPhrase == BattleController.TurnOrder.PlayerTurn )
            {
                CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

                // there is nothing assined to the current card
                // select card section!
                if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
                {
                    // Check if we can play the card, if we have the quantity of mana to cast
                    if (BattleController.instance.playerMana >= manaCost)
                    {
                        selectedPoint.activeCard = this;
                        assignedPlace = selectedPoint;

                        // We move to the point
                        MoveCardToPoint(selectedPoint.transform.position, Quaternion.identity);

                        // reset the in hand as it was placed and isnt selected as it is in place
                        inHand = false;
                        isSelected = false;

                        // Clear the current selected card reference (no longer in hand)
                        if (SelectedCard == this)
                            SelectedCard = null;

                        // Removing the card from the array
                        handController.RemoveCardFromHand(this);

                        // removing the player mana after playing the card
                        BattleController.instance.SpendPlayerMana(manaCost);
                    } else {
                        ReturnToHand();
                        UiController.instance.ShowManaWarning();
                    }
                } else {
                    ReturnToHand();
                }
            } else {
                ReturnToHand();
            }
        }
    }

    /**
     * Function that will be moving the card to a specific point
     */
    public void MoveCardToPoint(Vector3 pointToMoveTo, Quaternion rotationToMatch)
    {
        // This will set the point to move to
        targetPoint = pointToMoveTo;

        // This will set the rotation while we are moving the card
        targetRotation = rotationToMatch;
    }

    /**
     * This will be called when the mouse hover enters the card
     */
    protected override void OnHoverEnter()
    {
        // player card hands action
        if (inHand && isPlayer) {
            onHoverEnterPlayer();
        }

        // enemy card hover action
        if (!inHand && !isPlayer) {
            onHoverEnterEnemy();
        }
    }

    /**
     * This will be called when the mouse hover enters a player card
     */
    public void onHoverEnterPlayer() {
        MoveCardToPoint(
            handController.cardPositions[handPosition] + new Vector3(0f, 0.5f, 0.5f),
            Quaternion.identity
        );
    }

    /**
     * This will be called when the mouse hover enters an enemy card
     */
    public void onHoverEnterEnemy()
    {
        float enemyHoverLiftY = 1.6f;
        enemyHoverActive = true;
        MoveCardToPoint(
         defaultDeckPosition + Vector3.up * enemyHoverLiftY,
         Quaternion.identity
        );
    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        // player card hands action
        if (inHand && isPlayer)
        {
            onHoverExitPlayer();
        }

        // enemy card hover action
        if (!inHand && !isPlayer)
        {
            onHoverExitEnemy();
        }
    }

    /**
     * This will be called when the mouse hover exits a player card
     */
    public void onHoverExitPlayer()
    {
        MoveCardToPoint(
            handController.cardPositions[handPosition],
            handController.minPos.rotation
        );
    }

    /**
     * This will be called when the mouse hover exits an enemy card
     */
    public void onHoverExitEnemy()
    {
        try 
        {
            if (!enemyHoverActive) {
                return;
            }
            // reset hover state
            enemyHoverActive = false;

            MoveCardToPoint(defaultDeckPosition, defaultDeckRotation);

        } finally {
            assignedPlace = null;
        }
    }

    /**
     * This will be called when the mouse button is pressed down on the card
     */
    protected override void OnMouseDown()
    {
        // This will check if the card is in hand
        if (inHand && BattleController.instance.currentPhrase == BattleController.TurnOrder.PlayerTurn && isPlayer)
        {
            // Mark this as the selected card (the one being dragged/played)
            SelectedCard = this;

            isSelected = true;

            // Disabling the collider while we are dragging the card
            theCollider.enabled = false;

            justPressed = true;
        }
    }

    /**
     * Function that will be returning the card to hand
     */
    public void ReturnToHand()
    {
        isSelected = false;

        // If this was the selected card, clear it
        if (SelectedCard == this)
            SelectedCard = null;

        theCollider.enabled = true;
        MoveCardToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);
    }
}
