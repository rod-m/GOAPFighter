using UnityEngine;
namespace GoapComponents
{
    [System.Serializable]
    public class Weopon
    {
        public WeoponType weoponType = WeoponType.None;
        public int damage = 1;
    }
    public class BackpackComponent : MonoBehaviour
    {
       // public WeoponSupplyComponent weoponSupplyComponent;

        public Weopon weopon = new Weopon();
        public int ammo = 0;
        private void Awake()
        {
            
        }
    }
}