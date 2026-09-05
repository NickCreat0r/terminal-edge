using UnityEngine;
using System.Collections;
public class JumpOrb : MonoBehaviour
{   
    public float jumpForce = 6f;
    public float respawnTime = 2.5f; 
    private SpriteRenderer spriteRenderer;
    private Collider2D orbCollider;

    void Start() // TEMP DEBUG: Check SpriteRenderer and Collider2D references here.
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        orbCollider = GetComponent<Collider2D>();
        Debug.Log($"[JumpOrb] Started. SpriteRenderer: {spriteRenderer != null}, Collider: {orbCollider != null}", this);
    }

    public void ConsumeOrb() // Αυτή η μέθοδος θα καλείται από τον παίκτη όταν κάνει το άλμα
    {
        StartCoroutine(RespawnRoutine());
        Debug.Log("[JumpOrb] Consumed; respawn started.", this);  // TEMP DEBUG: Check when the orb is consumed and respawn is requested.
    }

    private IEnumerator RespawnRoutine() // Το IEnumerator επιτρέπει στον κώδικα να "περιμένει" χωρίς να παγώσει το παιχνίδι
    {
        spriteRenderer.enabled = false; // Κρύβουμε το orb και απενεργοποιούμε το hitbox 
        orbCollider.enabled = false; 
        yield return new WaitForSeconds(respawnTime); // Περιμένουμε...
        spriteRenderer.enabled = true; // Εμφανίζουμε ξανά το orb
        orbCollider.enabled = true;
    }
}