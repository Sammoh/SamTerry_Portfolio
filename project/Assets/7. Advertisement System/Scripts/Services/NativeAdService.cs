using System;
using System.Collections;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Native advertisement service implementation for LevelPlay SDK.
    /// Handles native ads that are designed to match the app's visual design
    /// and integrate seamlessly into the user interface.
    /// </summary>
    public class NativeAdService : INativeAdService
    {
        #region Private Fields
        
        private bool _isInitialized = false;
        private bool _isLoading = false;
        private string _lastError = string.Empty;
        private string _appId;
        private bool _testMode;
        private bool _isAdReady = false;
        private MonoBehaviour _coroutineRunner;
        private GameObject _currentNativeAdContainer;
        private NativeAdTemplate _currentTemplate = NativeAdTemplate.Medium;
        
        // Placement ID for native ads
        private const string NATIVE_PLACEMENT_ID = "DefaultNative";
        
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
                
                AdLogger.Log($"Initializing Native Ad Service - App ID: {appId}, Test Mode: {testMode}");
                
                // Get or create coroutine runner
                var manager = AdServiceManager.Instance;
                _coroutineRunner = manager;
                
                // In a real implementation, this would initialize the LevelPlay native SDK
                // For this demo, we'll simulate initialization
                SimulateInitialization();
                
            }
            catch (Exception ex)
            {
                _lastError = $"Native service initialization failed: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(NATIVE_PLACEMENT_ID, "Native", _lastError));
            }
        }
        
        public void LoadAd()
        {
            LoadNativeAd(NativeAdTemplate.Medium);
        }
        
        public void LoadNativeAd(NativeAdTemplate templateType)
        {
            if (!_isInitialized)
            {
                _lastError = "Native service not initialized";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(NATIVE_PLACEMENT_ID, "Native", _lastError));
                return;
            }
            
            if (_isLoading)
            {
                AdLogger.LogWarning("Native ad is already loading");
                return;
            }
            
            if (_isAdReady)
            {
                AdLogger.LogWarning("Native ad is already loaded and ready");
                return;
            }
            
            _currentTemplate = templateType;
            _isLoading = true;
            _isAdReady = false;
            AdLogger.Log($"Loading native ad with template: {templateType}...");
            
            // In a real implementation, this would call LevelPlay's native load method
            // For this demo, we'll simulate loading with a coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateNativeLoad());
            }
        }
        
        public bool IsAdReady()
        {
            return _isInitialized && _isAdReady && !_isLoading;
        }
        
        public void ShowNativeAd(Transform container)
        {
            if (!IsAdReady())
            {
                _lastError = "Native ad is not ready to show";
                AdLogger.LogWarning(_lastError);
                
                // Auto-load if not ready
                LoadAd();
                return;
            }
            
            if (container == null)
            {
                _lastError = "Container transform is null";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(NATIVE_PLACEMENT_ID, "Native", _lastError));
                return;
            }
            
            try
            {
                AdLogger.Log($"Showing native ad in container: {container.name}");
                
                // Create native ad UI
                CreateNativeAdUI(container);
                
                var eventArgs = new AdEventArgs(NATIVE_PLACEMENT_ID, "Native");
                OnAdDisplayed?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdDisplayed(eventArgs);
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to show native ad: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(NATIVE_PLACEMENT_ID, "Native", _lastError));
            }
        }
        
        public void RemoveNativeAd()
        {
            if (_currentNativeAdContainer == null)
            {
                AdLogger.LogWarning("No native ad is currently displayed");
                return;
            }
            
            try
            {
                UnityEngine.Object.Destroy(_currentNativeAdContainer);
                _currentNativeAdContainer = null;
                
                AdLogger.Log("Native ad removed");
                
                var eventArgs = new AdEventArgs(NATIVE_PLACEMENT_ID, "Native");
                OnAdDismissed?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdDismissed(eventArgs);
                
                // Mark as ready for next use (native ads can be reused)
                _isAdReady = true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to remove native ad: {ex.Message}";
                AdLogger.LogError(_lastError);
            }
        }
        
        public void Destroy()
        {
            try
            {
                RemoveNativeAd();
                
                _isInitialized = false;
                _isLoading = false;
                _isAdReady = false;
                
                AdLogger.Log("Native Ad Service destroyed");
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error destroying native service: {ex.Message}");
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
            yield return new WaitForSeconds(1.2f); // Simulate initialization delay
            
            _isInitialized = true;
            AdLogger.Log("Native Ad Service initialized successfully");
            
            // Auto-load the first ad
            LoadAd();
        }
        
        private IEnumerator SimulateNativeLoad()
        {
            yield return new WaitForSeconds(2.5f); // Simulate load time
            
            _isLoading = false;
            
            // Simulate success/failure (85% success rate for demo)
            bool success = UnityEngine.Random.value > 0.15f;
            
            if (success)
            {
                _isAdReady = true;
                AdLogger.Log($"Native ad loaded successfully with template: {_currentTemplate}");
                
                var eventArgs = new AdEventArgs(NATIVE_PLACEMENT_ID, "Native");
                OnAdLoaded?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoaded(eventArgs);
            }
            else
            {
                _lastError = "Simulated native load failure";
                AdLogger.LogError(_lastError);
                var errorArgs = new AdErrorEventArgs(NATIVE_PLACEMENT_ID, "Native", _lastError, 5001);
                OnAdLoadFailed?.Invoke(errorArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoadFailed(errorArgs);
            }
        }
        
        private void CreateNativeAdUI(Transform container)
        {
            // Remove existing native ad if present
            if (_currentNativeAdContainer != null)
            {
                UnityEngine.Object.Destroy(_currentNativeAdContainer);
            }
            
            // Create native ad container
            _currentNativeAdContainer = new GameObject("NativeAdContainer");
            var rectTransform = _currentNativeAdContainer.AddComponent<RectTransform>();
            
            // Set parent and fill container
            rectTransform.SetParent(container, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add background
            var background = _currentNativeAdContainer.AddComponent<UnityEngine.UI.Image>();
            background.color = new Color(0.95f, 0.95f, 0.95f, 1f); // Light gray background
            
            // Add click detection
            var button = _currentNativeAdContainer.AddComponent<UnityEngine.UI.Button>();
            button.onClick.AddListener(OnNativeAdClicked);
            
            // Create ad content based on template
            switch (_currentTemplate)
            {
                case NativeAdTemplate.Small:
                    CreateSmallNativeTemplate();
                    break;
                case NativeAdTemplate.Medium:
                    CreateMediumNativeTemplate();
                    break;
                case NativeAdTemplate.Large:
                    CreateLargeNativeTemplate();
                    break;
            }
        }
        
        private void CreateSmallNativeTemplate()
        {
            // Small template: Icon + Title + CTA button (horizontal layout)
            
            // Add icon
            var iconObj = new GameObject("AdIcon");
            iconObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.05f, 0.2f);
            iconRect.anchorMax = new Vector2(0.25f, 0.8f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            var iconImage = iconObj.AddComponent<UnityEngine.UI.Image>();
            iconImage.color = new Color(0.3f, 0.7f, 1f, 1f); // Blue icon placeholder
            
            // Add title
            var titleObj = new GameObject("AdTitle");
            titleObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Demo App";
            titleText.alignment = TextAnchor.MiddleLeft;
            titleText.color = Color.black;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 14;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.3f, 0.5f);
            titleRect.anchorMax = new Vector2(0.7f, 0.8f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add CTA button
            var ctaObj = new GameObject("CTAButton");
            ctaObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var ctaButton = ctaObj.AddComponent<UnityEngine.UI.Button>();
            var ctaImage = ctaObj.AddComponent<UnityEngine.UI.Image>();
            ctaImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green CTA button
            
            var ctaRect = ctaButton.GetComponent<RectTransform>();
            ctaRect.anchorMin = new Vector2(0.75f, 0.3f);
            ctaRect.anchorMax = new Vector2(0.95f, 0.7f);
            ctaRect.offsetMin = Vector2.zero;
            ctaRect.offsetMax = Vector2.zero;
            
            // Add CTA text
            var ctaTextObj = new GameObject("CTAText");
            ctaTextObj.transform.SetParent(ctaObj.transform, false);
            var ctaText = ctaTextObj.AddComponent<UnityEngine.UI.Text>();
            ctaText.text = "Install";
            ctaText.alignment = TextAnchor.MiddleCenter;
            ctaText.color = Color.white;
            ctaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ctaText.fontSize = 12;
            ctaText.fontStyle = FontStyle.Bold;
            
            var ctaTextRect = ctaText.GetComponent<RectTransform>();
            ctaTextRect.anchorMin = Vector2.zero;
            ctaTextRect.anchorMax = Vector2.one;
            ctaTextRect.offsetMin = Vector2.zero;
            ctaTextRect.offsetMax = Vector2.zero;
        }
        
        private void CreateMediumNativeTemplate()
        {
            // Medium template: Icon + Title + Description + CTA (vertical layout)
            
            // Add icon
            var iconObj = new GameObject("AdIcon");
            iconObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.05f, 0.7f);
            iconRect.anchorMax = new Vector2(0.25f, 0.95f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            var iconImage = iconObj.AddComponent<UnityEngine.UI.Image>();
            iconImage.color = new Color(0.3f, 0.7f, 1f, 1f); // Blue icon placeholder
            
            // Add title
            var titleObj = new GameObject("AdTitle");
            titleObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Amazing Demo App";
            titleText.alignment = TextAnchor.MiddleLeft;
            titleText.color = Color.black;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 16;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.3f, 0.8f);
            titleRect.anchorMax = new Vector2(0.95f, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add description
            var descObj = new GameObject("AdDescription");
            descObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var descText = descObj.AddComponent<UnityEngine.UI.Text>();
            descText.text = "Experience the best mobile app with amazing features and great user experience.";
            descText.alignment = TextAnchor.UpperLeft;
            descText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 12;
            
            var descRect = descText.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.4f);
            descRect.anchorMax = new Vector2(0.95f, 0.75f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            
            // Add CTA button
            var ctaObj = new GameObject("CTAButton");
            ctaObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var ctaButton = ctaObj.AddComponent<UnityEngine.UI.Button>();
            var ctaImage = ctaObj.AddComponent<UnityEngine.UI.Image>();
            ctaImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green CTA button
            
            var ctaRect = ctaButton.GetComponent<RectTransform>();
            ctaRect.anchorMin = new Vector2(0.7f, 0.1f);
            ctaRect.anchorMax = new Vector2(0.95f, 0.3f);
            ctaRect.offsetMin = Vector2.zero;
            ctaRect.offsetMax = Vector2.zero;
            
            // Add CTA text
            var ctaTextObj = new GameObject("CTAText");
            ctaTextObj.transform.SetParent(ctaObj.transform, false);
            var ctaText = ctaTextObj.AddComponent<UnityEngine.UI.Text>();
            ctaText.text = "Download Now";
            ctaText.alignment = TextAnchor.MiddleCenter;
            ctaText.color = Color.white;
            ctaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ctaText.fontSize = 14;
            ctaText.fontStyle = FontStyle.Bold;
            
            var ctaTextRect = ctaText.GetComponent<RectTransform>();
            ctaTextRect.anchorMin = Vector2.zero;
            ctaTextRect.anchorMax = Vector2.one;
            ctaTextRect.offsetMin = Vector2.zero;
            ctaTextRect.offsetMax = Vector2.zero;
            
            // Add ad label
            var adLabelObj = new GameObject("AdLabel");
            adLabelObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var adLabelText = adLabelObj.AddComponent<UnityEngine.UI.Text>();
            adLabelText.text = "Ad";
            adLabelText.alignment = TextAnchor.MiddleCenter;
            adLabelText.color = Color.gray;
            adLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            adLabelText.fontSize = 10;
            
            var adLabelRect = adLabelText.GetComponent<RectTransform>();
            adLabelRect.anchorMin = new Vector2(0.05f, 0.1f);
            adLabelRect.anchorMax = new Vector2(0.15f, 0.2f);
            adLabelRect.offsetMin = Vector2.zero;
            adLabelRect.offsetMax = Vector2.zero;
        }
        
        private void CreateLargeNativeTemplate()
        {
            // Large template: Image + Icon + Title + Description + CTA + Rating
            
            // Add main image
            var imageObj = new GameObject("AdImage");
            imageObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var imageRect = imageObj.AddComponent<RectTransform>();
            imageRect.anchorMin = new Vector2(0.05f, 0.6f);
            imageRect.anchorMax = new Vector2(0.95f, 0.95f);
            imageRect.offsetMin = Vector2.zero;
            imageRect.offsetMax = Vector2.zero;
            
            var adImage = imageObj.AddComponent<UnityEngine.UI.Image>();
            adImage.color = new Color(0.4f, 0.6f, 0.8f, 1f); // Blue image placeholder
            
            // Add icon
            var iconObj = new GameObject("AdIcon");
            iconObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.05f, 0.4f);
            iconRect.anchorMax = new Vector2(0.2f, 0.55f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;
            
            var iconImage = iconObj.AddComponent<UnityEngine.UI.Image>();
            iconImage.color = new Color(0.3f, 0.7f, 1f, 1f); // Blue icon placeholder
            
            // Add title
            var titleObj = new GameObject("AdTitle");
            titleObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Super Amazing Game";
            titleText.alignment = TextAnchor.MiddleLeft;
            titleText.color = Color.black;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.25f, 0.45f);
            titleRect.anchorMax = new Vector2(0.95f, 0.55f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add rating
            var ratingObj = new GameObject("AdRating");
            ratingObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var ratingText = ratingObj.AddComponent<UnityEngine.UI.Text>();
            ratingText.text = "★★★★☆ 4.5";
            ratingText.alignment = TextAnchor.MiddleLeft;
            ratingText.color = new Color(1f, 0.8f, 0f, 1f); // Gold color
            ratingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ratingText.fontSize = 14;
            
            var ratingRect = ratingText.GetComponent<RectTransform>();
            ratingRect.anchorMin = new Vector2(0.25f, 0.35f);
            ratingRect.anchorMax = new Vector2(0.6f, 0.45f);
            ratingRect.offsetMin = Vector2.zero;
            ratingRect.offsetMax = Vector2.zero;
            
            // Add description
            var descObj = new GameObject("AdDescription");
            descObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var descText = descObj.AddComponent<UnityEngine.UI.Text>();
            descText.text = "Join millions of players in this epic adventure! Amazing graphics, engaging gameplay, and endless fun await you.";
            descText.alignment = TextAnchor.UpperLeft;
            descText.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            descText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            descText.fontSize = 12;
            
            var descRect = descText.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.05f, 0.15f);
            descRect.anchorMax = new Vector2(0.65f, 0.35f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;
            
            // Add CTA button
            var ctaObj = new GameObject("CTAButton");
            ctaObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var ctaButton = ctaObj.AddComponent<UnityEngine.UI.Button>();
            var ctaImage = ctaObj.AddComponent<UnityEngine.UI.Image>();
            ctaImage.color = new Color(0.2f, 0.8f, 0.2f, 1f); // Green CTA button
            
            var ctaRect = ctaButton.GetComponent<RectTransform>();
            ctaRect.anchorMin = new Vector2(0.7f, 0.15f);
            ctaRect.anchorMax = new Vector2(0.95f, 0.35f);
            ctaRect.offsetMin = Vector2.zero;
            ctaRect.offsetMax = Vector2.zero;
            
            // Add CTA text
            var ctaTextObj = new GameObject("CTAText");
            ctaTextObj.transform.SetParent(ctaObj.transform, false);
            var ctaText = ctaTextObj.AddComponent<UnityEngine.UI.Text>();
            ctaText.text = "PLAY NOW";
            ctaText.alignment = TextAnchor.MiddleCenter;
            ctaText.color = Color.white;
            ctaText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ctaText.fontSize = 16;
            ctaText.fontStyle = FontStyle.Bold;
            
            var ctaTextRect = ctaText.GetComponent<RectTransform>();
            ctaTextRect.anchorMin = Vector2.zero;
            ctaTextRect.anchorMax = Vector2.one;
            ctaTextRect.offsetMin = Vector2.zero;
            ctaTextRect.offsetMax = Vector2.zero;
            
            // Add ad label
            var adLabelObj = new GameObject("AdLabel");
            adLabelObj.transform.SetParent(_currentNativeAdContainer.transform, false);
            var adLabelText = adLabelObj.AddComponent<UnityEngine.UI.Text>();
            adLabelText.text = "Sponsored";
            adLabelText.alignment = TextAnchor.MiddleCenter;
            adLabelText.color = Color.gray;
            adLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            adLabelText.fontSize = 10;
            
            var adLabelRect = adLabelText.GetComponent<RectTransform>();
            adLabelRect.anchorMin = new Vector2(0.05f, 0.05f);
            adLabelRect.anchorMax = new Vector2(0.2f, 0.15f);
            adLabelRect.offsetMin = Vector2.zero;
            adLabelRect.offsetMax = Vector2.zero;
        }
        
        private void OnNativeAdClicked()
        {
            AdLogger.Log("Native ad clicked");
            
            var eventArgs = new AdEventArgs(NATIVE_PLACEMENT_ID, "Native");
            OnAdClicked?.Invoke(eventArgs);
            
            // Notify event dispatcher
            AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(eventArgs);
        }
        
        #endregion
    }
}