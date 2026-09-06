using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float JumpForce = 10f;
    public LayerMask GroundLayer; // LayerMask to identify what is considered ground for the player
    public BoxCollider2D GroundCollider; // Reference to the BoxCollider2D used for ground detection
    public bool OnGround;
    public float horizontalInput;
    private Rigidbody2D rb;
        private OrbInteraction orbInteraction;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    private float lastGroundedTime;
    private float lastJumpPressedTime = -999f;
    private CeilingCrawler2D ceilingCrawler;
    private Animator anim;
    private SpriteRenderer playerSpriteRenderer;
    private bool dashRequested;
    private int requestedDashDirection;
    private float requestedDashSpeed;
    private float requestedDashDuration;
    private bool isDashing;
    private float dashTimer;
    private float originalGravity;
    private bool orbJumpRequested;
    private float requestedOrbJumpForce;

    void Start()
    {
        Debug.Log(
    $"Player layer: {gameObject.layer}, " +
    $"Ceiling layer: {LayerMask.NameToLayer("Ceiling")}, " +
    $"Player collider: {GetComponent<Collider2D>() != null}"
);
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        orbInteraction = GetComponent<OrbInteraction>();
        ceilingCrawler = GetComponent<CeilingCrawler2D>();
        anim = GetComponentInChildren<Animator>();
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // TEMP DEBUG: Check Rigidbody2D, GroundCollider, and Animator references here.
        Debug.Log($"[PlayerMovement] Started. Rigidbody: {rb != null}, GroundCollider: {GroundCollider != null}, Animator: {anim != null}", this);
    }

    void OnMove(InputValue value)
    {
        if (IsTouchingCeiling())
    {   
        return;
    }
        Vector2 inputVector = value.Get<Vector2>(); // The default Move action returns a 2D vector (X and Y). 
        horizontalInput = inputVector.x;
        
        if (playerSpriteRenderer != null && horizontalInput != 0f)
        {
            playerSpriteRenderer.flipX = horizontalInput < 0f;
        }
        if (horizontalInput != 0f) Debug.Log($"[PlayerMovement] Move input: {horizontalInput}", this); // TEMP DEBUG: Check whether the Move action reaches the movement script.
    }

    void OnJump(InputValue value)
    {
        if (IsTouchingCeiling())
    {
        return;
    }
        if (value.isPressed)
        {
            lastJumpPressedTime = Time.time;
            orbInteraction?.BufferOrbJump();
            Debug.Log("[PlayerMovement] Jump input received.", this); // TEMP DEBUG: Check whether the Jump action is being buffered.
        }
    }

    void FixedUpdate()
    {
        if (IsTouchingCeiling())
        {
        OnGround = false; // Prevents the player from being considered grounded while hanging from the ceiling
        return;
        }

        if (dashRequested)
        {
            isDashing = true;
            dashTimer = requestedDashDuration;
            dashRequested = false;
            rb.gravityScale = 0f;
        }

        if (isDashing)
        {
            if (dashTimer > 0f)
            {
                rb.linearVelocity = new Vector2(requestedDashDirection * requestedDashSpeed,rb.linearVelocity.y);
                dashTimer -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }

            return;
        }

        if (orbJumpRequested)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * requestedOrbJumpForce, ForceMode2D.Impulse);

            orbJumpRequested = false;
            lastJumpPressedTime = -999f;
            OnGround = false;

            return;
    }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y); //Apply horizontal speed,but keep current Y velocity
        
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
        return Physics2D.OverlapBox(groundCheckBounds.center,groundCheckBounds.size,0f,GroundLayer) != null;
    }

    private bool IsTouchingCeiling()
    {
        return ceilingCrawler != null && ceilingCrawler.isHanging;
    }

public bool RequestDash(int direction, float speed, float duration)
{
    if (dashRequested || isDashing)
    {
        return false;
    }

    requestedDashDirection = direction;
    requestedDashSpeed = speed;
    requestedDashDuration = duration;
    dashRequested = true;

    return true;
}

public bool RequestOrbJump(float jumpForce)
{
    if (orbJumpRequested || IsTouchingCeiling())
    {
        return false;
    }

    dashRequested = false;
    isDashing = false;
    dashTimer = 0f;
    rb.gravityScale = originalGravity;
    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

    requestedOrbJumpForce = jumpForce;
    orbJumpRequested = true;

    return true;
}

}

