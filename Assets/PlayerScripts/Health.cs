using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    private Animator _animator;
    //[SerializeField] DayNightCycle WorldTime;
    public float health = 100;
    public float temp = 37; //degrees in celcius
    public float stamina = 100;
    public float strength = 100;
    public float hydration = 100;
    public float hunger = 100;
    float st_jump = 10;
    bool wait;
    float scale = 1; 
    //UI Stuff
    [SerializeField]
    TextMeshProUGUI HP_txt;
    [SerializeField]
    TextMeshProUGUI ST_txt;
    [SerializeField]
    TextMeshProUGUI Hung_txt;
    [SerializeField]
    TextMeshProUGUI Hydr_txt;
    [SerializeField]
    TextMeshProUGUI tmp_txt;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        //scale = WorldTime.timeScale;
        update_Txt();
        InvokeRepeating("TimedStats", 0, 1); // call every second
    }

    void dead()
    {
        _animator.SetTrigger("Death");
    }

    void TimedStats() //stats that are effected over time - ex. stamina regeneration, hunger, thirst, etc.
    {
        
        if (hydration > 0)
        {
            hydration -= scale * .1f;
        }
        if (hunger > 0)
        {
            hunger -= scale * .05f;
        }
        if (stamina < 100)
        {
            stamina += scale * 1;
        }
        update_Txt();
    }

    public void update_Txt()
    {
        HP_txt.text = "Health: " + (int)health;
        ST_txt.text = "Stamina: " + (int)stamina;
        Hung_txt.text = "Hunger: " + (int)hunger;
        Hydr_txt.text = "Hydration: " + (int)hydration;
        //tmp_txt.text = "Temperature: " + temp;
    }

    public bool Stamina(float x) // checks if enough stamina for action
    {
        if (x > stamina)
        {
            return false;
        }
        else
        {
            stamina -= x;
            return true;
        }
        
    }
    void OnCollisionEnter(Collision collision)
    {
       // Debug.Log("Collided with: " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Weapon")
        {
            Debug.Log("Collided with Weapon");
            if (!wait) {
                wait = true;
                weapon weapScript = collision.gameObject.GetComponent<weapon>();
                health -= weapScript.dmg;
                update_Txt();
                StartCoroutine(WaitABit(1.0f));
                }


        }

    }
    IEnumerator WaitABit(float s)
    {
        yield return new WaitForSeconds(s);
        wait = false;
    }

        // Update is called once per frame
    void Update()
    {
        if (health <= 0 || hunger <= 0 || hydration <= 0)
        {
            dead();
        }
    }
}
