using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct SnapPosition : ISnapValue
{
    public string ID => snapID;
    public float Value => snapPosition;
    public float SnapDistance => snapDistance;

    [SerializeField] private string snapID;
    [SerializeField] private float snapDistance; // the amount of distance before snapping.
    [SerializeField] private float snapPosition; 
    
    public SnapPosition(string id, float snapPosition, float snapDistance)
    {
        // Set the ID and value of the snap position
        this.snapID = id;
        this.snapPosition = snapPosition;
        this.snapDistance = snapDistance;
    }

    public float SnapValue(float value)
    {
        // Handle free rotation and snap if it's close to a snap position
        if (Mathf.Abs(value - snapPosition) < snapDistance)
        {
            return snapPosition;
        }

        return value;
    }

    public string GetSnapID(float value)
    {
        // if the value is within the range, return the snap
        var inRange = Mathf.Abs(value - snapPosition) < 0.5f ? ID : "";
        if (inRange == ID)
        {
            Debug.Log($"Snap ID: {inRange}");
        }
        return inRange;    
    }
}