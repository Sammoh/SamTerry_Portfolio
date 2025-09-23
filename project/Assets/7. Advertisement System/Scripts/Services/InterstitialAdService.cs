using System;
using System.Collections;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Interstitial advertisement service implementation for LevelPlay SDK.
    /// Handles full-screen interstitial ads that are typically shown at natural
    /// break points in the application flow.
    /// </summary>
    public class InterstitialAdService : IInterstitialAdService
    {
        #region Private Fields
        
        private bool _isInitialized = false;
        private bool _isLoading = false;
        private string _lastError = string.Empty;
        private string _appId;
        private bool _testMode;
        private bool _isAdReady = false;
        private MonoBehaviour _coroutineRunner;
        private Action<bool> _currentShowCallback;
        
        // Placement ID for interstitial ads
        private const string INTERSTITIAL_PLACEMENT_ID = "DefaultInterstitial";
        
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
        
        #region Public Methods
        
        public void Initialize(string appId, bool testMode = false)
        {
            try
            {
                _appId = appId;
                _testMode = testMode;
                
                AdLogger.Log($"Initializing Interstitial Ad Service - App ID: {appId}, Test Mode: {testMode}");
                
                // Get or create coroutine runner
                var manager = AdServiceManager.Instance;
                _coroutineRunner = manager;
                
                // In a real implementation, this would initialize the LevelPlay interstitial SDK
                // For this demo, we'll simulate initialization
                SimulateInitialization();
                
            }
            catch (Exception ex)
            {
                _lastError = $"Interstitial service initialization failed: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial", _lastError));
            }
        }
        
        public void LoadAd()
        {
            if (!_isInitialized)
            {
                _lastError = "Interstitial service not initialized";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial", _lastError));
                return;
            }
            
            if (_isLoading)
            {
                AdLogger.LogWarning("Interstitial ad is already loading");
                return;
            }
            
            if (_isAdReady)
            {
                AdLogger.LogWarning("Interstitial ad is already loaded and ready");
                return;
            }
            
            _isLoading = true;
            _isAdReady = false;
            AdLogger.Log("Loading interstitial ad...");
            
            // In a real implementation, this would call LevelPlay's interstitial load method
            // For this demo, we'll simulate loading with a coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateInterstitialLoad());
            }
        }
        
        public bool IsAdReady()
        {
            return _isInitialized && _isAdReady && !_isLoading;
        }
        
        public void ShowInterstitial(Action<bool> onComplete = null)
        {
            if (!IsAdReady())
            {
                _lastError = "Interstitial ad is not ready to show";
                AdLogger.LogWarning(_lastError);
                onComplete?.Invoke(false);
                
                // Auto-load if not ready
                LoadAd();
                return;
            }
            
            _currentShowCallback = onComplete;
            
            try
            {
                AdLogger.Log("Showing interstitial ad...");
                
                // In a real implementation, this would call LevelPlay's show interstitial method
                // For this demo, we'll simulate showing with a coroutine
                if (_coroutineRunner != null)
                {
                    _coroutineRunner.StartCoroutine(SimulateInterstitialShow());
                }
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to show interstitial: {ex.Message}";
                AdLogger.LogError(_lastError);
                onComplete?.Invoke(false);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial", _lastError));
            }
        }
        
        public void Destroy()
        {
            try
            {
                _isInitialized = false;
                _isLoading = false;
                _isAdReady = false;
                _currentShowCallback = null;
                
                AdLogger.Log("Interstitial Ad Service destroyed");
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error destroying interstitial service: {ex.Message}");
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
            yield return new WaitForSeconds(1.5f); // Simulate initialization delay
            
            _isInitialized = true;
            AdLogger.Log("Interstitial Ad Service initialized successfully");
            
            // Auto-load the first ad
            LoadAd();
        }
        
        private IEnumerator SimulateInterstitialLoad()
        {
            yield return new WaitForSeconds(3f); // Simulate load time
            
            _isLoading = false;
            
            // Simulate success/failure (85% success rate for demo)
            bool success = UnityEngine.Random.value > 0.15f;
            
            if (success)
            {
                _isAdReady = true;
                AdLogger.Log("Interstitial ad loaded successfully");
                
                var eventArgs = new AdEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial");
                OnAdLoaded?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoaded(eventArgs);
            }
            else
            {
                _lastError = "Simulated interstitial load failure";
                AdLogger.LogError(_lastError);
                var errorArgs = new AdErrorEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial", _lastError, 2001);
                OnAdLoadFailed?.Invoke(errorArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoadFailed(errorArgs);
            }
        }
        
        private IEnumerator SimulateInterstitialShow()
        {
            // Mark ad as consumed
            _isAdReady = false;
            
            // Show display event
            var displayEventArgs = new AdEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial");
            OnAdDisplayed?.Invoke(displayEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDisplayed(displayEventArgs);
            
            // Create and show interstitial UI
            var interstitialUI = CreateInterstitialUI();
            
            // Simulate ad display time (5-10 seconds)
            float displayTime = UnityEngine.Random.Range(5f, 10f);
            yield return new WaitForSeconds(displayTime);
            
            // Simulate click chance (20% chance)
            bool wasClicked = UnityEngine.Random.value < 0.2f;
            if (wasClicked)
            {
                var clickEventArgs = new AdEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial");
                OnAdClicked?.Invoke(clickEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(clickEventArgs);
            }
            
            // Clean up UI
            if (interstitialUI != null)
            {
                UnityEngine.Object.Destroy(interstitialUI);
            }
            
            // Show dismiss event
            var dismissEventArgs = new AdEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial");
            OnAdDismissed?.Invoke(dismissEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDismissed(dismissEventArgs);
            
            // Notify completion callback
            _currentShowCallback?.Invoke(true);
            _currentShowCallback = null;
            
            AdLogger.Log("Interstitial ad dismissed");
            
            // Auto-load next ad
            LoadAd();
        }
        
        private GameObject CreateInterstitialUI()
        {
            // Find or create the ad overlay canvas
            var overlayCanvas = FindAdOverlayCanvas();
            
            // Create full-screen interstitial container
            var interstitialContainer = new GameObject("InterstitialAdContainer");
            var rectTransform = interstitialContainer.AddComponent<RectTransform>();
            
            // Set full screen
            rectTransform.SetParent(overlayCanvas.transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add background
            var background = interstitialContainer.AddComponent<UnityEngine.UI.Image>();
            background.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark semi-transparent
            
            // Add click detection
            var button = interstitialContainer.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(() => {
                var clickEventArgs = new AdEventArgs(INTERSTITIAL_PLACEMENT_ID, "Interstitial");
                OnAdClicked?.Invoke(clickEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(clickEventArgs);
            });
            
            // Add content area
            var contentArea = new GameObject("ContentArea");
            contentArea.transform.SetParent(interstitialContainer.transform, false);
            var contentRect = contentArea.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.1f, 0.2f);
            contentRect.anchorMax = new Vector2(0.9f, 0.8f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            var contentImage = contentArea.AddComponent<UnityEngine.UI.Image>();
            contentImage.color = new Color(0.3f, 0.7f, 1f, 1f); // Blue background
            
            // Add title text
            var titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(contentArea.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Demo Interstitial Ad";
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 32;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.7f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add description text
            var descObj = new GameObject("DescriptionText");
            descObj.transform.SetParent(contentArea.transform, false);
            var descText = descObj.AddComponent<UnityEngine.UI.Text>();
            descText.text = "This is a simulated interstitial advertisement.\\nClick anywhere to simulate ad interaction.\\nWill auto-close in a few seconds.";
            descText.alignment = TextAnchor.MiddleCenter;
            descText.color = Color.white;
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 18;
            
            var descRect = descText.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.3f);
            descRect.anchorMax = new Vector2(1, 0.7f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            
            // Add close button
            var closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(interstitialContainer.transform, false);
            var closeButton = closeButtonObj.AddComponent<UnityEngine.UI.Button>();
            var closeButtonImage = closeButtonObj.AddComponent<UnityEngine.UI.Image>();
            closeButtonImage.color = Color.red;
            
            var closeButtonRect = closeButton.GetComponent<RectTransform>();
            closeButtonRect.anchorMin = new Vector2(0.9f, 0.85f);
            closeButtonRect.anchorMax = new Vector2(0.95f, 0.95f);
            closeButtonRect.offsetMin = Vector2.zero;
            closeButtonRect.offsetMax = Vector2.zero;
            
            // Add X text to close button
            var closeTextObj = new GameObject("CloseText");
            closeTextObj.transform.SetParent(closeButtonObj.transform, false);
            var closeText = closeTextObj.AddComponent<UnityEngine.UI.Text>();
            closeText.text = "X";
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.color = Color.white;
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 24;
            closeText.fontStyle = FontStyle.Bold;
            
            var closeTextRect = closeText.GetComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            
            return interstitialContainer;
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