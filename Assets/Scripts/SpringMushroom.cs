using UnityEngine;
using System.Collections;

public class SpringMushroom : MonoBehaviour
{
    [Header("Bounce Settings")]
    [Tooltip("How high the player gets launched")]
    public float bounceForce = 15f;

    [Header("Visuals")]
    [Tooltip("The child object holding the SpriteRenderer")]
    public Transform mushroomVisual; 
    
    private bool isBouncing = false;
    private Vector3 originalScale;

    void Start()
    {
        if (mushroomVisual != null)
        {
            // Remember the starting size so we can pop back to it after squishing
            originalScale = mushroomVisual.localScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Check if it's the Player
        if (collision.gameObject.CompareTag("Player") && !isBouncing)
        {
            // 2. Make sure the player landed on TOP of the mushroom, not the side or bottom!
            if (collision.transform.position.y > transform.position.y)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                
                if (playerRb != null)
                {
                    // 3. Reset the player's Y velocity to 0. 
                    // (If we don't do this, falling really fast will cancel out the bounce force!)
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                    
                    // 4. Launch them into the air!
                    playerRb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);

                    // 5. Trigger the squish animation
                    if (mushroomVisual != null)
                    {
                        StartCoroutine(SquishAnimation());
                    }
                }
            }
        }
    }

    // --- A simple code-based animation to make the mushroom feel bouncy ---
    IEnumerator SquishAnimation()
    {
        isBouncing = true;
        
        float squishTime = 0.1f;
        float elapsed = 0f;
        
        // Target shape: Wider (x * 1.4) and Shorter (y * 0.5)
        Vector3 squishedScale = new Vector3(originalScale.x * 1.4f, originalScale.y * 0.5f, originalScale.z);

        // Squish down
        while (elapsed < squishTime)
        {
            elapsed += Time.deltaTime;
            mushroomVisual.localScale = Vector3.Lerp(originalScale, squishedScale, elapsed / squishTime);
            yield return null;
        }

        // Pop back up
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