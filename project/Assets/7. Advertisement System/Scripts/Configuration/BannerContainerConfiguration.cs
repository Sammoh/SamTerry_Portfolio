using UnityEngine;
using UnityEngine.UI;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// ScriptableObject configuration for banner ad container settings.
    /// Provides a user-friendly way to configure banner appearance, behavior and styling
    /// without modifying code, following the ScriptableObject pattern established in Global Data & Editor Tools.
    /// </summary>
    [CreateAssetMenu(fileName = "BannerContainerConfiguration", menuName = "Advertisement/Banner Container Configuration", order = 2)]
    public class BannerContainerConfiguration : ScriptableObject
    {
        [Header("Visual Appearance")]
        [SerializeField] 
        [Tooltip("Background color for the banner container")]
        private Color backgroundColor = new Color(0.2f, 0.6f, 1f, 0.8f);
        
        [SerializeField] 
        [Tooltip("Text color for the banner content")]
        private Color textColor = Color.white;
        
        [SerializeField]
        [Tooltip("Font to use for banner text. If null, uses Unity's default font")]
        private Font customFont;
        
        [SerializeField]
        [Tooltip("Text alignment within the banner")]
        private TextAnchor textAlignment = TextAnchor.MiddleCenter;
        
        [Header("Container Behavior")]
        [SerializeField]
        [Tooltip("Enable click interaction on the banner")]
        private bool enableClickInteraction = true;
        
        [SerializeField]
        [Tooltip("Should the banner automatically resize based on screen dimensions")]
        private bool enableSmartResize = true;
        
        [SerializeField]
        [Tooltip("Minimum width for smart resize (pixels)")]
        [Range(200f, 500f)]
        private float minWidth = 320f;
        
        [SerializeField]
        [Tooltip("Maximum width for smart resize (pixels)")]
        [Range(300f, 800f)]
        private float maxWidth = 600f;
        
        [Header("Animation Settings")]
        [SerializeField]
        [Tooltip("Enable fade-in animation when showing banner")]
        private bool enableFadeAnimation = true;
        
        [SerializeField]
        [Tooltip("Duration of fade-in/fade-out animation in seconds")]
        [Range(0.1f, 2f)]
        private float animationDuration = 0.5f;
        
        [Header("Size Overrides")]
        [SerializeField]
        [Tooltip("Custom size settings for each banner type")]
        private BannerSizeConfig[] bannerSizeConfigs = new BannerSizeConfig[]
        {
            new BannerSizeConfig { size = BannerSize.Standard, dimensions = new Vector2(320, 50) },
            new BannerSizeConfig { size = BannerSize.Large, dimensions = new Vector2(320, 100) },
            new BannerSizeConfig { size = BannerSize.Rectangle, dimensions = new Vector2(300, 250) },
            new BannerSizeConfig { size = BannerSize.Smart, dimensions = new Vector2(0, 50) } // Width will be calculated
        };
        
        [Header("Text Configuration")]
        [SerializeField]
        [Tooltip("Template for banner text. Use {size} and {position} as placeholders")]
        private string textTemplate = "Demo Banner Ad\n{size} - {position}";
        
        [SerializeField]
        [Tooltip("Font size for banner text")]
        [Range(8, 24)]
        private int fontSize = 12;
        
        [Header("Safe Area & Padding")]
        [SerializeField]
        [Tooltip("Respect device safe area (notches, home indicators)")]
        private bool respectSafeArea = true;
        
        [SerializeField]
        [Tooltip("Additional padding from screen edges")]
        private RectOffset padding = new RectOffset(10, 10, 10, 10);

        // Public Properties with runtime protection
        public Color BackgroundColor => backgroundColor;
        public Color TextColor => textColor;
        public Font CustomFont => customFont;
        public TextAnchor TextAlignment => textAlignment;
        public bool EnableClickInteraction => enableClickInteraction;
        public bool EnableSmartResize => enableSmartResize;
        public float MinWidth => minWidth;
        public float MaxWidth => maxWidth;
        public bool EnableFadeAnimation => enableFadeAnimation;
        public float AnimationDuration => animationDuration;
        public string TextTemplate => textTemplate;
        public int FontSize => fontSize;
        public bool RespectSafeArea => respectSafeArea;
        public RectOffset Padding => padding;
        
        /// <summary>
        /// Gets the configured dimensions for a specific banner size.
        /// </summary>
        /// <param name="bannerSize">The banner size to get dimensions for</param>
        /// <returns>Vector2 representing width and height</returns>
        public Vector2 GetDimensionsForSize(BannerSize bannerSize)
        {
            foreach (var config in bannerSizeConfigs)
            {
                if (config.size == bannerSize)
                {
                    if (bannerSize == BannerSize.Smart && enableSmartResize)
                    {
                        // Calculate smart width based on screen size
                        float smartWidth = Mathf.Clamp(Screen.width * 0.9f, minWidth, maxWidth);
                        return new Vector2(smartWidth, config.dimensions.y);
                    }
                    return config.dimensions;
                }
            }
            
            // Fallback to standard banner size
            return new Vector2(320, 50);
        }
        
        /// <summary>
        /// Formats the banner text using the configured template.
        /// </summary>
        /// <param name="bannerSize">Current banner size</param>
        /// <param name="position">Current banner position</param>
        /// <returns>Formatted text string</returns>
        public string GetFormattedText(BannerSize bannerSize, BannerPosition position)
        {
            return textTemplate
                .Replace("{size}", bannerSize.ToString())
                .Replace("{position}", position.ToString());
        }
        
        /// <summary>
        /// Validates the configuration and logs any issues.
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool ValidateConfiguration()
        {
            bool isValid = true;
            
            if (minWidth >= maxWidth)
            {
                AdLogger.LogError("Banner Container Configuration: Min width must be less than max width");
                isValid = false;
            }
            
            if (animationDuration <= 0)
            {
                AdLogger.LogWarning("Banner Container Configuration: Animation duration should be greater than 0");
            }
            
            if (fontSize < 8)
            {
                AdLogger.LogWarning("Banner Container Configuration: Font size is very small and may be hard to read");
            }
            
            if (bannerSizeConfigs == null || bannerSizeConfigs.Length == 0)
            {
                AdLogger.LogError("Banner Container Configuration: No banner size configurations defined");
                isValid = false;
            }
            
            return isValid;
        }
        
        /// <summary>
        /// Resets the configuration to default values.
        /// </summary>
        [ContextMenu("Reset to Defaults")]
        public void ResetToDefaults()
        {
            backgroundColor = new Color(0.2f, 0.6f, 1f, 0.8f);
            textColor = Color.white;
            customFont = null;
            textAlignment = TextAnchor.MiddleCenter;
            enableClickInteraction = true;
            enableSmartResize = true;
            minWidth = 320f;
            maxWidth = 600f;
            enableFadeAnimation = true;
            animationDuration = 0.5f;
            textTemplate = "Demo Banner Ad\n{size} - {position}";
            fontSize = 12;
            respectSafeArea = true;
            padding = new RectOffset(10, 10, 10, 10);
            
            // Reset banner size configs
            bannerSizeConfigs = new BannerSizeConfig[]
            {
                new BannerSizeConfig { size = BannerSize.Standard, dimensions = new Vector2(320, 50) },
                new BannerSizeConfig { size = BannerSize.Large, dimensions = new Vector2(320, 100) },
                new BannerSizeConfig { size = BannerSize.Rectangle, dimensions = new Vector2(300, 250) },
                new BannerSizeConfig { size = BannerSize.Smart, dimensions = new Vector2(0, 50) }
            };
            
            AdLogger.Log("Banner Container Configuration reset to defaults");
        }
    }
    
    /// <summary>
    /// Configuration data for individual banner sizes.
    /// </summary>
    [System.Serializable]
    public class BannerSizeConfig
    {
        [Tooltip("The banner size type this configuration applies to")]
        public BannerSize size;
        
        [Tooltip("Dimensions in pixels (width x height). For Smart banners, width of 0 means auto-calculate")]
        public Vector2 dimensions;
        
        [Tooltip("Optional override for text size for this banner size")]
        [Range(8, 24)]
        public int fontSizeOverride = 0; // 0 means use default from main config
    }
}