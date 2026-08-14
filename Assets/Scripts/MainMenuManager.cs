using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels (From image_fb6001.png)")]
    public GameObject albumContainer;
    public GameObject settingsContainer;
    public GameObject newGameConfirmationPanel;

    [Header("Transition Settings")]
    public Animator screenFader;
    public float fadeDuration = 1f;

    [Header("Level Data")]
    [Tooltip("The exact name of your first level scene (e.g., 'Level_1')")]
    public string firstLevelName = "Lvl1";
    
    // The PlayerPrefs key we will use to save/load the furthest level reached
    private string saveKey = "FurthestLevelReached";

    void Start()
    {
        // Ensure all popup menus are closed when the game boots up
        if (albumContainer != null) albumContainer.SetActive(false);
        if (settingsContainer != null) settingsContainer.SetActive(false);
        if (newGameConfirmationPanel != null) newGameConfirmationPanel.SetActive(false);
    }

    // ==========================================
    // MAIN BUTTON LOGIC
    // ==========================================

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (albumContainer.activeSelf || settingsContainer.activeSelf || newGameConfirmationPanel.activeSelf)
            {
                CloseAllPanels();
            }
        }
    }

    public void PlayButton()
    {
        // Get the furthest level from memory. If they've never played, it defaults to 1.
        int levelToLoad = PlayerPrefs.GetInt(saveKey, 1);
        
        // Let's assume your scenes are named "Level_1", "Level_2", etc.
        string sceneName = "Lvl" + levelToLoad;

        StartCoroutine(TransitionAndLoad(sceneName));
    }

    public void LevelSelectButton()
    {
        albumContainer.SetActive(true);
    }

    public void SettingsButton()
    {
        settingsContainer.SetActive(true);
    }

    public void QuitButton()
    {
        Debug.Log("Game Quit!");
        Application.Quit();
    }

    // ==========================================
    // NEW GAME LOGIC
    // ==========================================

    public void NewGameButton()
    {
        // Open the confirmation prompt instead of immediately wiping data
        newGameConfirmationPanel.SetActive(true);
    }

    public void ConfirmNewGame()
    {
        // Wipe all saved data (powerups, levels, etc.)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Close the panel and start Level 1
        newGameConfirmationPanel.SetActive(false);
        StartCoroutine(TransitionAndLoad(firstLevelName));
    }

    public void CancelNewGame()
    {
        // Just hide the prompt
        newGameConfirmationPanel.SetActive(false);
    }

    // ==========================================
    // HELPER METHODS
    // ==========================================

    // Use this to close the Album or Settings panels (attach to a "Back" button)
    public void CloseAllPanels()
    {
        albumContainer.SetActive(false);
        settingsContainer.SetActive(false);
        newGameConfirmationPanel.SetActive(false);
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        // Trigger the fade-to-black animation
        if (screenFader != null)
        {
            screenFader.SetTrigger("FadeOut"); // Make sure your Animator has a Trigger parameter named "FadeOut"
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(fadeDuration);

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }
}