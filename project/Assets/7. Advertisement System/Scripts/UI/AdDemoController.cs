using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Demo controller for the Advertisement System scene.
    /// Provides a user interface to test all advertisement types and
    /// demonstrates proper usage of the ad service manager.
    /// </summary>
    public class AdDemoController : MonoBehaviour
    {
        #region Inspector Fields
        
        [Header("UI References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Button bannerShowButton;
        [SerializeField] private Button bannerHideButton;
        [SerializeField] private Button interstitialLoadButton;
        [SerializeField] private Button interstitialShowButton;
        [SerializeField] private Button rewardedVideoLoadButton;
        [SerializeField] private Button rewardedVideoShowButton;
        [SerializeField] private Button playableLoadButton;
        [SerializeField] private Button playableShowButton;
        [SerializeField] private Button nativeLoadButton;
        [SerializeField] private Button nativeShowButton;
        [SerializeField] private Button nativeRemoveButton;
        [SerializeField] private Transform nativeAdContainer;
        
        [Header("Status Display")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI logText;
        [SerializeField] private ScrollRect logScrollRect;
        
        [Header("Configuration")]
        [SerializeField] private TMP_Dropdown bannerPositionDropdown;
        [SerializeField] private TMP_Dropdown bannerSizeDropdown;
        [SerializeField] private TMP_Dropdown nativeTemplateDropdown;
        [SerializeField] private Toggle testModeToggle;
        [SerializeField] private TMP_InputField appIdInputField;
        
        [Header("Rewards Display")]
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Button addCoinsButton;
        
        #endregion
        
        #region Private Fields
        
        private AdServiceManager _adManager;
        private List<string> _logMessages = new List<string>();
        private int _playerCoins = 0;
        private const int MAX_LOG_MESSAGES = 50;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Start()
        {
            InitializeUI();
            InitializeAdManager();
            SetupEventListeners();
            UpdateUI();
        }
        
        private void OnDestroy()
        {
            RemoveEventListeners();
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeUI()
        {
            // Setup dropdowns
            SetupBannerPositionDropdown();
            SetupBannerSizeDropdown();
            SetupNativeTemplateDropdown();
            
            // Set default values
            if (appIdInputField != null)
            {
                appIdInputField.text = "TEST_APP_ID_12345";
            }
            
            if (testModeToggle != null)
            {
                testModeToggle.isOn = true;
            }
            
            // Initial coin count
            UpdateCoinsDisplay();
            
            AddLog("Demo Controller initialized");
        }
        
        private void InitializeAdManager()
        {
            _adManager = AdServiceManager.Instance;
            
            if (_adManager != null)
            {
                AddLog("Ad Service Manager found");
            }
            else
            {
                AddLog("Ad Service Manager not found - creating new instance");
                var managerObj = new GameObject("AdServiceManager");
                _adManager = managerObj.AddComponent<AdServiceManager>();
            }
        }
        
        private void SetupEventListeners()
        {
            // Button listeners
            if (bannerShowButton != null)
                bannerShowButton.onClick.AddListener(OnShowBannerClicked);
            if (bannerHideButton != null)
                bannerHideButton.onClick.AddListener(OnHideBannerClicked);
            
            if (interstitialLoadButton != null)
                interstitialLoadButton.onClick.AddListener(OnLoadInterstitialClicked);
            if (interstitialShowButton != null)
                interstitialShowButton.onClick.AddListener(OnShowInterstitialClicked);
            
            if (rewardedVideoLoadButton != null)
                rewardedVideoLoadButton.onClick.AddListener(OnLoadRewardedVideoClicked);
            if (rewardedVideoShowButton != null)
                rewardedVideoShowButton.onClick.AddListener(OnShowRewardedVideoClicked);
            
            if (playableLoadButton != null)
                playableLoadButton.onClick.AddListener(OnLoadPlayableClicked);
            if (playableShowButton != null)
                playableShowButton.onClick.AddListener(OnShowPlayableClicked);
            
            if (nativeLoadButton != null)
                nativeLoadButton.onClick.AddListener(OnLoadNativeClicked);
            if (nativeShowButton != null)
                nativeShowButton.onClick.AddListener(OnShowNativeClicked);
            if (nativeRemoveButton != null)
                nativeRemoveButton.onClick.AddListener(OnRemoveNativeClicked);
            
            if (addCoinsButton != null)
                addCoinsButton.onClick.AddListener(OnAddCoinsClicked);
            
            // Ad Service Manager events
            if (_adManager != null)
            {
                _adManager.OnInitialized += OnAdManagerInitialized;
                _adManager.OnInitializationFailed += OnAdManagerInitializationFailed;
                
                // Global ad events
                if (_adManager.EventDispatcher != null)
                {
                    _adManager.EventDispatcher.OnAnyAdLoaded += OnAnyAdLoaded;
                    _adManager.EventDispatcher.OnAnyAdLoadFailed += OnAnyAdLoadFailed;
                    _adManager.EventDispatcher.OnAnyAdDisplayed += OnAnyAdDisplayed;
                    _adManager.EventDispatcher.OnAnyAdDismissed += OnAnyAdDismissed;
                    _adManager.EventDispatcher.OnAnyAdClicked += OnAnyAdClicked;
                    _adManager.EventDispatcher.OnAnyRewardEarned += OnAnyRewardEarned;
                    _adManager.EventDispatcher.OnAnyPlayableCompleted += OnAnyPlayableCompleted;
                }
            }
        }
        
        private void RemoveEventListeners()
        {
            if (_adManager != null)
            {
                _adManager.OnInitialized -= OnAdManagerInitialized;
                _adManager.OnInitializationFailed -= OnAdManagerInitializationFailed;
                
                if (_adManager.EventDispatcher != null)
                {
                    _adManager.EventDispatcher.OnAnyAdLoaded -= OnAnyAdLoaded;
                    _adManager.EventDispatcher.OnAnyAdLoadFailed -= OnAnyAdLoadFailed;
                    _adManager.EventDispatcher.OnAnyAdDisplayed -= OnAnyAdDisplayed;
                    _adManager.EventDispatcher.OnAnyAdDismissed -= OnAnyAdDismissed;
                    _adManager.EventDispatcher.OnAnyAdClicked -= OnAnyAdClicked;
                    _adManager.EventDispatcher.OnAnyRewardEarned -= OnAnyRewardEarned;
                    _adManager.EventDispatcher.OnAnyPlayableCompleted -= OnAnyPlayableCompleted;
                }
            }
        }
        
        #endregion
        
        #region Dropdown Setup
        
        private void SetupBannerPositionDropdown()
        {
            if (bannerPositionDropdown == null) return;
            
            bannerPositionDropdown.ClearOptions();
            var options = new List<string>();
            
            foreach (BannerPosition position in System.Enum.GetValues(typeof(BannerPosition)))
            {
                options.Add(position.ToString());
            }
            
            bannerPositionDropdown.AddOptions(options);
            bannerPositionDropdown.value = 1; // Default to Bottom
        }
        
        private void SetupBannerSizeDropdown()
        {
            if (bannerSizeDropdown == null) return;
            
            bannerSizeDropdown.ClearOptions();
            var options = new List<string>();
            
            foreach (BannerSize size in System.Enum.GetValues(typeof(BannerSize)))
            {
                options.Add(size.ToString());
            }
            
            bannerSizeDropdown.AddOptions(options);
        }
        
        private void SetupNativeTemplateDropdown()
        {
            if (nativeTemplateDropdown == null) return;
            
            nativeTemplateDropdown.ClearOptions();
            var options = new List<string>();
            
            foreach (NativeAdTemplate template in System.Enum.GetValues(typeof(NativeAdTemplate)))
            {
                options.Add(template.ToString());
            }
            
            nativeTemplateDropdown.AddOptions(options);
            nativeTemplateDropdown.value = 1; // Default to Medium
        }
        
        #endregion
        
        #region Button Handlers
        
        private void OnShowBannerClicked()
        {
            if (_adManager?.BannerService == null)
            {
                AddLog("Banner service not available");
                return;
            }
            
            var position = (BannerPosition)bannerPositionDropdown.value;
            var size = (BannerSize)bannerSizeDropdown.value;
            
            _adManager.BannerService.SetBannerSize(size);
            _adManager.BannerService.ShowBanner(position);
            
            AddLog($"Showing banner: {position}, {size}");
        }
        
        private void OnHideBannerClicked()
        {
            if (_adManager?.BannerService == null)
            {
                AddLog("Banner service not available");
                return;
            }
            
            _adManager.BannerService.HideBanner();
            AddLog("Hiding banner");
        }
        
        private void OnLoadInterstitialClicked()
        {
            if (_adManager?.InterstitialService == null)
            {
                AddLog("Interstitial service not available");
                return;
            }
            
            _adManager.InterstitialService.LoadAd();
            AddLog("Loading interstitial ad");
        }
        
        private void OnShowInterstitialClicked()
        {
            if (_adManager?.InterstitialService == null)
            {
                AddLog("Interstitial service not available");
                return;
            }
            
            _adManager.InterstitialService.ShowInterstitial((success) =>
            {
                AddLog($"Interstitial completed: {success}");
            });
            AddLog("Showing interstitial ad");
        }
        
        private void OnLoadRewardedVideoClicked()
        {
            if (_adManager?.RewardedVideoService == null)
            {
                AddLog("Rewarded video service not available");
                return;
            }
            
            _adManager.RewardedVideoService.LoadAd();
            AddLog("Loading rewarded video ad");
        }
        
        private void OnShowRewardedVideoClicked()
        {
            if (_adManager?.RewardedVideoService == null)
            {
                AddLog("Rewarded video service not available");
                return;
            }
            
            _adManager.RewardedVideoService.ShowRewardedVideo((reward) =>
            {
                if (reward.IsValid)
                {
                    AddCoins(reward.Amount);
                    AddLog($"Reward earned: {reward.Amount} {reward.RewardType}");
                }
                else
                {
                    AddLog("No reward earned - video not completed");
                }
            });
            AddLog("Showing rewarded video ad");
        }
        
        private void OnLoadPlayableClicked()
        {
            if (_adManager?.PlayableService == null)
            {
                AddLog("Playable service not available");
                return;
            }
            
            _adManager.PlayableService.LoadAd();
            AddLog("Loading playable ad");
        }
        
        private void OnShowPlayableClicked()
        {
            if (_adManager?.PlayableService == null)
            {
                AddLog("Playable service not available");
                return;
            }
            
            _adManager.PlayableService.ShowPlayable((completed) =>
            {
                AddLog($"Playable completed: {completed}");
            });
            AddLog("Showing playable ad");
        }
        
        private void OnLoadNativeClicked()
        {
            if (_adManager?.NativeService == null)
            {
                AddLog("Native service not available");
                return;
            }
            
            var template = (NativeAdTemplate)nativeTemplateDropdown.value;
            _adManager.NativeService.LoadNativeAd(template);
            AddLog($"Loading native ad: {template}");
        }
        
        private void OnShowNativeClicked()
        {
            if (_adManager?.NativeService == null)
            {
                AddLog("Native service not available");
                return;
            }
            
            if (nativeAdContainer == null)
            {
                AddLog("Native ad container not assigned");
                return;
            }
            
            _adManager.NativeService.ShowNativeAd(nativeAdContainer);
            AddLog("Showing native ad");
        }
        
        private void OnRemoveNativeClicked()
        {
            if (_adManager?.NativeService == null)
            {
                AddLog("Native service not available");
                return;
            }
            
            _adManager.NativeService.RemoveNativeAd();
            AddLog("Removing native ad");
        }
        
        private void OnAddCoinsClicked()
        {
            AddCoins(50);
            AddLog("Added 50 bonus coins");
        }
        
        #endregion
        
        #region Ad Event Handlers
        
        private void OnAdManagerInitialized()
        {
            AddLog("Ad Manager initialized successfully");
            UpdateUI();
        }
        
        private void OnAdManagerInitializationFailed(string error)
        {
            AddLog($"Ad Manager initialization failed: {error}");
            UpdateUI();
        }
        
        private void OnAnyAdLoaded(AdEventArgs eventArgs)
        {
            AddLog($"✓ {eventArgs.AdType} loaded");
            UpdateUI();
        }
        
        private void OnAnyAdLoadFailed(AdErrorEventArgs errorArgs)
        {
            AddLog($"✗ {errorArgs.AdType} load failed: {errorArgs.ErrorMessage}");
            UpdateUI();
        }
        
        private void OnAnyAdDisplayed(AdEventArgs eventArgs)
        {
            AddLog($"👁 {eventArgs.AdType} displayed");
        }
        
        private void OnAnyAdDismissed(AdEventArgs eventArgs)
        {
            AddLog($"✖ {eventArgs.AdType} dismissed");
        }
        
        private void OnAnyAdClicked(AdEventArgs eventArgs)
        {
            AddLog($"👆 {eventArgs.AdType} clicked");
        }
        
        private void OnAnyRewardEarned(AdRewardEventArgs rewardArgs)
        {
            AddLog($"🎁 Reward: {rewardArgs.Amount} {rewardArgs.RewardType}");
        }
        
        private void OnAnyPlayableCompleted(AdEventArgs eventArgs)
        {
            AddLog($"🎮 Playable completed");
        }
        
        #endregion
        
        #region UI Update Methods
        
        private void UpdateUI()
        {
            UpdateStatusDisplay();
            UpdateButtonStates();
        }
        
        private void UpdateStatusDisplay()
        {
            if (statusText == null || _adManager == null) return;
            
            var status = new System.Text.StringBuilder();
            status.AppendLine($"Ad Manager: {(_adManager.IsInitialized ? "Initialized" : "Not Initialized")}");
            
            if (_adManager.IsInitialized)
            {
                var servicesStatus = _adManager.GetServicesStatus();
                foreach (var service in servicesStatus)
                {
                    string serviceName = service.Key.Replace("AdService", "").Replace("I", "");
                    status.AppendLine($"{serviceName}: {(service.Value ? "Ready" : "Not Ready")}");
                }
            }
            
            statusText.text = status.ToString();
        }
        
        private void UpdateButtonStates()
        {
            bool managerReady = _adManager != null && _adManager.IsInitialized;
            
            // Banner buttons
            if (bannerShowButton != null)
                bannerShowButton.interactable = managerReady && _adManager.BannerService != null;
            if (bannerHideButton != null)
                bannerHideButton.interactable = managerReady && _adManager.BannerService?.IsBannerVisible == true;
            
            // Interstitial buttons
            if (interstitialLoadButton != null)
                interstitialLoadButton.interactable = managerReady && _adManager.InterstitialService != null;
            if (interstitialShowButton != null)
                interstitialShowButton.interactable = managerReady && _adManager.InterstitialService?.IsAdReady() == true;
            
            // Rewarded video buttons
            if (rewardedVideoLoadButton != null)
                rewardedVideoLoadButton.interactable = managerReady && _adManager.RewardedVideoService != null;
            if (rewardedVideoShowButton != null)
                rewardedVideoShowButton.interactable = managerReady && _adManager.RewardedVideoService?.IsAdReady() == true;
            
            // Playable buttons
            if (playableLoadButton != null)
                playableLoadButton.interactable = managerReady && _adManager.PlayableService != null;
            if (playableShowButton != null)
                playableShowButton.interactable = managerReady && _adManager.PlayableService?.IsAdReady() == true;
            
            // Native buttons
            if (nativeLoadButton != null)
                nativeLoadButton.interactable = managerReady && _adManager.NativeService != null;
            if (nativeShowButton != null)
                nativeShowButton.interactable = managerReady && _adManager.NativeService?.IsAdReady() == true;
            if (nativeRemoveButton != null)
                nativeRemoveButton.interactable = managerReady && _adManager.NativeService != null;
        }
        
        private void UpdateCoinsDisplay()
        {
            if (coinsText != null)
            {
                coinsText.text = $"Coins: {_playerCoins}";
            }
        }
        
        #endregion
        
        #region Logging
        
        private void AddLog(string message)
        {
            string timestampedMessage = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
            _logMessages.Add(timestampedMessage);
            
            // Keep only the last MAX_LOG_MESSAGES
            while (_logMessages.Count > MAX_LOG_MESSAGES)
            {
                _logMessages.RemoveAt(0);
            }
            
            UpdateLogDisplay();
            Debug.Log($"[AdDemo] {message}");
        }
        
        private void UpdateLogDisplay()
        {
            if (logText == null) return;
            
            logText.text = string.Join("\\n", _logMessages);
            
            // Scroll to bottom
            if (logScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                logScrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        [ContextMenu("Clear Log")]
        public void ClearLog()
        {
            _logMessages.Clear();
            UpdateLogDisplay();
        }
        
        #endregion
        
        #region Currency Management
        
        private void AddCoins(int amount)
        {
            _playerCoins += amount;
            UpdateCoinsDisplay();
        }
        
        [ContextMenu("Reset Coins")]
        public void ResetCoins()
        {
            _playerCoins = 0;
            UpdateCoinsDisplay();
            AddLog("Coins reset to 0");
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Initializes the ad manager with custom settings (can be called from UI)
        /// </summary>
        public void InitializeAdManagerWithSettings()
        {
            if (_adManager == null) return;
            
            string appId = appIdInputField?.text ?? "TEST_APP_ID";
            bool testMode = testModeToggle?.isOn ?? true;
            
            _adManager.Initialize(appId, testMode);
            AddLog($"Initializing with App ID: {appId}, Test Mode: {testMode}");
        }
        
        /// <summary>
        /// Shows information about popular advertisement techniques
        /// </summary>
        [ContextMenu("Show Ad Techniques Info")]
        public void ShowAdTechniquesInfo()
        {
            AddLog("=== Popular Advertisement Techniques ===");
            AddLog("1. Banner Refresh: Automatic rotation for increased revenue");
            AddLog("2. Interstitial Pacing: Strategic placement at break points");
            AddLog("3. Rewarded Opt-in: Optional videos for in-game benefits");
            AddLog("4. Native Integration: Ads matching app visual design");
            AddLog("5. Playable Previews: Interactive ads for better engagement");
            AddLog("6. Frequency Capping: Limit exposure to prevent fatigue");
            AddLog("7. Loading Indicators: Visual feedback during preparation");
            AddLog("8. Preloading: Load ads before display for instant showing");
        }
        
        #endregion
    }
}