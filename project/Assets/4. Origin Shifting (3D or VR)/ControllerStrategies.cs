using UnityEngine;

namespace Sammoh.Four
{
    public interface IControlStrategy
    {
        void Control(PlaneController plane);
        Vector3 Postion { get; }
        Vector3 Rotation { get; }
    }
}

namespace Sammoh.Four
{
    public class PlayerControlStrategy : IControlStrategy
    {
        private float forwardSpeed = 5f;
        private float turnSpeed = 0.5f;
        
        public float speed = 100.0f;
        public float maxSpeed = 500.0f;
        public float minSpeed = 100.0f;
        public float yawSpeed = 100.0f;
        public float rollSpeed = 25.0f;
        
        private float currentSpeed;
        public void Control(PlaneController plane)
        {
            Postion = HandleMovement(plane);
            var yaw = HandleYaw(plane);
            
            var transform = plane.transform;
            var rotations = new Vector3(0, yaw, 0);
            
            Rotation = rotations;
            transform.Rotate(rotations);
        }

        public Vector3 Postion { get; private set; }
        public Vector3 Rotation { get; private set; }

        Vector3 HandleMovement(PlaneController plane)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                currentSpeed += speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                currentSpeed -= speed * Time.deltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
            plane.transform.position += plane.transform.forward * (currentSpeed * Time.deltaTime);
            return plane.transform.position;
        }

        float HandleYaw(PlaneController plane)
        {
            float yaw = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                yaw = -yawSpeed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                yaw = yawSpeed * Time.deltaTime;
            }
            
            return yaw;
        }
    }
}

namespace Sammoh.Four
{
    public class AIControlStrategy : IControlStrategy
    {
        private float forwardSpeed = 5f;
        private float turnSpeed = 0.5f;
        
        public float speed = 100.0f;
        public float maxSpeed = 500.0f;
        public float minSpeed = 100.0f;
        public float yawSpeed = 100.0f;
        public float rollSpeed = 25.0f;
        
        public void Control(PlaneController plane)
        {
            // this should be the AI control logic
            // simple look towards and follow the player plane
            
            // get the player plane
            var playerPlane = PlaneManager.Instance.PlayerPlane;
            if (playerPlane == null) return;
            
            // get the direction to the player plane
            var direction = playerPlane.transform.position - plane.transform.position;
            var rotation = Quaternion.LookRotation(direction);
            
            // rotate towards the player plane
            plane.transform.rotation = Quaternion.Slerp(plane.transform.rotation, rotation, Time.deltaTime);
            
            // if the plane is too close, slow down
            var distance = Vector3.Distance(playerPlane.transform.position, plane.transform.position);
            if (distance < 20)
            {
                speed = 100.0f;
            }
            else
            {
                speed = 500.0f;
            }
            
            // move forward
            plane.transform.position += plane.transform.forward * (speed * Time.deltaTime);
            
            
            Postion = plane.transform.position;
            Rotation = plane.transform.rotation.eulerAngles;
        }

        public Vector3 Postion { get; private set; }
        public Vector3 Rotation { get; private set; }
    }
}