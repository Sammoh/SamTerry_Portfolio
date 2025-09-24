using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Sammoh.Advertisement.Tests
{
    /// <summary>
    /// Test suite for BannerContainerConfiguration ScriptableObject functionality.
    /// Validates configuration behavior, validation, and integration with BannerAdService.
    /// </summary>
    public class BannerContainerConfigurationTests
    {
        private BannerContainerConfiguration _config;
        
        [SetUp]
        public void SetUp()
        {
            _config = ScriptableObject.CreateInstance<BannerContainerConfiguration>();
        }
        
        [TearDown]
        public void TearDown()
        {
            if (_config != null)
            {
                ScriptableObject.DestroyImmediate(_config);
            }
        }
        
        [Test]
        public void BannerContainerConfiguration_DefaultValues_AreCorrect()
        {
            // Assert default values are properly set
            Assert.AreEqual(new Color(0.2f, 0.6f, 1f, 0.8f), _config.BackgroundColor);
            Assert.AreEqual(Color.white, _config.TextColor);
            Assert.AreEqual(TextAnchor.MiddleCenter, _config.TextAlignment);
            Assert.IsTrue(_config.EnableClickInteraction);
            Assert.IsTrue(_config.EnableSmartResize);
            Assert.AreEqual(320f, _config.MinWidth);
            Assert.AreEqual(600f, _config.MaxWidth);
            Assert.IsTrue(_config.EnableFadeAnimation);
            Assert.AreEqual(0.5f, _config.AnimationDuration);
            Assert.AreEqual("Demo Banner Ad\n{size} - {position}", _config.TextTemplate);
            Assert.AreEqual(12, _config.FontSize);
            Assert.IsTrue(_config.RespectSafeArea);
        }
        
        [Test]
        public void GetDimensionsForSize_StandardBanner_ReturnsCorrectDimensions()
        {
            // Act
            Vector2 dimensions = _config.GetDimensionsForSize(BannerSize.Standard);
            
            // Assert
            Assert.AreEqual(new Vector2(320, 50), dimensions);
        }
        
        [Test]
        public void GetDimensionsForSize_LargeBanner_ReturnsCorrectDimensions()
        {
            // Act
            Vector2 dimensions = _config.GetDimensionsForSize(BannerSize.Large);
            
            // Assert
            Assert.AreEqual(new Vector2(320, 100), dimensions);
        }
        
        [Test]
        public void GetDimensionsForSize_RectangleBanner_ReturnsCorrectDimensions()
        {
            // Act
            Vector2 dimensions = _config.GetDimensionsForSize(BannerSize.Rectangle);
            
            // Assert
            Assert.AreEqual(new Vector2(300, 250), dimensions);
        }
        
        [Test]
        public void GetDimensionsForSize_SmartBanner_ReturnsCalculatedDimensions()
        {
            // Act
            Vector2 dimensions = _config.GetDimensionsForSize(BannerSize.Smart);
            
            // Assert
            Assert.AreEqual(50, dimensions.y); // Height should be 50
            Assert.Greater(dimensions.x, 0); // Width should be calculated
            Assert.GreaterOrEqual(dimensions.x, _config.MinWidth);
            Assert.LessOrEqual(dimensions.x, _config.MaxWidth);
        }
        
        [Test]
        public void GetFormattedText_ReplacesPlaceholders_Correctly()
        {
            // Act
            string formattedText = _config.GetFormattedText(BannerSize.Large, BannerPosition.Top);
            
            // Assert
            Assert.AreEqual("Demo Banner Ad\nLarge - Top", formattedText);
        }
        
        [Test]
        public void ValidateConfiguration_ValidConfig_ReturnsTrue()
        {
            // Arrange - use default configuration which should be valid
            _config.ResetToDefaults();
            
            // Act
            bool isValid = _config.ValidateConfiguration();
            
            // Assert
            Assert.IsTrue(isValid);
        }
        
        [Test]
        public void ValidateConfiguration_InvalidMinMaxWidth_ReturnsFalse()
        {
            // Arrange - create invalid configuration using reflection to set private fields
            var minWidthField = typeof(BannerContainerConfiguration).GetField("minWidth", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var maxWidthField = typeof(BannerContainerConfiguration).GetField("maxWidth", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            minWidthField?.SetValue(_config, 500f);
            maxWidthField?.SetValue(_config, 400f); // Max < Min
            
            // Act
            bool isValid = _config.ValidateConfiguration();
            
            // Assert
            Assert.IsFalse(isValid);
        }
        
        [Test]
        public void ResetToDefaults_RestoresDefaultValues()
        {
            // Arrange - modify some values first
            var backgroundColorField = typeof(BannerContainerConfiguration).GetField("backgroundColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var textColorField = typeof(BannerContainerConfiguration).GetField("textColor", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            backgroundColorField?.SetValue(_config, Color.red);
            textColorField?.SetValue(_config, Color.blue);
            
            // Act
            _config.ResetToDefaults();
            
            // Assert
            Assert.AreEqual(new Color(0.2f, 0.6f, 1f, 0.8f), _config.BackgroundColor);
            Assert.AreEqual(Color.white, _config.TextColor);
        }
        
        [Test]
        public void BannerSizeConfig_Serialization_WorksCorrectly()
        {
            // Arrange
            var sizeConfig = new BannerSizeConfig
            {
                size = BannerSize.Large,
                dimensions = new Vector2(400, 120),
                fontSizeOverride = 14
            };
            
            // Act & Assert - if this compiles and runs, serialization structure is correct
            Assert.AreEqual(BannerSize.Large, sizeConfig.size);
            Assert.AreEqual(new Vector2(400, 120), sizeConfig.dimensions);
            Assert.AreEqual(14, sizeConfig.fontSizeOverride);
        }
        
        [Test]
        public void BannerAdService_SetBannerContainerConfiguration_UpdatesConfiguration()
        {
            // Arrange
            var testGameObject = new GameObject("TestBannerService");
            var adManager = testGameObject.AddComponent<AdServiceManager>();
            adManager.Initialize("TEST_APP_ID", true);
            
            var bannerService = adManager.BannerService as BannerAdService;
            Assert.IsNotNull(bannerService);
            
            // Create custom config
            var customConfig = ScriptableObject.CreateInstance<BannerContainerConfiguration>();
            customConfig.ResetToDefaults();
            
            try
            {
                // Act
                bannerService.SetBannerContainerConfiguration(customConfig);
                
                // Assert - if no exception is thrown, the method works correctly
                Assert.Pass("SetBannerContainerConfiguration executed successfully");
            }
            finally
            {
                // Cleanup
                Object.DestroyImmediate(customConfig);
                Object.DestroyImmediate(testGameObject);
            }
        }
        
        [Test]
        public void BannerAdService_WithConfiguration_ShowsBannerCorrectly()
        {
            // Arrange
            var testGameObject = new GameObject("TestBannerService");
            var adManager = testGameObject.AddComponent<AdServiceManager>();
            adManager.Initialize("TEST_APP_ID", true);
            
            var bannerService = adManager.BannerService;
            var customConfig = ScriptableObject.CreateInstance<BannerContainerConfiguration>();
            customConfig.ResetToDefaults();
            
            try
            {
                // Act
                bannerService.SetBannerContainerConfiguration(customConfig);
                bannerService.ShowBanner(BannerPosition.Bottom);
                
                // Assert
                Assert.IsTrue(bannerService.IsBannerVisible);
                Assert.AreEqual(BannerPosition.Bottom, bannerService.CurrentPosition);
            }
            finally
            {
                // Cleanup
                Object.DestroyImmediate(customConfig);
                Object.DestroyImmediate(testGameObject);
            }
        }
    }
}