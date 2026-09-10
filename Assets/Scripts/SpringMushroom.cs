using UnityEngine;
using System.Collections;

public class SpringMushroom : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceForce = 15f;

    [Header("Visuals")]
    public Transform mushroomVisual; 
    
    [Header("Sprite Puzzle (Optional)")]
    [Tooltip("Drag your different colored mushroom sprites here IN ORDER.")]
    public Sprite[] cycleSprites;
    [HideInInspector] public int currentColorIndex = 0;

    [Header("Audio")]
    public AudioSource shroomAudio;
    public AudioClip bounceSound;
    
    [Tooltip("Drag the Puzzle Controller here if this mushroom opens a door")]
    public ColorPuzzleController linkedPuzzle;

    private bool isBouncing = false;
    private Vector3 originalScale;
    private SpriteRenderer visualRenderer;

    void Start()
    {
        if (mushroomVisual != null)
        {
            originalScale = mushroomVisual.localScale;
            visualRenderer = mushroomVisual.GetComponent<SpriteRenderer>();
            
            if (cycleSprites.Length > 0 && visualRenderer != null)
            {
                visualRenderer.sprite = cycleSprites[currentColorIndex];
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBouncing)
        {
            if (collision.transform.position.y > transform.position.y)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                    playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                    if (cycleSprites.Length > 0 && visualRenderer != null)
                    {
                        currentColorIndex = (currentColorIndex + 1) % cycleSprites.Length;
                        visualRenderer.sprite = cycleSprites[currentColorIndex];

                        if (linkedPuzzle != null)
                        {
                            linkedPuzzle.CheckPuzzle();
                        }
                    }

                    if (mushroomVisual != null) StartCoroutine(SquishAnimation());
                }
            }
            if (shroomAudio != null && bounceSound != null)
            {
                shroomAudio.PlayOneShot(bounceSound);
            }
        }
    }

    IEnumerator SquishAnimation()
    {
        isBouncing = true;
        float squishTime = 0.1f;
        float elapsed = 0f;
        Vector3 squishedScale = new Vector3(originalScale.x * 1.4f, originalScale.y * 0.5f, originalScale.z);

        while (elapsed < squishTime)
        {
            elapsed += Time.deltaTime;
            mushroomVisual.localScale = Vector3.Lerp(originalScale, squishedScale, elapsed / squishTime);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < squishTime)
        {
            elapsed += Time.deltaTime;
            mushroomVisual.localScale = Vector3.Lerp(squishedScale, originalScale, elapsed / squishTime);
            yield return null;
        }

        mushroomVisual.localScale = originalScale;
        isBouncing = false;
    }
}