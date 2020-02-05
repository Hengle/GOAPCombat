using System;
using System.Collections;
#if PLANNER_DOMAIN_GENERATED
using AI.Planner.Domains;
using Unity.AI.Planner.DomainLanguage.TraitBased;

using Unity.AI.Planner;
using Unity.AI.Planner.Controller;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace HorrorAgent
{

    public class Agent : MonoBehaviour
    {
        enum ControllerType
        {
            Random,
            Planner
        }

        IDecisionController myController; //decision controller
        bool m_UpdateStateWithWorldQuery;
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
        const float k_TurnStrength = 1.0f;
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
        public float myQuery = 1f;
        EyeSight eye;

        void SetAnimationParams(bool atDestination = false) //here you set animation parameters
        {

            // Current position/orientation
            var currentDirection = transform.forward;
            var currentPosition = transform.position;

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


        //TODO: make the agent wander randomly by selecting a random point on the nav mesh.  This action can be used for other things now like directing the agent to specific locations in the planner.
        public IEnumerator agentMoveTo(GameObject target) //coroutine to move the agent to the game object based on the planner
        {

            if (m_agentMoveTo != null) //The action is in progress
                yield break;
            m_Animator.SetBool(k_LookAt, false);
            m_Animator.SetBool(k_Walk, true); //setting animator to walk here 
            m_Target = target; //assiging target to the gameobject target 
            m_MotionController.TargetPosition = m_Target.transform.position; //setting the movement controller target
                                                                             // Debug.Log("Is target reachable" + IsTargetReachable());

            while (m_Target != null && !IsTargetReachable() && inViewDistance())
            {


                Debug.DrawLine(transform.position, m_MotionController.TargetPosition, Color.red);
                m_MotionController.TargetPosition = m_Target.transform.position;
                // m_MotionController.StartMoving();
                if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Navigation")) //gets the navigator state and basically moves the agent to the target
                {
                    var position = transform.position;
                    m_MotionController.TargetOrientation = m_NavMeshAgent.nextPosition - position;
                    SetAnimationParams();

                    if (playerSeen())
                    {
                        //  Debug.Log("Player has been seen");
                        var forwards = m_Target.transform.forward; //stops you from moving forward when at the target 
                        m_MotionController.StopMoving(forwards);
                        m_Animator.SetBool(k_Walk, false); //stop the walking animation 
                        SetAnimationParams(true);


                        m_agentMoveTo = null;
                        yield break;
                    }


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

                    if (!inViewDistance())
                    {
                        m_MotionController.TargetPosition = transform.position;
                        var forwards = m_Target.transform.forward; //stops you from moving forward when at the target 
                        m_MotionController.StopMoving(forwards);
                        m_Animator.SetBool(k_Chase, false); //stop the walking animation
                        SetAnimationParams(true);

                        m_agentChase = null;
                        yield break;
                    }


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
                m_Animator.SetBool(k_Attack, true); //TODO: need to put this in a loop thats why the attack animation is no playing in full
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

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, myQuery, playerMask);

            if (targetsInViewRadius.Length >= 1)
            {
                  Debug.Log("Player Seen");
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
        // Start is called before the first frame update
        void Start()
        {

            m_StartTime = Time.time;
            m_AccumulatedTime = 0;
            m_Animator = GetComponent<Animator>(); //Get the animator component
            m_RigidBody = GetComponent<Rigidbody>(); //get rigidbody component
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_Animator = GetComponent<Animator>();
            m_MotionController = GetComponent<MotionController>();
            myController = GetComponent<IDecisionController>();

            eye = gameObject.GetComponent<EyeSight>();
        }

        // Update is called once per frame
        void Update()
        {

            UpdateController();
            Debug.Log("My eyesight" + eye.PlayerSeen); //testing to see if we can access and manipulate individual components of the planner outside the planner --Doesn't seem to be the case more testing needed--
        }

        //This function updates the planner constantly when there are no actions are being performed.
        void UpdateController()
        {
           
                 myController.AutoUpdate = true;
                 //Debug.Log("Is Controller idle? " + myController.IsIdle);
                  
                 if ( myController.IsIdle)
                 {
                     myController.UpdateStateWithWorldQuery(); //updates with the objects in the query which you set the range in the decision planner. 
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

            if (distance <= 6)
            {
                return true;
            }
            else { return false; }


        }

        public void lookAt(GameObject looking)
        {

            m_Animator.SetBool(k_LookAt, true);
            transform.LookAt(looking.transform);


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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, myQuery);

            Gizmos.color = Color.black;
            Vector3 eyeSight = transform.TransformDirection(Vector3.forward) * viewDistance;
            Gizmos.DrawRay(transform.position, eyeSight);
        }



    }

}
#endif