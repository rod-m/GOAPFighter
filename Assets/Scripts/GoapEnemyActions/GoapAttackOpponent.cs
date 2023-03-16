using GoapAI;
using UnityEngine;
using GoapComponents;
using GoapEnemy;
using UnityEngine.AI;

namespace GoapEnemyActions
{
    public class GoapAttackOpponent : GoapAction
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        
        private bool hitOpponent = false;
        public float maxDistance = 3f;
        private OpponentComponent opponentTarget;
        private OpponentComponent defender;
       
        private GoapEnemyAI goapEnemyAI;

        public float waitTime = 3f;
        private float startTime = 0;
        void Awake()
        {
            layerMask = ~layerMask;
            goapEnemyAI = GetComponent<GoapEnemyAI>();
            defender = GetComponent<OpponentComponent>();
            if (goapEnemyAI == null)
            {
                Debug.Log("Must attach this to a valid GoapEnemyAI type");
            }

            
        }

        public GoapAttackOpponent()
        {
            addPrecondition("hasWeopon", true);
            addPrecondition("hasOpponent", true);
            //addPrecondition("defenceMove", false);
           addEffect("patrol", true);
           addEffect("hasOpponent", false);
      
        }

        public override void reset()
        {
            hitOpponent = false;
            startTime = 0;
        }

        public override bool isDone()
        {
             
            return hitOpponent;
        }

        public override bool checkProceduralPrecondition(GameObject agent)
        {
            

            opponentTarget = null;
            // find the nearest OpponentComponent that is still standing
            OpponentComponent[] allOpponents = (OpponentComponent[])
                GameObject.FindObjectsOfType(typeof(OpponentComponent));
            OpponentComponent closest = null;
            float closestDist = 0;
            float dist = 0f;
            foreach (OpponentComponent _opponentComponent in allOpponents)
            {
             
                if (agent.name == _opponentComponent.gameObject.name)
                {
           
                    continue; // dont attack self
                }

              //  if (_opponentComponent.IamUnderFire != null && _opponentComponent.IamUnderFire != agent) continue; // dont attack if already engaed
                if (_opponentComponent.Health <= 0) continue; // dont attack dead guy

                bool pathFound = NavMeshHelper.NavMeshTools.GetPath(goapEnemyAI.path, agent.transform.position,
                    _opponentComponent.gameObject.transform.position, NavMesh.AllAreas);
                if (!pathFound) continue; // no path to it!


                dist = NavMeshHelper.NavMeshTools.GetPathLength(goapEnemyAI.path);

                if(dist > 20f) continue; // too far away to attack
                
                if (closest == null)
                {
                    // first one, so choose it for now
                    closest = _opponentComponent;
                    closestDist = dist;
                }
                else
                {
                    // is this one closer than the last?
                    if (dist < closestDist)
                    {
                        // we found a closer one, use it
                        closest = _opponentComponent;
                        closestDist = dist;
                    }
                }
            }

            if (closest == null)
            {
               Debug.Log("<color=red>NO TARGET:</color> " + agent.gameObject.name);
                return true;
            }

            //startTime = 0;
            opponentTarget = closest;
            opponentTarget.IamUnderFire = agent;
            target = closest.gameObject;
                       
            targetPosition = target.transform.position;
           
            return true;
        }

        public override bool perform(GameObject agent)
        {
            if (defender.Health <= 0) return true; // i died so cant carry on attcking
            if (startTime == 0)
            {
                startTime = Time.time;
                goapEnemyAI.AnimatorBool("Boxing", true);
                goapEnemyAI.AnimatorFloat("vely", 0f);
                goapEnemyAI.AnimatorFloat("velx", 0f);
            }


            if (Time.time - startTime > waitTime)
            {
               // Debug.Log(string.Format("<color=blue>Attacking:</color> waited {0} waitTime {1} Opp name {2}", (Time.time - startTime), waitTime , agent.name));

                if (opponentTarget.Health > 0)
                {
   
                    // look at player
                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance, layerMask))
                    {
                        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                        Debug.Log(string.Format( "<color=blue>Attacking:</color> Did Hit {0} with damage {1}",opponentTarget.Health, goapEnemyAI.backpack.weopon.damage ));
                        opponentTarget.Health -= goapEnemyAI.backpack.weopon.damage;
                        startTime = 0;
                       // hitOpponent = true;
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                        //Debug.Log("Did not Hit");
          
                        transform.LookAt(target.transform);
                    }
               
     
                }
                else
                {
                    hitOpponent = true;
                    opponentTarget.IamUnderFire = null;
                    goapEnemyAI.AnimatorBool("Boxing", false);
                 //   startTime = 0;
                    return false; //dead enemy
      
                }
            }
            


            return true;
        }

        public override bool requiresInRange()
        {
            return true;
        }
    }
}