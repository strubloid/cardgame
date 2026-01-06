using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/**
 * Class that will be adding the rules of a card
 */
public class Card : MonoBehaviourWithMouseControls
{
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

    // Reference to the collider of the card
    private Collider theCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupCard();

        // Finding the hand controller in the scene
        handController = Object.FindFirstObjectByType<HandController>();

        // Getting the collider component of the card
        theCollider = GetComponent<Collider>();
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
        // This will check if the card is in hand
        if (!inHand) return;

        MoveCardToPoint(
            handController.cardPositions[handPosition] + new Vector3(0f, 0.5f, 0.5f),
            Quaternion.identity
        );
    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        MoveCardToPoint(
            handController.cardPositions[handPosition],
            handController.minPos.rotation
        );
    }

    protected override void OnLeftClick()
    {
        Debug.Log("Card left click");
    }

    protected override void OnRightClick()
    {
        Debug.Log("Card right click");
    }
    protected override void OnWheelUp()
    {
        Debug.Log("Zoom card in");
    }

    protected override void OnWheelDown()
    {
        Debug.Log("Zoom card out");
    }

}