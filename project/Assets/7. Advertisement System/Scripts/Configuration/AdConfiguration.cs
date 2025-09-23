using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// ScriptableObject configuration for advertisement settings.
    /// Allows for easy configuration of ad behavior without code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "AdConfiguration", menuName = "Advertisement/Ad Configuration", order = 1)]
    public class AdConfiguration : ScriptableObject
    {
        [Header("General Settings")]
        [SerializeField] private string appId = "YOUR_APP_ID";
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool enableLogging = true;
        
        [Header("Service Configuration")]
        [SerializeField] private bool enableBannerAds = true;
        [SerializeField] private bool enableInterstitialAds = true;
        [SerializeField] private bool enableRewardedVideoAds = true;
        [SerializeField] private bool enablePlayableAds = true;
        [SerializeField] private bool enableNativeAds = true;
        
        [Header("Banner Settings")]
        [SerializeField] private BannerPosition defaultBannerPosition = BannerPosition.Bottom;
        [SerializeField] private BannerSize defaultBannerSize = BannerSize.Standard;
        [SerializeField] private float bannerRefreshRate = 60f; // seconds
        
        [Header("Interstitial Settings")]
        [SerializeField] private float interstitialCooldown = 300f; // seconds
        [SerializeField] private int interstitialFrequencyCap = 3; // per session
        
        [Header("Rewarded Video Settings")]
        [SerializeField] private string defaultRewardType = "Coins";
        [SerializeField] private int defaultRewardAmount = 100;
        [SerializeField] private float rewardedVideoCooldown = 180f; // seconds
        
        [Header("Native Ad Settings")]
        [SerializeField] private NativeAdTemplate defaultNativeTemplate = NativeAdTemplate.Medium;
        
        [Header("Debug Settings")]
        [SerializeField] private bool showDebugOverlay = true;
        [SerializeField] private bool simulateSlowNetwork = false;
        [SerializeField] private float networkDelayMultiplier = 1f;
        
        // Public Properties
        public string AppId => appId;
        public bool TestMode => testMode;
        public bool EnableLogging => enableLogging;
        
        public bool EnableBannerAds => enableBannerAds;
        public bool EnableInterstitialAds => enableInterstitialAds;
        public bool EnableRewardedVideoAds => enableRewardedVideoAds;
        public bool EnablePlayableAds => enablePlayableAds;
        public bool EnableNativeAds => enableNativeAds;
        
        public BannerPosition DefaultBannerPosition => defaultBannerPosition;
        public BannerSize DefaultBannerSize => defaultBannerSize;
        public float BannerRefreshRate => bannerRefreshRate;
        
        public float InterstitialCooldown => interstitialCooldown;
        public int InterstitialFrequencyCap => interstitialFrequencyCap;
        
        public string DefaultRewardType => defaultRewardType;
        public int DefaultRewardAmount => defaultRewardAmount;
        public float RewardedVideoCooldown => rewardedVideoCooldown;
        
        public NativeAdTemplate DefaultNativeTemplate => defaultNativeTemplate;
        
        public bool ShowDebugOverlay => showDebugOverlay;
        public bool SimulateSlowNetwork => simulateSlowNetwork;
        public float NetworkDelayMultiplier => networkDelayMultiplier;
        
        /// <summary>
        /// Validates the configuration settings and logs any issues.
        /// </summary>
        public bool ValidateConfiguration()
        {
            bool isValid = true;
            
            if (string.IsNullOrEmpty(appId) || appId == "YOUR_APP_ID")
            {
                AdLogger.LogError("Invalid App ID in configuration. Please set a valid LevelPlay App ID.");
                isValid = false;
            }
            
            if (bannerRefreshRate < 30f)
            {
                AdLogger.LogWarning("Banner refresh rate is very low. Recommended minimum is 30 seconds.");
            }
            
            if (interstitialCooldown < 60f)
            {
                AdLogger.LogWarning("Interstitial cooldown is very low. This may impact user experience.");
            }
            
            if (defaultRewardAmount <= 0)
            {
                AdLogger.LogError("Default reward amount must be greater than 0.");
                isValid = false;
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Creates a default configuration with sensible values.
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            appId = "YOUR_APP_ID";
            testMode = true;
            enableLogging = true;
            
            enableBannerAds = true;
            enableInterstitialAds = true;
            enableRewardedVideoAds = true;
            enablePlayableAds = true;
            enableNativeAds = true;
            
            defaultBannerPosition = BannerPosition.Bottom;
            defaultBannerSize = BannerSize.Standard;
            bannerRefreshRate = 60f;
            
            interstitialCooldown = 300f;
            interstitialFrequencyCap = 3;
            
            defaultRewardType = "Coins";
            defaultRewardAmount = 100;
            rewardedVideoCooldown = 180f;
            
            defaultNativeTemplate = NativeAdTemplate.Medium;
            
            showDebugOverlay = true;
            simulateSlowNetwork = false;
            networkDelayMultiplier = 1f;
            
            AdLogger.Log("Ad Configuration reset to defaults");
        }
    }
}