using UnityEngine;
using System.Collections.Generic;

namespace GoapAI
{

	public abstract class GoapAction : MonoBehaviour
	{
		private HashSet<KeyValuePair<string, object>> preconditions;
		private HashSet<KeyValuePair<string, object>> effects;

		private bool inRange = false;
		public float cost = 1f;

		//An action often has to perform on an object. This is that object. Can be null
		public GameObject target;
		public Vector3 targetPosition = Vector3.zero;

		public GoapAction()
		{
			preconditions = new HashSet<KeyValuePair<string, object>>();
			effects = new HashSet<KeyValuePair<string, object>>();
		}

		public void doReset()
		{
			inRange = false;
			target = null;
			reset();
		}

		public abstract void reset(); //reset any vars that need resetting
		public abstract bool isDone(); //is the action done

		// Procedurally check if this action can run. Not all actions will need this, but some might
		public abstract bool checkProceduralPrecondition(GameObject agent);

		/**
		 * Run the action.
		 * Returns True if the action performed successfully or false
		 * if something happened and it can no longer perform. In this case
		 * the action queue should clear out and the goal cannot be reached.
		 */
		public abstract bool perform(GameObject agent);

		/**
		 * Does this action need to be within range of a target game object?
		 * If not then the moveTo state will not need to run for this action.
		 */
		public abstract bool requiresInRange();


		// Are we in range of the target?
		// The MoveTo state will set this and it gets reset each time this action is performed.
		public bool isInRange()
		{
			return inRange;
		}

		public void setInRange(bool inRange)
		{
			this.inRange = inRange;
		}

		public void addPrecondition(string key, object value)
		{
			preconditions.Add(new KeyValuePair<string, object>(key, value));
		}

		public void removePrecondition(string key)
		{
			KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
			foreach (KeyValuePair<string, object> kvp in preconditions)
			{
				if (kvp.Key.Equals(key))
					remove = kvp;
			}

			if (!default(KeyValuePair<string, object>).Equals(remove))
				preconditions.Remove(remove);
		}


		public void addEffect(string key, object value)
		{
			effects.Add(new KeyValuePair<string, object>(key, value));
		}


		public void removeEffect(string key)
		{
			KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
			foreach (KeyValuePair<string, object> kvp in effects)
			{
				if (kvp.Key.Equals(key))
					remove = kvp;
			}

			if (!default(KeyValuePair<string, object>).Equals(remove))
				effects.Remove(remove);
		}


		public HashSet<KeyValuePair<string, object>> Preconditions
		{
			get { return preconditions; }
		}

		public HashSet<KeyValuePair<string, object>> Effects
		{
			get { return effects; }
		}

	
	}
}