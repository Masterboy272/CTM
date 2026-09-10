using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RiverCurrent : MonoBehaviour
{
    [Header("Current Settings")]
    [Tooltip("Direction the water flows. (e.g., X=1, Y=0 is Right. X=0, Y=-1 is Down)")]
    public Vector2 flowDirection = Vector2.right;
    
    [Tooltip("How hard the water pushes")]
    public float currentStrength = 20f;
    [Header("River Audio")]
    public AudioSource riverAudio;
    public AudioClip flowingSound;
    
    [Header("Dynamic Size")]
    [Tooltip("How long the river is when NOTHING is blocking it")]
    public float maxRiverLength = 20f;
    
    [Tooltip("How wide the river is")]
    public float riverWidth = 3f;

    [Tooltip("The layers that cut the river off (Check Gameplay AND Default)")]
    public LayerMask blockingLayers;

    private BoxCollider2D riverCollider;

    void Start()
    {
        riverCollider = GetComponent<BoxCollider2D>();
        riverCollider.isTrigger = true; 
    }

    void Update()
    {
        Vector2 dir = flowDirection.normalized;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (dir * 0.1f), dir, maxRiverLength, blockingLayers);

        float currentLength = maxRiverLength;

        if (hit.collider != null && !hit.collider.isTrigger)
        {
            currentLength = Vector2.Distance(transform.position, hit.point);
        }

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            riverCollider.size = new Vector2(currentLength, riverWidth);
            riverCollider.offset = new Vector2(dir.x * (currentLength / 2f), 0);
        }
        else
        {
            riverCollider.size = new Vector2(riverWidth, currentLength);
            riverCollider.offset = new Vector2(0, dir.y * (currentLength / 2f));
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        
        if (rb != null)
        {
            rb.AddForce(flowDirection.normalized * currentStrength, ForceMode2D.Force);
        }
            if (riverAudio != null && flowingSound != null && !riverAudio.isPlaying)
            {
                riverAudio.clip = flowingSound;
                riverAudio.loop = true;
                riverAudio.Play();
            }
    }
}