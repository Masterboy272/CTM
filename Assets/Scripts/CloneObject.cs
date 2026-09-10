using UnityEngine;

public class CloneablePhotoObject : MonoBehaviour
{
    [Tooltip("Drag the Prefab of this object into this slot so the camera knows what to spawn!")]
    public GameObject myPrefab;

    [Tooltip("If true, it stays forever. If false, it deletes itself when the photo rots away.")]
    public bool isPermanent = false;
}