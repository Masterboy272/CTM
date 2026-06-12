using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    [Header("Rain Settings")]
    public GameObject rainDropPrefab;
    
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
            float randomX = Random.Range(-rainWidth / 2f, rainWidth / 2f);
            Vector3 spawnPos = transform.position + new Vector3(randomX, 0, 0);

            // Spawn the drop
            Instantiate(rainDropPrefab, spawnPos, Quaternion.identity);
        }
    }

    // This draws a helpful yellow line in the editor so you can see exactly how wide your rain cloud is!
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position - new Vector3(rainWidth / 2f, 0, 0), transform.position + new Vector3(rainWidth / 2f, 0, 0));
    }
}