using System.Collections;
using UnityEngine;
/**
 * This class is responsible for controlling the card powers, it will handle the activation and deactivation of the powers, as well as the cooldowns and durations of the powers.
 * It will also handle the visual effects of the powers, such as the particle effects and the animations.
 * It will also handle the interactions with the cards, such as when a card is played or when a card is destroyed.
 */
public class CardPowerController : MonoBehaviour
{
    // Effect prefab for the power, this will be used to instantiate the visual effects of the powers
    public GameObject PowerEffectPrefab;

    // Particle system for the power, this will be used to play the particle effects of the powers
    public ParticleSystem PowerParticle;
    
    // Coroutine reference for the active power animation, this will be used to stop the animation if a new one is triggered before the previous one finishes
    private Coroutine activePowerRoutine;

    /**
    * Awake is called when the script instance is being loaded
    */
    private void Awake()
    {
        // stoping the particle system and hiding the prefab at the start of the game
        PowerParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

    }

    // add destination for the particle system to be the card itself, so that it will follow the card when it moves
    public void ActivatePowerAnimation( Vector3 destinationPosition, float travelTime = 0.5f, float lingerTime = 0.2f)
    {
        if (activePowerRoutine != null)
            StopCoroutine(activePowerRoutine);

        activePowerRoutine = StartCoroutine(
            MovePowerParticle(destinationPosition, travelTime, lingerTime)
        );
    }

    /**
     * This coroutine moves the power particle from its current position to the destination position over a specified travel time, then allows it to linger for a specified time before stopping the particle effect and hiding the prefab.
     */
    IEnumerator MovePowerParticle( Vector3 destination, float travelTime, float lingerTime)
    {
        // Restart the particle system to ensure it plays from the beginning
        PowerParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        PowerParticle.Play(true);

        // Store the starting position of the particle system
        Vector3 startPosition = PowerParticle.transform.position;
        float elapsed = 0f;

        // Move the particle system towards the destination over the travel time
        while (elapsed < travelTime)
        {
            // Increment the elapsed time
            elapsed += Time.deltaTime;
            float t = elapsed / travelTime;

            // Moving the particle using Lerp for smooth movement
            PowerParticle.transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return null;
        }

        // Ensure the particle system is exactly at the destination after the loop
        PowerParticle.transform.position = destination;

        // waiting for a bit
        yield return new WaitForSeconds(lingerTime);

        // Stop the particle system and clear it to hide the effect
        PowerParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // Clear the particle system to reset it for the next activation
        PowerParticle.Clear();

        // Reset the active power routine reference
        activePowerRoutine = null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}