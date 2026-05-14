using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpBug : MonoBehaviour
{
    public float jumpForce = 7f;
    public float jumpInterval = 4f;
    
    private Rigidbody2D rb;
    private float timer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        // When the timer hits 4, jump and reset the timer!
        if (timer >= jumpInterval)
        {
            Jump();
            timer = 0f;
        }
    }

    void Jump()
    {
        // Zero out current vertical velocity so jumps are always exactly the same height
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
        
        // Apply an upward push
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}