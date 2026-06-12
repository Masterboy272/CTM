using UnityEngine;

public class RainDrop : MonoBehaviour
{
    [Tooltip("The layers that will destroy the raindrop (Check Gameplay and Default)")]
    public LayerMask destroyLayers;
    
    public float maxLifetime = 5f;

    void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    // --- CHANGED: Now uses real, solid physical collisions! ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the solid object we smashed into is the floor or a pasted photo
        if (((1 << collision.gameObject.layer) & destroyLayers) != 0)
        {
            Destroy(gameObject);
        }
    }
}