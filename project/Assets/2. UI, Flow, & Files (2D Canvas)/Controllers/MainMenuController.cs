using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.Two
{
    public class MainMenuController : MonoBehaviour
    {
        public static MainMenuController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MainMenuController>();
                    if (_instance == null)
                    {
                        Debug.LogError("There is no MainMenuController in the scene. Creating a new one.");
                        var gameObject = new GameObject("MainMenuController");
                        _instance = gameObject.AddComponent<MainMenuController>();
                    }
                }

                return _instance;
            }
        }

        private static MainMenuController _instance;

        [SerializeField] private Button listReportsButton;
        [SerializeField] private Button newReportButton;

        [SerializeField] private OpenReportController _openReportController;
        [SerializeField] private ListReportsController _listReportsController;
        [SerializeField] private NewReportController _newReportController;

        private ReportManager _reportManager;

        private void Awake()
        {

            listReportsButton.onClick.AddListener(OnListReportsButton);
            newReportButton.onClick.AddListener(OnCreateNewReportButton);
        }

        private void Start()
        {
            _openReportController.gameObject.SetActive(false);
            _listReportsController.gameObject.SetActive(false);
            _newReportController.gameObject.SetActive(false);
        }

        private void OnListReportsButton()
        {
            Debug.Log("listing current reports");
            // a main menu item that will enable the list menu canvas.
            _listReportsController.InitializeListController();
            _listReportsController.gameObject.SetActive(true);
        }

        private void OnCreateNewReportButton()
        {
            Debug.Log("creating new report");
            // this will open a new prompt to create a new report.
            _newReportController.gameObject.SetActive(true);
            _newReportController.Initialize();
        }

        public void OpenReport(Report reportObject)
        {
            _openReportController.OnLoadReportButton(reportObject);
            _openReportController.gameObject.SetActive(true);

            // _listReportsController.gameObject.SetActive(false);
            // _listReportsController.DisableReports();
        }
    }
}