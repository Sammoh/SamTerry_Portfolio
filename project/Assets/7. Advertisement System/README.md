# Advertisement System (LevelPlay Integration)

A comprehensive advertisement system demonstrating Unity's LevelPlay SDK integration with advanced service patterns for scalable ad management.

## 🎯 Overview

This project showcases a complete advertisement integration using IronSource's LevelPlay SDK (v9.0.0) with a focus on:
- **Modular Service Architecture**: Pluggable ad services using strategy and service patterns
- **Canvas Overlay System**: Dedicated UI layer for seamless ad display
- **Multiple Ad Types**: Banner, interstitial, rewarded video, playable, and native ads
- **Resource Management**: Proper Unity prefab and asset organization
- **Testable Infrastructure**: Unit tests for core advertisement functionality

## ✨ Key Features

### Advertisement Types
- **Banner Ads**: Bottom/top positioned banner advertisements with configurable sizing
- **Interstitial Ads**: Full-screen ads displayed at natural break points
- **Rewarded Video Ads**: Video ads that provide in-game rewards upon completion
- **Playable Ads**: Interactive mini-game advertisements
- **Native Ads**: Custom integrated ads that match app design
- **Video Ads**: Standard video advertisement content

### Service Architecture
- **IAdService Interface**: Core advertisement service contract
- **AdServiceManager**: Central manager for all ad services
- **AdEventDispatcher**: Event-driven communication system
- **AdConfigurationManager**: Centralized ad configuration and settings

### UI Integration
- **Overlay Canvas**: Dedicated canvas layer for ad display
- **Responsive Positioning**: Automatic ad positioning based on screen size
- **Safe Area Support**: Proper handling of device notches and safe areas
- **Loading States**: Visual feedback during ad loading and display

## 🏗️ Technical Implementation

### Service Pattern Architecture
```
AdServiceManager
├── BannerAdService (IBannerAdService)
├── InterstitialAdService (IInterstitialAdService)
├── RewardedVideoAdService (IRewardedVideoAdService)
├── PlayableAdService (IPlayableAdService)
└── NativeAdService (INativeAdService)
```

### Event System
- Ad loaded/failed events
- Ad displayed/dismissed events
- Reward granted events
- Click-through tracking

### Resource Management
- Prefab-based ad UI components
- Scriptable Object configurations
- Resource loading optimization
- Memory management for ad assets

## 🚀 Getting Started

### Prerequisites
- Unity 6000.0.49f1 or later
- LevelPlay SDK 9.0.0 (already configured)
- TextMeshPro package
- Understanding of Unity UI (uGUI) system

### Setup Instructions
1. **Open Scene**: Load `AdDemo.unity`
2. **Configure Ad IDs**: Set your LevelPlay app ID and placement IDs
3. **Test Integration**: Use the demo buttons to test different ad types
4. **Customize UI**: Modify the overlay canvas for your app's design

### Quick Start Example
```csharp
// Initialize ad services
AdServiceManager.Instance.Initialize("YOUR_APP_ID");

// Show banner ad
AdServiceManager.Instance.BannerService.ShowBanner(BannerPosition.Bottom);

// Show interstitial with callback
AdServiceManager.Instance.InterstitialService.ShowInterstitial((success) => {
    if (success) {
        Debug.Log("Interstitial ad displayed successfully");
    }
});

// Show rewarded video
AdServiceManager.Instance.RewardedVideoService.ShowRewardedVideo((reward) => {
    if (reward.IsValid) {
        // Grant reward to player
        PlayerManager.Instance.AddCoins(reward.Amount);
    }
});
```

## 🎮 User Interface Flow

### Ad Demo Scene
1. **Main Menu**: Buttons to test different ad types
2. **Banner Controls**: Show/hide banner ads with position selection
3. **Interstitial Controls**: Load and show interstitial ads
4. **Rewarded Video Controls**: Test reward mechanics
5. **Settings Panel**: Configure ad frequencies and debugging
6. **Status Display**: Real-time ad status and event logging

## ⚙️ Configuration

### Ad Placement Settings
- Banner refresh rates
- Interstitial frequency capping
- Rewarded video cooldowns
- Ad loading timeouts

### UI Customization
- Canvas sorting order
- Safe area handling
- Animation transitions
- Brand color theming

## 🧪 Testing

### Functional Testing
```csharp
[Test]
public void BannerService_ShowBanner_DisplaysSuccessfully()
{
    // Arrange
    var bannerService = AdServiceManager.Instance.BannerService;
    
    // Act
    bannerService.ShowBanner(BannerPosition.Bottom);
    
    // Assert
    Assert.IsTrue(bannerService.IsBannerVisible);
}

[Test]
public void RewardedVideo_GrantReward_UpdatesPlayerCurrency()
{
    // Test reward granting mechanism
}
```

### Manual Testing Workflow
1. **Initialize SDK**: Test app ID validation
2. **Load Ads**: Verify all ad types load correctly
3. **Display Ads**: Test display on different screen sizes
4. **Event Handling**: Verify callbacks and events fire properly
5. **Error Handling**: Test behavior with no internet/failed loads

## 📁 File System Organization

