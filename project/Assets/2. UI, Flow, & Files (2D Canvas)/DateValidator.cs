using UnityEngine;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;


namespace Sammoh.Two
{
    public class DateValidator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField dateInputField;
        [SerializeField] private TMP_Text feedbackText;

        private const string DateFormat = "MM/dd/yyyy";
        private static readonly Regex DateRegex = new Regex(@"^(0[1-9]|1[0-2])/([0-2][0-9]|3[01])/([0-9]{4})$");

        private void Start()
        {
            dateInputField.onValueChanged.AddListener(OnDateInputChanged);
            feedbackText.text = "";
        }

        private void OnDateInputChanged(string input)
        {
            string formattedDate = FormatDateInput(input);
            dateInputField.text = formattedDate;

            if (IsValidDate(formattedDate))
            {
                feedbackText.text = "Valid Date";
                feedbackText.color = Color.green;
            }
            else
            {
                feedbackText.text = "Invalid Date";
                feedbackText.color = Color.red;
            }
        }

        private string FormatDateInput(string input)
        {
            // Remove any non-numeric characters
            string numericInput = Regex.Replace(input, @"[^0-9]", "");

            // Insert slashes at the appropriate positions
            if (numericInput.Length >= 4)
            {
                numericInput = numericInput.Insert(4, "/");
            }

            if (numericInput.Length >= 2)
            {
                numericInput = numericInput.Insert(2, "/");
            }

            // Format the input to "MM/dd/yyyy"
            if (numericInput.Length > 10)
            {
                numericInput = numericInput.Substring(0, 10);
            }

            // Ensure two-digit month and day
            if (numericInput.Length >= 1 && numericInput.Length < 2 && numericInput[0] > '1')
            {
                numericInput = "0" + numericInput;
            }
            else if (numericInput.Length >= 4 && numericInput.Length < 5 && numericInput[3] > '3')
            {
                numericInput = numericInput.Substring(0, 3) + "0" + numericInput.Substring(3);
            }

            return numericInput;
        }

        private bool IsValidDate(string input)
        {
            if (DateRegex.IsMatch(input) && DateTime.TryParseExact(input, DateFormat, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
            {
                return true;
            }

            return false;
        }
    }
}