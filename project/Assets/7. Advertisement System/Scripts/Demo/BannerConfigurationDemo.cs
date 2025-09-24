using UnityEngine;

namespace Sammoh.Advertisement.Demo
{
    /// <summary>
    /// Demonstration script showing how to use the new ScriptableObject-based
    /// banner configuration system. This script can be added to a GameObject
    /// in the AdDemo scene to showcase the improvements.
    /// </summary>
    public class BannerConfigurationDemo : MonoBehaviour
    {
        [Header("Configuration Assets")]
        [SerializeField] 
        [Tooltip("Different banner configurations to demonstrate")]
        private BannerContainerConfiguration[] bannerConfigurations;
        
        [Header("Demo Controls")]
        [SerializeField]
        [Tooltip("Automatically cycle through configurations")]
        private bool autoCycle = true;
        
        [SerializeField]
        [Tooltip("Time between configuration changes (seconds)")]
        private float cycleInterval = 5f;
        
        private int currentConfigIndex = 0;
        private float lastCycleTime = 0f;
        private IBannerAdService bannerService;
        
        void Start()
        {
            // Get the banner service
            bannerService = AdServiceManager.Instance?.BannerService;
            
            if (bannerService == null)
            {
                AdLogger.LogError("BannerConfigurationDemo: AdServiceManager or BannerService not found. " +
                                 "Make sure AdServiceManager is initialized in the scene.");
                enabled = false;
                return;
            }
            
            // Validate configurations
            ValidateConfigurations();
            
            // Apply first configuration if available
            if (bannerConfigurations != null && bannerConfigurations.Length > 0)
            {
                ApplyConfiguration(0);
            }
            
            lastCycleTime = Time.time;
        }
        
        void Update()
        {
            if (autoCycle && bannerConfigurations != null && bannerConfigurations.Length > 1)
            {
                if (Time.time - lastCycleTime >= cycleInterval)
                {
                    CycleToNextConfiguration();
                    lastCycleTime = Time.time;
                }
            }
        }
        
        /// <summary>
        /// Cycles to the next banner configuration in the array.
        /// </summary>
        public void CycleToNextConfiguration()
        {
            if (bannerConfigurations == null || bannerConfigurations.Length == 0)
                return;
                
            currentConfigIndex = (currentConfigIndex + 1) % bannerConfigurations.Length;
            ApplyConfiguration(currentConfigIndex);
        }
        
        /// <summary>
        /// Applies a specific configuration by index.
        /// </summary>
        /// <param name="index">Index of the configuration to apply</param>
        public void ApplyConfiguration(int index)
        {
            if (bannerConfigurations == null || index < 0 || index >= bannerConfigurations.Length)
            {
                AdLogger.LogWarning($"BannerConfigurationDemo: Invalid configuration index {index}");
                return;
            }
            
            var config = bannerConfigurations[index];
            if (config == null)
            {
                AdLogger.LogWarning($"BannerConfigurationDemo: Configuration at index {index} is null");
                return;
            }
            
            // Apply the configuration to the banner service
            bannerService.SetBannerContainerConfiguration(config);
            
            // Show the banner with the new configuration
            bannerService.ShowBanner(BannerPosition.Bottom);
            
            AdLogger.Log($"BannerConfigurationDemo: Applied configuration '{config.name}' (index {index})");
        }
        
        /// <summary>
        /// Demonstrates different banner positions with current configuration.
        /// </summary>
        public void DemonstrateBannerPositions()
        {
            if (bannerService == null) return;
            
            StartCoroutine(PositionDemonstrationCoroutine());
        }
        
        private System.Collections.IEnumerator PositionDemonstrationCoroutine()
        {
            BannerPosition[] positions = { BannerPosition.Top, BannerPosition.Center, BannerPosition.Bottom };
            
            foreach (var position in positions)
            {
                AdLogger.Log($"BannerConfigurationDemo: Showing banner at {position}");
                bannerService.ShowBanner(position);
                
                yield return new WaitForSeconds(2f);
            }
            
            // Return to bottom position
            bannerService.ShowBanner(BannerPosition.Bottom);
        }
        
