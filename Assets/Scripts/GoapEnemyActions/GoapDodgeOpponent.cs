using GoapAI;
using UnityEngine;
using GoapComponents;
using GoapEnemy;

namespace GoapEnemyActions
{
    public class GoapDodgeOpponent : GoapAction
    {
  
        int layerMask = 1 << 8;
        private OpponentComponent opponentMe;
        private GoapEnemyAI goapEnemyAI;
        private bool dodged = false;
        public float dodgeRange = 2f;
        public float waitTime = 1f;
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
        public GoapDodgeOpponent()
        {
            
            addPrecondition("defenseMove", true);
            addPrecondition("hasOpponent", true);
            addEffect("defenceMove", false);
       
    
        
        }
        public override void reset()
        {
            dodged = false;
     

        }

        public override bool isDone()
        {
            
            return dodged;
        }

        public override bool checkProceduralPrecondition(GameObject agent)
        {
            // move enemy after an attack
            
            target = agent.gameObject;
            if (target == null)
            {
                 Debug.Log("<color=red>NO DODGE TARGET:</color> " + agent.gameObject.name);
                return false;
            }
            if (opponentMe.IamUnderFire)
            {
                dodged = false;
                //Debug.Log("<color=orange>Dodging</color>");
               // opponentMe.IamUnderFire = null;
                return true;
            }
            
            return false;
        }

        public override bool perform(GameObject agent)
        {
            if (startTime == 0)
            {
                startTime = Time.time;
                goapEnemyAI.AnimatorBool("Dodge", true);
                goapEnemyAI.AnimatorBool("Boxing", false);
            }
            
           
     
            if (Time.time - startTime > waitTime)
            {
                Debug.Log("<color=orange>Dodged!!!</color>" + agent.name);
               
               // goapEnemyAI.AnimatorTrigger("Dodge");
               // goapEnemyAI.AnimatorBool("Dodge", false);
                goapEnemyAI.AnimatorBool("Dodge", false);
                dodged = true;
         

            }
           

            return true;
        }

        public override bool requiresInRange()
        {
            return true;
        }
    }
}