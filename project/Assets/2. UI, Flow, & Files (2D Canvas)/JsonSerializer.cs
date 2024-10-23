using UnityEngine;

/// <summary>
/// Provides methods to serialize and deserialize reports to and from JSON format.
/// </summary>

namespace Sammoh.Two
{
    public class JsonSerializer : ISerializer
    {
        /// <summary>
        /// Serializes the given report to a JSON string.
        /// </summary>
        /// <param name="report">The report to serialize.</param>
        /// <returns>A JSON string representation of the report.</returns>
        public string Serialize(Report report)
        {
            return JsonUtility.ToJson(report, true);
        }

        /// <summary>
        /// Deserializes the given JSON string to a report.
        /// </summary>
        /// <param name="data">The JSON string representation of the report.</param>
        /// <returns>The deserialized report object.</returns>
        public Report Deserialize(string data)
        {
            return JsonUtility.FromJson<Report>(data);
        }
    }
}