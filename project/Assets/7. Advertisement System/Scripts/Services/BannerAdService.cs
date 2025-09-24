using System;
using System.Collections;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Banner advertisement service implementation for LevelPlay SDK.
    /// Handles banner ads that can be positioned at different locations on screen
    /// with support for various banner sizes and automatic refresh functionality.
    /// </summary>
    public class BannerAdService : IBannerAdService
    {
        #region Private Fields
        
        private bool _isInitialized = false;
        private bool _isLoading = false;
        private bool _isBannerVisible = false;
        private string _lastError = string.Empty;
        private string _appId;
        private bool _testMode;
        private BannerPosition _currentPosition = BannerPosition.Bottom;
        private BannerSize _currentSize = BannerSize.Standard;
        private GameObject _bannerContainer;
        private MonoBehaviour _coroutineRunner;
        private BannerContainerConfiguration _containerConfig;
        
        // Placement IDs for different banner positions (in real implementation, these would be configured)
        private const string BANNER_PLACEMENT_ID = "DefaultBanner";
        
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
        
        #region IBannerAdService Implementation
        
        public bool IsBannerVisible => _isBannerVisible;
        public BannerPosition CurrentPosition => _currentPosition;
        
        #endregion
        
        #region Public Methods
        
        public void Initialize(string appId, bool testMode = false)
        {
            try
            {
                _appId = appId;
                _testMode = testMode;
                
                AdLogger.Log($"Initializing Banner Ad Service - App ID: {appId}, Test Mode: {testMode}");
                
                // Get or create coroutine runner
                var manager = AdServiceManager.Instance;
                _coroutineRunner = manager;
                
                // Load banner container configuration
                LoadBannerContainerConfiguration();
                
                // In a real implementation, this would initialize the LevelPlay banner SDK
                // For this demo, we'll simulate initialization
                SimulateInitialization();
                
            }
            catch (Exception ex)
            {
                _lastError = $"Banner service initialization failed: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(BANNER_PLACEMENT_ID, "Banner", _lastError));
            }
        }
        
        public void LoadAd()
        {
            if (!_isInitialized)
            {
                _lastError = "Banner service not initialized";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(BANNER_PLACEMENT_ID, "Banner", _lastError));
                return;
            }
            
            if (_isLoading)
            {
                AdLogger.LogWarning("Banner ad is already loading");
                return;
            }
            
            _isLoading = true;
            AdLogger.Log("Loading banner ad...");
            
            // In a real implementation, this would call LevelPlay's banner load method
            // For this demo, we'll simulate loading with a coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateBannerLoad());
            }
        }
        
        public bool IsAdReady()
        {
            // In a real implementation, this would check LevelPlay's banner ready state
            return _isInitialized && !_isLoading;
        }
        
        public void ShowBanner(BannerPosition position = BannerPosition.Bottom)
        {
            if (!IsAdReady())
            {
                AdLogger.LogWarning("Banner ad is not ready to show");
                LoadAd(); // Auto-load if not ready
                return;
            }
            
            _currentPosition = position;
            
            try
            {
                // Create or update banner container
                CreateBannerContainer(position);
                
                _isBannerVisible = true;
                AdLogger.Log($"Banner ad shown at position: {position}");
                
                var eventArgs = new AdEventArgs(BANNER_PLACEMENT_ID, "Banner");
                OnAdDisplayed?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdDisplayed(eventArgs);
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to show banner: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(BANNER_PLACEMENT_ID, "Banner", _lastError));
            }
        }
        
        public void HideBanner()
        {
            if (!_isBannerVisible)
            {
                AdLogger.LogWarning("No banner is currently visible");
                return;
            }
            
            try
            {
                if (_bannerContainer != null)
                {
                    _bannerContainer.SetActive(false);
                }
                
                _isBannerVisible = false;
                AdLogger.Log("Banner ad hidden");
                
                var eventArgs = new AdEventArgs(BANNER_PLACEMENT_ID, "Banner");
                OnAdDismissed?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdDismissed(eventArgs);
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to hide banner: {ex.Message}";
                AdLogger.LogError(_lastError);
            }
        }
        
        public void SetBannerSize(BannerSize size)
        {
            _currentSize = size;
            AdLogger.Log($"Banner size set to: {size}");
            
            // Update banner container size if it exists
            if (_bannerContainer != null)
            {
                UpdateBannerSize();
            }
        }
        
        public void Destroy()
        {
            try
            {
                HideBanner();
                
                if (_bannerContainer != null)
                {
                    UnityEngine.Object.Destroy(_bannerContainer);
                    _bannerContainer = null;
                }
                
                _isInitialized = false;
                _isLoading = false;
                _isBannerVisible = false;
                
                AdLogger.Log("Banner Ad Service destroyed");
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error destroying banner service: {ex.Message}");
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
            yield return new WaitForSeconds(1f); // Simulate initialization delay
            
            _isInitialized = true;
            AdLogger.Log("Banner Ad Service initialized successfully");
        }
        
        private IEnumerator SimulateBannerLoad()
        {
            yield return new WaitForSeconds(2f); // Simulate load time
            
            _isLoading = false;
            
            // Simulate success/failure (90% success rate for demo)
            bool success = UnityEngine.Random.value > 0.1f;
            
            if (success)
            {
                AdLogger.Log("Banner ad loaded successfully");
                var eventArgs = new AdEventArgs(BANNER_PLACEMENT_ID, "Banner");
                OnAdLoaded?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoaded(eventArgs);
            }
            else
            {
                _lastError = "Simulated banner load failure";
                AdLogger.LogError(_lastError);
                var errorArgs = new AdErrorEventArgs(BANNER_PLACEMENT_ID, "Banner", _lastError, 1001);
                OnAdLoadFailed?.Invoke(errorArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoadFailed(errorArgs);
            }
        }
        
        private void CreateBannerContainer(BannerPosition position)
        {
            // Find or create the ad overlay canvas
            var overlayCanvas = FindAdOverlayCanvas();
            
            if (_bannerContainer == null)
            {
                _bannerContainer = new GameObject("BannerAdContainer");
                var rectTransform = _bannerContainer.AddComponent<RectTransform>();
                
                // Use configuration for visual representation
                var image = _bannerContainer.AddComponent<UnityEngine.UI.Image>();
                image.color = _containerConfig != null ? _containerConfig.BackgroundColor : new Color(0.2f, 0.6f, 1f, 0.8f);
                
                // Add click detection if enabled in configuration
                if (_containerConfig == null || _containerConfig.EnableClickInteraction)
                {
                    var button = _bannerContainer.AddComponent<UnityEngine.UI.Button>();
                    button.onClick.AddListener(OnBannerClicked);
                }
                
                // Create text with configuration settings
                CreateBannerText();
            }
            
            // Set parent and position
            _bannerContainer.transform.SetParent(overlayCanvas.transform, false);
            UpdateBannerPosition(position);
            UpdateBannerSize();
            
            _bannerContainer.SetActive(true);
            
            // Apply fade animation if enabled
            if (_containerConfig != null && _containerConfig.EnableFadeAnimation)
            {
                ApplyFadeInAnimation();
            }
        }
        
        private void UpdateBannerPosition(BannerPosition position)
        {
            if (_bannerContainer == null) return;
            
            var rectTransform = _bannerContainer.GetComponent<RectTransform>();
            
            switch (position)
            {
                case BannerPosition.Top:
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    rectTransform.pivot = new Vector2(0.5f, 1);
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    break;
                    
                case BannerPosition.Bottom:
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    rectTransform.pivot = new Vector2(0.5f, 0);
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    break;
                    
                case BannerPosition.Center:
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                    
                default:
                    // Default to bottom
                    goto case BannerPosition.Bottom;
            }
        }
        
        private void UpdateBannerSize()
        {
            if (_bannerContainer == null) return;
            
            var rectTransform = _bannerContainer.GetComponent<RectTransform>();
            Vector2 size;
            
            // Use configuration if available, otherwise fallback to original logic
            if (_containerConfig != null)
            {
                size = _containerConfig.GetDimensionsForSize(_currentSize);
            }
            else
            {
                // Fallback to original hardcoded values
                switch (_currentSize)
                {
                    case BannerSize.Standard:
                        size = new Vector2(320, 50);
                        break;
                    case BannerSize.Large:
                        size = new Vector2(320, 100);
                        break;
                    case BannerSize.Rectangle:
                        size = new Vector2(300, 250);
                        break;
                    case BannerSize.Smart:
                        size = new Vector2(Screen.width * 0.9f, 50);
                        break;
                    default:
                        size = new Vector2(320, 50);
                        break;
                }
            }
            
            rectTransform.sizeDelta = size;
            
            // Apply safe area adjustments if configured
            if (_containerConfig != null && _containerConfig.RespectSafeArea)
            {
                ApplySafeAreaAdjustments(rectTransform);
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
        
        private void OnBannerClicked()
        {
            AdLogger.Log("Banner ad clicked");
            
            var eventArgs = new AdEventArgs(BANNER_PLACEMENT_ID, "Banner");
            OnAdClicked?.Invoke(eventArgs);
            
            // Notify event dispatcher
            AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(eventArgs);
        }
        
        /// <summary>
        /// Loads the banner container configuration from Resources or creates a default one.
        /// This provides ScriptableObject-based configuration for user-friendly editing.
        /// </summary>
        private void LoadBannerContainerConfiguration()
        {
            // Try to load configuration from Resources folder
            _containerConfig = Resources.Load<BannerContainerConfiguration>("BannerContainerConfiguration");
            
            if (_containerConfig == null)
            {
                AdLogger.LogWarning("No BannerContainerConfiguration found in Resources. Using default settings. " +
                                   "Create a BannerContainerConfiguration asset via Create > Advertisement > Banner Container Configuration");
            }
            else
            {
                // Validate the loaded configuration
                if (!_containerConfig.ValidateConfiguration())
                {
                    AdLogger.LogWarning("BannerContainerConfiguration has validation issues. Check configuration settings.");
                }
            }
        }
        
        /// <summary>
        /// Creates the banner text component using configuration settings.
        /// </summary>
        private void CreateBannerText()
        {
            var textObj = new GameObject("BannerText");
            textObj.transform.SetParent(_bannerContainer.transform);
            var text = textObj.AddComponent<UnityEngine.UI.Text>();
            
            // Use configuration for text settings
            if (_containerConfig != null)
            {
                text.text = _containerConfig.GetFormattedText(_currentSize, _currentPosition);
                text.alignment = _containerConfig.TextAlignment;
                text.color = _containerConfig.TextColor;
                text.fontSize = _containerConfig.FontSize;
                
                // Use custom font if specified
                if (_containerConfig.CustomFont != null)
                {
                    text.font = _containerConfig.CustomFont;
                }
                else
                {
                    text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
            }
            else
            {
                // Fallback to original hardcoded values
                text.text = $"Demo Banner Ad\\n{_currentSize} - {_currentPosition}";
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            
            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        /// <summary>
        /// Applies fade-in animation to the banner container.
        /// </summary>
        private void ApplyFadeInAnimation()
        {
            if (_containerConfig == null || !_containerConfig.EnableFadeAnimation)
                return;
                
            var canvasGroup = _bannerContainer.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = _bannerContainer.AddComponent<CanvasGroup>();
            }
            
            // Start fade-in animation using coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(FadeInCoroutine(canvasGroup, _containerConfig.AnimationDuration));
            }
        }
        
        /// <summary>
        /// Coroutine for fade-in animation.
        /// </summary>
        private System.Collections.IEnumerator FadeInCoroutine(CanvasGroup canvasGroup, float duration)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        
        /// <summary>
        /// Applies safe area adjustments to the banner container.
        /// </summary>
        private void ApplySafeAreaAdjustments(RectTransform rectTransform)
        {
            if (_containerConfig == null)
                return;
                
            // Get safe area
            Rect safeArea = Screen.safeArea;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            
            // Apply padding from configuration
            var padding = _containerConfig.Padding;
            
            switch (_currentPosition)
            {
                case BannerPosition.Top:
                    float topSafeOffset = screenSize.y - (safeArea.y + safeArea.height);
                    rectTransform.anchoredPosition = new Vector2(0, -(topSafeOffset + padding.top));
                    break;
                    
                case BannerPosition.Bottom:
                    float bottomSafeOffset = safeArea.y;
                    rectTransform.anchoredPosition = new Vector2(0, bottomSafeOffset + padding.bottom);
                    break;
                    
                case BannerPosition.Center:
                    // Center position doesn't need safe area adjustments
                    break;
            }
        }
        
        /// <summary>
        /// Sets a custom banner container configuration. Useful for runtime configuration changes.
        /// </summary>
        /// <param name="config">The new configuration to use</param>
        public void SetBannerContainerConfiguration(BannerContainerConfiguration config)
        {
            _containerConfig = config;
            
            if (_containerConfig != null && !_containerConfig.ValidateConfiguration())
            {
                AdLogger.LogWarning("Provided BannerContainerConfiguration has validation issues.");
            }
            
            // Update existing banner if visible
            if (_isBannerVisible && _bannerContainer != null)
            {
                UpdateBannerAppearance();
            }
        }
        
        /// <summary>
        /// Updates the appearance of an existing banner container with current configuration.
        /// </summary>
        private void UpdateBannerAppearance()
        {
            if (_bannerContainer == null || _containerConfig == null)
                return;
                
            // Update background color
            var image = _bannerContainer.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                image.color = _containerConfig.BackgroundColor;
            }
            
            // Update text
            var textComponent = _bannerContainer.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComponent != null)
            {
                textComponent.text = _containerConfig.GetFormattedText(_currentSize, _currentPosition);
                textComponent.color = _containerConfig.TextColor;
                textComponent.alignment = _containerConfig.TextAlignment;
                textComponent.fontSize = _containerConfig.FontSize;
                
                if (_containerConfig.CustomFont != null)
                {
                    textComponent.font = _containerConfig.CustomFont;
                }
            }
            
            // Update size
            UpdateBannerSize();
        }
        
        #endregion
    }
}