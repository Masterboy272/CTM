using UnityEngine;

public class VirusPlant : MonoBehaviour
{
    [Header("Shooting Setup")]
    public GameObject seedPrefab;
    [Tooltip("An empty child object placed in front of the plant where the seed spawns")]
    public Transform shootPoint; 
    
    [Header("Timer")]
    public float minShootTime = 6f;
    public float maxShootTime = 7f;

    private float shootTimer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        shootTimer -= Time.deltaTime;
        
        if (shootTimer <= 0)
        {
            Shoot();
            ResetTimer();
        }
    }

    void Shoot()
    {
        if (seedPrefab != null && shootPoint != null)
        {
            // Spawn the seed at the shootPoint, matching its exact rotation
            Instantiate(seedPrefab, shootPoint.position, shootPoint.rotation);
        }
    }

    void ResetTimer()
    {
        // Pick a random time between 6 and 7 seconds
        shootTimer = Random.Range(minShootTime, maxShootTime);
    }

    // --- DIRECT TOUCH PENALTY ---
    
    // If your plant uses a Trigger collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null) photoMechanic.LoseSavedPhoto();
        }
    }

    // If your plant uses a Solid collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null) photoMechanic.LoseSavedPhoto();
        }
    }
}