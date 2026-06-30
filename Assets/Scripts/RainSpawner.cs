using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    [Header("Rain Settings")]
    public GameObject rainDropPrefab;
    
    [Tooltip("Direction the rain falls. (X=0, Y=-1 is Down. X=1, Y=-1 is Diagonal!)")]
    public Vector2 fallDirection = Vector2.down;
    
    [Tooltip("How fast the raindrops shoot out")]
    public float dropSpeed = 15f;

    [Tooltip("How wide the rain cloud is")]
    public float rainWidth = 4f;
    
    [Tooltip("How many seconds between each drop spawning")]
    public float spawnRate = 0.05f;

    private float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            SpawnDrop();
            timer = spawnRate;
        }
    }

    void SpawnDrop()
    {
        if (rainDropPrefab != null)
        {
            // Pick a random spot along the width of the spawner
            // (Using transform.right means if you rotate the spawner in Unity, the spawn line rotates with it!)
            float randomX = Random.Range(-rainWidth / 2f, rainWidth / 2f);
            Vector3 spawnPos = transform.position + (transform.right * randomX);

            // Spawn the drop
            GameObject drop = Instantiate(rainDropPrefab, spawnPos, Quaternion.identity);

            // --- THE NEW DIRECTIONAL LOGIC ---
            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Ensure the direction is perfectly measured
                Vector2 dir = fallDirection.normalized;
                
                // Shoot the drop!
                rb.linearVelocity = dir * dropSpeed;
                
                // Rotate the raindrop sprite so it points exactly where it is flying
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                drop.transform.rotation = Quaternion.Euler(0, 0, angle + 90f); 
            }
        }
    }

    // This draws a helpful line in the editor so you can see exactly how wide your rain cloud is!
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        // Updated to draw the line relative to the object's rotation
        Vector3 leftPoint = transform.position - (transform.right * (rainWidth / 2f));
        Vector3 rightPoint = transform.position + (transform.right * (rainWidth / 2f));
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}