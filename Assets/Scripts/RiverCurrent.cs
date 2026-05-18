using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RiverCurrent : MonoBehaviour
{
    [Header("Current Settings")]
    [Tooltip("Direction the water flows. (e.g., X=1, Y=0 is Right. X=0, Y=-1 is Down)")]
    public Vector2 flowDirection = Vector2.right;
    
    [Tooltip("How hard the water pushes")]
    public float currentStrength = 20f;
    
    [Tooltip("The layers that can block the current (Check Gameplay AND Default)")]
    public LayerMask blockingLayers;

    private Collider2D riverZone;

    void Start()
    {
        riverZone = GetComponent<Collider2D>();
        
        // Safety check to ensure the river acts as an overlapping zone, not a solid wall!
        riverZone.isTrigger = true; 
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Use attachedRigidbody in case the collider is on a child object
        Rigidbody2D rb = other.attachedRigidbody;
        
        if (rb != null)
        {
            // 1. Determine which way is "Upstream" (against the current)
            Vector2 upstream = -flowDirection.normalized;
            
            // Max distance to check is roughly the largest size of the river area
            float maxDistance = Mathf.Max(riverZone.bounds.size.x, riverZone.bounds.size.y);

            // 2. Shoot a laser upstream to see if anything is blocking the water
            RaycastHit2D[] hits = Physics2D.RaycastAll(rb.position, upstream, maxDistance, blockingLayers);
            
            bool isShielded = false;

            foreach (RaycastHit2D hit in hits)
            {
                // Ignore the player/bug's own body! We only care about walls and photos.
                if (hit.collider.gameObject != other.gameObject)
                {
                    // Push the hit point slightly away from the wall to prevent floating-point math errors
                    Vector2 checkPoint = hit.point - (upstream * 0.1f);
                    
                    // Is this blocking wall actually inside the river?
                    // (This prevents walls entirely outside the river from magically blocking the current)
                    if (riverZone.OverlapPoint(checkPoint))
                    {
                        isShielded = true;
                    }
                    
                    // Since we found the first solid wall upstream, stop checking any further!
                    break; 
                }
            }

            // 3. If nothing is shielding the object, sweep it away!
            if (!isShielded)
            {
                rb.AddForce(flowDirection.normalized * currentStrength, ForceMode2D.Force);
            }
        }
    }
}