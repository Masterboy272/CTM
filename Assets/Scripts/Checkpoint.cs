using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("Optional: Drop a particle effect here to play when touched")]
    public GameObject activationEffect;
[Header("Checkpoint Audio")]
    public AudioSource checkpointAudio;
    public AudioClip activationSound;
    private bool isActivated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            PlayerRespawn respawnScript = other.GetComponent<PlayerRespawn>();
            if (respawnScript != null)
            {
                respawnScript.UpdateRespawnPoint(transform.position);

                if (activationEffect != null)
                {
                    Instantiate(activationEffect, transform.position, Quaternion.identity);
                }
                if (checkpointAudio != null && activationSound != null)
            {
                checkpointAudio.PlayOneShot(activationSound);
            }
            }
        }
    }
}