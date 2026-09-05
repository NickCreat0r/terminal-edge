using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float JumpForce = 10f;
    public LayerMask GroundLayer;
    public BoxCollider2D GroundCollider;
    public bool OnGround;

    private float horizontalInput;
    private Rigidbody2D rb;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    private float lastGroundedTime;
    private float lastJumpPressedTime = -999f;
    private GhostDash ghostDash;
    private Animator anim;
    private SpriteRenderer playerSpriteRenderer;

    // TEMP DEBUG: Check Rigidbody2D, GhostDash, and Animator references here.
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ghostDash = GetComponent<GhostDash>();
        anim = GetComponentInChildren<Animator>();
        playerSpriteRenderer = anim != null
            ? anim.GetComponent<SpriteRenderer>()
            : GetComponentInChildren<SpriteRenderer>();
        Debug.Log($"[PlayerMovement] Started. Rigidbody: {rb != null}, GroundCollider: {GroundCollider != null}, Animator: {anim != null}", this);
    }

    // TEMP DEBUG: Check whether the Move action reaches the movement script.
    void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>(); // The default Move action returns a 2D vector (X and Y). 
        horizontalInput = inputVector.x;
        if (playerSpriteRenderer != null && horizontalInput != 0f)
        {
            playerSpriteRenderer.flipX = horizontalInput < 0f;
        }
        if (horizontalInput != 0f) Debug.Log($"[PlayerMovement] Move input: {horizontalInput}", this);
    }

    // TEMP DEBUG: Check whether the Jump action is being buffered.
    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            lastJumpPressedTime = Time.time;
            Debug.Log("[PlayerMovement] Jump input received.", this);
        }
    }

    // TEMP DEBUG: Check velocity assignment, ground detection, and jump conditions.
    void FixedUpdate()
    {

        if (ghostDash == null || !ghostDash.isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y); //Apply horizontal speed,but keep current Y velocity
        }

        OnGround = IsGrounded();

        
        if (OnGround) lastGroundedTime = Time.time;
        bool canJump = Time.time - lastGroundedTime <= coyoteTime; //42-48 forgiving jump mechanic
        bool jumpBuffered = Time.time - lastJumpPressedTime <= jumpBufferTime;
        if (jumpBuffered)
        {
            Debug.Log($"[PlayerMovement] Jump check. OnGround: {OnGround}, CanJump: {canJump}, GroundLayer: {GroundLayer.value}, VerticalSpeed: {rb.linearVelocity.y}", this);
        }
        if (canJump && jumpBuffered)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpForce);
            lastJumpPressedTime = -999f; // Reset jump buffer to prevent double jump
            Debug.Log($"[PlayerMovement] Ground jump executed. OnGround: {OnGround}", this);
        }
    }

    private bool IsGrounded()
    {
        if (GroundCollider == null)
        {
            return false;
        }

        Bounds groundCheckBounds = GroundCollider.bounds;
        return Physics2D.OverlapBox(
            groundCheckBounds.center,
            groundCheckBounds.size,
            0f,
            GroundLayer) != null;
    }
}

