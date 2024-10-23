using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sammoh.Two
{
    /// <summary>
    /// NewReportController is responsible for creating a new report in the system.
    /// </summary>
    public class NewReportController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField titleInputField;
        [SerializeField] private TMP_InputField emailInputField;
        [SerializeField] private TMP_InputField dateInputField;
        [SerializeField] private Toggle priorityToggle;
        [SerializeField] private Toggle typeToggle;
        [SerializeField] private TMP_Dropdown stateDropdown;
        [SerializeField] private DropDownClickHandler clickHandler;
        [SerializeField] private Button saveButton;

        private ReportState _currentState;

        private void Awake()
        {
            saveButton.onClick.AddListener(OnCreateReportButton);
            dateInputField.onValueChanged.AddListener(OnDateChanged);
            dateInputField.onSelect.AddListener(OnDateSelected);
            stateDropdown.onValueChanged.AddListener(OnStateChanged);
            clickHandler.Clicked += OnClicked;
        }

        public void Initialize()
        {
            // remove all the options from the text, dropdown, and toggles.
            titleInputField.text = string.Empty;
            emailInputField.text = string.Empty;
            dateInputField.text = string.Empty;
            priorityToggle.isOn = false;
            typeToggle.isOn = false;
            stateDropdown.value = 0;
        }

        private void OnClicked()
        {
            Debug.Log("Clicked State Dropdown");
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log(this.gameObject.name + " was selected");
        }

        private void OnStateChanged(int stateId)
        {
            _currentState = (ReportState)stateId;
        }

        private void OnDateChanged(string arg0)
        {
            // check the string and make sure it follows the rules: 01/12/2024
        }

        private void OnDateSelected(string arg0)
        {
        }

        private void OnCreateReportButton()
        {
            // check to make sure there is content in the fields.
            if (string.IsNullOrEmpty(titleInputField.text) || string.IsNullOrEmpty(emailInputField.text))
            {
                Debug.LogError("Title and Email fields are required.");
                return;
            }

            var report = new Report
            {
                Title = titleInputField.text,
                ReporterEmail = emailInputField.text,
                SubmissionDate = DateTime.Now,
                // short string date to string
                Description = DateTime.Now.ToShortDateString(),
                Priority = priorityToggle.isOn ? PriorityLevel.High : PriorityLevel.Low,
                Type = typeToggle.isOn ? "External" : "Internal",
                State = _currentState
            };

            if (string.IsNullOrEmpty(report.Id))
            {
                report.Id = GenerateUniqueId();
            }

            ReportManager.Instance.CreateNewReport(report);
            Debug.Log("Report created successfully.");

        }


        /// <summary>
        /// Generates a unique identifier for a report.
        /// </summary>
        /// <returns>A unique identifier string.</returns>
        private string GenerateUniqueId()
        {
            return $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

    }
}