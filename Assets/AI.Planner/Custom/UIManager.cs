#if PLANNER_DOMAIN_GENERATED
using System;
using System.Collections.Generic;
#if PLANNER_DOMAINS_GENERATED
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
#endif
using Unity.Collections;
using Unity.Entities;
using UnityEngine.UI;
using UnityEngine;
using Unity.AI.Planner.Controller;
using Unity.AI.Planner;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using System.Linq;

//This Script also sets your inventory
public class UIManager : MonoBehaviour
{

    public Image UIBarHealth;
    public Image UIBarStamina;
   
    public float InventoryOffset;

    
   List<Image> m_Health = new List<Image>();
   List<Image> m_Stamina = new List<Image>();

    

    Material m_healthMat;
    Material m_staminaMat;

    StateData stateD;

    static readonly int s_Amount = Shader.PropertyToID("_Amount");

    static ComponentType[] k_Stats;
    ITraitData m_Stats;

    IDecisionController myController;
   // IStateData myState;
    public void Awake()
    {
        myController = GetComponent<IDecisionController>();
        UIBarHealth.material = Instantiate(UIBarHealth.material);
        UIBarStamina.material = Instantiate(UIBarStamina.material);
        m_healthMat = UIBarHealth.material;
        m_staminaMat = UIBarStamina.material;

        m_healthMat.SetFloat(s_Amount, 1);
        m_staminaMat.SetFloat(s_Amount, 1);
        k_Stats = new ComponentType[] { typeof(Stats) };
       
    }

    public void Start()
    {
        InitializeAgentStats();
    }

    public void Update()
    {
        InitializeAgentStats();
        var myState = myController.GetPlannerState(false);
      //  Debug.Log("My State " + myState);
     
    }

   void InitializeAgentStats( )
    {
#if PLANNER_DOMAINS_GENERATED
       
        var traitHolder = transform.GetComponent<ITraitBasedObjectData>();
        m_Stats = traitHolder.TraitData.FirstOrDefault(t => t.TraitDefinitionName == nameof(Stats));
       // Debug.Log("My stats HP " + m_Stats.GetValue(Stats.FieldHp));
        //Debug.Log("My stats HP " + m_Stats.GetValue(Stats.FieldStamina));
#endif
    }
    
    


    
}
#endif
