using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int health = 3;
    private bool hasDied;

    // TEMP DEBUG: Check initial health state.
    void Start()
    {
        Debug.Log($"[PlayerHealth] Started. Health: {health}", this);
    }

    // TEMP DEBUG: Check damage input, health changes, and death conditions.
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !hasDied)
        {

            TakeDamage(1);
        }

        if (health <= 0 && !hasDied)
        {
            hasDied = true;
            Die();
        }
    }

    // TEMP DEBUG: Check every damage request and resulting health value.
    public void TakeDamage(int damage)
    {
        health -= damage;
        print("Player health: " + health);
        Debug.Log($"[PlayerHealth] Damage received: {damage}. Health: {health}", this);
    }

    // TEMP DEBUG: Check scene reload when the player dies.
    public void Die()
    {
        print("Player has died");
        Debug.Log("[PlayerHealth] Die called; reloading scene.", this);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //respawn 
    }

}

