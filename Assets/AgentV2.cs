using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentV2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Eat(GameObject eatable)
    {

        //int k_eat = Animator.StringToHash("Eat");
        //m_Animator.SetTrigger(k_eat);

        Destroy(eatable);
        
    }


}
