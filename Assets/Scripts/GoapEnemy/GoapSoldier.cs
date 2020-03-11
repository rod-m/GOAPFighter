using System.Collections.Generic;
using UnityEngine;
using GoapAI;
using GoapComponents;
namespace GoapEnemy
{
    public class GoapSoldier : GoapEnemyAI
    {
        public override HashSet<KeyValuePair<string, object>> createGoalState()
        {
            HashSet<KeyValuePair<string,object>> goal = new HashSet<KeyValuePair<string,object>> ();
		
            //goal.Add(new KeyValuePair<string, object>("hasWeopon", true ));
          //  
           goal.Add(new KeyValuePair<string, object>("patrol", true ));
          //  goal.Add(new KeyValuePair<string, object>("killOpponent", true ));
           // goal.Add(new KeyValuePair<string, object>("retreat", true ));
            return goal;
        }


    }
}