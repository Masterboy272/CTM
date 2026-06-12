using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletSeed : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 4f;

    void Start()
    {
        // Make the seed fly forward immediately upon spawning
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        // 'transform.right' moves it in the direction the red arrow points in the editor!
        rb.linearVelocity = transform.right * speed; 
        
        // Destroy the seed after a few seconds so it doesn't fly off to infinity
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // We hit the player! Find the camera script and delete their photo.
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null)
            {
                photoMechanic.LoseSavedPhoto();
            }
            
            Destroy(gameObject); // Seed dies on impact
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Gameplay") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            // If the seed hits a wall, the floor, or a pasted photo, it splats and dies.
            Destroy(gameObject);
        }
    }
}