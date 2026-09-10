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
    [Header("Audio")]
    public AudioSource plantAudio;
    public AudioClip shootSound;

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
            Instantiate(seedPrefab, shootPoint.position, shootPoint.rotation);
        }
        if (plantAudio != null && shootSound != null)
        {
            plantAudio.PlayOneShot(shootSound);
        }
    }

    void ResetTimer()
    {
        shootTimer = Random.Range(minShootTime, maxShootTime);
    }

    // --- DIRECT TOUCH PENALTY ---
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null) photoMechanic.LoseSavedPhoto();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null) photoMechanic.LoseSavedPhoto();
        }
    }
}