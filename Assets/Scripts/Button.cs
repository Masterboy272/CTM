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

    // Internal trackers
    private Vector3 doorClosedPos;
    private Vector3 doorTargetPos;
    
    private Vector3 buttonUpPos;
    private Vector3 buttonDownPos;

    // The secret to puzzle buttons: Counting what is on it!
    private int objectsOnButton = 0; 

    void Start()
    {
        // Record where the door starts so we know how to close it
        if (doorToOpen != null)
        {
            doorClosedPos = doorToOpen.transform.position;
            doorTargetPos = doorClosedPos;
        }

        // Record where the button starts so we know how to press it
        if (buttonVisual != null)
        {
            buttonUpPos = buttonVisual.localPosition;
            buttonDownPos = buttonUpPos - new Vector3(0, buttonPressDistance, 0);
        }
    }

    void Update()
    {
        // Smoothly slide the door to its target position
        if (doorToOpen != null)
        {
            doorToOpen.transform.position = Vector3.Lerp(doorToOpen.transform.position, doorTargetPos, Time.deltaTime * doorSpeed);
        }

        // Smoothly push the button visual down or pop it back up
        if (buttonVisual != null)
        {
            Vector3 targetButtonPos = (objectsOnButton > 0) ? buttonDownPos : buttonUpPos;
            buttonVisual.localPosition = Vector3.Lerp(buttonVisual.localPosition, targetButtonPos, Time.deltaTime * doorSpeed * 2f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object stepping on us is allowed to press the button
        Debug.Log("The plate was touched by: " + other.gameObject.name + " with the tag: " + other.tag);
        if (other.CompareTag("Player") || other.CompareTag("Creatures"))
        {
            objectsOnButton++;
            
            // If this is the FIRST object to press it, open the door!
            if (objectsOnButton == 1) 
            {
                doorTargetPos = doorClosedPos + doorOpenOffset;
            }
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Creatures"))
        {
            objectsOnButton--;
            
            // Safety catch: If the count hits 0, close the door!
            if (objectsOnButton <= 0) 
            {
                objectsOnButton = 0;
                doorTargetPos = doorClosedPos; 
            }
        }
    }
}