using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 5f;
    private bool isNearLadder = false;
    private bool isClimbing = false;
    private float startingGravity; 
    private Rigidbody2D rb;

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;

    void Start()
    {
        startingGravity = rb.gravityScale; 
    }

    
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Movement", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("IsJumping", true);
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
        }
        else
        {
            rb.gravityScale = startingGravity; 
        }
    }

public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
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
            // When we walk or jump off the ladder, reset everything
            isNearLadder = false;
            isClimbing = false;
        }
    }
}
