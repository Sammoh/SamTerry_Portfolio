using System;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Core advertisement service interface defining the base contract for all ad services.
    /// This provides a foundation for implementing different types of advertisements with consistent behavior.
    /// </summary>
    public interface IAdService
    {
        /// <summary>
        /// Gets whether this ad service is currently initialized and ready to use.
        /// </summary>
        bool IsInitialized { get; }
        
        /// <summary>
        /// Gets whether this ad service is currently loading an advertisement.
        /// </summary>
        bool IsLoading { get; }
        
        /// <summary>
        /// Gets the last error message if an ad operation failed.
        /// </summary>
        string LastError { get; }
        
        /// <summary>
        /// Event fired when an ad has been successfully loaded and is ready to display.
        /// </summary>
        event Action<AdEventArgs> OnAdLoaded;
        
        /// <summary>
        /// Event fired when an ad fails to load.
        /// </summary>
        event Action<AdErrorEventArgs> OnAdLoadFailed;
        
        /// <summary>
        /// Event fired when an ad is displayed to the user.
        /// </summary>
        event Action<AdEventArgs> OnAdDisplayed;
        
        /// <summary>
        /// Event fired when an ad is dismissed or closed by the user.
        /// </summary>
        event Action<AdEventArgs> OnAdDismissed;
        
        /// <summary>
        /// Event fired when the user clicks on an advertisement.
        /// </summary>
        event Action<AdEventArgs> OnAdClicked;
        
        /// <summary>
        /// Initializes the ad service with the specified configuration.
        /// </summary>
        /// <param name="appId">The app ID for the ad network</param>
        /// <param name="testMode">Whether to enable test mode for development</param>
        void Initialize(string appId, bool testMode = false);
        
        /// <summary>
        /// Loads an advertisement asynchronously.
        /// </summary>
        void LoadAd();
        
        /// <summary>
        /// Gets whether an ad is currently available and ready to display.
        /// </summary>
        /// <returns>True if an ad is ready, false otherwise</returns>
        bool IsAdReady();
        
        /// <summary>
        /// Releases resources and cleans up the ad service.
        /// </summary>
        void Destroy();
    }
    
    /// <summary>
    /// Banner-specific advertisement service interface extending the base ad service.
    /// Handles banner ads that can be positioned at different locations on screen.
    /// </summary>
    public interface IBannerAdService : IAdService
    {
        /// <summary>
        /// Gets whether a banner ad is currently visible on screen.
        /// </summary>
        bool IsBannerVisible { get; }
        
        /// <summary>
        /// Gets the current banner position.
        /// </summary>
        BannerPosition CurrentPosition { get; }
        
        /// <summary>
        /// Shows a banner ad at the specified position.
        /// </summary>
        /// <param name="position">The position where the banner should be displayed</param>
        void ShowBanner(BannerPosition position = BannerPosition.Bottom);
        
        /// <summary>
        /// Hides the currently displayed banner ad.
        /// </summary>
        void HideBanner();
        
        /// <summary>
        /// Sets the banner size for display.
        /// </summary>
        /// <param name="size">The desired banner size</param>
        void SetBannerSize(BannerSize size);
    }
    
    /// <summary>
    /// Interstitial advertisement service interface for full-screen ads.
    /// These ads are typically shown at natural break points in the app flow.
    /// </summary>
    public interface IInterstitialAdService : IAdService
    {
        /// <summary>
        /// Shows an interstitial ad with an optional completion callback.
        /// </summary>
        /// <param name="onComplete">Callback invoked when the ad is dismissed</param>
        void ShowInterstitial(Action<bool> onComplete = null);
    }
    
    /// <summary>
    /// Rewarded video advertisement service interface.
    /// These ads provide rewards to users upon successful completion.
    /// </summary>
    public interface IRewardedVideoAdService : IAdService
    {
        /// <summary>
        /// Event fired when the user completes watching a rewarded video and earns a reward.
        /// </summary>
        event Action<AdRewardEventArgs> OnRewardEarned;
        
        /// <summary>
        /// Shows a rewarded video ad with an optional reward callback.
        /// </summary>
        /// <param name="onReward">Callback invoked when the user earns a reward</param>
        void ShowRewardedVideo(Action<AdRewardEventArgs> onReward = null);
    }
    
    /// <summary>
    /// Playable advertisement service interface for interactive ads.
    /// These ads allow users to interact with a mini-game or demo.
    /// </summary>
    public interface IPlayableAdService : IAdService
    {
        /// <summary>
        /// Event fired when the user completes the playable ad interaction.
        /// </summary>
        event Action<AdEventArgs> OnPlayableCompleted;
        
        /// <summary>
        /// Shows a playable ad with an optional completion callback.
        /// </summary>
        /// <param name="onComplete">Callback invoked when the playable is completed</param>
        void ShowPlayable(Action<bool> onComplete = null);
    }
    
    /// <summary>
    /// Native advertisement service interface for custom integrated ads.
    /// These ads are designed to match the app's visual design and user experience.
    /// </summary>
    public interface INativeAdService : IAdService
    {
        /// <summary>
        /// Loads a native ad with the specified template type.
        /// </summary>
        /// <param name="templateType">The template type for the native ad</param>
        void LoadNativeAd(NativeAdTemplate templateType);
        
        /// <summary>
        /// Shows the loaded native ad in the specified container.
        /// </summary>
        /// <param name="container">The UI container for the native ad</param>
        void ShowNativeAd(Transform container);
        
        /// <summary>
        /// Removes the currently displayed native ad.
        /// </summary>
        void RemoveNativeAd();
    }
    
    /// <summary>
    /// Enumeration of available banner positions on screen.
    /// </summary>
    public enum BannerPosition
    {
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }
    
    /// <summary>
    /// Enumeration of available banner sizes.
    /// </summary>
    public enum BannerSize
    {
        Standard,    // 320x50
        Large,       // 320x100
        Rectangle,   // 300x250
        Smart        // Device-specific adaptive size
    }
    
    /// <summary>
    /// Enumeration of native ad template types.
    /// </summary>
    public enum NativeAdTemplate
    {
        Small,       // Compact layout
        Medium,      // Standard layout
        Large        // Expanded layout with image
    }
    
    /// <summary>
    /// Base event arguments for advertisement events.
    /// </summary>
    public class AdEventArgs : EventArgs
    {
        public string PlacementId { get; set; }
        public string AdType { get; set; }
        public DateTime Timestamp { get; set; }
        
        public AdEventArgs(string placementId, string adType)
        {
            PlacementId = placementId;
            AdType = adType;
            Timestamp = DateTime.UtcNow;
        }
    }
    
    /// <summary>
    /// Event arguments for advertisement errors.
    /// </summary>
    public class AdErrorEventArgs : AdEventArgs
    {
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        
        public AdErrorEventArgs(string placementId, string adType, string errorMessage, int errorCode = 0) 
            : base(placementId, adType)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
    
    /// <summary>
    /// Event arguments for rewarded advertisement completion.
    /// </summary>
    public class AdRewardEventArgs : AdEventArgs
    {
        public bool IsValid { get; set; }
        public string RewardType { get; set; }
        public int Amount { get; set; }
        
        public AdRewardEventArgs(string placementId, string rewardType, int amount, bool isValid = true) 
            : base(placementId, "RewardedVideo")
        {
            RewardType = rewardType;
            Amount = amount;
            IsValid = isValid;
        }
    }
}