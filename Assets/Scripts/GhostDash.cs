using UnityEngine;
using UnityEngine.InputSystem;
public class GhostDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 25f;
    public int totalDashes = 4; 
    public float dashDuration = 0.15f; // Πόσο διαρκεί το dash
    private int lastDirection = 1; // 1 = Δεξιά, -1 = Αριστερά
    private PlayerMovement playerMovement; // Reference to the PlayerMovement script
    void Start(){
        playerMovement = GetComponent<PlayerMovement>();
    }
    
    void OnDash(InputValue value){
    
        if (playerMovement != null && playerMovement.horizontalInput != 0f)
        {
        lastDirection = playerMovement.horizontalInput > 0f ? 1 : -1;
        }

        if (value.isPressed && totalDashes > 0)
        {
            Debug.Log("Trying to dash! Direction: " + lastDirection + " Charges: " + totalDashes); 
            if (lastDirection == 1 && totalDashes >= 1) RightDash();
            else if (lastDirection == -1 && totalDashes >= 2) LeftDash();
        }
    }
    void RightDash()
    {
        if (playerMovement != null && playerMovement.RequestDash(1, dashSpeed, dashDuration))
        {
        totalDashes -= 1;
        Debug.Log($"[GhostDash] Right dash requested. Charges: {totalDashes}", this);
        }
    }

    void LeftDash()
    {
        if (playerMovement != null && playerMovement.RequestDash(-1, dashSpeed, dashDuration))
        {
        totalDashes -= 2;
        Debug.Log($"[GhostDash] Left dash requested. Charges: {totalDashes}", this);
        }
    }
}