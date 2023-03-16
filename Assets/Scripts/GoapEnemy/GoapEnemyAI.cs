using System;
using System.Collections.Generic;
using System.Net;
using GoapAI;
using GoapComponents;
using UnityEngine;
using UnityEngine.AI;

namespace GoapEnemy
{
    [RequireComponent(typeof(BackpackComponent))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class GoapEnemyAI : MonoBehaviour, IGoap
    {
        Vector2 smoothDeltaPosition = Vector2.zero;
        Vector2 velocity = Vector2.zero;
        Animator anim;
        [System.NonSerialized] public NavMeshAgent navAgent;
        [System.NonSerialized] public BackpackComponent backpack;
        public float moveSpeed = 1f;
        public Transform goal;
        public NavMeshPath path;
 
        public int numberOfOpponents = 0;
        private OpponentComponent opponentMe;
        public float maxDistance = 2f;
        private OpponentComponent[] allOpponents;
        void Start()
        {
            gameObject.AddComponent <GoapAgent>();
            anim = GetComponent<Animator>();
            navAgent = GetComponent<NavMeshAgent>();
            backpack = GetComponent<BackpackComponent>();
            // Don’t update position automatically
            path = new NavMeshPath();
            navAgent.updatePosition = false;
            opponentMe = GetComponent<OpponentComponent>();
            FindOpponents();
            
        }

        public void FindOpponents()
        {
            
            OpponentComponent[] _allOpponents = (OpponentComponent[]) 
                GameObject.FindObjectsOfType ( typeof(OpponentComponent) );
            numberOfOpponents = 0;
            foreach (OpponentComponent opponentComponent in _allOpponents)
            {
                if(opponentComponent.Health <= 0) continue;
                if(opponentComponent.gameObject.name == this.gameObject.name) continue;
                numberOfOpponents++;
                
            }

            allOpponents = _allOpponents;
   
           // Debug.Log(String.Format("**** FindObjectsOfType {0}", numberOfOpponents));
            
        }
        public HashSet<KeyValuePair<string, object>> getWorldState()
        {
            
            HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

            worldData.Add(
                new KeyValuePair<string, object>("hasWeopon", (backpack.weopon.weoponType != WeoponType.None)));
            FindOpponents();
            worldData.Add(
                new KeyValuePair<string, object>("hasOpponent", numberOfOpponents > 0));
            bool defenceMove = opponentMe.attacker != null && opponentMe.healthPercent < 50.0f;
            worldData.Add(
                new KeyValuePair<string, object>("defenceMove", defenceMove));
            worldData.Add(
                new KeyValuePair<string, object>("ammo", backpack.ammo));
            return worldData;
        }

/**
	 * Implement in subclasses
	 */
        public abstract HashSet<KeyValuePair<string, object>> createGoalState();


        public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
        {
            // Not handling this here since we are making sure our goals will always succeed.
            // But normally you want to make sure the world state has changed before running
            // the same goal again, or else it will just fail.
            Debug.Log("<color=red>Plan Failed</color> " + GoapAgent.prettyPrint(failedGoal));
        }

        public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
        {
            // Yay we found a plan for our goal
            Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
        }

        public void actionsFinished()
        {
            // Everything is done, we completed our actions for this gool. Hooray!
            Debug.Log("<color=yellow>Actions completed</color>");
        }

        public void planAborted(GoapAction aborter)
        {
            // An action bailed out of the plan. State has been reset to plan again.
            // Take note of what happened and make sure if you run the same goal again
            // that it can succeed.
            Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
        }

        public bool moveAgent(GoapAction nextAction)
        {
            // move towards the NextAction's target
            if (opponentMe.Health <= 0)
            {
                navAgent.isStopped = true;
                return false;
            }

      
            goal = nextAction.target.transform;
      
            navAgent.destination = goal.position;
          

            //navAgent.steeringTarget = goal.position;
            float distance = Mathf.Infinity;
            bool pathFound = NavMeshHelper.NavMeshTools.GetPath(this.path, gameObject.transform.position,
                goal.position, NavMesh.AllAreas);
            if (pathFound)
            {
                distance = NavMeshHelper.NavMeshTools.GetPathLength(this.path);
            }
            else
            {
                Debug.Log(string.Format("No Path found to {0}",nextAction.gameObject.name ));
                return false;
            }
            
            
            if (distance < maxDistance)
            {
                // at the target location,  done
                
               // Debug.Log(string.Format("READY! Moved to {0} from {1}",nextAction.gameObject.name, gameObject.name ));
                nextAction.setInRange(true);
                navAgent.destination = gameObject.transform.position; // force play idle
                UpdateMe();
              
                return true;
            }
            else
            {
                UpdateMe();
                return false;
            }

        }

        void UpdateMe()
        {
            Vector3 worldDeltaPosition = navAgent.nextPosition - transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);

            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if time advances
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.magnitude > 0.5f && navAgent.remainingDistance > navAgent.radius;

            // Update animation parameters
            anim.SetBool("move", shouldMove);
         
            anim.SetFloat("velx", velocity.x);
            anim.SetFloat("vely", velocity.y);
          
            GetComponent<LookAt>().lookAtTargetPosition = navAgent.steeringTarget + transform.forward;
        }

        void OnAnimatorMove()
        {
            // Update position to agent position
            transform.position = navAgent.nextPosition;
        }

        public void AnimatorTrigger(string name){
            anim.SetTrigger(name);
            
        }
        public void AnimatorFloat(string name, float value){
            anim.SetFloat(name, value);
            
        }
        public void AnimatorBool(string name, bool state){
            anim.SetBool(name, state);
            
        }
}
}