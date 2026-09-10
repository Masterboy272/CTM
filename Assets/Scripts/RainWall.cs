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
        rainRenderer = rainVisualObj.GetComponent<SpriteRenderer>();
        rainCollider = rainVisualObj.GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, maxDropDistance, layersThatBlockRain);

        float currentLength = maxDropDistance;

        if (hit.collider != null)
        {
            currentLength = hit.distance;
        }

        rainRenderer.size = new Vector2(rainWidth, currentLength);

        rainCollider.size = new Vector2(rainWidth, currentLength);

        rainVisualObj.localPosition = new Vector3(0, -currentLength / 2f, 0);
    }
}