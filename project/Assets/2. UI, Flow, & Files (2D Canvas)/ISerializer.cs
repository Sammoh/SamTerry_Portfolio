
namespace Sammoh.Two
{
    /// <summary>
    /// Defines methods for serializing and deserializing reports.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given report to a string.
        /// </summary>
        /// <param name="report">The report to serialize.</param>
        /// <returns>A string representation of the report.</returns>
        string Serialize(Report report);

        /// <summary>
        /// Deserializes the given string to a report.
        /// </summary>
        /// <param name="data">The string representation of the report.</param>
        /// <returns>The deserialized report object.</returns>
        Report Deserialize(string data);
    }
}