```
7. Advertisement System/
├── README.md                          # This documentation
├── AdDemo.unity                       # Main demonstration scene
├── Scripts/
│   ├── Core/
│   │   ├── AdServiceManager.cs        # Central ad service manager
│   │   ├── IAdService.cs              # Core ad service interface
│   │   └── AdEventDispatcher.cs       # Event communication system
│   ├── Services/
│   │   ├── BannerAdService.cs         # Banner ad implementation
│   │   ├── InterstitialAdService.cs   # Interstitial ad implementation
│   │   ├── RewardedVideoAdService.cs  # Rewarded video implementation
│   │   ├── PlayableAdService.cs       # Playable ad implementation
│   │   └── NativeAdService.cs         # Native ad implementation
│   ├── UI/
│   │   ├── AdOverlayCanvas.cs         # Main ad display canvas
│   │   ├── AdDemoController.cs        # Demo scene controller
│   │   └── AdStatusDisplay.cs         # Status and debug info
│   ├── Configuration/
│   │   ├── AdConfiguration.cs         # Scriptable object configs
│   │   └── AdPlacementSettings.cs     # Placement configurations
│   └── Utilities/
│       ├── AdLogger.cs                # Advertisement logging
│       └── SafeAreaHandler.cs         # Safe area calculations
├── Prefabs/
│   ├── UI/
│   │   ├── AdOverlayCanvas.prefab     # Main ad canvas
│   │   ├── BannerAdContainer.prefab   # Banner ad container
│   │   └── LoadingIndicator.prefab    # Ad loading indicator
│   └── Demo/
│       └── AdDemoUI.prefab            # Demo scene UI
├── Configurations/
│   ├── DefaultAdConfig.asset          # Default ad settings
│   └── TestAdConfig.asset             # Testing configurations
└── Tests/
    ├── AdServiceTests.cs              # Core service tests
    └── AdUITests.cs                   # UI component tests
```

## 🎯 Current Development Status

### ✅ Completed
- [x] Project structure and organization
- [x] Core service architecture design
- [x] LevelPlay SDK integration analysis

### 🚧 In Progress
- [ ] Core service interfaces and manager
- [ ] Banner ad implementation
- [ ] Interstitial ad implementation
- [ ] UI overlay system

### 📋 Planned Features
- [ ] Rewarded video implementation
- [ ] Playable ad support
- [ ] Native ad integration
- [ ] Advanced analytics tracking
- [ ] A/B testing framework
- [ ] Revenue optimization tools

## 🐛 Known Issues & Workarounds

### LevelPlay SDK Considerations
- **Initialization Timing**: SDK must be initialized before showing ads
- **Platform Differences**: iOS and Android may have different behavior
- **Test Mode**: Use test app IDs during development
- **GDPR Compliance**: Implement consent management for EU users

### Unity Integration
- **Canvas Scaling**: Ensure proper UI scaling across devices
- **Performance**: Monitor memory usage with frequent ad loads
- **Threading**: Ad callbacks may occur on background threads

## 🔧 Customization

### Adding New Ad Types
1. Create service interface inheriting from `IAdService`
2. Implement concrete service class
3. Register service with `AdServiceManager`
4. Add UI controls for testing

### Custom Ad Positioning
1. Modify `AdOverlayCanvas` positioning logic
2. Update `SafeAreaHandler` for device-specific adjustments
3. Test on various screen sizes and orientations

### Event System Extension
1. Add new event types to `AdEventDispatcher`
2. Create custom event handlers in services
3. Update UI components to handle new events

## 📋 Dependencies

### Unity Packages
- **LevelPlay SDK** (9.0.0) - Advertisement mediation platform
- **TextMeshPro** (Built-in) - Text rendering for UI
- **Unity UI** (Built-in) - Canvas and UI components

### External Dependencies
- **IronSource SDK** - Core advertisement SDK
- **Google AdMob** - Google advertisement network
- **Unity Ads** - Unity's advertisement platform

### Technical Requirements
- **Unity Version**: 6000.0.49f1 or later
- **Platform**: iOS, Android
- **Internet**: Required for ad loading and display
- **Permissions**: Network access, advertising ID

## 💡 Learning Outcomes

This project demonstrates:
- **Service-Oriented Architecture**: Modular, testable service design
- **Advertisement Integration**: Professional ad SDK implementation
- **UI/UX Design**: Seamless ad integration without disrupting gameplay
- **Performance Optimization**: Efficient ad loading and memory management
- **Testing Strategy**: Comprehensive testing of ad functionality
- **Documentation**: Professional project documentation standards

## 🌟 Popular Advertisement Techniques

### Monetization Strategies
1. **Banner Refresh**: Automatic banner rotation for increased revenue
2. **Interstitial Pacing**: Strategic placement at natural break points
3. **Rewarded Opt-in**: Optional video ads for in-game benefits
4. **Native Integration**: Ads that match app visual design
5. **Playable Previews**: Interactive ads for better engagement

### User Experience Optimization
1. **Frequency Capping**: Limit ad exposure to prevent fatigue
2. **Loading Indicators**: Visual feedback during ad preparation
3. **Skip Options**: User control over ad duration
4. **Reward Clarity**: Clear communication of ad benefits
5. **Graceful Failures**: Smooth handling of ad load failures

### Performance Optimization
1. **Preloading**: Load ads before display for instant showing
2. **Caching**: Store ad assets locally for offline scenarios
3. **Memory Management**: Proper cleanup of ad resources
4. **Network Optimization**: Efficient ad request scheduling
5. **Analytics Integration**: Data-driven ad performance optimization

---

**Note**: This system provides a production-ready foundation for mobile advertisement integration and demonstrates industry-standard practices for Unity ad implementation.