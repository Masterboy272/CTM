using UnityEngine;
using UnityEngine.UI;
using TMPro; 

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
        UpdatePhotoDisplay(null);
    }

    void Update()
    {
        // --- TIMER LOGIC ---
        levelTimer += Time.deltaTime;
        
        int minutes = Mathf.FloorToInt(levelTimer / 60F);
        int seconds = Mathf.FloorToInt(levelTimer % 60F);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

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