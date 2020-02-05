using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float HP = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void hit(float dmg)
    {
        HP -= dmg;
        if (HP <= 0)
        {
            Die();
        }
    }

    void Die() // This is called when health <= 0
    {
        Debug.Log("I am ded");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
