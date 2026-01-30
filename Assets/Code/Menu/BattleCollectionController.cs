using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * This will be responsible for controlling the battle selection screen.
 */
public class BattleCollectionControler : MonoBehaviour
{
    // Folder where battle scenes are located
    private const string BattleFolder = "/Scenes/Battles/";

    // we will be sotring all the battle scenes available
    public List<string> AvaiableBattleScenes = new List<string>();

    // we will store the object for each line at the battle selection
    public List<GameObject> BattleLines = new List<GameObject>();

    // Current page being viewed
    public int CurrentPage = 1;

    // Maximum number of pages available
    private int MaxPages;

    // Reference to the Next and Previous Button GameObject
    public GameObject NextButton;
    public GameObject PreviousButton;

    // Number of items to display per page
    public int itemsPerPage = 6;

    // This will start with some initial setup
    private void Awake()
    {
        // loading the available battle scenes
        AvaiableBattleScenes = GetBattleScenes();

        // setting the max pages
        MaxPages = BattleLines.Count;

        // disabling the previous button at start
        DisablingButton(PreviousButton);

        // disapearing the previous button at start
        PreviousButton.SetActive(false);

        // enabling the next button at start if exist pages
        if (MaxPages > 1) {
            EnablingButton(NextButton);
        } else {
            // disapearing the next button at start
            DisablingButton(NextButton);
            NextButton.SetActive(false);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // we start enabling and disabling buttons based on current page
        EnablingDisablingButtons();
    }

    /**
     * This will enable or disable the next and previous buttons based on the current page.
     */
    private void EnablingDisablingButtons() 
    {
        // first we check we should disable the next button
        if(CurrentPage == MaxPages){
            DisablingButton(NextButton);
        } else {
            EnablingButton(NextButton);
        }

        // checking if is between pages to enable next button
        if (CurrentPage == 1) {
            DisablingButton(PreviousButton);
        } else {
            EnablingButton(PreviousButton);
        }
    }

    /**
     * This will provide a list of battles scenes that are localized at:
     * /Assets/Scenes/Battles/
     * Each file inside of this folde will be considered a battle scene.
     */
    public List<string> GetBattleScenes()
    {
        // this will hold the list of scenes found
        List<string> scenes = new List<string>();

        // Getting a total count of scenes in build settings, some of them arent battle scenes
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        // Looping through all scenes in build settings
        for (int i = 0; i < sceneCount; i++)
        {
            // Getting the path of the scene at index i
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            // Ensure scene is inside Assets/Scenes/Battles
            if (!scenePath.Contains(BattleFolder))
                continue;

            // Extracting the scene name from the path
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            // adding the scene name to the list
            scenes.Add(sceneName);
        }

        // we will be getting all the battle scenes found
        return scenes;
    }

    /**
     * This will disable the button provided.
     */
    private void DisablingButton(GameObject ButtonToDisable)
    {
        // getting the button component
        Button Button = ButtonToDisable.GetComponent<Button>();

        // disabling the button
        Button.interactable = false;
    }

    /**
     * This will enable the button provided.
     */
    private void EnablingButton(GameObject ButtonToEnable)
    {
        // we also active the button if wasnt able to be seen
        ButtonToEnable.SetActive(true);

        // getting the button component
        Button Button = ButtonToEnable.GetComponent<Button>();

        // disabling the button
        Button.interactable = true;
    }

    /**
     * This will turn on only the line provided and turn off all others.
     */
    private void TurningOnLine(int LineToTurnOn)
    {
        // adjusting for zero index
        int indexToTurnOn = LineToTurnOn - 1;

        // we loop through all the battle lines
        for (int line = 0; line < BattleLines.Count; line++)
        {
            // we turn off all lines
            BattleLines[line].SetActive(false);

            // and we turn on only one line
            if (line == indexToTurnOn)
            {
                BattleLines[line].SetActive(true);
            }
        }
    }


    /**
     * This will be called when the player selects a battle to play.
     */
    public void NextPage()
    {
        // we check if is the last page so we can loop back to the start
        if (CurrentPage >= MaxPages)
        {
            DisablingButton(NextButton);
            return;
        }

        // We increment the current page
        CurrentPage++;

        // turning on the current line
        TurningOnLine(CurrentPage);

        // Play a sound effect to indicate a battle has been selected
        AudioManager.instance.PlayButtonPress();
    }

    /**
     * This will be called when the player selects to go back a page.
     */
    public void PreviousPage()
    {

        // checking if is the first page to disable previous button
        if (CurrentPage == 1)
        {
            DisablingButton(PreviousButton);
            return;
        }

        // We decrement the current page
        CurrentPage--;

        // We Enable the previous button
        EnablingButton(PreviousButton);

        // Turning on the current line
        TurningOnLine(CurrentPage);

        // Play a sound effect to indicate a battle has been selected
        AudioManager.instance.PlayButtonPress();
    }
}
