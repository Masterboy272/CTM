using UnityEngine;
using UnityEngine.UI;
using TMPro; // Unity's standard for crisp UI text

public class GameHUD : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    
    [Tooltip("The actual Image component that shows the photo")]
    public Image photoDisplayImage;
    
    [Tooltip("The parent background frame for the photo (so we can hide it when empty)")]
    public GameObject photoFrameObject; 

    private float levelTimer = 0f;

    void Start()
    {
        // Hide the photo frame when the level starts
        UpdatePhotoDisplay(null);
    }

    void Update()
    {
        // --- TIMER LOGIC ---
        levelTimer += Time.deltaTime;
        
        // Convert the raw seconds into minutes and seconds
        int minutes = Mathf.FloorToInt(levelTimer / 60F);
        int seconds = Mathf.FloorToInt(levelTimer % 60F);
        
        // Format the string so it always shows two digits (e.g., 01:05)
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // --- PHOTO DISPLAY LOGIC ---
    // Your Camera script will call this whenever you snap or paste a picture!
    public void UpdatePhotoDisplay(Sprite newPhoto)
    {
        if (newPhoto != null)
        {
            photoDisplayImage.sprite = newPhoto;
            photoFrameObject.SetActive(true);
        }
        else
        {
            photoDisplayImage.sprite = null;
            photoFrameObject.SetActive(false);
        }
    }
}