using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Central manager for all advertisement services implementing the Singleton pattern.
    /// Provides a unified interface for managing different types of ads and coordinates
    /// between various ad services while maintaining the service pattern architecture.
    /// </summary>
    public class AdServiceManager : MonoBehaviour
    {
        #region Singleton Implementation
        
        private static AdServiceManager _instance;
        
        /// <summary>
        /// Gets the singleton instance of the AdServiceManager.
        /// </summary>
        public static AdServiceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AdServiceManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AdServiceManager");
                        _instance = go.AddComponent<AdServiceManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region Inspector Fields
        
        [Header("Ad Configuration")]
        [SerializeField] private string appId = "YOUR_APP_ID";
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool autoInitialize = true;
        
        [Header("Service Configuration")]
        [SerializeField] private bool enableBannerAds = true;
        [SerializeField] private bool enableInterstitialAds = true;
        [SerializeField] private bool enableRewardedVideoAds = true;
        [SerializeField] private bool enablePlayableAds = true;
        [SerializeField] private bool enableNativeAds = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        #endregion
        
        #region Private Fields
        
        private Dictionary<Type, IAdService> _services = new Dictionary<Type, IAdService>();
        private bool _isInitialized = false;
        private AdEventDispatcher _eventDispatcher;
        
        #endregion
        
        #region Public Properties
        
        /// <summary>
        /// Gets whether the Ad Service Manager is initialized.
        /// </summary>
        public bool IsInitialized => _isInitialized;
        
        /// <summary>
        /// Gets the banner advertisement service.
        /// </summary>
        public IBannerAdService BannerService => GetService<IBannerAdService>();
        
        /// <summary>
        /// Gets the interstitial advertisement service.
        /// </summary>
        public IInterstitialAdService InterstitialService => GetService<IInterstitialAdService>();
        
        /// <summary>
        /// Gets the rewarded video advertisement service.
        /// </summary>
        public IRewardedVideoAdService RewardedVideoService => GetService<IRewardedVideoAdService>();
        
        /// <summary>
        /// Gets the playable advertisement service.
        /// </summary>
        public IPlayableAdService PlayableService => GetService<IPlayableAdService>();
        
        /// <summary>
        /// Gets the native advertisement service.
        /// </summary>
        public INativeAdService NativeService => GetService<INativeAdService>();
        
        /// <summary>
        /// Gets the event dispatcher for ad-related events.
        /// </summary>
        public AdEventDispatcher EventDispatcher => _eventDispatcher;
        
        /// <summary>
        /// Event fired when the Ad Service Manager is successfully initialized.
        /// </summary>
        public event Action OnInitialized;
        
        /// <summary>
        /// Event fired when Ad Service Manager initialization fails.
        /// </summary>
        public event Action<string> OnInitializationFailed;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Ensure singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize event dispatcher
            _eventDispatcher = gameObject.GetComponent<AdEventDispatcher>() 
                ?? gameObject.AddComponent<AdEventDispatcher>();
            
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        private void Start()
        {
            if (autoInitialize && !_isInitialized)
            {
                Initialize();
            }
        }
        
        private void OnDestroy()
        {
            if (_instance == this)
            {
                // Clean up all services
                foreach (var service in _services.Values)
                {
                    service?.Destroy();
                }
                _services.Clear();
                _isInitialized = false;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Initializes the Ad Service Manager and all enabled ad services.
        /// </summary>
        /// <param name="customAppId">Optional custom app ID to override the inspector setting</param>
        /// <param name="customTestMode">Optional custom test mode to override the inspector setting</param>
        public void Initialize(string customAppId = null, bool? customTestMode = null)
        {
            if (_isInitialized)
            {
                AdLogger.LogWarning("AdServiceManager is already initialized.");
                return;
            }
            
            try
            {
                string actualAppId = customAppId ?? appId;
                bool actualTestMode = customTestMode ?? testMode;
                
                AdLogger.Log($"Initializing AdServiceManager with App ID: {actualAppId}, Test Mode: {actualTestMode}");
                
                // Validate app ID
                if (string.IsNullOrEmpty(actualAppId) || actualAppId == "YOUR_APP_ID")
                {
                    string error = "Invalid App ID. Please set a valid LevelPlay App ID.";
                    AdLogger.LogError(error);
                    OnInitializationFailed?.Invoke(error);
                    return;
                }
                
                // Initialize services based on configuration
                InitializeServices(actualAppId, actualTestMode);
                
                _isInitialized = true;
                AdLogger.Log("AdServiceManager initialized successfully.");
                OnInitialized?.Invoke();
            }
            catch (Exception ex)
            {
                string error = $"Failed to initialize AdServiceManager: {ex.Message}";
                AdLogger.LogError(error);
                OnInitializationFailed?.Invoke(error);
            }
        }
        
        /// <summary>
        /// Registers a custom ad service with the manager.
        /// </summary>
        /// <typeparam name="T">The service interface type</typeparam>
        /// <param name="service">The service implementation</param>
        public void RegisterService<T>(T service) where T : class, IAdService
        {
            Type serviceType = typeof(T);
            
            if (_services.ContainsKey(serviceType))
            {
                AdLogger.LogWarning($"Service {serviceType.Name} is already registered. Replacing existing service.");
                _services[serviceType]?.Destroy();
            }
            
            _services[serviceType] = service;
            AdLogger.Log($"Registered service: {serviceType.Name}");
            
            // Initialize the service if the manager is already initialized
            if (_isInitialized)
            {
                service.Initialize(appId, testMode);
            }
        }
        
        /// <summary>
        /// Gets a registered ad service by its interface type.
        /// </summary>
        /// <typeparam name="T">The service interface type</typeparam>
        /// <returns>The service implementation or null if not registered</returns>
        public T GetService<T>() where T : class, IAdService
        {
            Type serviceType = typeof(T);
            _services.TryGetValue(serviceType, out IAdService service);
            return service as T;
        }
        
        /// <summary>
        /// Unregisters an ad service from the manager.
        /// </summary>
        /// <typeparam name="T">The service interface type</typeparam>
        public void UnregisterService<T>() where T : class, IAdService
        {
            Type serviceType = typeof(T);
            
            if (_services.TryGetValue(serviceType, out IAdService service))
            {
                service?.Destroy();
                _services.Remove(serviceType);
                AdLogger.Log($"Unregistered service: {serviceType.Name}");
            }
        }
        
        /// <summary>
        /// Gets the status of all registered ad services.
        /// </summary>
        /// <returns>Dictionary of service types and their initialization status</returns>
        public Dictionary<string, bool> GetServicesStatus()
        {
            var status = new Dictionary<string, bool>();
            foreach (var kvp in _services)
            {
                status[kvp.Key.Name] = kvp.Value?.IsInitialized ?? false;
            }
            return status;
        }
        
        /// <summary>
        /// Enables or disables logging for advertisement services.
        /// </summary>
        /// <param name="enabled">Whether to enable logging</param>
        public void SetLoggingEnabled(bool enabled)
        {
            enableLogging = enabled;
            AdLogger.EnableLogging = enabled;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Initializes all enabled ad services based on configuration.
        /// </summary>
        /// <param name="actualAppId">The app ID to use for initialization</param>
        /// <param name="actualTestMode">Whether to enable test mode</param>
        private void InitializeServices(string actualAppId, bool actualTestMode)
        {
            // Note: In a real implementation, these would be the actual LevelPlay service implementations
            // For this demo, we'll create mock services that simulate the behavior
            
            if (enableBannerAds)
            {
                var bannerService = new BannerAdService();
                RegisterService<IBannerAdService>(bannerService);
            }
            
            if (enableInterstitialAds)
            {
                var interstitialService = new InterstitialAdService();
                RegisterService<IInterstitialAdService>(interstitialService);
            }
            
            if (enableRewardedVideoAds)
            {
                var rewardedVideoService = new RewardedVideoAdService();
                RegisterService<IRewardedVideoAdService>(rewardedVideoService);
            }
            
            if (enablePlayableAds)
            {
                var playableService = new PlayableAdService();
                RegisterService<IPlayableAdService>(playableService);
            }
            
            if (enableNativeAds)
            {
                var nativeService = new NativeAdService();
                RegisterService<INativeAdService>(nativeService);
            }
            
            // Initialize all registered services
            foreach (var service in _services.Values)
            {
                service.Initialize(actualAppId, actualTestMode);
            }
        }
        
        #endregion
        
        #region Debug and Development
        
        /// <summary>
        /// Logs debug information about the current state of all services.
        /// </summary>
        [ContextMenu("Log Services Status")]
        public void LogServicesStatus()
        {
            if (!showDebugInfo) return;
            
            AdLogger.Log("=== Ad Services Status ===");
            AdLogger.Log($"Manager Initialized: {_isInitialized}");
            AdLogger.Log($"App ID: {appId}");
            AdLogger.Log($"Test Mode: {testMode}");
            
            foreach (var kvp in _services)
            {
                var service = kvp.Value;
                AdLogger.Log($"{kvp.Key.Name}: Initialized={service?.IsInitialized}, Loading={service?.IsLoading}, Ready={service?.IsAdReady()}");
            }
        }
        
        #endregion
    }
}