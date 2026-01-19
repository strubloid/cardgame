using System.Collections.Generic;
using UnityEngine;

/**
 * This will be responsible for managing all the audio relaetd actions thoughout the game
 * In here we will handle background music, sound effects, volume control, etc.
 */
public class AudioManager : MonoBehaviour
{
    // Singleton instance
    public static AudioManager instance;

    // Audio sources for different game states
    public AudioSource menuMusic;
    public AudioSource battleSelect;

    // this will store all the background music available
    public List<AudioSource> backgroundMusic = new List<AudioSource>();
    public List<AudioSource> playedBackgroundMusic = new List<AudioSource>();

    // This will store all the sound effects available
    public List<AudioSource> soundEffects = new List<AudioSource>();

    /**
     * Awake is called when the script instance is being loaded
     */
    private void Awake()
    {
        // Ensure only one instance of BattleController exists
        if (instance == null)
        {
            instance = this;

            // Make sure this object persists across scenes
            DontDestroyOnLoad(gameObject);

        } // Making sure we have one audio source
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * This will stop all the music that is currently playing
     */
    public void StopMusic()
    {
        // stoping the menu music
        menuMusic.Stop();

        // also stoping the battle select music
        battleSelect.Stop();

        // and we loop though all the background music and stop them as well
        if (backgroundMusic == null) return;

        // and we loop though all the background music and stop them as well
        foreach (var music in backgroundMusic)
        {
            if (music != null) music.Stop();
        }

        // stopping played background music as well
        foreach (var music in playedBackgroundMusic)
        {
            if (music != null) music.Stop();
        }
    }


    /**
     * This will be responsible for playing the menu music, and be sure that
     * we wont have any other music playing as well.
     */
    public void PlayMenuMusic()
    {
        // only play if not already playing
        if (menuMusic.isPlaying == false)
        {
            // ensure all other music is stopped
            StopMusic();

            // Playing the menu music
            if (menuMusic != null)
            {
                menuMusic.Play();
            }
        }            

    }

    /**
     * This will be responsible for playing the battle select music, and be sure that
     * we wont have any other music playing as well.
     */
    public void PlayBattleSelectMusic()
    {
        // only play if not already playing
        if (battleSelect.isPlaying == false) 
        {
            // ensure all other music is stopped
            StopMusic();

            // Playing the menu music
            if (battleSelect != null)
            {
                battleSelect.Play();
            }
        }
        

    }

    /**
     * This will be responsible for playing the background music, and be sure that
     * we wont have any other music playing as well.
     */
    public void PlayBackgroundMusic()
    {
        // ensure all other music is stopped
        StopMusic();

        // if we finished all background music, refill it from played list
        if (backgroundMusic.Count == 0)
        {
            backgroundMusic.AddRange(playedBackgroundMusic);
            playedBackgroundMusic.Clear();
        }

        // if still empty, nothing to play
        if (backgroundMusic.Count == 0)
            return;

        // pick a random index [0..Count-1]
        int index = Random.Range(0, backgroundMusic.Count);

        // choose the audio source
        AudioSource chosen = backgroundMusic[index];

        // move it: add to played, remove from background
        playedBackgroundMusic.Add(chosen);
        backgroundMusic.RemoveAt(index);

        // play it
        chosen.Play();
    }

    /**
     * This will be responsible for playing the requested sound effect
     */
    public void PlaySoundEffect(int soundEffectToPlay) {

        // validating the index
        if (soundEffects == null || soundEffects.Count == 0)
            return;

        // validating the index
        if (soundEffectToPlay < 0 || soundEffectToPlay >= soundEffects.Count)
        {
            return;
        }

        // getting the sound effect to play
        AudioSource sound = soundEffects[soundEffectToPlay];

        // validating the sound effect
        if (sound == null)
            return;

        //  stoping whatver sound effect is playing
        sound.Stop();

        // playing the requested sound effect
        sound.Play();

    }

}
