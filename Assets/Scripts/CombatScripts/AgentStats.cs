/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentStats : MonoBehaviour
{
    // Health
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    //Stamina
    public int maxStamina = 100;
    public int currentStamina { get; private set; }



    public Stat damage; //adding this to the stat modifier list 
    public Stat armor;

    // Set current health to max health
    // when starting the game.
    void Awake()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // Damage the character
    public void TakeDamage(int damage)
    {
        // Subtract the armor value
        damage -= armor.GetValue(); //getting the value from "Stats.cs"
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // Damage the character
        currentHealth -= damage;
        //currentStamina 
        Debug.Log(transform.name + " takes " + damage + " damage.");

        // If health reaches zero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // Die in some way
        // This method is meant to be overwritten
        Debug.Log(transform.name + " died.");
    }

}*/
