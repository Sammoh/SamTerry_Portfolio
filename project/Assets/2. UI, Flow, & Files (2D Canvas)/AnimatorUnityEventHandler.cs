using UnityEngine;

/// <summary>
/// Handles Unity events for controlling the Animator component.
/// </summary>
namespace Sammoh.Two
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorUnityEventHandler : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private string boolName = "IsOn";

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Sets the specified boolean parameter in the Animator.
        /// </summary>
        /// <param name="value">The value to set the boolean parameter to.</param>
        public void SetBool(bool value)
        {
            if (_animator == null)
            {
                Debug.LogError("Animator component is not assigned.");
                return;
            }

            _animator.SetBool(boolName, value);
        }
    }
}