        /// <summary>
        /// Demonstrates different banner sizes with current configuration.
        /// </summary>
        public void DemonstrateBannerSizes()
        {
            if (bannerService == null) return;
            
            StartCoroutine(SizeDemonstrationCoroutine());
        }
        
        private System.Collections.IEnumerator SizeDemonstrationCoroutine()
        {
            BannerSize[] sizes = { BannerSize.Standard, BannerSize.Large, BannerSize.Rectangle, BannerSize.Smart };
            
            foreach (var size in sizes)
            {
                AdLogger.Log($"BannerConfigurationDemo: Showing banner size {size}");
                bannerService.SetBannerSize(size);
                bannerService.ShowBanner(BannerPosition.Bottom);
                
                yield return new WaitForSeconds(3f);
            }
            
            // Return to standard size
            bannerService.SetBannerSize(BannerSize.Standard);
            bannerService.ShowBanner(BannerPosition.Bottom);
        }
        
        /// <summary>
        /// Validates all assigned configurations and logs any issues.
        /// </summary>
        private void ValidateConfigurations()
        {
            if (bannerConfigurations == null || bannerConfigurations.Length == 0)
            {
                AdLogger.LogWarning("BannerConfigurationDemo: No banner configurations assigned. " +
                                   "Assign some BannerContainerConfiguration assets to see the demo in action.");
                return;
            }
            
            for (int i = 0; i < bannerConfigurations.Length; i++)
            {
                var config = bannerConfigurations[i];
                if (config == null)
                {
                    AdLogger.LogWarning($"BannerConfigurationDemo: Configuration at index {i} is null");
                    continue;
                }
                
                if (!config.ValidateConfiguration())
                {
                    AdLogger.LogWarning($"BannerConfigurationDemo: Configuration '{config.name}' has validation issues");
                }
            }
        }
        
        /// <summary>
        /// Creates a demo configuration at runtime for testing purposes.
        /// </summary>
        [ContextMenu("Create Test Configuration")]
        public void CreateTestConfiguration()
        {
            var testConfig = ScriptableObject.CreateInstance<BannerContainerConfiguration>();
            testConfig.name = "Runtime Test Configuration";
            testConfig.ResetToDefaults();
            
            // Customize for demonstration
            var backgroundColorField = typeof(BannerContainerConfiguration).GetField("backgroundColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var textColorField = typeof(BannerContainerConfiguration).GetField("textColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            backgroundColorField?.SetValue(testConfig, Color.green);
            textColorField?.SetValue(testConfig, Color.black);
            
            // Apply the test configuration
            bannerService?.SetBannerContainerConfiguration(testConfig);
            bannerService?.ShowBanner(BannerPosition.Bottom);
            
            AdLogger.Log("BannerConfigurationDemo: Created and applied runtime test configuration");
        }
        
        void OnGUI()
        {
            if (bannerConfigurations == null || bannerConfigurations.Length == 0)
                return;
                
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Banner Configuration Demo", GUI.skin.label);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Cycle Configuration"))
            {
                CycleToNextConfiguration();
            }
            
            if (GUILayout.Button("Demo Positions"))
            {
                DemonstrateBannerPositions();
            }
            
            if (GUILayout.Button("Demo Sizes"))
            {
                DemonstrateBannerSizes();
            }
            
            GUILayout.Space(10);
            GUILayout.Label($"Current Config: {currentConfigIndex + 1}/{bannerConfigurations.Length}");
            if (bannerConfigurations[currentConfigIndex] != null)
            {
                GUILayout.Label($"Name: {bannerConfigurations[currentConfigIndex].name}");
            }
            
            autoCycle = GUILayout.Toggle(autoCycle, "Auto Cycle");
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}