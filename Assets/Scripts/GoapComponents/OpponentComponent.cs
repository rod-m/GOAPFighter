using GoapEnemy;
using UnityEngine;

namespace GoapComponents
{
    public class OpponentComponent : MonoBehaviour
    {
        [SerializeField]
        private int health = 100;
        private int startHealth = 100;
        public float healthPercent = 100f;
        public GameObject attacker;
        private GameObject _iamUnderFire = null;

        void Awake()
        {

            startHealth = health;
        }
        public GameObject IamUnderFire
        {
            get
            {
                
                return _iamUnderFire;
            }
            set
            {
                attacker = _iamUnderFire;
                _iamUnderFire = value;
            }
        }

        public int Health
        {
            get { return health; }
            set
            {
                
                health = value;
                if (health <= 0)
                {
                    health = 0;
                    GoapEnemyAI me = GetComponent<GoapEnemyAI>();
                    if (me != null)
                    {
                        me.AnimatorBool("Boxing", false);
                        me.AnimatorTrigger("Die");
                       
                        
                    }
                    else
                    {
                        //Get the Renderer component from the new cube
                         var cubeRenderer = GetComponent<Renderer>();

                        //Call SetColor using the shader property name "_Color" and setting the color to red
                        cubeRenderer.material.SetColor("_Color", Color.red);
                        Destroy(gameObject, 2f);
                    }

                    IamUnderFire = null;
                   // this.gameObject.SetActive(false);
                }
                
                healthPercent = Mathf.Round(100f * health / startHealth);
            }
        }
    }
}