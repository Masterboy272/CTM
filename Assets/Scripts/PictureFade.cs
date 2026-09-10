using UnityEngine;
using System.Collections;

public class StickerDecay : MonoBehaviour
{
    public float lifetime = 10f;
    public SpriteRenderer colorRenderer;

    void Start()
    {
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        Color startColor = colorRenderer.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            
            float alpha = 1f - (elapsed / lifetime); 
            
            colorRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            
            yield return null;
        }
        Destroy(gameObject);
    }
}