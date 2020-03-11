using UnityEngine;
using System.Collections;

namespace GoapAI
{

	public interface FSMState
	{
		void Update(FSM fsm, GameObject gameObject);
	}

}