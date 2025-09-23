using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Event dispatcher system for advertisement-related events.
    /// Provides a centralized event management system that allows for decoupled
    /// communication between ad services and other game systems.
    /// </summary>
    public class AdEventDispatcher : MonoBehaviour
    {
        #region Event Definitions
        
        /// <summary>
        /// Global event fired when any ad is loaded successfully.
        /// </summary>
        public event Action<AdEventArgs> OnAnyAdLoaded;
        
        /// <summary>
        /// Global event fired when any ad fails to load.
        /// </summary>
        public event Action<AdErrorEventArgs> OnAnyAdLoadFailed;
        
        /// <summary>
        /// Global event fired when any ad is displayed.
        /// </summary>
        public event Action<AdEventArgs> OnAnyAdDisplayed;
        
        /// <summary>
        /// Global event fired when any ad is dismissed.
        /// </summary>
        public event Action<AdEventArgs> OnAnyAdDismissed;
        
        /// <summary>
        /// Global event fired when any ad is clicked.
        /// </summary>
        public event Action<AdEventArgs> OnAnyAdClicked;
        
        /// <summary>
        /// Global event fired when a reward is earned from any rewarded ad.
        /// </summary>
        public event Action<AdRewardEventArgs> OnAnyRewardEarned;
        
        /// <summary>
        /// Global event fired when any playable ad is completed.
        /// </summary>
        public event Action<AdEventArgs> OnAnyPlayableCompleted;
        
        #endregion
        
        #region Private Fields
        
        private Dictionary<string, List<Action<AdEventArgs>>> _customEventHandlers = 
            new Dictionary<string, List<Action<AdEventArgs>>>();
        private Queue<AdEventArgs> _eventQueue = new Queue<AdEventArgs>();
        private bool _enableEventLogging = true;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Gets or sets whether event logging is enabled.
        /// </summary>
        public bool EnableEventLogging
        {
            get => _enableEventLogging;
            set => _enableEventLogging = value;
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Update()
        {
            // Process queued events on the main thread
            ProcessEventQueue();
        }
        
        #endregion
        
        #region Public Methods - Event Dispatching
        
        /// <summary>
        /// Dispatches an ad loaded event.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing ad information</param>
        public void DispatchAdLoaded(AdEventArgs eventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Ad Loaded: {eventArgs.AdType} - {eventArgs.PlacementId}");
            }
            
            EnqueueEvent(() => OnAnyAdLoaded?.Invoke(eventArgs));
            DispatchCustomEvent($"AdLoaded_{eventArgs.AdType}", eventArgs);
        }
        
        /// <summary>
        /// Dispatches an ad load failed event.
        /// </summary>
        /// <param name="errorEventArgs">Error event arguments</param>
        public void DispatchAdLoadFailed(AdErrorEventArgs errorEventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.LogError($"Ad Load Failed: {errorEventArgs.AdType} - {errorEventArgs.ErrorMessage}");
            }
            
            EnqueueEvent(() => OnAnyAdLoadFailed?.Invoke(errorEventArgs));
            DispatchCustomEvent($"AdLoadFailed_{errorEventArgs.AdType}", errorEventArgs);
        }
        
        /// <summary>
        /// Dispatches an ad displayed event.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing ad information</param>
        public void DispatchAdDisplayed(AdEventArgs eventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Ad Displayed: {eventArgs.AdType} - {eventArgs.PlacementId}");
            }
            
            EnqueueEvent(() => OnAnyAdDisplayed?.Invoke(eventArgs));
            DispatchCustomEvent($"AdDisplayed_{eventArgs.AdType}", eventArgs);
        }
        
        /// <summary>
        /// Dispatches an ad dismissed event.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing ad information</param>
        public void DispatchAdDismissed(AdEventArgs eventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Ad Dismissed: {eventArgs.AdType} - {eventArgs.PlacementId}");
            }
            
            EnqueueEvent(() => OnAnyAdDismissed?.Invoke(eventArgs));
            DispatchCustomEvent($"AdDismissed_{eventArgs.AdType}", eventArgs);
        }
        
        /// <summary>
        /// Dispatches an ad clicked event.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing ad information</param>
        public void DispatchAdClicked(AdEventArgs eventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Ad Clicked: {eventArgs.AdType} - {eventArgs.PlacementId}");
            }
            
            EnqueueEvent(() => OnAnyAdClicked?.Invoke(eventArgs));
            DispatchCustomEvent($"AdClicked_{eventArgs.AdType}", eventArgs);
        }
        
        /// <summary>
        /// Dispatches a reward earned event.
        /// </summary>
        /// <param name="rewardEventArgs">Reward event arguments</param>
        public void DispatchRewardEarned(AdRewardEventArgs rewardEventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Reward Earned: {rewardEventArgs.RewardType} x{rewardEventArgs.Amount}");
            }
            
            EnqueueEvent(() => OnAnyRewardEarned?.Invoke(rewardEventArgs));
            DispatchCustomEvent("RewardEarned", rewardEventArgs);
        }
        
        /// <summary>
        /// Dispatches a playable completed event.
        /// </summary>
        /// <param name="eventArgs">Event arguments containing ad information</param>
        public void DispatchPlayableCompleted(AdEventArgs eventArgs)
        {
            if (_enableEventLogging)
            {
                AdLogger.Log($"Playable Completed: {eventArgs.PlacementId}");
            }
            
            EnqueueEvent(() => OnAnyPlayableCompleted?.Invoke(eventArgs));
            DispatchCustomEvent("PlayableCompleted", eventArgs);
        }
        
        #endregion
        
        #region Public Methods - Custom Event Management
        
        /// <summary>
        /// Registers a custom event handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event to listen for</param>
        /// <param name="handler">The handler function to invoke</param>
        public void RegisterCustomEventHandler(string eventType, Action<AdEventArgs> handler)
        {
            if (!_customEventHandlers.ContainsKey(eventType))
            {
                _customEventHandlers[eventType] = new List<Action<AdEventArgs>>();
            }
            
            _customEventHandlers[eventType].Add(handler);
            
            if (_enableEventLogging)
            {
                AdLogger.Log($"Registered custom event handler for: {eventType}");
            }
        }
        
        /// <summary>
        /// Unregisters a custom event handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event to stop listening for</param>
        /// <param name="handler">The handler function to remove</param>
        public void UnregisterCustomEventHandler(string eventType, Action<AdEventArgs> handler)
        {
            if (_customEventHandlers.ContainsKey(eventType))
            {
                _customEventHandlers[eventType].Remove(handler);
                
                // Clean up empty handler lists
                if (_customEventHandlers[eventType].Count == 0)
                {
                    _customEventHandlers.Remove(eventType);
                }
                
                if (_enableEventLogging)
                {
                    AdLogger.Log($"Unregistered custom event handler for: {eventType}");
                }
            }
        }
        
        /// <summary>
        /// Clears all custom event handlers.
        /// </summary>
        public void ClearAllCustomEventHandlers()
        {
            _customEventHandlers.Clear();
            
            if (_enableEventLogging)
            {
                AdLogger.Log("Cleared all custom event handlers");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Enqueues an event action to be processed on the main thread.
        /// </summary>
        /// <param name="eventAction">The event action to enqueue</param>
        private void EnqueueEvent(Action eventAction)
        {
            // Since we can't directly enqueue actions, we'll execute immediately for now
            // In a production implementation, you might use a proper thread-safe queue
            try
            {
                eventAction?.Invoke();
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error dispatching event: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Dispatches a custom event to registered handlers.
        /// </summary>
        /// <param name="eventType">The type of custom event</param>
        /// <param name="eventArgs">Event arguments</param>
        private void DispatchCustomEvent(string eventType, AdEventArgs eventArgs)
        {
            if (_customEventHandlers.ContainsKey(eventType))
            {
                var handlers = _customEventHandlers[eventType];
                foreach (var handler in handlers)
                {
                    try
                    {
                        handler?.Invoke(eventArgs);
                    }
                    catch (Exception ex)
                    {
                        AdLogger.LogError($"Error in custom event handler for {eventType}: {ex.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Processes the event queue on the main thread.
        /// </summary>
        private void ProcessEventQueue()
        {
            // Process any queued events
            while (_eventQueue.Count > 0)
            {
                var eventArgs = _eventQueue.Dequeue();
                // Process the event...
                // This is a placeholder for future queue-based event processing
            }
        }
        
        #endregion
        
        #region Analytics and Debugging
        
        /// <summary>
        /// Gets analytics data about dispatched events.
        /// </summary>
        /// <returns>Dictionary containing event counts and statistics</returns>
        public Dictionary<string, object> GetAnalyticsData()
        {
            var analytics = new Dictionary<string, object>
            {
                ["CustomEventHandlerCount"] = _customEventHandlers.Count,
                ["QueuedEventCount"] = _eventQueue.Count,
                ["EventLoggingEnabled"] = _enableEventLogging,
                ["RegisteredEventTypes"] = new List<string>(_customEventHandlers.Keys)
            };
            
            return analytics;
        }
        
        /// <summary>
        /// Logs current event dispatcher statistics.
        /// </summary>
        [ContextMenu("Log Event Statistics")]
        public void LogEventStatistics()
        {
            var analytics = GetAnalyticsData();
            
            AdLogger.Log("=== Ad Event Dispatcher Statistics ===");
            foreach (var kvp in analytics)
            {
                AdLogger.Log($"{kvp.Key}: {kvp.Value}");
            }
        }
        
        #endregion
    }
}