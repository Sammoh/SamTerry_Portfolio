using UnityEngine;
using System.Collections.Generic;

namespace SpecularTheory.Four
{
    public class OriginShifting : MonoBehaviour
    {
        public Transform currentCharacter; // The currently controlled character
        public float thresholdDistance = 1000f; // The distance at which to shift the origin
        private List<Transform> allCharacters = new List<Transform>(); // List to track all characters

        private Dictionary<Transform, Vector3>
            previousPositions = new Dictionary<Transform, Vector3>(); // Track previous positions

        private float lastShiftTime = 0f; // Track the time of the last shift

        void Update()
        {
            if (currentCharacter == null)
                return;

            Vector3 characterPosition = currentCharacter.position;

            if (characterPosition.magnitude > thresholdDistance)
            {
                ShiftOrigin(characterPosition);
            }
        }

        public void SwitchCharacter(Transform newCharacter)
        {
            if (currentCharacter != null && !allCharacters.Contains(currentCharacter))
            {
                allCharacters.Add(currentCharacter);
                if (!previousPositions.ContainsKey(currentCharacter))
                {
                    previousPositions[currentCharacter] = currentCharacter.position;
                }
            }

            currentCharacter = newCharacter;

            if (!allCharacters.Contains(currentCharacter))
            {
                allCharacters.Add(currentCharacter);
            }

            if (!previousPositions.ContainsKey(currentCharacter))
            {
                previousPositions[currentCharacter] = currentCharacter.position;
            }
        }

        void ShiftOrigin(Vector3 shiftAmount)
        {
            float currentTime = Time.time;
            float timeElapsed = currentTime - lastShiftTime;
            lastShiftTime = currentTime;

            // Shift the position of all objects in the scene
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (!allCharacters.Contains(obj.transform))
                {
                    Vector3 originalPosition = obj.transform.position;
                    obj.transform.position -= shiftAmount;
                    Vector3 newPosition = obj.transform.position;
                    Debug.Log($"{obj.name} shifted from {originalPosition} to {newPosition}");
                }
            }

            // Reset the position of all characters
            foreach (Transform character in allCharacters)
            {
                Vector3 originalPosition = character.position;
                character.position -= shiftAmount;
                Vector3 newPosition = character.position;

                // Calculate and log speed in km/h
                if (previousPositions.ContainsKey(character) && timeElapsed > 0)
                {
                    float distance = Vector3.Distance(previousPositions[character], newPosition);
                    float speedKmh = (distance / timeElapsed) * 3.6f; // Convert m/s to km/h
                    Debug.Log(
                        $"{character.name} shifted from {originalPosition} to {newPosition} with speed {speedKmh} km/h");
                }

                previousPositions[character] = newPosition;
            }

            // Ensure the current character is centered
            currentCharacter.position = Vector3.zero;
            Debug.Log($"{currentCharacter.name} reset to origin");
        }
    }
}