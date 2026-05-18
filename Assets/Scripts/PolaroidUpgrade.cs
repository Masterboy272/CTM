using UnityEngine;

public class PhotoUpgradeItem : MonoBehaviour
{
    [Header("Visuals")]
    public GameObject collectionEffect; // Optional: A particle effect to spawn when collected

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object touching us is the Player
        if (other.CompareTag("Player"))
        {
            // Find the main photo mechanic script in the scene
            PuzzlePhotoMechanic cameraMechanic = FindObjectOfType<PuzzlePhotoMechanic>();

            if (cameraMechanic != null)
            {
                // Tell the camera to upgrade itself!
                cameraMechanic.UpgradeTimer();

                // Optional: Spawn a cool effect
                if (collectionEffect != null)
                {
                    Instantiate(collectionEffect, transform.position, Quaternion.identity);
                }

                // Destroy the power-up so it can't be collected twice
                Destroy(gameObject);
            }
        }
    }
}