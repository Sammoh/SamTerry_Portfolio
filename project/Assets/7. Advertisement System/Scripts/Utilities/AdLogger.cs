using UnityEngine;
using System;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Centralized logging utility for the advertisement system.
    /// Provides consistent logging with prefixes, filtering, and debug controls.
    /// </summary>
    public static class AdLogger
    {
        #region Constants
        
        private const string LOG_PREFIX = "[AdSystem]";
        private const string ERROR_PREFIX = "[AdSystem ERROR]";
        private const string WARNING_PREFIX = "[AdSystem WARNING]";
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Gets or sets whether logging is enabled for the advertisement system.
        /// </summary>
        public static bool EnableLogging { get; set; } = true;
        
        /// <summary>
        /// Gets or sets whether to include timestamps in log messages.
        /// </summary>
        public static bool IncludeTimestamp { get; set; } = true;
        
        /// <summary>
        /// Gets or sets whether to include stack traces for errors.
        /// </summary>
        public static bool IncludeStackTrace { get; set; } = false;
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="context">Optional Unity object context</param>
        public static void Log(string message, UnityEngine.Object context = null)
        {
            if (!EnableLogging) return;
            
            string formattedMessage = FormatMessage(LOG_PREFIX, message);
            Debug.Log(formattedMessage, context);
        }
        
        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message to log</param>
        /// <param name="context">Optional Unity object context</param>
        public static void LogWarning(string message, UnityEngine.Object context = null)
        {
            if (!EnableLogging) return;
            
            string formattedMessage = FormatMessage(WARNING_PREFIX, message);
            Debug.LogWarning(formattedMessage, context);
        }
        
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The error message to log</param>
        /// <param name="context">Optional Unity object context</param>
        public static void LogError(string message, UnityEngine.Object context = null)
        {
            if (!EnableLogging) return;
            
            string formattedMessage = FormatMessage(ERROR_PREFIX, message);
            
            if (IncludeStackTrace)
            {
                formattedMessage += "\\n" + System.Environment.StackTrace;
            }
            
            Debug.LogError(formattedMessage, context);
        }
        
        /// <summary>
        /// Logs an exception with additional context.
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="context">Optional Unity object context</param>
        /// <param name="additionalMessage">Optional additional message</param>
        public static void LogException(Exception exception, UnityEngine.Object context = null, string additionalMessage = null)
        {
            if (!EnableLogging) return;
            
            string message = string.IsNullOrEmpty(additionalMessage) 
                ? exception.Message 
                : $"{additionalMessage}: {exception.Message}";
                
            string formattedMessage = FormatMessage(ERROR_PREFIX, message);
            Debug.LogError(formattedMessage, context);
            Debug.LogException(exception, context);
        }
        
        /// <summary>
        /// Logs a formatted message with specific ad service context.
        /// </summary>
        /// <param name="serviceName">Name of the ad service</param>
        /// <param name="message">The message to log</param>
        /// <param name="logLevel">The log level</param>
        /// <param name="context">Optional Unity object context</param>
        public static void LogWithService(string serviceName, string message, LogLevel logLevel = LogLevel.Info, UnityEngine.Object context = null)
        {
            if (!EnableLogging) return;
            
            string servicePrefix = $"[{serviceName}]";
            string fullMessage = $"{servicePrefix} {message}";
            
            switch (logLevel)
            {
                case LogLevel.Info:
                    Log(fullMessage, context);
                    break;
                case LogLevel.Warning:
                    LogWarning(fullMessage, context);
                    break;
                case LogLevel.Error:
                    LogError(fullMessage, context);
                    break;
            }
        }
        
        /// <summary>
        /// Logs ad event information in a structured format.
        /// </summary>
        /// <param name="eventArgs">Ad event arguments</param>
        /// <param name="eventType">Type of ad event</param>
        public static void LogAdEvent(AdEventArgs eventArgs, string eventType)
        {
            if (!EnableLogging) return;
            
            string message = $"[{eventType}] {eventArgs.AdType} - Placement: {eventArgs.PlacementId} - Time: {eventArgs.Timestamp:HH:mm:ss}";
            Log(message);
        }
        
        /// <summary>
        /// Logs ad error information in a structured format.
        /// </summary>
        /// <param name="errorArgs">Ad error event arguments</param>
        public static void LogAdError(AdErrorEventArgs errorArgs)
        {
            if (!EnableLogging) return;
            
            string message = $"[AD_ERROR] {errorArgs.AdType} - Placement: {errorArgs.PlacementId} - Error: {errorArgs.ErrorMessage} (Code: {errorArgs.ErrorCode})";
            LogError(message);
        }
        
        /// <summary>
        /// Logs ad reward information in a structured format.
        /// </summary>
        /// <param name="rewardArgs">Ad reward event arguments</param>
        public static void LogAdReward(AdRewardEventArgs rewardArgs)
        {
            if (!EnableLogging) return;
            
            string status = rewardArgs.IsValid ? "GRANTED" : "FAILED";
            string message = $"[REWARD_{status}] {rewardArgs.RewardType} x{rewardArgs.Amount} - Placement: {rewardArgs.PlacementId}";
            Log(message);
        }
        
        /// <summary>
        /// Logs performance metrics for ad operations.
        /// </summary>
        /// <param name="operation">The operation being measured</param>
        /// <param name="duration">Duration in milliseconds</param>
        /// <param name="success">Whether the operation was successful</param>
        public static void LogPerformance(string operation, float duration, bool success)
        {
            if (!EnableLogging) return;
            
            string status = success ? "SUCCESS" : "FAILED";
            string message = $"[PERFORMANCE] {operation} - {duration:F2}ms - {status}";
            Log(message);
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Formats a log message with prefix and timestamp.
        /// </summary>
        /// <param name="prefix">The log prefix</param>
        /// <param name="message">The message to format</param>
        /// <returns>Formatted message string</returns>
        private static string FormatMessage(string prefix, string message)
        {
            if (IncludeTimestamp)
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                return $"{prefix} [{timestamp}] {message}";
            }
            else
            {
                return $"{prefix} {message}";
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Enumeration of log levels for the advertisement system.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}