using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class JumpBug : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource bugAudio;
    public AudioClip jumpSound;
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
        
        if (timer >= jumpInterval)
        {
            Jump();
            timer = 0f;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        if (bugAudio != null && jumpSound != null)
        {
            bugAudio.PlayOneShot(jumpSound);
        }
    }
}