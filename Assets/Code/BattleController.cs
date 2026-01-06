using UnityEngine;

public class BattleController : MonoBehaviour
{
    // Singleton instance
    public static BattleController instance;

    /**
     * Awake is called when the script instance is being loaded
     */
    private void Awake()
    {
        // Ensure only one instance of BattleController exists
        if (instance == null)
        {
            instance = this;
        }
    }

    public int startMana = 4;
    public int maxMana = 12;
    public int playerMana;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMana = startMana;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * This will be spending the mana of the player, 
     * after check if is with a value bellow zero, if does, will set it to zero
     */
    public void spendPlayerMana(int ammountToSpend) {

        // remove mana spent
        playerMana -= ammountToSpend;

        // double check if the value gets below 0
        if (playerMana < 0) {
            playerMana = 0;
        }
    }

}
