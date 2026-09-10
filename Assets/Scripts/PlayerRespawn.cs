using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("How much higher to spawn the player so they don't get stuck in the floor")]
    public float heightOffset = 1f;

    private Vector3 currentRespawnPoint;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        currentRespawnPoint = transform.position;
    }

    public void UpdateRespawnPoint(Vector3 newPosition)
    {
        currentRespawnPoint = newPosition + new Vector3(0, heightOffset, 0);
        
        Debug.Log("Checkpoint Saved!");
    }

    public void Respawn()
    {
        transform.position = currentRespawnPoint;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        PuzzlePhotoMechanic photoMechanic = GetComponent<PuzzlePhotoMechanic>();
        if (photoMechanic != null)
        {
            photoMechanic.LoseSavedPhoto();
        }

        Debug.Log("Player Respawned!");
    }
}