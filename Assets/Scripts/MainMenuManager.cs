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

    [Header("Level Select (Album)")]
    [Tooltip("Drag all your level buttons here IN ORDER (Level 1 first, Level 2 second, etc.)")]
    public UnityEngine.UI.Button[] levelButtons;
    
    private string saveKey = "FurthestLevelReached";

    void Start()
    {
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
        int levelToLoad = PlayerPrefs.GetInt(saveKey, 1);
        
        string sceneName = "Lvl" + levelToLoad;

        StartCoroutine(TransitionAndLoad(sceneName));
    }

    public void LevelSelectButton()
    {
        albumContainer.SetActive(true);
        RefreshAlbumButtons();
    }
    private void RefreshAlbumButtons()
    {
        int furthestLevel = PlayerPrefs.GetInt(saveKey, 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1; 

            if (levelNumber <= furthestLevel)
            {
                levelButtons[i].gameObject.SetActive(true);
            }
            else
            {
                levelButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void LoadSpecificLevel(int levelNumber)
    {
        string sceneName = "Lvl" + levelNumber;
        StartCoroutine(TransitionAndLoad(sceneName));
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
        newGameConfirmationPanel.SetActive(true);
    }

    public void ConfirmNewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        newGameConfirmationPanel.SetActive(false);
        StartCoroutine(TransitionAndLoad(firstLevelName));
    }

    public void CancelNewGame()
    {
        newGameConfirmationPanel.SetActive(false);
    }

    // ==========================================
    // HELPER METHODS
    // ==========================================

    public void CloseAllPanels()
    {
        albumContainer.SetActive(false);
        settingsContainer.SetActive(false);
        newGameConfirmationPanel.SetActive(false);
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        if (screenFader != null)
        {
            screenFader.SetTrigger("FadeOut");
        }

        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(sceneName);
    }
}