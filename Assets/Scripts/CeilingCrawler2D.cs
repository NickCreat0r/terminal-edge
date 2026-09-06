using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class CeilingCrawler2D : MonoBehaviour
{
    [Header("Ceiling Settings")]
    public float crawlSpeed = 4f;
    public float upwardReach = 5f; // How high the ceiling can be above the player
    public string gripTag = "CeilingGrip";
    public float hangOffset = 0.1f; // How far below the zone's bottom edge the player hangs
    public string ceilingLayerName = "Ceiling"; // Layer to ignore collision with while hanging
    private Rigidbody2D rb;
    private float defaultGravity;
    public bool isHanging = false;
    private int playerLayer;
    private int ceilingLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        playerLayer = gameObject.layer;
        ceilingLayer = LayerMask.NameToLayer(ceilingLayerName);
        Debug.Log("CeilingCrawler2D Awake: Default gravity saved as " + defaultGravity);
    }

    private void CheckForCeilingAndGrab()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, upwardReach);
        Debug.Log($"Raycast fired upward. Reached {upwardReach} units. Found {hits.Length} objects.");
        for (int i = 0; i < hits.Length; i++) // Check every object the line touched
        {
            Collider2D col = hits[i].collider;
            Debug.Log($"Hit #{i}: Object='{col.gameObject.name}', Tag='{col.tag}'");
            if (col.CompareTag(gripTag))
            {
                Debug.Log($"SUCCESS: Found Grip Tag '{gripTag}' on object '{col.gameObject.name}'. Attaching!");
                isHanging = true;
                Physics2D.IgnoreLayerCollision(playerLayer, ceilingLayer, true); // Stop physical push while hanging
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                float targetY = col.bounds.min.y - hangOffset; // Snap the player to the bottom edge of the grip zone
                transform.position = new Vector2(transform.position.x, targetY);

                return; // Stop checking once we successfully grab
            }
        }
        Debug.LogWarning("Raycast check finished, but no valid CeilingGrip was found in range.");
    }

    private void DropFromCeiling()
    {
        isHanging = false;
        Physics2D.IgnoreLayerCollision(playerLayer, ceilingLayer, false); // Restore normal solid collision
        rb.gravityScale = defaultGravity;
        Debug.Log("Player detached. Gravity restored to " + defaultGravity);
    }

    // Replaces OnTriggerExit2D entirely — checked every physics step instead of relying
    // on whether a matching OnTriggerEnter2D happened to fire first, which was the
    // source of the inconsistent instant-drop / never-drop behavior.
    private bool IsStillGripped()
{
    Vector2 checkPoint = (Vector2)transform.position + Vector2.up * (hangOffset + 0.03f);
    Collider2D[] overlaps = Physics2D.OverlapPointAll(checkPoint);

    Debug.Log($"[Grip Check] checkPoint={checkPoint}, overlaps found: {overlaps.Length}");
    for (int i = 0; i < overlaps.Length; i++)
    {
        Debug.Log($"  -> '{overlaps[i].gameObject.name}' tag='{overlaps[i].tag}' bounds min={overlaps[i].bounds.min} max={overlaps[i].bounds.max}");
        if (overlaps[i].CompareTag(gripTag)) return true;
    }
    return false;
}

    void Update()
    {
        if (!isHanging) // Latch on from the ground
        {
            if (Input.GetKeyDown(KeyCode.W)) 
            {
                Debug.Log("Input: 'W' pressed. Attempting to grab ceiling...");
                CheckForCeilingAndGrab();
            }
        }
        else // Drop down manually
        {
            if (Input.GetKeyDown(KeyCode.S)) 
            {
                Debug.Log("Input: 'S' pressed. Manually dropping from ceiling.");
                DropFromCeiling();
            }
        }
    }

    void FixedUpdate()
    {
        if (isHanging)
        {
            if (!IsStillGripped()) // Crawled off the edge of the zone
            {
                Debug.Log("No longer overlapping grip zone. Auto-dropping.");
                DropFromCeiling();
                return;
            }

            float moveX = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(moveX * crawlSpeed, 0f);
        }
    }

    private void OnDrawGizmos() // Draws a red line in the Scene view to visualize the raycast reach
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * upwardReach);
    }
}