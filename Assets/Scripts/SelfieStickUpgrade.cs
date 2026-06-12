using UnityEngine;

public class SelfieStickUpgrade : MonoBehaviour
{
    public GameObject collectionEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PuzzlePhotoMechanic cameraMechanic = FindObjectOfType<PuzzlePhotoMechanic>();

            if (cameraMechanic != null)
            {
                cameraMechanic.UnlockSelfieStick();

                if (collectionEffect != null)
                {
                    Instantiate(collectionEffect, transform.position, Quaternion.identity);
                }

                Destroy(gameObject);
            }
        }
    }
}