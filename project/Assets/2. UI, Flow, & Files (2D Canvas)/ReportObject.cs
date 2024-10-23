using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.Two
{
    /// <summary>
    /// Represents a UI element for displaying report information.
    /// </summary>
    public class ReportObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text typeText;
        [SerializeField] private TMP_Text stateText;
        [SerializeField] private TMP_Text dateText;
        [SerializeField] private Button selectButton;

        private Report _report;
        public Action<Report> OnSelected;
        public Report Report => _report;


        private void Awake()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnClicked);
            }
            else
            {
                Debug.LogError("SelectButton is not assigned.");
            }
        }
        
        private void OnClicked()
        {
            OnSelected?.Invoke(_report);
        }

        /// <summary>
        /// Initializes the report object with the given report.
        /// </summary>
        /// <param name="report">The report to display.</param>
        public void Initialize(Report report)
        {
            _report = report;
            if (_report != null)
            {
                titleText.text = report.Title;
                typeText.text = report.Type;
                stateText.text = report.State.ToString();
                dateText.text = report.SubmissionDate.ToShortDateString();
            }
            else
            {
                Debug.LogError("Report is null.");
            }
        }

        public void CleanUp()
        {
            _report = null;
        }
    }
}