using GoapAI;
using UnityEngine;
using UnityEngine.AI;
using GoapComponents;
using GoapEnemy;

namespace GoapEnemyActions
{
    public class GoapCoverDefence : GoapAction
    {
  
        int layerMask = 1 << 8;
        private OpponentComponent opponentMe;
        private GoapEnemyAI goapEnemyAI;
        private bool gotCover = false;

        public float waitTime = 4f;
        private float startTime = 0;
        void Awake()
        {
            goapEnemyAI = GetComponent<GoapEnemyAI>();
            if (goapEnemyAI == null)
            {
                Debug.Log("Must attach this to a valid GoapEnemyAI type");
            }

            opponentMe = GetComponent<OpponentComponent>();
        }
        public GoapCoverDefence()
        {
            
            addPrecondition("defenseMove", true);
            addPrecondition("hasOpponent", true);
            addEffect("defenceMove", false);
       
    
        
        }
        public override void reset()
        {
            gotCover = false;
     

        }

        public override bool isDone()
        {
            
            return gotCover;
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
                
                if ( closest == null)
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

           
              
            if (closest == null)
            {
                // Debug.Log("<color=red>NO TARGET:</color> " + agent.gameObject.name);
                return false;
            }

        
            target = closest.gameObject;
            targetPosition = target.transform.position;

            //Debug.Log("---- Check patrol dist " + target.name + " _lastVisitTime " + _lastVisitTime);
            return true;
        }

        public override bool perform(GameObject agent)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
                goapEnemyAI.AnimatorBool("Dodge", false);
                goapEnemyAI.AnimatorBool("Boxing", false);
            }
            
           
     
            if (Time.time - startTime > waitTime)
            {
                
                gotCover = true;
                opponentMe.Health += 10;

            }
       

            return true;
        }

        public override bool requiresInRange()
        {
            return true;
        }
    }
}