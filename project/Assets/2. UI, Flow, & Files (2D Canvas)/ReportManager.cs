using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages the saving, loading, and listing of reports.
/// </summary>

namespace Sammoh.Two
{
    public class ReportManager : MonoBehaviour
    {
        private static ReportManager _instance;

        public static ReportManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ReportManager>();
                    if (_instance == null)
                    {
                        var gameObject = new GameObject("ReportManager");
                        _instance = gameObject.AddComponent<ReportManager>();
                    }
                }

                return _instance;
            }
        }

        private string FolderPath => Path.Combine(Application.persistentDataPath, "Reports");
        private ISerializer _serializer;

        [SerializeField] private bool useJson = true;

        private string FileFormat => useJson ? "json" : "xml";

        private void Awake()
        {
            _serializer = useJson ? new JsonSerializer() : new XmlSerializer();

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }

        /// <summary>
        /// Lists all reports in the folder.
        /// </summary>
        /// <returns>A list of all reports.</returns>
        public List<Report> ListReports()
        {
            var reports = new List<Report>();
            foreach (var file in Directory.GetFiles(FolderPath, $"*.{FileFormat}"))
            {
                var data = File.ReadAllText(file);
                reports.Add(_serializer.Deserialize(data));
            }

            return reports;
        }

        /// <summary>
        /// Opens a report with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the report to open.</param>
        /// <returns>The opened report, or null if not found.</returns>
        public Report OpenReport(string id)
        {
            var filePath = Path.Combine(FolderPath, $"{id}.{FileFormat}");
            if (File.Exists(filePath))
            {
                var data = File.ReadAllText(filePath);
                return _serializer.Deserialize(data);
            }

            return null;
        }

        /// <summary>
        /// Saves the specified report.
        /// </summary>
        /// <param name="report">The report to save.</param>
        private void SaveReport(Report report)
        {
            var filePath = Path.Combine(FolderPath, $"{report.Id}.{FileFormat}");
            var data = _serializer.Serialize(report);
            File.WriteAllText(filePath, data);
            Debug.Log($"File saved to: {filePath}");
        }

        /// <summary>
        /// Creates a new report.
        /// </summary>
        /// <param name="report">The report to create.</param>
        public void CreateNewReport(Report report)
        {
            SaveReport(report);
        }

        /// <summary>
        /// Updates an existing report.
        /// </summary>
        /// <param name="report">The report to update.</param>
        public void UpdateReport(Report report)
        {
            SaveReport(report);
        }
    }
}