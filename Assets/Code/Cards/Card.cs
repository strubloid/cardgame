using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
    public int currentHealth, attackPower, manaCost, shieldValue;

    // Reference to the text components that we will be using it
    public TMP_Text healthText, attackText, manaText, shieldText;
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
    private Collider CardCollider;

    // Layer mask to identify the desktop area
    public LayerMask whatIsDesktop, whatIsPlacement;

    // Boolean to check if the card was just pressed
    private bool justPressed;

    // this will store the asigned place point
    public CardPlacePoint assignedPlace;

    // Default deck position
    public Vector3 defaultDeckPosition;

    // Default deck rotation
    public Quaternion defaultDeckRotation;
    public bool hasDefaultDeckPosition;

    // To know when we are hovering an enemy card
    private bool enemyHoverActive;

    // Time to destroy a card after being moved to discard pile
    private float TimeToDestroyACard = 1.6f;
    private float TimeToHide = 0.9f;

    // Reference to the animator component
    public Animator animator;

    // Reference to the dead card effect animator
    public GameObject DeadEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Failsafe in case the main target point is not set
        if (targetPoint == Vector3.zero) {
            targetPoint = transform.position;
            targetRotation = transform.rotation;
        } 

        SetupCard();



        // getting what is the controller of the hand, if the card is a player one will be the player hand controller
        // otherwise the enemy hand controller
        if (isPlayer)
        {
            handController = Object.FindFirstObjectByType<PlayerHandController>();
        }
        else {             
            handController = Object.FindFirstObjectByType<EnemyHandController>();
        }

        // Getting the collider component of the card
        CardCollider = GetComponent<Collider>();
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

        // Initial update of the card display
        UpdateCardDisplay();

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

        // doing nothing when we are paused the game (the timescale == 0f means the game is paused)
        if (Time.timeScale == 0f)
        {
            return;
        }

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
        // if the battle has ended, no actions
        if (BattleController.instance.battleEnded == true)
        {
            return;
        }

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

            // rule to check if we are at our turn and we can place the card
            bool areYouAtYourTurn = BattleController.instance.currentPhrase == BattleController.TurnOrder.PlayerTurn;
            bool isItAPlacement = Physics.Raycast(ray, out hit, 100f, whatIsPlacement);

            // Must be a placement point and must be our turn
            if (!isItAPlacement || !areYouAtYourTurn)
            {
                ReturnToHand();
                return;
            }

            // Getting the component that we are pointing at
            CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

            // Must be a player placement point and must be empty
            if (!selectedPoint.isPlayerPoint || selectedPoint.activeCard != null)
            {
                ReturnToHand();
                return;
            }

            // Must have enough mana
            if (BattleController.instance.playerMana < manaCost)
            {
                ReturnToHand();
                UiController.instance.ShowManaWarning();
                return;
            }

            selectedPoint.activeCard = this;
            assignedPlace = selectedPoint;

            // here we will be assinging the default deck position to be the place point position
            // If no deck position assigned, use current as default
            if (!hasDefaultDeckPosition)
            {
                defaultDeckPosition = selectedPoint.transform.position;
                defaultDeckRotation = selectedPoint.transform.rotation;
                hasDefaultDeckPosition = true;
            }
            
            // Action of moving the card to the selected point
            MoveCardToPoint(selectedPoint.transform.position, Quaternion.identity);

            // after placing the card, enabling the collider again
            CardCollider.enabled = true;

            // Playing the card place sound effect
            AudioManager.instance.PlayCardPlace();

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
        // Doing nothing when we are paused the game (the timescale == 0f means the game is paused)
        if (Time.timeScale == 0f)
        {
            return;
        }

        // if the battle has ended, no hover actions
        if (BattleController.instance.battleEnded == true)
        {
            return;
        }

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

        // rotation basic when a card is back to the center
        Quaternion finalRotation = Quaternion.Euler(0f, 0f, 0f);

        // point to move to when hovering
        Vector3 pointToMoveTo = defaultDeckPosition + Vector3.up * enemyHoverLiftY;

        // moving to the point
        MoveCardToPoint(pointToMoveTo, finalRotation);

    }

    /**
     * This will be called when the mouse hover exits the card
     */
    protected override void OnHoverExit()
    {
        // if the battle has ended, no hover actions
        if (BattleController.instance.battleEnded == true) {
            return;
        }

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
            Quaternion finalRotation = defaultDeckRotation;
            Vector3 pointToMoveTo = defaultDeckPosition;

            MoveCardToPoint(pointToMoveTo, finalRotation);

        } finally {
            assignedPlace = null;
        }
    }

    /**
     * This will be called when the mouse button is pressed down on the card
     */
    protected override void OnMouseDown()
    {

        // Doing nothing when we are paused the game (the timescale == 0f means the game is paused)
        if (Time.timeScale == 0f)
        {
            return;
        }

        // if the battle has ended, no hover actions
        if (BattleController.instance.battleEnded == true)
        {
            return;
        }

        // Doing actions when we click with right button
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log("Right click detected, ignoring hover on player card");
            return;
        }

        // This will check if the card is in hand
        if (inHand && BattleController.instance.currentPhrase == BattleController.TurnOrder.PlayerTurn && isPlayer)
        {
            // Mark this as the selected card (the one being dragged/played)
            SelectedCard = this;

            isSelected = true;

            // Disabling the collider while we are dragging the card
            CardCollider.enabled = false;

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

        CardCollider.enabled = true;
        MoveCardToPoint(handController.cardPositions[handPosition], handController.minPos.rotation);
    }

    /**
     * This will be triggering the dead card effect
     */
    public void DamageDeadCardEffect()
    {
        StartCoroutine(DamageDeadCardEffectCo());
    }

    IEnumerator DamageDeadCardEffectCo() 
    {
        // Trigger the Jump animation while being destroyed
        animator.SetTrigger("Hide");

        // waiting for a second before showing the dead effect
        yield return new WaitForSeconds(TimeToHide);

        // turning on the dead card effect
        DeadEffect.SetActive(true);

        // getting the slice effect transform
        Transform SliceEffectTransform = DeadEffect.transform.Find("SliceEffect");

        // add here the code for playing the slice effect
        ParticleSystem CardSliceEffect = SliceEffectTransform.GetComponentInChildren<ParticleSystem>(true);
        CardSliceEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        CardSliceEffect.Play(true);

        // add here the code for playing the blood effect
        Transform BloodEffectTransform = DeadEffect.transform.Find("BloodEffect");
        ParticleSystem BloodEffect = BloodEffectTransform.GetComponentInChildren<ParticleSystem>(true);
        BloodEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        BloodEffect.Play(true);
    }

    /**
     * This will be damaging the card with a specific ammount
     */
    public void DamageCard(int damageAmmount) {

        // calculating the damage minus shield
        int damagedMinusShield = damageAmmount - shieldValue;

        // reducing the current health
        currentHealth -= damagedMinusShield;

        if (currentHealth <= 0)
        {

            // updating the health text
            currentHealth = 0;

            // checkinf if we can remove the active card from the place point
            if (assignedPlace != null)
            {
                assignedPlace.activeCard = null;
            }

            // This will trigger the dead effect
            DamageDeadCardEffect();

            // moving to the point
            //MoveCardToPoint(
            //    BattleController.instance.DiscardPoint.position,
            //    BattleController.instance.DiscardPoint.rotation
            //);


            // destroy the card if health is zero, will wait for 5 seconds before destroying
            Destroy(gameObject, TimeToDestroyACard);

            // Playing the card destroy sound effect
            AudioManager.instance.PlayCardDefeat();

        } else {

            // Playing the card hurt sound effect
            AudioManager.instance.PlayCardAttack();
        }
    
        // This will trigger the hurt animation
        animator.SetTrigger("Hurt");

        // updating the ui text
        UpdateCardDisplay();
    }

    /**
     * This will be updating the card display values
     */
    public void UpdateCardDisplay() {

        // Loading the shield value to be the current shield
        shieldText.text = shieldValue.ToString();

        // Loading the health of the card to be the current health
        healthText.text = currentHealth.ToString();

        // Loading the attack power configured at the card to be the current attack
        attackText.text = attackPower.ToString();

        // Loading the current mana cost to be at the mana text 
        manaText.text = manaCost.ToString();

    }
}
