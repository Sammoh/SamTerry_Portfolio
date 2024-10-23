// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(SnapPosition))]
// public class KnobSnapPositionEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//
//         // Draw the default inspector for snapType
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("snapType"), new GUIContent("Snap Type"));
//
//         // Get the current value of snapType
//         SnapType snapType = (SnapType)serializedObject.FindProperty("snapType").enumValueIndex;
//
//         // Conditionally draw additional fields based on snapType
//         switch (snapType)
//         {
//             case SnapType.Position:
//                 // Draw fields relevant to Position
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("id"), new GUIContent("ID"));
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("Value"));
//                 break;
//             case SnapType.Range:
//                 // Draw fields relevant to Range
//                 // Assuming you have additional fields for Range, draw them here
//                 EditorGUILayout.LabelField("Range specific fields here");
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("minValue"), new GUIContent("Min Value"));
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("maxValue"), new GUIContent("Max Value"));
//
//                 break;
//         }
//
//         serializedObject.ApplyModifiedProperties();
//     }
// }