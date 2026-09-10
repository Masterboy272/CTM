using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BulletSeed : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 4f;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed; 
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic photoMechanic = FindObjectOfType<PuzzlePhotoMechanic>();
            if (photoMechanic != null)
            {
                photoMechanic.LoseSavedPhoto();
            }
            
            Destroy(gameObject); 
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Gameplay") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Destroy(gameObject);
        }
    }
}