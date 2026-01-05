using UnityEngine;
using TMPro;
using UnityEngine.UI;

/**
 * Class that will be adding the rules of a card
 */
public class Card : MonoBehaviour
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
    public float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        SetupCard();
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

        // loading the health of the card to be the current health
        healthText.text = currentHealth.ToString();

        // loading the attack power configured at the card to be the current attack
        attackText.text = attackPower.ToString();

        // loading the current mana cost to be at the mana text 
        manaText.text = manaCost.ToString();

        // loading the text values from the scriptable object to the card
        nameText.text = cardData.cardName;
        actionDescriptionText.text = cardData.actionDescription;
        loreText.text = cardData.cardLore;

        // getting the images from the scriptable object to the card
        characterImage.sprite = cardData.characterSprite;

        // getting the background image from the scriptable object to the card
        backgroundImage.sprite = cardData.backgroundSprite;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
    }

    /**
     * Function that will be moving the card to a specific point
     */
    public void MoveCardToPoint(Vector3 pointToMoveTo) 
    {
        targetPoint = pointToMoveTo;
    }
}
