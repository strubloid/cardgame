using System.Collections;
using UnityEngine;
/**
 * This class is responsible for controlling the card powers, it will handle the activation and deactivation of the powers, as well as the cooldowns and durations of the powers.
 * It will also handle the visual effects of the powers, such as the particle effects and the animations.
 * It will also handle the interactions with the cards, such as when a card is played or when a card is destroyed.
 */
public class CardPowerController : MonoBehaviour
{
    // Reference to the card this power controller belongs to
    private Card card;

    // Current active particle system instance
    private ParticleSystem currentPowerParticle;
    private GameObject currentParticleInstance;

    // Coroutine reference for the active power animation, this will be used to stop the animation if a new one is triggered before the previous one finishes
    private Coroutine activePowerRoutine;

    // Target card for damage
    private Card targetCard;
    private int damageAmount;

    // Event for base damage (when no target card)
    public event System.Action<int> OnBaseDamageOnImpact;

    /**
    * Awake is called when the script instance is being loaded
    */
    private void Awake()
    {
        // Get the card component from parent
        card = GetComponentInParent<Card>();

        if (card == null)
        {
            Debug.LogError("CardPowerController: Could not find Card component in parent!");
        }
    }

    // Activate power animation for this card's type
    public void ActivatePowerAnimation(Vector3 destinationPosition, Card target, int damage)
    {
        if (card == null || card.cardData == null)
        {
            Debug.LogError("CardPowerController: Card or cardData is null!");
            return;
        }

        // Get the power data for this card type (creates PowerManager if needed)
        PowerData powerData = PowerManager.GetPowerDataForType(card.cardData.cardType);
        if (powerData == null)
        {
            Debug.LogWarning($"CardPowerController: No PowerData found for {card.cardData.cardType}");
            return;
        }

        // Validate particle system prefab exists
        if (powerData.particleSystemPrefab == null)
        {
            Debug.LogWarning("CardPowerController: PowerData has no particle system prefab assigned!");
            return;
        }

        // Stop previous animation if running
        if (activePowerRoutine != null)
            StopCoroutine(activePowerRoutine);

        // Clean up previous particle instance
        if (currentParticleInstance != null)
            Destroy(currentParticleInstance);

        // Instantiate and get particle system
        currentParticleInstance = Instantiate(powerData.particleSystemPrefab, transform);
        currentPowerParticle = currentParticleInstance.GetComponentInChildren<ParticleSystem>();

        if (currentPowerParticle == null)
        {
            Debug.LogWarning("CardPowerController: Instantiated prefab does not have a ParticleSystem component!");
            return;
        }

        // Store target for damage
        targetCard = target;
        damageAmount = damage;

        // Start animation
        activePowerRoutine = StartCoroutine(
            MovePowerParticle(currentPowerParticle, destinationPosition, powerData.impactEffectPrefab, powerData.travelTime, powerData.lingerTime)
        );
    }

    /**
     * Moves the particle prefab from start to destination over the specified travel time
     */
    private IEnumerator MoveToDestination(Vector3 startPosition, Vector3 destination, float travelTime)
    {
        float elapsed = 0f;
        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;
            currentParticleInstance.transform.position = Vector3.Lerp(startPosition, destination, t);
            yield return null;
        }
        currentParticleInstance.transform.position = destination;
    }

    /**
     * This coroutine moves the power particle from its current position to the destination position over a specified travel time, then allows it to linger for a specified time before stopping the particle effect and hiding the prefab.
     */
    IEnumerator MovePowerParticle(ParticleSystem powerParticle, Vector3 destination, GameObject impactEffectPrefab, float travelTime, float lingerTime)
    {
        if (powerParticle == null)
        {
            Debug.LogError("CardPowerController: ParticleSystem is null in MovePowerParticle!");
            activePowerRoutine = null;
            yield break;
        }

        // Restart the particle system to ensure it plays from the beginning
        powerParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        powerParticle.Play(true);

        // Move the entire prefab towards the destination over the travel time
        Vector3 startPosition = currentParticleInstance.transform.position;
        yield return StartCoroutine(MoveToDestination(startPosition, destination, travelTime));

        // Apply damage on impact
        if (targetCard != null) {
            targetCard.DamageCard(damageAmount); // Card damage - triggers OnDamageTaken event
        } else if (damageAmount > 0) {
            // Base damage - triggers event for BattleController to handle
            OnBaseDamageOnImpact?.Invoke(damageAmount);
        }

        // Instantiate impact effect if provided
        GameObject impactInstance = null;
        if (impactEffectPrefab != null) {
            impactInstance = Instantiate(impactEffectPrefab, destination, Quaternion.identity);
        } else {
            Debug.LogWarning("CardPowerController: No impact effect prefab assigned for this power type");
        }

        // waiting for a bit
        yield return new WaitForSeconds(lingerTime);

        // Cleanup
        if (powerParticle != null)
        {
            powerParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            powerParticle.Clear();
        }

        // Destroy the impact effect instance if it was created
        if (impactInstance != null) Destroy(impactInstance);
        if (currentParticleInstance != null) Destroy(currentParticleInstance);
        
        // Reset references
        currentParticleInstance = null;
        activePowerRoutine = null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}