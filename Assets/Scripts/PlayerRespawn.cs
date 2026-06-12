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
        
        // When the game starts, your very first spawn point is exactly where you are standing!
        // (We don't add the offset here, because you are already safely standing on the floor)
        currentRespawnPoint = transform.position;
    }

    // Checkpoints will call this method to save a new safe location
    public void UpdateRespawnPoint(Vector3 newPosition)
    {
        // --- NEW: Add the height offset to the saved position! ---
        currentRespawnPoint = newPosition + new Vector3(0, heightOffset, 0);
        
        Debug.Log("Checkpoint Saved!");
    }

    // Death pits and spikes will call this method to kill the player
    public void Respawn()
    {
        // 1. Move the player back to the checkpoint (now slightly in the air!)
        transform.position = currentRespawnPoint;
        
        // 2. Kill any leftover falling momentum so they don't instantly fall through the floor!
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 3. Punish the player by destroying their saved photo!
        PuzzlePhotoMechanic photoMechanic = GetComponent<PuzzlePhotoMechanic>();
        if (photoMechanic != null)
        {
            photoMechanic.LoseSavedPhoto();
        }

        Debug.Log("Player Respawned!");
    }
}