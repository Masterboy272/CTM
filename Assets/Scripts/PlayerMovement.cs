using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Drag the Player's Audio Source here")]
    public AudioSource playerAudio;
    [Tooltip("The sound effect for jumping")]
    public AudioClip jumpSound;

    [Header("State")]
    public bool canMove = true;

    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    private bool isNearLadder = false;
    private bool isClimbing = false;
    private float startingGravity; 
    private Rigidbody2D rb;
    public bool isGrounded;

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    public float jumpAnimDelay = 0.01f; 

    float horizontalMove = 0f;
    bool jump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingGravity = rb.gravityScale; 
    }

    
    void Update()
    {
        if (!canMove)
        {

            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            return; 
        }
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("isMoving", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;

            animator.SetBool("isJumping", true);
            if (playerAudio != null && jumpSound != null)
            {
                playerAudio.PlayOneShot(jumpSound);
            }
            StartCoroutine(DelayedJumpAnimation());
        }
        

        float verticalInput = Input.GetAxisRaw("Vertical");

        if (isNearLadder && Mathf.Abs(verticalInput) > 0.1f)
        {
            isClimbing = true;
        }

        if (isClimbing)
        {
            rb.gravityScale = 0f; 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalInput * climbSpeed);
            if (animator != null)
                {
                    animator.SetBool("isClimbing", true);
                 animator.SetFloat("ClimbInput", verticalInput);
                }
        }
        else
        {
            rb.gravityScale = startingGravity; 
            if (animator != null)
                {
                    animator.SetBool("isClimbing", false);
                }
        }
    }

private System.Collections.IEnumerator DelayedJumpAnimation()
    {
        yield return new WaitForSeconds(jumpAnimDelay);
 
            animator.SetBool("isJumping", true);
            
        
    }
public void OnLanding()
    {
        animator.SetBool("isJumping", false);
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isNearLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            isNearLadder = false;
            isClimbing = false;
        }
    }
}
