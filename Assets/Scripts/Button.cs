using UnityEngine;

public class Button : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorToOpen;
    [Tooltip("How far the door moves when opened (e.g., X=0, Y=3 moves it up 3 units)")]
    public Vector3 doorOpenOffset = new Vector3(0, 3f, 0); 
    public float doorSpeed = 5f;

    [Header("Button Settings")]
    [Tooltip("The actual sprite of the button that will sink downwards")]
    public Transform buttonVisual; 
    [Tooltip("How far down the button visually sinks when pressed")]
    public float buttonPressDistance = 0.2f; 
    [Header("Audio")]
    public AudioSource plateAudio;
    public AudioClip pressSound;
    public AudioClip releaseSound;
    public AudioSource doorAudio;
    public AudioClip openSound;
    public AudioClip closeSound;

    private Vector3 doorClosedPos;
    private Vector3 doorTargetPos;
    
    private Vector3 buttonUpPos;
    private Vector3 buttonDownPos;

    private int objectsOnButton = 0; 

    void Start()
    {
        if (doorToOpen != null)
        {
            doorClosedPos = doorToOpen.transform.position;
            doorTargetPos = doorClosedPos;
        }

        if (buttonVisual != null)
        {
            buttonUpPos = buttonVisual.localPosition;
            buttonDownPos = buttonUpPos - new Vector3(0, buttonPressDistance, 0);
        }
    }

    void Update()
    {
        if (doorToOpen != null)
        {
            doorToOpen.transform.position = Vector3.Lerp(doorToOpen.transform.position, doorTargetPos, Time.deltaTime * doorSpeed);
        }

        if (buttonVisual != null)
        {
            Vector3 targetButtonPos = (objectsOnButton > 0) ? buttonDownPos : buttonUpPos;
            buttonVisual.localPosition = Vector3.Lerp(buttonVisual.localPosition, targetButtonPos, Time.deltaTime * doorSpeed * 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("The plate was touched by: " + other.gameObject.name + " with the tag: " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("Creatures"))
        {
            objectsOnButton++;

            if (objectsOnButton == 1) 
            {
                doorTargetPos = doorClosedPos + doorOpenOffset;
            }
            if (plateAudio != null && pressSound != null)
        {
            plateAudio.PlayOneShot(pressSound);
        }
        if (doorAudio != null && openSound != null)
        {
            doorAudio.PlayOneShot(openSound);
        }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Creatures"))
        {
            objectsOnButton--;
            
            if (objectsOnButton <= 0) 
            {
                objectsOnButton = 0;
                doorTargetPos = doorClosedPos; 
            }
            if (plateAudio != null && releaseSound != null)
        {
            plateAudio.PlayOneShot(releaseSound);
        }
        if (doorAudio != null && closeSound != null)
        {
            doorAudio.PlayOneShot(closeSound);
        }
        }
    }
}