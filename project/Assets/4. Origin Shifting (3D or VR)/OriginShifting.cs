using System;
using UnityEngine;
using System.Collections.Generic;

//NOTE:
// 1. The script is attached to an empty GameObject in the scene
// 2. The script is used to shift the origin of the scene when the current character moves beyond a certain distance
// 3. The script keeps track of all characters in the scene and shifts their positions accordingly
// 4. The script also calculates and logs the speed of each character after shifting the origin
// 5. The script assumes that the current character is always centered after shifting the origin

// TODO:
// Spawn a new charcter in the scene and switch to it to test the origin shifting functionality
namespace Sammoh.Four
{
    public class OriginShifting : MonoBehaviour
    {
        public Transform currentCharacter; // The currently controlled character
        public float thresholdDistance = 1000f; // The distance at which to shift the origin

        private Dictionary<Transform, Vector3>
            previousPositions = new Dictionary<Transform, Vector3>(); // Track previous positions

        private float lastShiftTime = 0f; // Track the time of the last shift
        
        [SerializeField] private List<Transform> allCharacters = new List<Transform>(); // List to track all characters
        [SerializeField] private GameObject[] allObjects; // Track all objects in the scene
        
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
            // GameObject[] allObjects = FindObjectsOfType<GameObject>();

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