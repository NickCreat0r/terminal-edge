using UnityEngine;
using UnityEngine.InputSystem;
public class GhostDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f; // Πιο γρήγορο για dash (το 10f είναι σαν απλό τρέξιμο)
    public int totalDashes = 4; // Total number of dashes available
    public float dashDuration = 0.15f; // Πόσο διαρκεί το dash στον αέρα

    private Rigidbody2D rb;
    private int lastDirection = 1; // 1 = Δεξιά, -1 = Αριστερά
    private bool triggerDash = false; // Γέφυρα μεταξύ Update και FixedUpdate
    // Μεταβλητές για την ώρα που γίνεται το dash
    public bool isDashing = false;
    private float dashTimer = 0f;
    private float originalGravity; // Αποθηκεύει τη βαρύτητα για να την επαναφέρει
    private float horizontalInput; // Αποθηκεύει την τελευταία κατεύθυνση που πατήθηκε
    private Animator anim; 

    // TEMP DEBUG: Check Rigidbody2D and initial gravity setup here.
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale; // Αποθηκεύουμε την αρχική βαρύτητα του παίκτη
        Debug.Log($"[GhostDash] Started. Rigidbody: {rb != null}, gravity: {originalGravity}", this);
    }

    // TEMP DEBUG: Check whether the Move action reaches the dash script.
    void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>().x;
        if (horizontalInput != 0f) Debug.Log($"[GhostDash] Move input: {horizontalInput}", this);
    }

    // TEMP DEBUG: Check whether the Dash action reaches this callback.
    void OnDash(InputValue value)
    {
        Debug.Log("Unity heard the Dash button!"); // TEST 1

        if (value.isPressed && !isDashing)
        {
            Debug.Log("Trying to dash! Direction: " + lastDirection + " Charges: " + totalDashes); // TEST 2

            if (lastDirection == 1 && totalDashes >= 1) RightDash();
            else if (lastDirection == -1 && totalDashes >= 2) LeftDash();
        }
    }

    // TEMP DEBUG: Check lastDirection changes before physics runs.
    void Update()
    {
       if (horizontalInput > 0) lastDirection = 1; 
       else if (horizontalInput < 0) lastDirection = -1; 
    }

    // TEMP DEBUG: Check dash trigger, gravity, velocity, and timer transitions.
void FixedUpdate()
    {
        if (triggerDash)
        {
            isDashing = true;
            dashTimer = dashDuration; 
            triggerDash = false;
            rb.gravityScale = 0f; 

        }

        if (isDashing) 
        {
            if (dashTimer > 0)
            {
                rb.linearVelocity = new Vector2(lastDirection * dashSpeed, rb.linearVelocity.y); 
                dashTimer -= Time.fixedDeltaTime; 
            }
            else
            {
                isDashing = false; 
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); 

                // ADD THIS: Stop dash animation
                anim.SetBool("IsDashing", false);
            }
        }
    }

    // TEMP DEBUG: Check right-dash charge consumption and trigger scheduling.
    void RightDash()
    {
        totalDashes = totalDashes - 1;
        triggerDash = true; // Δίνει το σήμα στο FixedUpdate
        Debug.Log($"[GhostDash] Right dash queued. Charges: {totalDashes}", this);
    }

    // TEMP DEBUG: Check left-dash charge consumption and trigger scheduling.
    void LeftDash()
    {
        totalDashes = totalDashes - 1;
        triggerDash = true; // Δίνει το σήμα στο FixedUpdate
        Debug.Log($"[GhostDash] Left dash queued. Charges: {totalDashes}", this);
    }
}