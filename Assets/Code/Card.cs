using UnityEngine;
using TMPro;

/**
 * Class that will be adding the rules of a card
 */
public class Card : MonoBehaviour
{
    // This will be the reference to the scriptable object of the card
    public CardScriptableObject cardData;

    // Basic Integer values of a card
    public int currentHealth, attackPower, manaCost;

    public TMP_Text healthText, attackText, manaText;
    public TMP_Text nameText, actionDescriptionText, loreText;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
