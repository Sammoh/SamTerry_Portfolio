using UnityEngine;

namespace Sammoh.Four
{
    public interface IControlStrategy
    {
        void Control(PlaneController plane);
    }
}

namespace Sammoh.Four
{
    public class PlayerControlStrategy : IControlStrategy
    {
        public void Control(PlaneController plane)
        {
            // float move = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal");

            plane.Rigidbody.AddForce(plane.transform.forward * 5f);
            plane.Rigidbody.AddTorque(Vector3.up * turn * 5f);
        }
    }
}

namespace Sammoh.Four
{
    public class AIControlStrategy : IControlStrategy
    {
        public void Control(PlaneController plane)
        {
            // Example simple AI behavior
            plane.Rigidbody.AddForce(plane.transform.forward * 5f);
        }
    }
}