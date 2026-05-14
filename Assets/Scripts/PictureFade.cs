using UnityEngine;
using System.Collections;

public class StickerDecay : MonoBehaviour
{
    public float lifetime = 10f;
    public SpriteRenderer colorRenderer; // The top layer that fades out

    void Start()
    {
        // Start the fading process the moment the sticker spawns
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        Color startColor = colorRenderer.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            
            // Calculate how transparent the color should be (goes from 1 down to 0)
            float alpha = 1f - (elapsed / lifetime); 
            
            // Apply the fading transparency
            colorRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            yield return null; // Wait for the next frame
        }

        // Once 10 seconds have passed, destroy the sticker
        Destroy(gameObject);
    }
}