using UnityEngine;

namespace Sammoh.Three
{
    public class InputManager : MonoBehaviour
    {
        private Camera mainCamera;
        private IInteractable _interactable;
        private bool isInteracting = false;
        public bool IsInteracting => isInteracting;

        void Awake()
        {
            _interactable = GetComponent<IInteractable>();
            mainCamera = Camera.main;
        }

        void Update()
        {
#if UNITY_EDITOR
            HandleMouseRaycast();
            HandleMouseInput();
            HandleMouseRelease();
#else
        HandleVRInput();
        HandleMouseInput();

#endif
        }

        private void HandleVRInput()
        {
            
        }

        private void HandleMouseRaycast()
        {
            if (Input.GetMouseButtonDown(0))
            {
                int layerMask = 1 << 8;
                layerMask = ~layerMask;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    _interactable = hit.collider.GetComponent<IInteractable>();
                    if (_interactable != null)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                        _interactable.OnStartInteraction();

                        isInteracting = true;
                        _interactable.OnValueChanged += HandleValueChanged;
                        _interactable.OnValueChangeFinished += HandleValueChangeFinished;
                    }
                }
            }
        }

        private void HandleMouseInput()
        {
            if (isInteracting && _interactable != null)
            {
                float mouseX = Input.GetAxis("Mouse X");
                _interactable.SetValue(_interactable.GetValue() + mouseX);
            }
        }

        private void HandleMouseRelease()
        {
            if (Input.GetMouseButtonUp(0) && isInteracting)
            {
                isInteracting = false;
                if (_interactable != null)
                {
                    Debug.Log("Mouse released");
                    _interactable.OnValueChanged -= HandleValueChanged;
                    _interactable.OnValueChangeFinished -= HandleValueChangeFinished;
                    // Any additional logic to finalize the interaction
                    _interactable.OnEndInteraction();
                }
            }
        }

        private void HandleValueChanged(float value)
        {
            // Handle value change logic
            // Debug.LogError("Value changed to: " + value);
        }

        private void HandleValueChangeFinished(float value)
        {
            // Handle value change finished logic
            // Debug.LogError("Value change finished at: " + value);
        }
    }
}