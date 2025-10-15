using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Sammoh.Two
{
    /// <summary>
    /// CommandController provides a simple command system for the application.
    /// Currently supports /help command to list all available commands.
    /// </summary>
    public class CommandController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField commandInput;
        [SerializeField] private TMP_Text outputText;
        
        private Dictionary<string, CommandInfo> _commands;
        
        private struct CommandInfo
        {
            public string Command;
            public string Description;
            
            public CommandInfo(string command, string description)
            {
                Command = command;
                Description = description;
            }
        }
        
        private void Awake()
        {
            InitializeCommands();
            
            if (commandInput != null)
            {
                commandInput.onSubmit.AddListener(ProcessCommand);
            }
        }
        
        private void InitializeCommands()
        {
            _commands = new Dictionary<string, CommandInfo>
            {
                { "/help", new CommandInfo("/help", "Lists all available commands with descriptions") },
                { "/clear", new CommandInfo("/clear", "Clears the output text") },
                { "/list", new CommandInfo("/list", "Lists all reports in the system") },
                { "/new", new CommandInfo("/new", "Opens the new report creation screen") },
                { "/refresh", new CommandInfo("/refresh", "Refreshes the reports list") }
            };
        }
        
        private void ProcessCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;
                
            var command = input.Trim().ToLower();
            
            switch (command)
            {
                case "/help":
                    ShowHelp();
                    break;
                case "/clear":
                    ClearOutput();
                    break;
                case "/list":
                    ListReports();
                    break;
                case "/new":
                    CreateNewReport();
                    break;
                case "/refresh":
                    RefreshReports();
                    break;
                default:
                    ShowOutput($"Unknown command: {input}\nType /help to see available commands.");
                    break;
            }
            
            // Clear input after processing
            if (commandInput != null)
            {
                commandInput.text = string.Empty;
                commandInput.ActivateInputField();
            }
        }
        
        private void ShowHelp()
        {
            var helpText = "Available Commands:\n\n";
            
            foreach (var cmd in _commands.Values.OrderBy(c => c.Command))
            {
                helpText += $"{cmd.Command}\n  {cmd.Description}\n\n";
            }
            
            ShowOutput(helpText);
        }
        
        private void ClearOutput()
        {
            if (outputText != null)
            {
                outputText.text = string.Empty;
            }
        }
        
        private void ListReports()
        {
            var reports = ReportManager.Instance.ListReports();
            
            if (reports.Count == 0)
            {
                ShowOutput("No reports found.");
                return;
            }
            
            var output = $"Found {reports.Count} report(s):\n\n";
            
            foreach (var report in reports)
            {
                output += $"[{report.State}] {report.Title}\n";
                output += $"  Priority: {report.Priority} | Type: {report.Type}\n";
                output += $"  Date: {report.SubmissionDate.ToShortDateString()}\n\n";
            }
            
            ShowOutput(output);
            
            // Also open the list screen
            if (MainMenuController.Instance != null)
            {
                // This will be handled by the main menu
                Debug.Log("Use the 'List Reports' button to view reports in the UI.");
            }
        }
        
        private void CreateNewReport()
        {
            ShowOutput("Opening new report screen...");
            
            if (MainMenuController.Instance != null)
            {
                // This would trigger the new report screen
                Debug.Log("Use the 'New Report' button to create a new report.");
            }
        }
        
        private void RefreshReports()
        {
            if (ListReportsController.Instance != null)
            {
                ListReportsController.Instance.InitializeListController();
                ShowOutput("Reports list refreshed.");
            }
            else
            {
                ShowOutput("Reports list is not currently active.");
            }
        }
        
        private void ShowOutput(string message)
        {
            if (outputText != null)
            {
                outputText.text = message;
            }
            
            Debug.Log($"[Command] {message}");
        }
        
        /// <summary>
        /// Public method to execute a command programmatically
        /// </summary>
        public void ExecuteCommand(string command)
        {
            ProcessCommand(command);
        }
    }
}
