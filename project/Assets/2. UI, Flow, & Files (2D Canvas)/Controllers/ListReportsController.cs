using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// ListReportsController is responsible for displaying a list of reports in the UI.
    /// </summary>
    ///
    /// Need to have access to the content viewport.
    /// Scale it based on the amount of content that is recieved.
    public class ListReportsController : MonoBehaviour
    {
        private static ListReportsController _instance;

        public static ListReportsController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ListReportsController>();
                    if (_instance == null)
                    {
                        Debug.LogError("There is no ListReportsController in the scene. Creating a new one.");
                        var gameObject = new GameObject("ListReportsController");
                        _instance = gameObject.AddComponent<ListReportsController>();
                    }
                }

                return _instance;
            }
        }
        
        [SerializeField] private ReportObject reportPrefab;
        [SerializeField] private Transform contentPanel;

        [SerializeField] private TMP_Dropdown priorityDropdown;
        [SerializeField] private TMP_Dropdown stateDropdown;
        [SerializeField] private TMP_Dropdown dateDropdown;


        [SerializeField] private List<Report> currentReports;

        [SerializeField] private int initialPoolSize = 10;
        private ObjectPooler<ReportObject> _reportObjectPooler;

        private void InitializePooler()
        {
            if (_reportObjectPooler == null)
            {
                _reportObjectPooler = new ObjectPooler<ReportObject>(CreateMyComponent, initialPoolSize);
                Debug.Log("Initialized the pooler");
            }
        }

        private ReportObject CreateMyComponent()
        {
            return Instantiate(reportPrefab, contentPanel);
        }


        // TODO Desplay the reports when the user opens the window, not when the app starts...
        // do not want to destroy all of the objects.
        public void InitializeListController()
        {
            priorityDropdown.ClearOptions();
            priorityDropdown.AddOptions(new List<string> { "High", "Low" });
            stateDropdown.onValueChanged.AddListener(FilterByPriority);


            // initialize the dropdown with states
            stateDropdown.ClearOptions();
            stateDropdown.AddOptions(Enum.GetNames(typeof(ReportState)).ToList());


            // stateTempText.gameObject.SetActive(true);
            stateDropdown.captionText.gameObject.SetActive(false);
            stateDropdown.onValueChanged.AddListener(FilterByState);

            dateDropdown.ClearOptions();
            dateDropdown.AddOptions(new List<string> { "Ascending", "Descending" });
            dateDropdown.onValueChanged.AddListener(SortByDate);

            InitializePooler(); // Ensure pooler is initialized before use

            currentReports = ReportManager.Instance.ListReports();
            SortByDate(0);
        }

        /// <summary>
        /// Displays the list of reports.
        /// </summary>
        private void DisplayReports(List<Report> reports)
        {
            // Clear existing report objects
            foreach (Transform child in contentPanel)
            {
                var reportObject = child.GetComponent<ReportObject>();
                if (reportObject != null)
                {
                    _reportObjectPooler.ReturnToPool(reportObject);
                }
            }

            // Create new report objects from the pool
            foreach (var report in reports)
            {
                var reportObject = _reportObjectPooler.Get();
                reportObject.OnSelected += OnReportSelected;
                reportObject.Initialize(report);
            }
        }

        private void OnReportSelected(Report report)
        {
            var obj = _reportObjectPooler.Get();
            obj.OnSelected -= OnReportSelected;

            MainMenuController.Instance.OpenReport(report);
        }

        /// <summary>
        /// Filters the list of reports by priority.
        /// </summary>
        /// <param name="priority"></param>
        private void FilterByPriority(int priority)
        {
            var isPriority = priority == 0;
            var filteredReports = FilterReports(isPriority ? PriorityLevel.High : PriorityLevel.Low, null, null);
            DisplayReports(filteredReports);
        }

        /// <summary>
        /// Filters the list of reports by state.
        /// </summary>
        /// <param name="stateId"></param>
        private void FilterByState(int stateId)
        {
            stateDropdown.captionText.gameObject.SetActive(true);
            // stateTempText.gameObject.SetActive(false);

            var filteredReports = FilterReports(null, (ReportState)stateId, null);
            DisplayReports(filteredReports);
        }

        /// <summary>
        /// Filters the list of reports by date.
        /// </summary>
        /// <param name="date"></param>
        private void SortByDate(int stateId)
        {
            var ascending = stateId == 0;
            var sortedReports = OrderReports(SortOptions.Date, ascending);
            
            // _reportObjectPooler.Reorder(CompareBySubmissionDate);
            DisplayReports(sortedReports);
        }
        
        int CompareBySubmissionDate(ReportObject a, ReportObject b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;
            return a.Report.SubmissionDate.CompareTo(b.Report.SubmissionDate);
        }

        private List<Report> FilterReports(PriorityLevel? priority, ReportState? state, DateTime? startDate)
        {
            return currentReports
                .Where(r => !priority.HasValue || r.Priority == priority.Value)
                .Where(r => !state.HasValue || r.State == state.Value)
                .Where(r => !startDate.HasValue || r.SubmissionDate >= startDate.Value)
                .ToList();
        }

        private List<Report> OrderReports(SortOptions sortBy, bool ascending)
        {
            switch (sortBy)
            {
                case SortOptions.Priority:
                    return ascending
                        ? currentReports.OrderBy(r => r.Priority).ToList()
                        : currentReports.OrderByDescending(r => r.Priority).ToList();
                case SortOptions.State:
                    return ascending
                        ? currentReports.OrderBy(r => r.State).ToList()
                        : currentReports.OrderByDescending(r => r.State).ToList();
                case SortOptions.Date:
                    return ascending
                        ? currentReports.OrderBy(r => r.SubmissionDate).ToList()
                        : currentReports.OrderByDescending(r => r.SubmissionDate).ToList();
                default:
                    return currentReports;
            }
        }
        
        public enum SortOptions
        {
            Priority,
            State,
            Date
        }
    }
}