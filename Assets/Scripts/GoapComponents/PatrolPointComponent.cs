using UnityEngine;
namespace GoapComponents
{
    public class PatrolPointComponent : MonoBehaviour
    {
        public float lastVisitTime = 0f;

        void Awake()
        {
            lastVisitTime = Time.time;
        }
        
    }
}