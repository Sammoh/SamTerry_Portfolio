using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This class is used to rotate the knob and snap it.
/// It will snap to different given positions while it's interacted with.
/// It comes with an image slider that can be used to keep track of the current value.
///
/// NOTES
/// instead, get the lowest value and add the offset to it.

/// </summary>

namespace Sammoh.Three
{
    [RequireComponent(typeof(KnobLabelManager))]
    public class RotatableKnob : MonoBehaviour, IInteractable
    {
        [SerializeField] private string KnobID;

        [Tooltip("The list of snap positions that the knob will snap to.")] [SerializeField]
        private List<SnapPosition> knobSnapPositions = new();

        [Tooltip("The snap range that the knob will snap to.")] [SerializeField]
        private SnapRange snapRange;

        [Tooltip("The transform of the handle that will be rotated.")] [SerializeField]
        private Transform handleTransform;

        [SerializeField] private float testResetValue = 2;

        private float _currentValue;
        private float _previousValue;
        private string _currentID;

        private readonly List<ISnapValue> _snapValues = new();

        Coroutine _delayCoroutine = null;

        public List<ISnapValue> SnapValues => _snapValues;

        public event Action OnInteraction;
        public event Action<float> OnValueChanged;
        public event Action<float> OnValueChangeFinished;

        [SerializeField] KnobLabelManager _knobLabelManager;

        void Awake()
        {
            // adding the list of snap positions to the snap values.
            foreach (var snapPosition in knobSnapPositions)
                _snapValues.Add(snapPosition);
            // adding the snap range to the snap values.
            _snapValues.Add(snapRange);

            if (_knobLabelManager == null)
            {
                _knobLabelManager = GetComponent<KnobLabelManager>();
            }

            _knobLabelManager.Initialize(this, _snapValues);
        }

        #region Interactions

        public void OnStartInteraction()
        {
            OnInteraction?.Invoke();
        }

        public void OnEndInteraction()
        {
            if (_currentID != "Test") return;

            if (_delayCoroutine != null)
            {
                StopCoroutine(_delayCoroutine);
            }
            else
                _delayCoroutine = StartCoroutine(ChangeToOffAfterDelay(_currentID));
        }

        #endregion

        // TODO the snap value should be the closest snap value to the current value.
        // FIX the currentRotation does not allow for the current rotation to escape the snap range.
        // TODO the snap value should be the closest snap value to the current value.
        private float GetSnappedRotation(float currentRotation)
        {
            foreach (var snap in _snapValues)
            {
                currentRotation = snap.SnapValue(currentRotation);
            }
            
            return currentRotation;
        }

        private IEnumerator ChangeToOffAfterDelay(string id)
        {
            if (id != "Test") yield break;

            yield return new WaitForSeconds(testResetValue); // Wait for 1 second or your desired delay
            Debug.Log("Changing to Off");
            SetValueID("Off");

            var offSnapPosition = GetSnapPositionByID("Off");
            SetValue(offSnapPosition.Value);
            
            _delayCoroutine = null;
        }

        #region Setters

        public void SetValueID(string id)
        {
            _currentID = id;
        }

        public void SetValue(float value)
        {
            var snappedRotation = GetSnappedRotation(value);
            var snapID = _snapValues.FirstOrDefault(snap => snap.GetSnapID(snappedRotation) != "");

            _currentID = snapID?.ID ?? "";
            _currentValue = snappedRotation;

            // add new rotation to the handle
            if (handleTransform)
                handleTransform.localRotation = Quaternion.Euler(0, snappedRotation, 0);
            
            _previousValue = _currentValue;

            // double check this because it might be timed wrong.
            OnValueChanged?.Invoke(_currentValue);
        }

        public void SetSnappers(List<ISnapValue> snapValues)
        {
            _snapValues.Clear();
            _snapValues.AddRange(snapValues);
        }

        #endregion

        #region Getters

        public float GetValue()
        {
            return _currentValue;
        }

        // TODO if given and invaid id, should not return the invalid id
        // probably in the form on an enum
        public string GetValueID()
        {
            return _currentID;
        }

        public float GetSnapValue(int index)
        {
            return _snapValues[index].SnapValue(_currentValue);
        }

        public ISnapValue GetSnapPositionByID(string ignition)
        {
            return _snapValues.FirstOrDefault(snap => snap.ID == ignition);
        }

        #endregion

        #region Debugging
        
        

