using UnityEngine;

public class LensUpgradeItem : MonoBehaviour
{
    public GameObject collectionEffect;
    [Header("Audio")]
    [Tooltip("The chime or sound effect for collecting this item")]
    public AudioClip collectSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic cameraMechanic = FindObjectOfType<PuzzlePhotoMechanic>();

            if (cameraMechanic != null)
            {
                cameraMechanic.UnlockTelephotoLens();

                if (collectionEffect != null)
                {
                    Instantiate(collectionEffect, transform.position, Quaternion.identity);
                }
                if (AudioManager.instance != null && collectSound != null)
            {
                AudioManager.instance.PlaySFX(collectSound);
            }

                Destroy(gameObject);
            }
        }
    }
}