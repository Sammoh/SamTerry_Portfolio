using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Sammoh.Advertisement.Tests
{
    /// <summary>
    /// Test suite for the Advertisement System core functionality.
    /// Validates that all ad services can be initialized and basic operations work correctly.
    /// </summary>
    public class AdServiceTests
    {
        private AdServiceManager _adManager;
        private GameObject _testGameObject;
        
        [SetUp]
        public void SetUp()
        {
            // Create a test GameObject with AdServiceManager
            _testGameObject = new GameObject("TestAdServiceManager");
            _adManager = _testGameObject.AddComponent<AdServiceManager>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_testGameObject != null)
            {
                Object.DestroyImmediate(_testGameObject);
            }
        }
        
        [Test]
        public void AdServiceManager_Initialize_SetsInitializedFlag()
        {
            // Arrange & Act
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Assert
            Assert.IsTrue(_adManager.IsInitialized);
        }
        
        [Test]
        public void AdServiceManager_GetServicesStatus_ReturnsServiceStates()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var status = _adManager.GetServicesStatus();
            
            // Assert
            Assert.IsNotNull(status);
            Assert.Greater(status.Count, 0);
            Assert.IsTrue(status.ContainsKey("IBannerAdService"));
            Assert.IsTrue(status.ContainsKey("IInterstitialAdService"));
            Assert.IsTrue(status.ContainsKey("IRewardedVideoAdService"));
        }
        
        [Test]
        public void BannerService_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var bannerService = _adManager.BannerService;
            
            // Assert
            Assert.IsNotNull(bannerService);
            Assert.IsTrue(bannerService.IsInitialized);
        }
        
        [Test]
        public void InterstitialService_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var interstitialService = _adManager.InterstitialService;
            
            // Assert
            Assert.IsNotNull(interstitialService);
            Assert.IsTrue(interstitialService.IsInitialized);
        }
        
        [Test]
        public void RewardedVideoService_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var rewardedVideoService = _adManager.RewardedVideoService;
            
            // Assert
            Assert.IsNotNull(rewardedVideoService);
            Assert.IsTrue(rewardedVideoService.IsInitialized);
        }
        
        [Test]
        public void PlayableService_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var playableService = _adManager.PlayableService;
            
            // Assert
            Assert.IsNotNull(playableService);
            Assert.IsTrue(playableService.IsInitialized);
        }
        
        [Test]
        public void NativeService_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var nativeService = _adManager.NativeService;
            
            // Assert
            Assert.IsNotNull(nativeService);
            Assert.IsTrue(nativeService.IsInitialized);
        }
        
        [Test]
        public void EventDispatcher_IsAvailable_AfterInitialization()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            
            // Act
            var eventDispatcher = _adManager.EventDispatcher;
            
            // Assert
            Assert.IsNotNull(eventDispatcher);
        }
        
        [UnityTest]
        public IEnumerator BannerService_LoadAd_FiresEvents()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            yield return new WaitForSeconds(2f); // Wait for initialization
            
            bool adLoadedFired = false;
            var bannerService = _adManager.BannerService;
            bannerService.OnAdLoaded += (args) => adLoadedFired = true;
            
            // Act
            bannerService.LoadAd();
            yield return new WaitForSeconds(3f); // Wait for simulated load
            
            // Assert
            Assert.IsTrue(adLoadedFired);
        }
        
        [UnityTest]
        public IEnumerator InterstitialService_LoadAd_FiresEvents()
        {
            // Arrange
            _adManager.Initialize("TEST_APP_ID", true);
            yield return new WaitForSeconds(2f); // Wait for initialization
            
            bool adLoadedFired = false;
            var interstitialService = _adManager.InterstitialService;
            interstitialService.OnAdLoaded += (args) => adLoadedFired = true;
            
            // Act
            interstitialService.LoadAd();
            yield return new WaitForSeconds(4f); // Wait for simulated load
            
            // Assert
            Assert.IsTrue(adLoadedFired);
        }
        
        [Test]
        public void AdEventArgs_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var eventArgs = new AdEventArgs("TestPlacement", "Banner");
            
            // Assert
            Assert.AreEqual("TestPlacement", eventArgs.PlacementId);
            Assert.AreEqual("Banner", eventArgs.AdType);
            Assert.IsTrue((System.DateTime.UtcNow - eventArgs.Timestamp).TotalSeconds < 1);
        }
        
        [Test]
        public void AdErrorEventArgs_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var errorArgs = new AdErrorEventArgs("TestPlacement", "Banner", "Test error", 1001);
            
            // Assert
            Assert.AreEqual("TestPlacement", errorArgs.PlacementId);
            Assert.AreEqual("Banner", errorArgs.AdType);
            Assert.AreEqual("Test error", errorArgs.ErrorMessage);
            Assert.AreEqual(1001, errorArgs.ErrorCode);
        }
        
        [Test]
        public void AdRewardEventArgs_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var rewardArgs = new AdRewardEventArgs("TestPlacement", "Coins", 100, true);
            
            // Assert
            Assert.AreEqual("TestPlacement", rewardArgs.PlacementId);
            Assert.AreEqual("RewardedVideo", rewardArgs.AdType);
            Assert.AreEqual("Coins", rewardArgs.RewardType);
            Assert.AreEqual(100, rewardArgs.Amount);
            Assert.IsTrue(rewardArgs.IsValid);
        }
        
        [Test]
        public void AdLogger_EnableLogging_ControlsOutput()
        {
            // Arrange
            AdLogger.EnableLogging = false;
            
            // Act & Assert (should not throw or output)
            AdLogger.Log("Test message");
            AdLogger.LogWarning("Test warning");
            AdLogger.LogError("Test error");
            
            // Reset
            AdLogger.EnableLogging = true;
        }
        
        [Test]
        public void BannerPositions_EnumValues_AreCorrect()
        {
            // Assert enum values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerPosition), BannerPosition.Top));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerPosition), BannerPosition.Bottom));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerPosition), BannerPosition.Center));
        }
        
        [Test]
        public void BannerSizes_EnumValues_AreCorrect()
        {
            // Assert enum values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerSize), BannerSize.Standard));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerSize), BannerSize.Large));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerSize), BannerSize.Rectangle));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BannerSize), BannerSize.Smart));
        }
        
        [Test]
        public void NativeAdTemplates_EnumValues_AreCorrect()
        {
            // Assert enum values exist
            Assert.IsTrue(System.Enum.IsDefined(typeof(NativeAdTemplate), NativeAdTemplate.Small));
            Assert.IsTrue(System.Enum.IsDefined(typeof(NativeAdTemplate), NativeAdTemplate.Medium));
            Assert.IsTrue(System.Enum.IsDefined(typeof(NativeAdTemplate), NativeAdTemplate.Large));
        }
    }
}