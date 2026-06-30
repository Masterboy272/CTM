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
            
            // --- CHANGED: Set starting sprite if we have any ---
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

                    // --- CHANGED: SPRITE CYCLING LOGIC ---
                    if (cycleSprites.Length > 0 && visualRenderer != null)
                    {
                        // Move to the next sprite, loop back to 0 if at the end
                        currentColorIndex = (currentColorIndex + 1) % cycleSprites.Length;
                        visualRenderer.sprite = cycleSprites[currentColorIndex];

                        // Tell the door controller to check if the puzzle is solved!
                        if (linkedPuzzle != null)
                        {
                            linkedPuzzle.CheckPuzzle();
                        }
                    }

                    if (mushroomVisual != null) StartCoroutine(SquishAnimation());
                }
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