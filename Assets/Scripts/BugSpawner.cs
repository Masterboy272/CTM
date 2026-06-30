using UnityEngine;
using System.Collections.Generic;

public class BugSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject bugPrefab;
    
    [Tooltip("How many seconds between each spawn")]
    public float spawnInterval = 5f;
    
    [Tooltip("The maximum number of bugs this spawner can have alive at once")]
    public int maxBugsActive = 3;

    [Header("Spawn Location")]
    [Tooltip("An empty child object showing exactly where the bug should pop out")]
    public Transform spawnPoint;

    private float timer;
    private List<GameObject> activeBugs = new List<GameObject>();

    void Start()
    {
        // Set the timer so it spawns a bug almost immediately when the level starts
        timer = 0.5f; 
    }

    void Update()
    {
        // 1. Clean up the list! 
        // If a bug fell in a death pit or got vaporized by a photo, this removes the "empty" memory.
        activeBugs.RemoveAll(bug => bug == null);

        // 2. Only run the timer if we are under the maximum bug limit
        if (activeBugs.Count < maxBugsActive)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SpawnBug();
                timer = spawnInterval;
            }
        }
    }

    void SpawnBug()
    {
        if (bugPrefab != null && spawnPoint != null)
        {
            // Spawn the bug and add it to our tracking list
            GameObject newBug = Instantiate(bugPrefab, spawnPoint.position, Quaternion.identity);
            activeBugs.Add(newBug);
        }
    }
}