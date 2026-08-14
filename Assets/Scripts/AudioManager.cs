using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // This creates a static reference so we never accidentally spawn two music players
    public static AudioManager instance;

    void Awake()
    {
        // If there isn't an Audio Manager yet, make this the official one and keep it alive
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <--- The magic line that keeps music playing between levels!
        }
        // If one already exists (because we went back to the Main Menu), destroy this duplicate
        else
        {
            Destroy(gameObject);
        }
    }
}