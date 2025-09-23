using System;
using System.Collections;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Rewarded video advertisement service implementation for LevelPlay SDK.
    /// Handles video ads that provide rewards to users upon successful completion.
    /// These ads are typically opt-in and provide in-game currency, items, or other benefits.
    /// </summary>
    public class RewardedVideoAdService : IRewardedVideoAdService
    {
        #region Private Fields
        
        private bool _isInitialized = false;
        private bool _isLoading = false;
        private string _lastError = string.Empty;
        private string _appId;
        private bool _testMode;
        private bool _isAdReady = false;
        private MonoBehaviour _coroutineRunner;
        private Action<AdRewardEventArgs> _currentRewardCallback;
        
        // Placement ID for rewarded video ads
        private const string REWARDED_VIDEO_PLACEMENT_ID = "DefaultRewardedVideo";
        
        // Default reward configuration
        private const string DEFAULT_REWARD_TYPE = "Coins";
        private const int DEFAULT_REWARD_AMOUNT = 100;
        
        #endregion
        
        #region IAdService Implementation
        
        public bool IsInitialized => _isInitialized;
        public bool IsLoading => _isLoading;
        public string LastError => _lastError;
        
        public event Action<AdEventArgs> OnAdLoaded;
        public event Action<AdErrorEventArgs> OnAdLoadFailed;
        public event Action<AdEventArgs> OnAdDisplayed;
        public event Action<AdEventArgs> OnAdDismissed;
        public event Action<AdEventArgs> OnAdClicked;
        
        #endregion
        
        #region IRewardedVideoAdService Implementation
        
        public event Action<AdRewardEventArgs> OnRewardEarned;
        
        #endregion
        
        #region Public Methods
        
        public void Initialize(string appId, bool testMode = false)
        {
            try
            {
                _appId = appId;
                _testMode = testMode;
                
                AdLogger.Log($"Initializing Rewarded Video Ad Service - App ID: {appId}, Test Mode: {testMode}");
                
                // Get or create coroutine runner
                var manager = AdServiceManager.Instance;
                _coroutineRunner = manager;
                
                // In a real implementation, this would initialize the LevelPlay rewarded video SDK
                // For this demo, we'll simulate initialization
                SimulateInitialization();
                
            }
            catch (Exception ex)
            {
                _lastError = $"Rewarded video service initialization failed: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo", _lastError));
            }
        }
        
        public void LoadAd()
        {
            if (!_isInitialized)
            {
                _lastError = "Rewarded video service not initialized";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo", _lastError));
                return;
            }
            
            if (_isLoading)
            {
                AdLogger.LogWarning("Rewarded video ad is already loading");
                return;
            }
            
            if (_isAdReady)
            {
                AdLogger.LogWarning("Rewarded video ad is already loaded and ready");
                return;
            }
            
            _isLoading = true;
            _isAdReady = false;
            AdLogger.Log("Loading rewarded video ad...");
            
            // In a real implementation, this would call LevelPlay's rewarded video load method
            // For this demo, we'll simulate loading with a coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateRewardedVideoLoad());
            }
        }
        
        public bool IsAdReady()
        {
            return _isInitialized && _isAdReady && !_isLoading;
        }
        
        public void ShowRewardedVideo(Action<AdRewardEventArgs> onReward = null)
        {
            if (!IsAdReady())
            {
                _lastError = "Rewarded video ad is not ready to show";
                AdLogger.LogWarning(_lastError);
                onReward?.Invoke(new AdRewardEventArgs(REWARDED_VIDEO_PLACEMENT_ID, DEFAULT_REWARD_TYPE, 0, false));
                
                // Auto-load if not ready
                LoadAd();
                return;
            }
            
            _currentRewardCallback = onReward;
            
            try
            {
                AdLogger.Log("Showing rewarded video ad...");
                
                // In a real implementation, this would call LevelPlay's show rewarded video method
                // For this demo, we'll simulate showing with a coroutine
                if (_coroutineRunner != null)
                {
                    _coroutineRunner.StartCoroutine(SimulateRewardedVideoShow());
                }
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to show rewarded video: {ex.Message}";
                AdLogger.LogError(_lastError);
                onReward?.Invoke(new AdRewardEventArgs(REWARDED_VIDEO_PLACEMENT_ID, DEFAULT_REWARD_TYPE, 0, false));
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo", _lastError));
            }
        }
        
        public void Destroy()
        {
            try
            {
                _isInitialized = false;
                _isLoading = false;
                _isAdReady = false;
                _currentRewardCallback = null;
                
                AdLogger.Log("Rewarded Video Ad Service destroyed");
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error destroying rewarded video service: {ex.Message}");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private void SimulateInitialization()
        {
            // Simulate async initialization
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateInitCoroutine());
            }
        }
        
        private IEnumerator SimulateInitCoroutine()
        {
            yield return new WaitForSeconds(2f); // Simulate initialization delay
            
            _isInitialized = true;
            AdLogger.Log("Rewarded Video Ad Service initialized successfully");
            
            // Auto-load the first ad
            LoadAd();
        }
        
        private IEnumerator SimulateRewardedVideoLoad()
        {
            yield return new WaitForSeconds(4f); // Simulate load time (longer for video)
            
            _isLoading = false;
            
            // Simulate success/failure (80% success rate for demo)
            bool success = UnityEngine.Random.value > 0.2f;
            
            if (success)
            {
                _isAdReady = true;
                AdLogger.Log("Rewarded video ad loaded successfully");
                
                var eventArgs = new AdEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo");
                OnAdLoaded?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoaded(eventArgs);
            }
            else
            {
                _lastError = "Simulated rewarded video load failure";
                AdLogger.LogError(_lastError);
                var errorArgs = new AdErrorEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo", _lastError, 3001);
                OnAdLoadFailed?.Invoke(errorArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoadFailed(errorArgs);
            }
        }
        
        private IEnumerator SimulateRewardedVideoShow()
        {
            // Mark ad as consumed
            _isAdReady = false;
            
            // Show display event
            var displayEventArgs = new AdEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo");
            OnAdDisplayed?.Invoke(displayEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDisplayed(displayEventArgs);
            
            // Create and show rewarded video UI
            var rewardedVideoUI = CreateRewardedVideoUI();
            
            // Simulate video watch time (15-30 seconds)
            float watchTime = UnityEngine.Random.Range(15f, 30f);
            float elapsedTime = 0f;
            
            // Update progress during video
            while (elapsedTime < watchTime)
            {
                elapsedTime += Time.deltaTime;
                UpdateVideoProgress(rewardedVideoUI, elapsedTime / watchTime);
                yield return null;
            }
            
            // Simulate click chance (10% chance during video)
            bool wasClicked = UnityEngine.Random.value < 0.1f;
            if (wasClicked)
            {
                var clickEventArgs = new AdEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo");
                OnAdClicked?.Invoke(clickEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(clickEventArgs);
            }
            
            // Determine if user completed the video (95% completion rate)
            bool videoCompleted = UnityEngine.Random.value > 0.05f;
            
            // Clean up UI
            if (rewardedVideoUI != null)
            {
                UnityEngine.Object.Destroy(rewardedVideoUI);
            }
            
            // Show dismiss event
            var dismissEventArgs = new AdEventArgs(REWARDED_VIDEO_PLACEMENT_ID, "RewardedVideo");
            OnAdDismissed?.Invoke(dismissEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDismissed(dismissEventArgs);
            
            // Handle reward
            if (videoCompleted)
            {
                var rewardEventArgs = new AdRewardEventArgs(REWARDED_VIDEO_PLACEMENT_ID, DEFAULT_REWARD_TYPE, DEFAULT_REWARD_AMOUNT, true);
                
                OnRewardEarned?.Invoke(rewardEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchRewardEarned(rewardEventArgs);
                
                _currentRewardCallback?.Invoke(rewardEventArgs);
                
                AdLogger.Log($"Reward earned: {DEFAULT_REWARD_AMOUNT} {DEFAULT_REWARD_TYPE}");
            }
            else
            {
                var noRewardEventArgs = new AdRewardEventArgs(REWARDED_VIDEO_PLACEMENT_ID, DEFAULT_REWARD_TYPE, 0, false);
                _currentRewardCallback?.Invoke(noRewardEventArgs);
                
                AdLogger.Log("Video not completed - no reward earned");
            }
            
            _currentRewardCallback = null;
            
            AdLogger.Log("Rewarded video ad dismissed");
            
            // Auto-load next ad
            LoadAd();
        }
        
        private GameObject CreateRewardedVideoUI()
        {
            // Find or create the ad overlay canvas
            var overlayCanvas = FindAdOverlayCanvas();
            
            // Create full-screen rewarded video container
            var rewardedVideoContainer = new GameObject("RewardedVideoAdContainer");
            var rectTransform = rewardedVideoContainer.AddComponent<RectTransform>();
            
            // Set full screen
            rectTransform.SetParent(overlayCanvas.transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add background
            var background = rewardedVideoContainer.AddComponent<UnityEngine.UI.Image>();
            background.color = Color.black; // Black background for video
            
            // Add video area
            var videoArea = new GameObject("VideoArea");
            videoArea.transform.SetParent(rewardedVideoContainer.transform, false);
            var videoRect = videoArea.AddComponent<RectTransform>();
            videoRect.anchorMin = new Vector2(0.05f, 0.2f);
            videoRect.anchorMax = new Vector2(0.95f, 0.8f);
            videoRect.offsetMin = Vector2.zero;
            videoRect.offsetMax = Vector2.zero;
            
            var videoImage = videoArea.AddComponent<UnityEngine.UI.Image>();
            videoImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray for video placeholder
            
            // Add video title
            var titleObj = new GameObject("VideoTitle");
            titleObj.transform.SetParent(videoArea.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Rewarded Video Ad";
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 28;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add progress bar
            var progressBarBG = new GameObject("ProgressBarBackground");
            progressBarBG.transform.SetParent(rewardedVideoContainer.transform, false);
            var progressBGRect = progressBarBG.AddComponent<RectTransform>();
            progressBGRect.anchorMin = new Vector2(0.1f, 0.1f);
            progressBGRect.anchorMax = new Vector2(0.9f, 0.15f);
            progressBGRect.offsetMin = Vector2.zero;
            progressBGRect.offsetMax = Vector2.zero;
            
            var progressBGImage = progressBarBG.AddComponent<UnityEngine.UI.Image>();
            progressBGImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            var progressBar = new GameObject("ProgressBar");
            progressBar.transform.SetParent(progressBarBG.transform, false);
            var progressRect = progressBar.AddComponent<RectTransform>();
            progressRect.anchorMin = Vector2.zero;
            progressRect.anchorMax = new Vector2(0, 1); // Start at 0 width
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = Vector2.zero;
            
            var progressImage = progressBar.AddComponent<UnityEngine.UI.Image>();
            progressImage.color = new Color(0f, 0.8f, 0f, 1f); // Green progress bar
            
            // Add reward info
            var rewardInfoObj = new GameObject("RewardInfo");
            rewardInfoObj.transform.SetParent(rewardedVideoContainer.transform, false);
            var rewardInfoText = rewardInfoObj.AddComponent<UnityEngine.UI.Text>();
            rewardInfoText.text = $"Watch to earn {DEFAULT_REWARD_AMOUNT} {DEFAULT_REWARD_TYPE}!";
            rewardInfoText.alignment = TextAnchor.MiddleCenter;
            rewardInfoText.color = Color.yellow;
            rewardInfoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            rewardInfoText.fontSize = 20;
            rewardInfoText.fontStyle = FontStyle.Bold;
            
            var rewardInfoRect = rewardInfoText.GetComponent<RectTransform>();
            rewardInfoRect.anchorMin = new Vector2(0, 0.85f);
            rewardInfoRect.anchorMax = new Vector2(1, 0.95f);
            rewardInfoRect.offsetMin = Vector2.zero;
            rewardInfoRect.offsetMax = Vector2.zero;
            
            // Add timer text
            var timerObj = new GameObject("Timer");
            timerObj.transform.SetParent(rewardedVideoContainer.transform, false);
            var timerText = timerObj.AddComponent<UnityEngine.UI.Text>();
            timerText.text = "00:00";
            timerText.alignment = TextAnchor.MiddleCenter;
            timerText.color = Color.white;
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            timerText.fontSize = 18;
            
            var timerRect = timerText.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0, 0.05f);
            timerRect.anchorMax = new Vector2(1, 0.1f);
            timerRect.offsetMin = Vector2.zero;
            timerRect.offsetMax = Vector2.zero;
            
            return rewardedVideoContainer;
        }
        
        private void UpdateVideoProgress(GameObject rewardedVideoUI, float progress)
        {
            if (rewardedVideoUI == null) return;
            
            // Update progress bar
            var progressBar = rewardedVideoUI.transform.Find("ProgressBarBackground/ProgressBar");
            if (progressBar != null)
            {
                var progressRect = progressBar.GetComponent<RectTransform>();
                progressRect.anchorMax = new Vector2(progress, 1);
            }
            
            // Update timer
            var timer = rewardedVideoUI.transform.Find("Timer");
            if (timer != null)
            {
                var timerText = timer.GetComponent<UnityEngine.UI.Text>();
                int seconds = Mathf.FloorToInt(progress * 30f); // Assume 30 second video
                timerText.text = $"00:{seconds:D2}";
            }
        }
        
        private Canvas FindAdOverlayCanvas()
        {
            // Try to find existing ad overlay canvas
            var existingCanvas = GameObject.Find("AdOverlayCanvas");
            if (existingCanvas != null)
            {
                return existingCanvas.GetComponent<Canvas>();
            }
            
            // Create ad overlay canvas if it doesn't exist
            var canvasObj = new GameObject("AdOverlayCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // Ensure ads appear on top
            
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            return canvas;
        }
        
        #endregion
    }
}