using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelEndPlatform : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject levelCompleteCanvas;
    public string mainMenuSceneName = "MainMenu";
    public string nextLevelSceneName = "Level_2";

    [Header("Cinemachine Settings")]
    public CinemachineCamera portraitCam;
    public float zoomDuration = 1.5f;

    [Header("Audio")]
    public AudioClip winSound;

    [Header("Animation")]
    [Tooltip("The exact name of the Trigger parameter in your Animator")]
    public string victoryTriggerName = "Victory";

    private bool hasTriggered = false;

    void Start()
    {
        if (levelCompleteCanvas != null) levelCompleteCanvas.SetActive(false);
        if (portraitCam != null) portraitCam.Priority = 0; 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(LevelCompleteSequence(other.gameObject));
        }
    }

    private IEnumerator LevelCompleteSequence(GameObject player)
    {
        PlayerMovement movementScript = player.GetComponent<PlayerMovement>();
        if (movementScript != null)
        {
            movementScript.canMove = false;
            
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetTrigger(victoryTriggerName);
        }

        if (AudioManager.instance != null)
        {
            if (AudioManager.instance.musicSource != null) AudioManager.instance.musicSource.Stop();
            if (winSound != null) AudioManager.instance.PlaySFX(winSound);
        }

        if (portraitCam != null)
        {
            portraitCam.Priority = 100;
        }

        yield return new WaitForSeconds(zoomDuration);

        if (levelCompleteCanvas != null)
        {
            levelCompleteCanvas.SetActive(true);
        }
    }

    // ==========================================
    // BUTTON METHODS
    // ==========================================

    public void MainMenuButton()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void NextLevelButton()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }
}