using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Safe area handler utility for proper ad positioning on devices with notches or rounded corners.
    /// Ensures ads don't overlap with device UI elements like status bars or home indicators.
    /// </summary>
    public static class SafeAreaHandler
    {
        /// <summary>
        /// Gets the safe area rectangle for the current device.
        /// </summary>
        /// <returns>Safe area rectangle in screen coordinates</returns>
        public static Rect GetSafeArea()
        {
            return Screen.safeArea;
        }
        
        /// <summary>
        /// Applies safe area constraints to a RectTransform.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to apply safe area to</param>
        /// <param name="respectTop">Whether to respect the top safe area (status bar)</param>
        /// <param name="respectBottom">Whether to respect the bottom safe area (home indicator)</param>
        public static void ApplySafeArea(RectTransform rectTransform, bool respectTop = true, bool respectBottom = true)
        {
            if (rectTransform == null) return;
            
            var safeArea = GetSafeArea();
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            
            if (canvas == null) return;
            
            var canvasRect = canvas.GetComponent<RectTransform>();
            var screenSize = new Vector2(Screen.width, Screen.height);
            
            // Calculate safe area as normalized values
            var anchorMin = rectTransform.anchorMin;
            var anchorMax = rectTransform.anchorMax;
            
            if (respectBottom)
            {
                anchorMin.y = safeArea.yMin / screenSize.y;
            }
            
            if (respectTop)
            {
                anchorMax.y = safeArea.yMax / screenSize.y;
            }
            
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            
            AdLogger.Log($"Applied safe area: {safeArea} to {rectTransform.name}");
        }
        
        /// <summary>
        /// Checks if the device has a notch or rounded corners that affect the safe area.
        /// </summary>
        /// <returns>True if the device has safe area considerations</returns>
        public static bool HasSafeAreaConsiderations()
        {
            var safeArea = GetSafeArea();
            var screenSize = new Vector2(Screen.width, Screen.height);
            
            // Check if safe area is smaller than screen size
            return safeArea.width < screenSize.x || safeArea.height < screenSize.y;
        }
        
        /// <summary>
        /// Gets the safe area insets (margins) for each edge.
        /// </summary>
        /// <returns>Vector4 with left, bottom, right, top insets</returns>
        public static Vector4 GetSafeAreaInsets()
        {
            var safeArea = GetSafeArea();
            var screenSize = new Vector2(Screen.width, Screen.height);
            
            float left = safeArea.xMin;
            float bottom = safeArea.yMin;
            float right = screenSize.x - safeArea.xMax;
            float top = screenSize.y - safeArea.yMax;
            
            return new Vector4(left, bottom, right, top);
        }
        
        /// <summary>
        /// Positions a banner ad considering safe area constraints.
        /// </summary>
        /// <param name="bannerRect">The banner's RectTransform</param>
        /// <param name="position">Desired banner position</param>
        public static void PositionBannerWithSafeArea(RectTransform bannerRect, BannerPosition position)
        {
            if (bannerRect == null) return;
            
            var insets = GetSafeAreaInsets();
            var anchoredPosition = bannerRect.anchoredPosition;
            
            switch (position)
            {
                case BannerPosition.Top:
                    bannerRect.anchorMin = new Vector2(0, 1);
                    bannerRect.anchorMax = new Vector2(1, 1);
                    bannerRect.pivot = new Vector2(0.5f, 1);
                    anchoredPosition.y = -insets.w; // Account for top inset
                    break;
                    
                case BannerPosition.Bottom:
                    bannerRect.anchorMin = new Vector2(0, 0);
                    bannerRect.anchorMax = new Vector2(1, 0);
                    bannerRect.pivot = new Vector2(0.5f, 0);
                    anchoredPosition.y = insets.y; // Account for bottom inset
                    break;
                    
                case BannerPosition.TopLeft:
                    bannerRect.anchorMin = new Vector2(0, 1);
                    bannerRect.anchorMax = new Vector2(0, 1);
                    bannerRect.pivot = new Vector2(0, 1);
                    anchoredPosition.x = insets.x; // Account for left inset
                    anchoredPosition.y = -insets.w; // Account for top inset
                    break;
                    
                case BannerPosition.TopRight:
                    bannerRect.anchorMin = new Vector2(1, 1);
                    bannerRect.anchorMax = new Vector2(1, 1);
                    bannerRect.pivot = new Vector2(1, 1);
                    anchoredPosition.x = -insets.z; // Account for right inset
                    anchoredPosition.y = -insets.w; // Account for top inset
                    break;
                    
                case BannerPosition.BottomLeft:
                    bannerRect.anchorMin = new Vector2(0, 0);
                    bannerRect.anchorMax = new Vector2(0, 0);
                    bannerRect.pivot = new Vector2(0, 0);
                    anchoredPosition.x = insets.x; // Account for left inset
                    anchoredPosition.y = insets.y; // Account for bottom inset
                    break;
                    
                case BannerPosition.BottomRight:
                    bannerRect.anchorMin = new Vector2(1, 0);
                    bannerRect.anchorMax = new Vector2(1, 0);
                    bannerRect.pivot = new Vector2(1, 0);
                    anchoredPosition.x = -insets.z; // Account for right inset
                    anchoredPosition.y = insets.y; // Account for bottom inset
                    break;
                    
                case BannerPosition.Center:
                    bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
                    bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
                    bannerRect.pivot = new Vector2(0.5f, 0.5f);
                    anchoredPosition = Vector2.zero; // Center doesn't need safe area adjustment
                    break;
            }
            
            bannerRect.anchoredPosition = anchoredPosition;
            
            AdLogger.Log($"Positioned banner at {position} with safe area insets: {insets}");
        }
        
        /// <summary>
        /// Logs current safe area information for debugging.
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogSafeAreaInfo()
        {
            var safeArea = GetSafeArea();
            var insets = GetSafeAreaInsets();
            var hasSafeArea = HasSafeAreaConsiderations();
            
            AdLogger.Log($"Safe Area Info:");
            AdLogger.Log($"  Screen Size: {Screen.width}x{Screen.height}");
            AdLogger.Log($"  Safe Area: {safeArea}");
            AdLogger.Log($"  Insets (L,B,R,T): {insets}");
            AdLogger.Log($"  Has Safe Area Considerations: {hasSafeArea}");
        }
    }
}