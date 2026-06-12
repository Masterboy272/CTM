using UnityEngine;

public class DeathPit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Did the player fall in?
        if (other.CompareTag("Player"))
        {
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            if (respawnScript != null)
            {
                // Trigger the respawn!
                respawnScript.Respawn();
            }
        }
    }
}