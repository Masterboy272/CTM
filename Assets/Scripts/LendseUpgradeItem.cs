using UnityEngine;

public class LensUpgradeItem : MonoBehaviour
{
    public GameObject collectionEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic cameraMechanic = FindObjectOfType<PuzzlePhotoMechanic>();

            if (cameraMechanic != null)
            {
                // Tell the camera to unlock the zoom feature!
                cameraMechanic.UnlockTelephotoLens();

                if (collectionEffect != null)
                {
                    Instantiate(collectionEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}