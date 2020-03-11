using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoapAI
{

/**
	 * Used for building up the graph and holding the running costs of actions.
	 */
    public class Node
    {
        public Node parent; // links to another node - parent > children
        public float runningCost;
        public HashSet<KeyValuePair<string, object>> state;
        public GoapAction action;

        public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, GoapAction action)
        {
            this.parent = parent;
            this.runningCost = runningCost;
            this.state = state;
            this.action = action;
        }
    }
}