using UnityEngine;
using TMPro;

/**
 * Class that will be adding the rules of a card
 */
public class Card : MonoBehaviour
{
    // Basic Integer values of a card
    public int currentHealth, attackPower, manaCost;

    public TMP_Text healthText, attackText, manaText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // loading the health of the card to be the current health
        healthText.text = currentHealth.ToString();
        
        // loading the attack power configured at the card to be the current attack
        attackText.text = attackPower.ToString();

        // loading the current mana cost to be at the mana text 
        manaText.text = manaCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
