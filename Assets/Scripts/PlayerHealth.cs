using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    private bool hasDied;
    void Start()
    {
        Debug.Log($"[PlayerHealth] Started. Health: {health}", this); //TEMP DEBUG: Check initial health value here.
    }
    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.C) && !hasDied) TakeDamage(1); // Check damage input for testing purposes (press 'C' to take damage).
        
        if (health <= 0 && !hasDied) // Check if the player has died and prevent multiple death triggers.
        {
            hasDied = true;
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        print("Player health: " + health);
        Debug.Log($"[PlayerHealth] Damage received: {damage}. Health: {health}", this); // TEMP DEBUG: Check every damage request and health value.
    }

    public void Die()
    {
        print("Player has died");
        Debug.Log("[PlayerHealth] Die called; reloading scene.", this); // TEMP DEBUG: Check scene reload when the player dies.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Respawn 
    }
}