        // consider only the selected object
        // private void OnDrawGizmosSelected()
        private void OnDrawGizmosSelected()
        {
            DrawDebugLinesForSnappedValues();
            DrawDebugHeading();
            DrawDebugArch();
            DrawDebugHeadingDistance();
        }
        void DrawDebugLinesForSnappedValues()
        {
            float innerRadius = 0.5f; // Example inner radius
            float outerRadius = 1f; // Example outer radius
            Vector3 knobCenter = transform.localPosition; // Assuming this script is attached to the knob
            float knobHeading = transform.rotation.eulerAngles.y;

            foreach (var snapPosition in knobSnapPositions)
            {
                // Calculate direction based on the knob heading
                var headingOffset = knobHeading + snapPosition.Value;
                Vector3 direction = Quaternion.Euler(0, headingOffset, 0) * Vector3.forward;
                // snap distance direction is the same as the snap position direction with a different offset.
                var snapDistanceHeadingOffset = knobHeading + snapPosition.Value + snapPosition.SnapDistance;
                var snapDistanceHeadingOffset1 = knobHeading + snapPosition.Value - snapPosition.SnapDistance;
                var snapDistanceDirection = Quaternion.Euler(0, snapDistanceHeadingOffset, 0) * Vector3.forward;
                var snapDistanceDirection1 = Quaternion.Euler(0, snapDistanceHeadingOffset1, 0) * Vector3.forward;

                // Calculate start and end points
                Vector3 startPoint = knobCenter + direction * innerRadius;
                Vector3 endPoint = knobCenter + direction * outerRadius;
                Vector3 endLine = knobCenter + direction * outerRadius * 0.9f;
                
                // Draw the debug line
                Debug.DrawLine(startPoint, endLine, Color.red, 0.1f); // Last parameter is duration the line should be visible

                
                // Calculate snap distance
                Vector3 snapDistanceStartPoint = knobCenter + snapDistanceDirection * innerRadius;
                Vector3 snapDistancePoint = knobCenter + snapDistanceDirection * outerRadius;
                
                // Calculate snap 
                Vector3 snapDistance1StartPoint = knobCenter + snapDistanceDirection1 * innerRadius;
                Vector3 snapDistance1Point = knobCenter + snapDistanceDirection1 * outerRadius;
                
                
                // Calculate and draw distance of snap distance.
                // Draw the debug line on either side of the snap position
                Debug.DrawLine(snapDistanceStartPoint, snapDistancePoint, Color.yellow, 0.1f); // Last parameter is duration the line should be visible
                Debug.DrawLine(snapDistance1StartPoint, snapDistance1Point, Color.yellow, 0.1f); // Last parameter is duration the line should be visible
                
                // label the snap position
                Handles.Label((endPoint), snapPosition.ID.ToString());
            }
        }
        
        // Draw an arch to show the range of the knob from green to blue
        private void DrawDebugArch()
        {
            var innerRadius = 0.5f; // Example inner radius
            var outerRadius = 1f; // Example outer radius
            var knobCenter = transform.position; // Assuming this script is attached to the knob
            var knobHeading = transform.rotation.eulerAngles.y;

            // Calculate direction based on the knob heading for MinValue
            var headingOffset = knobHeading + snapRange.MinValue;
            var direction = Quaternion.Euler(0, headingOffset, 0) * Vector3.forward;

            // Calculate start and end points for MinValue
            var startPoint = knobCenter + direction * innerRadius;
            var endPoint = knobCenter + direction * outerRadius;

            // Draw the debug line for MinValue
            Debug.DrawLine(startPoint, endPoint, Color.green, 0.1f); // Last parameter is duration the line should be visible

            // Add label for MinValue
            Handles.Label(endPoint, $"Min: {snapRange.MinValue}");

            // Calculate direction based on the knob heading for MaxValue
            headingOffset = knobHeading + snapRange.MaxValue;
            direction = Quaternion.Euler(0, headingOffset, 0) * Vector3.forward;

            // Calculate start and end points for MaxValue
            startPoint = knobCenter + direction * innerRadius;
            endPoint = knobCenter + direction * outerRadius;

            // Draw the debug line for MaxValue
            Debug.DrawLine(startPoint, endPoint, Color.blue, 0.1f); // Last parameter is duration the line should be visible

            // Add label for MaxValue
            Handles.Label(endPoint, $"Max: {snapRange.MaxValue}");
        }
        
        private void DrawDebugHeading()
        {
            // get the knob current heading.
            var yRotation = handleTransform.rotation.eulerAngles.y;

            var knobCenter = handleTransform.position;
            var radius = 0.75f;

            var direction = Quaternion.Euler(0, yRotation, 0) * Vector3.forward;
            var endPoint = knobCenter + direction * radius;

            Debug.DrawLine(knobCenter, endPoint, Color.green, 0.01f);
        }
        
        private void DrawDebugHeadingDistance()
        {
            // Get the knob current heading.
            var yRotation = handleTransform.rotation.eulerAngles.y;
            var knobCenter = handleTransform.position;
            var radius = 1f;

            var dir = Quaternion.Euler(0, yRotation + _currentValue, 0) * Vector3.forward;

            // Calculate end point for the closest snap position
            var endPoint = knobCenter + dir * radius;

            // Draw a debug line from the previous heading to the current heading
            var previousDir = Quaternion.Euler(0, yRotation + _previousValue, 0) * Vector3.forward;
            var previousEndPoint = knobCenter + previousDir * radius;
            Debug.DrawLine(previousEndPoint, endPoint, Color.cyan, 0.1f); // Last parameter is duration the line should be visible

            // Update the previous heading
            // _previousValue = yRotation;
        }
        
        #endregion

    }
}