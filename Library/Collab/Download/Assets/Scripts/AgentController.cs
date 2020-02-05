using System;
using System.Collections;
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
#endif
using Unity.AI.Planner;
using Unity.AI.Planner.Controller;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace HorrorAgent
{
    public class AgentController : MonoBehaviour
    {
        IDecisionController myController; //decision controller
        GameObject m_Target; // the target that we are moving to
        Coroutine m_agentMoveTo; //coroutine to move the agent;
        Coroutine m_agentChase;

        public bool AnimationComplete { get; set; } //for animations
        protected int m_AccumulatedTime;
        protected Animator m_Animator;
        protected Rigidbody m_RigidBody;
        protected float m_MinUnitTime = 1f;

        //movement controllers 
        NavMeshAgent m_NavMeshAgent;
        MotionController m_MotionController;
        protected float m_StartTime;

        //Animation parameters
        Vector3 m_TargetPosition;
        bool m_Arrived;
        float m_TotalDistanceTravelled;
        float m_TotalTime;
        float m_PredictedDeltaTime;
        const float k_TurnStrength = 1f;
        //_____ANIMATIONS_____
        //Navigation animations
        static readonly int k_Walk = Animator.StringToHash("Walk"); //sets the animator trigger to walk 
        static readonly int k_Forward = Animator.StringToHash("Forward");
        static readonly int k_Turn = Animator.StringToHash("Turn");

        //Combat
        static readonly int k_LookAt = Animator.StringToHash("LookAt");
        static readonly int k_Chase = Animator.StringToHash("Chase");
        static readonly int k_Attack = Animator.StringToHash("Attack");
        //Player Interactions

        Combat combat; // to get the combat scripts
        public LayerMask playerMask;
        public float viewDistance = 5f;


        //Multiagents
        ITraitBasedObjectData m_agents;
        ITraitBasedObjectData m_targets;
        GameObject agent;
        [SerializeField]
        GameObject[] allAgents;
        
        // Start is called before the first frame update
        void Start()
        {
            m_StartTime = Time.time;
            m_AccumulatedTime = 0;

            //*
            agent = GameObject.FindGameObjectWithTag("Agent");
            if(agent != null)
            {
                m_Animator = agent.GetComponent<Animator>();
                m_RigidBody = agent.GetComponent<Rigidbody>(); //get rigidbody componen
                m_NavMeshAgent = agent.GetComponent<NavMeshAgent>();
                m_MotionController =agent.GetComponent<MotionController>();
            }
            //*/

            /*
            allAgents = GameObject.FindGameObjectsWithTag("Agent");
            //Debug.Log(" " + allAgents.Length);
            for (int i = 0; i < allAgents.Length; i++)
            {

                m_Animator = allAgents[i].GetComponent<Animator>();
                m_RigidBody = allAgents[i].GetComponent<Rigidbody>(); //get rigidbody componen
                m_NavMeshAgent = allAgents[i].GetComponent<NavMeshAgent>();
                m_MotionController = allAgents[i].GetComponent<MotionController>();
            }
            //*/
        }

        // Update is called once per frame
        void Update()
        {
            // playerSeen();
            //agentMoveTo(m_agents, m_targets);
            // Debug.Log("Update senses test");
        }

        void SetAnimationParams(bool atDestination = false) //here you set animation parameters
        {
            // Current position/orientation
            var currentDirection = agent.transform.forward;
            var currentPosition = agent.transform.position;

            // Turn
            var targetDirection = m_NavMeshAgent.nextPosition - currentPosition;
            var turnVal = Vector3.SignedAngle(currentDirection, targetDirection, Vector3.up) / 90.0f;
            var realTurnStrength = atDestination ? 0.0f : k_TurnStrength;
            m_Animator.SetFloat(k_Turn, turnVal * realTurnStrength);
            //Debug.Log("Turn Value: " + turnVal);

            // Forward
            var minForwardVelocity = atDestination ? 0.0f : 0.2f;

            var forwardVal = Mathf.Clamp((m_NavMeshAgent.nextPosition - currentPosition).magnitude, minForwardVelocity, 1f);
            m_Animator.SetFloat(k_Forward, forwardVal);
            m_Arrived = forwardVal <= .24;
            //Debug.Log(" Forward Value: " + forwardVal);
        }

        public IEnumerator agentMoveTo(ITraitBasedObjectData agents, ITraitBasedObjectData target) //coroutine to move the agent to the game object based on the planner
        {
            var agentObject = (agents.ParentObject as GameObject);
            var targetObject = (target.ParentObject as GameObject);

            if (m_agentMoveTo != null) //The action is in progress
                yield break;
            //m_Animator.SetBool(k_LookAt, false);
            m_Animator.SetBool(k_Walk, true); //setting animator to walk here 
            m_Target = targetObject; //assiging target to the gameobject target 
            m_MotionController.TargetPosition = m_Target.transform.position; //setting the movement controller target
            
            while (m_Target != null && !IsTargetReachable() && inViewDistance())
            {

                var distance = Vector3.Distance(agent.transform.position, m_MotionController.TargetPosition);
               // Debug.Log("Distance " + distance);  
              
                Debug.DrawLine(agent.transform.position, m_MotionController.TargetPosition, Color.red);
                m_MotionController.TargetPosition = m_Target.transform.position;

                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Navigation")) //gets the navigator state and basically moves the agent to the target
                {
                    var position = agent.transform.position;
                    m_MotionController.TargetOrientation = m_NavMeshAgent.nextPosition - position;
                    SetAnimationParams();

                 /*   if (playerSeen())
                    {
                        //  Debug.Log("Player has been seen");
                        var forwards = m_Target.transform.forward; //stops you from moving forward when at the target 
                        m_MotionController.StopMoving(forwards);
                        //  transform.position = m_Target.transform.position; //set your transform to the target 
                        m_Animator.SetBool(k_Walk, false); //stop the walking animation 
                        SetAnimationParams(true);


                        m_agentMoveTo = null;
                        //  Debug.Log("Reached end of player seen debug");
                        yield break;
                    }*/
                }
                yield return null; //set action to being complete
            }

            var forward = m_Target.transform.forward; //stops you from moving forward when at the target 
            m_MotionController.StopMoving(forward);
            m_Animator.SetBool(k_Walk, false); //stop the walking animation 
            SetAnimationParams(true);


            m_agentMoveTo = null;
        }

        public IEnumerator agentChase(GameObject target) //coroutine to chase the player
        {
            if (m_agentChase != null) //The action is in progress
                yield break;
            m_Animator.SetBool(k_LookAt, false);
            m_Animator.SetBool(k_Chase, true); //setting animator to walk here 
            m_Target = target; //assiging target to the gameobject target 
            m_MotionController.TargetPosition = m_Target.transform.position; //setting the movement controller target
                                                                             // Debug.Log("Is target reachable" + IsTargetReachable());

            while (m_Target != null && !IsTargetReachable() && !inAttackDistance())
            {

                Debug.DrawLine(transform.position, m_MotionController.TargetPosition, Color.white);
                m_MotionController.TargetPosition = m_Target.transform.position;

                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Chasing")) //gets the navigator state and basically moves the agent to the target
                {
                    var position = transform.position;
                    m_MotionController.TargetOrientation = m_NavMeshAgent.nextPosition - position;
                    SetAnimationParams();
                }
                yield return null; //set action to being complete
            }

            var forward = m_Target.transform.forward; //stops you from moving forward when at the target 
            m_MotionController.StopMoving(forward);
            m_Animator.SetBool(k_Chase, false); //stop the walking animation 
            SetAnimationParams(true);
            
            m_agentChase = null;
        }

        public void attack(GameObject Target)
        {
            transform.LookAt(Target.transform);
            if (inAttackDistance())
            {
                m_Animator.SetBool(k_Attack, true);
            }
            else
            {
                m_Animator.SetBool(k_Attack, false);
            }
        }

        public void TeleportTo(GameObject target)
        {
            transform.position = target.transform.position;
            m_Target = null;
        }

        //Instead lets try can you see the target or not / is it a certain distance from you

        bool IsTargetReachable()
        {
            var distance = Vector3.Distance(transform.position, m_Target.transform.position);
            //  Debug.Log("Distance " + distance);
            return Vector3.Distance(transform.position, m_Target.transform.position) < 1.55f; //return true if this true
        }

        bool playerSeen()
        {
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, 10, playerMask);

            if (targetsInViewRadius.Length >= 1)
            {
                //   Debug.Log("Player Seen");
                //  Debug.Log("Players: " + targetsInViewRadius.Length);
                return true;
            }
            else
            {
                //  Debug.Log("Players: " + targetsInViewRadius.Length);
                //  Debug.Log("Player Not Seen");
                return false;
            }
        }

        //The eat state - lets consider making this an animator transition which frees up more space in the planner
        //For what you're about to eat is dead(eventually)
        public void Eat(GameObject eatable)
        {
            Destroy(eatable);
        }

        //Looking at something action
        public bool inAttackDistance()
        {
            var distance = Vector3.Distance(transform.position, m_Target.transform.position);

            if (distance <= 4)
            {
                return true;
            }
            else { return false; }
        }

        public void lookAt(ITraitBasedObjectData targets)
        {
           var targetObject = (targets.ParentObject as GameObject);

           // var distance = Vector3.Distance(transform.position, m_Target.transform.position);
            m_Animator.SetBool(k_LookAt, true);
            transform.LookAt(targetObject.transform);
            //  m_Animator.SetBool(k_LookAt, false);
        }

        public bool inViewDistance()
        {
            var distance = Vector3.Distance(transform.position, m_Target.transform.position);
            if (distance <= viewDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
