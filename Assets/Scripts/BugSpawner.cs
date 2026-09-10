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
        timer = 0.5f; 
    }

    void Update()
    {
        activeBugs.RemoveAll(bug => bug == null);

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
            GameObject newBug = Instantiate(bugPrefab, spawnPoint.position, Quaternion.identity);
            activeBugs.Add(newBug);
        }
    }
}