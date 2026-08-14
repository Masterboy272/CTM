using UnityEngine;
using UnityEngine.UI; 

public class PauseMenu : MonoBehaviour
{
    [Header("Menu Containers")]
    public GameObject container;
    [Tooltip("Drag the Album Panel here")]
    public GameObject albumContainer;
    public GameObject SettingsContainer;
    [Header("Dynamic Backgrounds")]
    [Tooltip("Drag the Image component of your pause menu background here")]
    public Image menuBackgroundImage;
    
    public Sprite defaultBackground;
    public Sprite onePowerupBackground;
    public Sprite twoPowerupBackground;
    public Sprite threePowerupBackground;

    [Header("Player Reference")]
    [Tooltip("Drag your Player here so the menu can check their upgrades")]
    public PuzzlePhotoMechanic playerCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!container.activeSelf)
            {
                OpenPauseMenu();
            }
            if (albumContainer.activeSelf)
            {
                CloseAlbumButton();
            }
            if (SettingsContainer.activeSelf)
            {
                CloseSettingsButton();
            }
            else
            {
                ResumeButton(); 
            }
        }
    }

    public void OpenPauseMenu()
    {
        container.SetActive(true);
        Time.timeScale = 0;
        
        if (albumContainer != null) albumContainer.SetActive(false); 

        UpdateBackground();
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }

    public void AlbumButton()
    {
        if (albumContainer != null)
        {
            albumContainer.SetActive(true);
        }
    }
    
        public void CloseAlbumButton() 
    {
        if (albumContainer != null)
        {
            albumContainer.SetActive(false);
        }
    }
    public void CloseSettingsButton() 
    {
        if (SettingsContainer != null)
        {
            SettingsContainer.SetActive(false);
        }
    }

    void UpdateBackground()
    {
        if (playerCamera == null || menuBackgroundImage == null) return;

        int powerUpCount = 0;
        if (playerCamera.hasExtendedTimer) powerUpCount++;
        if (playerCamera.hasLensUpgrade) powerUpCount++;
        if (playerCamera.hasSelfieStick) powerUpCount++;

        if (powerUpCount == 0)
        {
            menuBackgroundImage.sprite = defaultBackground;
        }
        else if (powerUpCount == 1)
        {
            menuBackgroundImage.sprite = onePowerupBackground;
        }
        else if (powerUpCount == 2)
        {
            menuBackgroundImage.sprite = twoPowerupBackground;
        }
        else if (powerUpCount >= 3)
        {
            menuBackgroundImage.sprite = threePowerupBackground;
        }
    }

    public void SettingsButton()
    {
        if (SettingsContainer != null)
        {
            SettingsContainer.SetActive(true);
        }
    }

    public void QuitButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}