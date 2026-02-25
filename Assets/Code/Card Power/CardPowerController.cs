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
    public void ActivatePowerAnimation(Vector3 destinationPosition)
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

        // Stop previous animation if running
        if (activePowerRoutine != null)
            StopCoroutine(activePowerRoutine);

        // Clean up previous particle instance
        if (currentParticleInstance != null)
            Destroy(currentParticleInstance);

        // Instantiate the particle system for this power type
        if (powerData.particleSystemPrefab != null)
        {
            currentParticleInstance = Instantiate(powerData.particleSystemPrefab, transform);
            currentPowerParticle = currentParticleInstance.GetComponent<ParticleSystem>();

            if (currentPowerParticle != null)
            {
                activePowerRoutine = StartCoroutine(
                    MovePowerParticle(currentPowerParticle, destinationPosition, powerData.impactEffectPrefab, powerData.travelTime, powerData.lingerTime)
                );
            }
            else
            {
                Debug.LogWarning("CardPowerController: Instantiated prefab does not have a ParticleSystem component!");
            }
        }
        else
        {
            Debug.LogWarning("CardPowerController: PowerData has no particle system prefab assigned!");
        }
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

        // Store the starting position of the particle system
        Vector3 startPosition = powerParticle.transform.position;
        float elapsed = 0f;

        // Move the particle system towards the destination over the travel time
        while (elapsed < travelTime)
        {
            // Increment the elapsed time
            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;

            // Moving the particle using Lerp for smooth movement
            powerParticle.transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        // Ensure the particle system is exactly at the destination after the loop
        powerParticle.transform.position = destination;

        // Instantiate impact effect at destination if provided
        GameObject impactInstance = null;
        if (impactEffectPrefab != null)
        {
            impactInstance = Instantiate(impactEffectPrefab, destination, Quaternion.identity);
            Debug.Log($"CardPowerController: Impact effect instantiated at {destination}");
        }
        else
        {
            Debug.LogWarning("CardPowerController: No impact effect prefab assigned for this power type");
        }

        // waiting for a bit
        yield return new WaitForSeconds(lingerTime);

        // Stop the particle system and clear it to hide the effect
        powerParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // Clear the particle system to reset it for the next activation
        powerParticle.Clear();

        // Clean up impact effect
        if (impactInstance != null)
        {
            Destroy(impactInstance);
        }

        // Reset the active power routine reference
        activePowerRoutine = null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}