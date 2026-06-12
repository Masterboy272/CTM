using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("Optional: Drop a particle effect here to play when touched")]
    public GameObject activationEffect;

    private bool isActivated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the player touches us, and we haven't been activated yet
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // Find the respawn script on the player
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            if (respawnScript != null)
            {
                // Tell the player to remember THIS checkpoint's exact position
                respawnScript.UpdateRespawnPoint(transform.position);

                if (activationEffect != null)
                {
                    Instantiate(activationEffect, transform.position, Quaternion.identity);
                }
            }
        }
    }
}