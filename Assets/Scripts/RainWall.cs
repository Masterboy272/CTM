using UnityEngine;

public class DynamicRainWall : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How far the rain falls if there is no floor beneath it")]
    public float maxDropDistance = 20f;
    
    [Tooltip("How wide the rain barrier is")]
    public float rainWidth = 2f;
    
    [Tooltip("Check the boxes for layers that stop the rain (Gameplay, Default, etc.)")]
    public LayerMask layersThatBlockRain;

    [Header("References")]
    [Tooltip("The child object that holds the Sprite and Collider")]
    public Transform rainVisualObj;
    
    private SpriteRenderer rainRenderer;
    private BoxCollider2D rainCollider;

    void Start()
    {
        // Grab the components from the child object
        rainRenderer = rainVisualObj.GetComponent<SpriteRenderer>();
        rainCollider = rainVisualObj.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // 1. Shoot an invisible laser straight down
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxDropDistance, layersThatBlockRain);

        // Default to max length if we hit nothing
        float currentLength = maxDropDistance;

        // If the laser hits something (like your pasted photo), shrink the length!
        if (hit.collider != null)
        {
            currentLength = hit.distance;
        }

        // 2. Update the Visual Size
        // NOTE: This only works correctly if the SpriteRenderer's Draw Mode is set to 'Tiled' in the Inspector!
        rainRenderer.size = new Vector2(rainWidth, currentLength);

        // 3. Update the Physics Collider Size so it perfectly matches the visuals
        rainCollider.size = new Vector2(rainWidth, currentLength);

        // 4. Position the rain block
        // Because Unity sizes things from the center out, we have to push the block down 
        // by exactly half its length so the top always stays attached to the spawner ceiling.
        rainVisualObj.localPosition = new Vector3(0, -currentLength / 2f, 0);
    }
}