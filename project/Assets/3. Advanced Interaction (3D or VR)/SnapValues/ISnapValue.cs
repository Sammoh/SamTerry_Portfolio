public interface ISnapValue
{
    string ID { get; }
    float  Value { get; }
    float SnapDistance { get; }
    float SnapValue(float value); // when this is given a value, it will return the snapped value
    string GetSnapID(float value); // when this is given a value, it will return the snap ID if it meets the criteria
}