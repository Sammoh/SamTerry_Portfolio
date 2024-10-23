using System;

namespace Sammoh.Two
{
    /// <summary>
    /// A report is a collection of information about a specific issue or problem.
    /// </summary>
    [Serializable]
    public class Report
    {
        /// <summary>
        /// Gets or sets the title of the report.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the email of the reporter.
        /// </summary>
        public string ReporterEmail { get; set; }

        /// <summary>
        /// Gets or sets the submission date of the report.
        /// </summary>
        public DateTime SubmissionDate { get; set; }
        
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the report is a priority.
        /// </summary>
        public PriorityLevel Priority { get; set; }

        /// <summary>
        /// Gets or sets the type of the report.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the state of the report.
        /// </summary>
        public ReportState State { get; set; }

        /// <summary>
        /// Gets or sets the ID of the report.
        /// </summary>
        public string Id { get; set; }
    }

    /// <summary>
    /// Represents the state of a report.
    /// </summary>
    public enum ReportState
    {
        none,

        /// <summary>
        /// The report has just been created.
        /// </summary>
        New,

        /// <summary>
        /// The report is being reviewed.
        /// </summary>
        Review,

        /// <summary>
        /// The review of the report is complete.
        /// </summary>
        Complete
    }

    /// <summary>
    /// Represents the priority level of a report.
    /// </summary>
    public enum PriorityLevel
    {
        none,

        /// <summary>
        /// The report is of low priority.
        /// </summary>
        Low,

        /// <summary>
        /// The report is of high priority.
        /// </summary>
        High
    }
}