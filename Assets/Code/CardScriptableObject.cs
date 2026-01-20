using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Objects/Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    // current values of health, power and mana cost
    public int currentHealth, attackPower, manaCost, shieldValue;

    // name of the card value
    public string cardName;

    // Description is what is the card do, lore is the extra description bellow
    [TextArea]
    public string actionDescription, cardLore;

    // Reference to the images that we will be using it
    public Sprite characterSprite, backgroundSprite;

    // This is the type of card it is
    public enum CardType
    {
        fire,
        water,
        wind,
        earth
    }

    // The type of the card
    public CardType cardType;
}
