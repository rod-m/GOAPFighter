using UnityEngine;

namespace GoapComponents
{
    public enum WeoponType
    {
        None,
        Melee,
        Gun

    }
    public class WeoponSupplyComponent : MonoBehaviour
    {
        public int damage = 100; // melee weopon 10, gun 100
        public int numberOfWeopons = 1;
        public WeoponType weoponType;


        private void Awake()
        {
//            switch (weoponType)
//            {
//                    case WeoponType.Melee:
//                        damage = 10;
//                        break;
//                    case WeoponType.Gun:
//                        damage = 100;
//                        break;
//            }
        }
    }
}