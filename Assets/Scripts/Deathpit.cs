using UnityEngine;

public class DeathPit : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip deathSound;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (AudioManager.instance != null && deathSound != null)
            {
                AudioManager.instance.PlaySFX(deathSound);
            }
            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            if (respawnScript != null)
            {
                respawnScript.Respawn();
            }
        }
        
    }
}