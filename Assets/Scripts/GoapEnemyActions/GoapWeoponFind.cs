using GoapAI;
using UnityEngine;
using GoapComponents;
namespace GoapEnemyActions
{
    public class GoapWeoponFind : GoapAction
    {
  
  
        public bool gotWeopon = false;
        public float waitTime = 3f;
        private float startTime = 0;
        private WeoponSupplyComponent targetWeoponSupply; // where we get the logs from
        private BackpackComponent backpack;
        public GoapWeoponFind()
        {
            addPrecondition("hasWeopon", false);
            //addEffect("hasWeopon", true);
            addEffect("patrol", true);
        }

   
        public override void reset()
        {
            gotWeopon = false;
            startTime = 0;
        }

        public override bool isDone()
        {
            return gotWeopon;
        }

        public override bool checkProceduralPrecondition(GameObject agent)
        {
            if (backpack == null) backpack = agent.GetComponent<BackpackComponent>();
            if (backpack.weopon.weoponType != WeoponType.None) return true;
            // find the nearest supply pile that has spare logs
            WeoponSupplyComponent[] weoponSupplySpply 
                = (WeoponSupplyComponent[])
                       GameObject.FindObjectsOfType ( typeof(WeoponSupplyComponent) );
            WeoponSupplyComponent closest = null;
            float closestDist = 0;
		
            foreach (WeoponSupplyComponent supply in weoponSupplySpply) {
                if(backpack.weopon != null && supply.damage < backpack.weopon.damage)
                {
                    continue; // has better weopon
                }
                if (supply.numberOfWeopons > 0) {
                    if (closest == null) {
                        // first one, so choose it for now
                        closest = supply;
                        closestDist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
                    } else {
                        // is this one closer than the last?
                        float dist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
                        if (dist < closestDist) {
                            // we found a closer one, use it
                            closest = supply;
                            closestDist = dist;
                        }
                    }
                }
            }

            if (closest == null)
            {
                // no more guns - use melleee
                //backpack.weopon.weoponType = WeoponType.Melee;
                //backpack.weopon.damage = 1;
                return false;
            }
             

            targetWeoponSupply = closest;
            target = targetWeoponSupply.gameObject;
            targetPosition = target.transform.position;
            return true;
        }

        public override bool perform(GameObject agent)
        { 
            if (startTime == 0)
                startTime = Time.time;

            if (Time.time - startTime > waitTime)
            {
                if (backpack == null) backpack = agent.GetComponent<BackpackComponent>();

                if (targetWeoponSupply.numberOfWeopons > 0)
                {
                    targetWeoponSupply.numberOfWeopons--;


                    backpack.weopon.weoponType = targetWeoponSupply.weoponType;
                    backpack.weopon.damage = targetWeoponSupply.damage;
                    gotWeopon = true;
                    //return true;
                }
                else
                {    
                    gotWeopon = false;
                    Debug.Log(string.Format("no weopons here! Someone got there first! {0}" , agent.name));
                    //Destroy(targetWeoponSupply.gameObject);
                    targetWeoponSupply.gameObject.SetActive(false);
                    return false; // no weopons here! Someone got there first!
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