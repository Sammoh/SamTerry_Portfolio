
using System;
using UnityEngine;
using Sammoh.Four;

namespace Sammoh.Four
{
    public class FlightController : MonoBehaviour
    {

        #region Singleton

        private static FlightController _instance;

        public static FlightController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FlightController>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject("FlightController");
                        _instance = gameObject.AddComponent<FlightController>();
                        Debug.LogError($"Creating {gameObject.name}r");
                    }
                }

                return _instance;
            }
        }

        #endregion

        // public PlayerObject PlayerObject { get; set; }

        public float speed = 50.0f;
        public float maxSpeed = 500.0f;
        public float minSpeed = 50.0f;
        public float yawSpeed = 100.0f;
        public GameObject otherPlanePrefab;

        private float currentSpeed;
        private bool isControlled = true;
        private GameObject otherPlane;
        private bool isPrimaryPlane = true;

        public Action OnFlightUpdate;

        void Start()
        {
            currentSpeed = speed;
        }

        // TODO Make this work with the editor and with VR controls.
        void Update()
        {
            if (isControlled)
            {
                HandleMovement();
                HandleYaw();
            }

            if (Input.GetKeyDown(KeyCode.Space) && isPrimaryPlane)
            {
                SpawnSecondPlane();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SwitchControl();
            }
        }

        void HandleMovement()
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

            transform.position += transform.forward * (currentSpeed * Time.deltaTime);
        }

        void HandleYaw()
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

            transform.Rotate(0, yaw, 0);
        }

        void SpawnSecondPlane()
        {
            if (otherPlane == null)
            {
                otherPlane = Instantiate(otherPlanePrefab, transform.position + transform.forward * 10,
                    transform.rotation);
                otherPlane.GetComponent<FlightController>().SetControlled(false);
            }
        }

        void SwitchControl()
        {
            isControlled = !isControlled;
            if (otherPlane != null)
            {
                otherPlane.GetComponent<FlightController>().SetControlled(!isControlled);
                isPrimaryPlane = !isPrimaryPlane;
            }
        }

        public void SetControlled(bool controlled)
        {
            isControlled = controlled;
        }

        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }
    }
}
