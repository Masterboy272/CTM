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
    [Header("Audio")]
    [Tooltip("The speaker playing the looping rain/waterfall sound")]
    public AudioSource rainAudio;
    

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
            float randomX = Random.Range(-rainWidth / 2f, rainWidth / 2f);
            Vector3 spawnPos = transform.position + (transform.right * randomX);

            GameObject drop = Instantiate(rainDropPrefab, spawnPos, Quaternion.identity);

            Rigidbody2D rb = drop.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = fallDirection.normalized;
                
                rb.linearVelocity = dir * dropSpeed;
                
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                drop.transform.rotation = Quaternion.Euler(0, 0, angle + 90f); 
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        
        Vector3 leftPoint = transform.position - (transform.right * (rainWidth / 2f));
        Vector3 rightPoint = transform.position + (transform.right * (rainWidth / 2f));
        Gizmos.DrawLine(leftPoint, rightPoint);
    }
}