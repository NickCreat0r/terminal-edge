using UnityEngine;
using UnityEngine.InputSystem;
public class OrbInteraction : MonoBehaviour
{
    private Rigidbody2D rb;
    private JumpOrb activeOrb = null;
    private bool isTouchingOrb = false;

    [Header("Forgiveness Settings")]
    public float inputBufferTime = 0.2f; // Remembers your click for 0.2 seconds early
    private float inputBufferCounter;
    public float orbGraceTime = 0.30f; // Lets you click 0.15 seconds late
    private float orbGraceCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log($"[OrbInteraction] Started. Rigidbody: {rb != null}", this);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            inputBufferCounter = inputBufferTime;
            Debug.Log("[OrbInteraction] Jump input buffered for orb.", this); // TEMP DEBUG: Check whether Jump input is buffered for an orb.
        }
    }

    void Update()
    {       
        inputBufferCounter -= Time.deltaTime;
        // 2. ORB GRACE PERIOD: Are we in the orb, or did we just leave it?
        if (isTouchingOrb)
        {
            orbGraceCounter = orbGraceTime; // Keep timer at max while inside the trigger
        }
        else
        {
            orbGraceCounter -= Time.deltaTime; // Count down after leaving
            // Only forget the orb when the grace period completely runs out
            if (orbGraceCounter <= 0f) 
            {
                activeOrb = null;
            }
        }
        
        if (inputBufferCounter > 0f && activeOrb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset momentum before applying jump force
            rb.AddForce(Vector2.up * activeOrb.jumpForce, ForceMode2D.Impulse); // Launch
            activeOrb.ConsumeOrb();
            // Consume the timers so we don't accidentally double jump
            inputBufferCounter = 0f;
            orbGraceCounter = 0f;
            activeOrb = null;
            isTouchingOrb = false;
            Debug.Log("[OrbInteraction] Orb jump executed.", this); // TEMP DEBUG: Check when the orb jump is executed and timers are reset.
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Orb"))
        {
            activeOrb = other.GetComponent<JumpOrb>();
            isTouchingOrb = true;
            Debug.Log($"[OrbInteraction] Entered orb. JumpOrb found: {activeOrb != null}", this); // TEMP DEBUG: Check when the player enters an orb and whether a JumpOrb component is found.
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Orb"))
        {
            isTouchingOrb = false; 
            Debug.Log("[OrbInteraction] Exited orb.", this); // 2. ORB GRACE PERIOD: Are we in the orb, or did we just leave it?
        }
    }
}