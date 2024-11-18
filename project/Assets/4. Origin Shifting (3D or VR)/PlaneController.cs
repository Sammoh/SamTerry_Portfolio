using UnityEngine;

namespace Sammoh.Four
{
    public class PlaneController : MonoBehaviour
    {
        public Rigidbody Rigidbody { get; private set; }
        private IControlStrategy controlStrategy;

        void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            controlStrategy?.Control(this);
        }

        public void SetControlStrategy(IControlStrategy strategy)
        {
            controlStrategy = strategy;
        }
    }
}