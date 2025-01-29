using UnityEngine;

namespace Sammoh.Four
{
    public class PlaneController : MonoBehaviour
    {
        public Vector3 Position => controlStrategy.Postion;
        public Vector3 Rotation => controlStrategy.Rotation;

        private IControlStrategy controlStrategy;

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