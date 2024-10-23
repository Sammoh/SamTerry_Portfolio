using System.IO;
using System.Xml.Serialization;

namespace Sammoh.Two
{
    /// <summary>
    /// Provides methods to serialize and deserialize reports to and from XML format.
    /// </summary>
    public class XmlSerializer : ISerializer
    {
        private readonly System.Xml.Serialization.XmlSerializer _xmlSerializer =
            new System.Xml.Serialization.XmlSerializer(typeof(Report));

        /// <summary>
        /// Serializes the given report to an XML string.
        /// </summary>
        /// <param name="report">The report to serialize.</param>
        /// <returns>An XML string representation of the report.</returns>
        public string Serialize(Report report)
        {
            using (var stringWriter = new StringWriter())
            {
                _xmlSerializer.Serialize(stringWriter, report);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Deserializes the given XML string to a report.
        /// </summary>
        /// <param name="data">The XML string representation of the report.</param>
        /// <returns>The deserialized report object.</returns>
        public Report Deserialize(string data)
        {
            using (var stringReader = new StringReader(data))
            {
                return (Report)_xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}