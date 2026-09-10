using UnityEngine;

public class TutorialBubble : MonoBehaviour
{
    [Header("UI Setup")]
    [Tooltip("Drag the Canvas Group from your text bubble here")]
    public CanvasGroup bubbleCanvasGroup;
    
    [Header("Settings")]
    public float fadeSpeed = 5f;

    private bool isPlayerNear = false;

    void Start()
    {
        if (bubbleCanvasGroup != null)
        {
            bubbleCanvasGroup.alpha = 0f; 
        }
    }

    void Update()
    {
        if (bubbleCanvasGroup != null)
        {
            float targetAlpha = isPlayerNear ? 1f : 0f;
            bubbleCanvasGroup.alpha = Mathf.Lerp(bubbleCanvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}