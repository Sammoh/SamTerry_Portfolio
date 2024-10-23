using UnityEngine;

[System.Serializable]
public struct SnapRange : ISnapValue
{
    public string ID => snapID;
    public float Value { get; }
    public float SnapDistance => snapDistance;

    public float MinValue => minValue;
    public float MaxValue => maxValue;

    [SerializeField] private string snapID;
    [SerializeField] private  float minValue;
    [SerializeField] private  float maxValue;
    [SerializeField] private  float increment;
    [SerializeField] private float snapDistance; // the amount of distance before snapping. This should be outside the range.


    public SnapRange(string id, float minValue, float maxValue, float increment)
    {
        // Set the ID, min value, max value, and increment of the snap range
        this.snapID = id;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.increment = increment;
        
        // Value should be the average of the min and max values
        Value = (minValue + maxValue) / 2;
        snapDistance = 1;
    }
    
    public float SnapValue(float value)
    {
        // Handle free rotation and snap if it's close to a snap position
        // If it's within the range, snap to the nearest snap value
        if (value > minValue - snapDistance && value < maxValue + snapDistance)
        {
            float snapValue = Mathf.Round(value / increment) * increment;
            return snapValue;
        }

        return value;
    }

    public string GetSnapID(float value)
    {
        // if the value is within the range, return the snap ID
        return value > minValue && value < maxValue ? ID : "";
    }
}