using UnityEngine;
using System.Collections; // Απαραίτητο για τα χρονόμετρα (Coroutines)
public class JumpOrb : MonoBehaviour
{
    public float jumpForce = 6f;
    public float respawnTime = 2.5f; // Πόσο χρόνο κάνει να ξαναεμφανιστεί

    private SpriteRenderer spriteRenderer;
    private Collider2D orbCollider;

    // TEMP DEBUG: Check SpriteRenderer and Collider2D references here.
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        orbCollider = GetComponent<Collider2D>();
        Debug.Log($"[JumpOrb] Started. SpriteRenderer: {spriteRenderer != null}, Collider: {orbCollider != null}", this);
    }

    // TEMP DEBUG: Check when the orb is consumed and respawn is requested.
    public void ConsumeOrb() // Αυτή η μέθοδος θα καλείται από τον παίκτη όταν κάνει το άλμα
    {
        StartCoroutine(RespawnRoutine());
        Debug.Log("[JumpOrb] Consumed; respawn started.", this);
    }

    // TEMP DEBUG: Check orb visibility and collider state across the respawn delay.
    private IEnumerator RespawnRoutine() // Το IEnumerator επιτρέπει στον κώδικα να "περιμένει" χωρίς να παγώσει το παιχνίδι
    {
        spriteRenderer.enabled = false; // 1. Κρύβουμε το orb και απενεργοποιούμε το hitbox του
        orbCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime); // 2. Περιμένουμε...

        spriteRenderer.enabled = true; // 3. Εμφανίζουμε ξανά το orb
        orbCollider.enabled = true;
    }
}