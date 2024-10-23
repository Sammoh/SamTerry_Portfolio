using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.Two
{
    /// <summary>
    /// OpenReportController is responsible for loading and updating an existing report in the system.
    /// </summary>
    public class OpenReportController : MonoBehaviour
    {
        public static OpenReportController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<OpenReportController>();
                    if (_instance == null)
                    {
                        Debug.LogError("There is no OpenReportController in the scene. Creating a new one.");
                        var gameObject = new GameObject("OpenReportController");
                        _instance = gameObject.AddComponent<OpenReportController>();
                    }
                }

                return _instance;
            }
        }

        private static OpenReportController _instance;

        
        // these will be set on load.
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text emailText;
        [SerializeField] private TMP_Text type;
        [SerializeField] private TMP_Text state;
        [SerializeField] private TMP_Text date;


        // these are player inputs
        [SerializeField] private Button closeButton;
        [SerializeField] private Toggle priorityToggle;
        [SerializeField] private Button setReviewButton;
        [SerializeField] private Button setCompleteButton;

        private Report _currentReport;

        public void OnLoadReportButton(Report reportObject)
        {
            _currentReport = ReportManager.Instance.OpenReport(reportObject.Id);

            // TODO fix this
            // var text = reportObject.GetComponentInChildren<ReportObject>().ID;
            // text = $"Title: {report.Title}, Reporter: {report.ReporterEmail}, Date: {report.SubmissionDate}, Priority: {report.Priority}, Type: {report.Type}, State: {report.State}";

            if (_currentReport != null)
            {
                titleText.text = _currentReport.Title;
                emailText.text = _currentReport.ReporterEmail;

                priorityToggle.isOn = _currentReport.Priority == PriorityLevel.High;

                // fix these.
                type.text = _currentReport.Type;
                state.text = _currentReport.State.ToString();
                date.text = _currentReport.SubmissionDate.ToShortDateString();
                
                // set the buttons
                priorityToggle.onValueChanged.AddListener(OnPriorityToggle);
                setReviewButton.onClick.AddListener(OnSetToReview);
                setCompleteButton.onClick.AddListener(OnSetToComplete);

            }
            else
            {
                Debug.Log("Report not found.");
            }
            
            closeButton.onClick.AddListener(OnCloseButton);
        }

        private void OnSetToComplete()
        {
            _currentReport.State = ReportState.Complete;
            ReportManager.Instance.UpdateReport(_currentReport);
            ListReportsController.Instance.InitializeListController();
            UpdateUI();
        }

        private void OnSetToReview()
        {
            _currentReport.State = ReportState.Review;
            ReportManager.Instance.UpdateReport(_currentReport);
            UpdateUI();
        }

        private void OnPriorityToggle(bool value)
        {
            _currentReport.Priority = value ? PriorityLevel.High : PriorityLevel.Low;
            UpdateUI();
        }
        
        // update the ui with the current report.
        private void UpdateUI()
        {
            if (_currentReport != null)
            {
                titleText.text = _currentReport.Title;
                emailText.text = _currentReport.ReporterEmail;
                priorityToggle.isOn = _currentReport.Priority == PriorityLevel.High;
                type.text = _currentReport.Type;
                state.text = _currentReport.State.ToString();
                date.text = _currentReport.SubmissionDate.ToShortDateString();
            }
        }

        // todo action buttons should automatically save the changes.
        // changes should be made before saving. 
        // take out all changes from this method.
        private void OnSaveChangesButton()
        {
            if (_currentReport != null)
            {
                _currentReport.Priority = priorityToggle.isOn ? PriorityLevel.High : PriorityLevel.Low;
                // todo parse the state from a selection.
                // _currentReport.State = state.ToString();

                ReportManager.Instance.UpdateReport(_currentReport);
            }
        }

        private void OnCloseButton()
        {
            // update the list of reports.
            ListReportsController.Instance.InitializeListController();
            // turn off the open report controller.
            gameObject.SetActive(false);
        }
    }
}