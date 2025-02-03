using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class OverlappingTransformFinder : EditorWindow
{
    // The parent GameObject to search under.
    GameObject parentObject = null;

    [MenuItem("Tools/Find Overlapping Transforms")]
    public static void ShowWindow()
    {
        // Open the window, with the title "Overlapping Transforms"
        GetWindow<OverlappingTransformFinder>("Overlapping Transforms");
    }

    void OnGUI()
    {
        GUILayout.Label("Find Overlapping Transforms", EditorStyles.boldLabel);

        // Allow the user to assign a parent object in the window.
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        if (GUILayout.Button("Find Overlapping Game Objects"))
        {
            if (parentObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a Parent GameObject.", "OK");
                return;
            }
            FindOverlappingTransforms();
        }
    }

    /// <summary>
    /// Finds all children under the assigned parent that have identical transform properties.
    /// </summary>
    void FindOverlappingTransforms()
    {
        // This dictionary will group game objects by a key that represents
        // their transform properties (position, rotation, scale).
        Dictionary<string, List<GameObject>> transformDict = new Dictionary<string, List<GameObject>>();

        // Get all child transforms (including inactive ones) under the parent.
        Transform[] children = parentObject.GetComponentsInChildren<Transform>(true);

        // Iterate over each child (skipping the parent itself)
        foreach (Transform t in children)
        {
            if (t.gameObject == parentObject)
                continue;

            // Create a string key based on the transform properties.
            // We use formatting ("F3") to round to three decimal places to avoid floating point imprecision issues.
            string key = t.position.ToString("F3") + "_" +
                         t.rotation.eulerAngles.ToString("F3") + "_" +
                         t.localScale.ToString("F3");

            if (!transformDict.ContainsKey(key))
            {
                transformDict[key] = new List<GameObject>();
            }
            transformDict[key].Add(t.gameObject);
        }

        // Collect all game objects that have overlapping transform values.
        List<GameObject> overlappingObjects = new List<GameObject>();
        foreach (var kvp in transformDict)
        {
            if (kvp.Value.Count > 1)
            {
                overlappingObjects.AddRange(kvp.Value);
            }
        }

        // If we found any overlapping objects, select them in the editor.
        if (overlappingObjects.Count > 0)
        {
            Selection.objects = overlappingObjects.ToArray();
            Debug.Log("Found " + overlappingObjects.Count + " overlapping objects.");
        }
        else
        {
            EditorUtility.DisplayDialog("Result", "No overlapping objects found.", "OK");
        }
    }
}
