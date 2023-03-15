using GoapAI;
using GoapComponents;
using UnityEngine;
using UnityEngine.AI;
using GoapEnemy;

namespace GoapEnemyActions
{
    public class GoapPatrol : GoapAction
    {
        private bool patrolPointReached = false;
        private PatrolPointComponent lastPatrolPoint; // where we get the logs from
        private GoapEnemyAI goapEnemyAI;
        private float startTime = 0;
        private float waitTime = 1f; // seconds

        void Awake()
        {
            goapEnemyAI = GetComponent<GoapEnemyAI>();
            if (goapEnemyAI == null)
            {
                Debug.Log("Must attach this to a valid GoapEnemyAI type");
            }
        }

        public GoapPatrol()
        {
           // addPrecondition("hasOpponent", false);
    
            addEffect("patrol", true);
           // addEffect("hasOpponent", true);
        
        }

        public override void reset()
        {
            patrolPointReached = false;
            startTime = 0;
        }

        public override bool isDone()
        {
            return patrolPointReached;
        }

        public override bool checkProceduralPrecondition(GameObject agent)
        {
            // find the nearest supply pile that has spare logs
            PatrolPointComponent[] patrolPointsComponent
                = (PatrolPointComponent[])
                GameObject.FindObjectsOfType(typeof(PatrolPointComponent));
            PatrolPointComponent closest = null;
            float closestDist = 0;

            float dist = 0f;
    
            foreach (PatrolPointComponent _patrolPoint in patrolPointsComponent)
            {
                

                bool pathFound = NavMeshHelper.NavMeshTools.GetPath(goapEnemyAI.path, agent.transform.position,
                    _patrolPoint.gameObject.transform.position, NavMesh.AllAreas);
                if (!pathFound) continue;
                
              //  dist = NavMeshHelper.NavMeshTools.GetPathLength(goapEnemyAI.path);
                
                if (lastPatrolPoint == null || closest == null)
                {
                    // first one, so choose it for now
                    closest = _patrolPoint;
                    closestDist = dist;
        
                }

                if (Time.time - _patrolPoint.lastVisitTime > 10f)
                {
                   
                    closest = _patrolPoint;
                }else if (dist < closestDist)
                {
                    // is this one closer than the last?
                    // we found a closer one, use it
                    closest = _patrolPoint;
                    closestDist = dist;
                }
            }

           
              

            lastPatrolPoint = closest;
        
            target = lastPatrolPoint.gameObject;
            targetPosition = target.transform.position;

            //Debug.Log("---- Check patrol dist " + target.name + " _lastVisitTime " + _lastVisitTime);
            return true;
        }

        public override bool perform(GameObject agent)
        {
            
            if (startTime == 0)
                startTime = Time.time;

            if (Time.time - startTime > waitTime)
            {
                // finished forging a tool
                patrolPointReached = true;
                lastPatrolPoint.lastVisitTime = Time.time;
              //  Debug.Log(string.Format("AT Check Point {0}", target.name));
                
            }


            return true;
        }
        
        public override bool requiresInRange()
        {
            return true;
        }
    }
}