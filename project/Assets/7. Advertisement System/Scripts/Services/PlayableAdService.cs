using System;
using System.Collections;
using UnityEngine;

namespace Sammoh.Advertisement
{
    /// <summary>
    /// Playable advertisement service implementation for LevelPlay SDK.
    /// Handles interactive playable ads that allow users to try a mini-game
    /// or experience before downloading the full application.
    /// </summary>
    public class PlayableAdService : IPlayableAdService
    {
        #region Private Fields
        
        private bool _isInitialized = false;
        private bool _isLoading = false;
        private string _lastError = string.Empty;
        private string _appId;
        private bool _testMode;
        private bool _isAdReady = false;
        private MonoBehaviour _coroutineRunner;
        private Action<bool> _currentShowCallback;
        
        // Placement ID for playable ads
        private const string PLAYABLE_PLACEMENT_ID = "DefaultPlayable";
        
        #endregion
        
        #region IAdService Implementation
        
        public bool IsInitialized => _isInitialized;
        public bool IsLoading => _isLoading;
        public string LastError => _lastError;
        
        public event Action<AdEventArgs> OnAdLoaded;
        public event Action<AdErrorEventArgs> OnAdLoadFailed;
        public event Action<AdEventArgs> OnAdDisplayed;
        public event Action<AdEventArgs> OnAdDismissed;
        public event Action<AdEventArgs> OnAdClicked;
        
        #endregion
        
        #region IPlayableAdService Implementation
        
        public event Action<AdEventArgs> OnPlayableCompleted;
        
        #endregion
        
        #region Public Methods
        
        public void Initialize(string appId, bool testMode = false)
        {
            try
            {
                _appId = appId;
                _testMode = testMode;
                
                AdLogger.Log($"Initializing Playable Ad Service - App ID: {appId}, Test Mode: {testMode}");
                
                // Get or create coroutine runner
                var manager = AdServiceManager.Instance;
                _coroutineRunner = manager;
                
                // In a real implementation, this would initialize the LevelPlay playable SDK
                // For this demo, we'll simulate initialization
                SimulateInitialization();
                
            }
            catch (Exception ex)
            {
                _lastError = $"Playable service initialization failed: {ex.Message}";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(PLAYABLE_PLACEMENT_ID, "Playable", _lastError));
            }
        }
        
        public void LoadAd()
        {
            if (!_isInitialized)
            {
                _lastError = "Playable service not initialized";
                AdLogger.LogError(_lastError);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(PLAYABLE_PLACEMENT_ID, "Playable", _lastError));
                return;
            }
            
            if (_isLoading)
            {
                AdLogger.LogWarning("Playable ad is already loading");
                return;
            }
            
            if (_isAdReady)
            {
                AdLogger.LogWarning("Playable ad is already loaded and ready");
                return;
            }
            
            _isLoading = true;
            _isAdReady = false;
            AdLogger.Log("Loading playable ad...");
            
            // In a real implementation, this would call LevelPlay's playable load method
            // For this demo, we'll simulate loading with a coroutine
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulatePlayableLoad());
            }
        }
        
        public bool IsAdReady()
        {
            return _isInitialized && _isAdReady && !_isLoading;
        }
        
        public void ShowPlayable(Action<bool> onComplete = null)
        {
            if (!IsAdReady())
            {
                _lastError = "Playable ad is not ready to show";
                AdLogger.LogWarning(_lastError);
                onComplete?.Invoke(false);
                
                // Auto-load if not ready
                LoadAd();
                return;
            }
            
            _currentShowCallback = onComplete;
            
            try
            {
                AdLogger.Log("Showing playable ad...");
                
                // In a real implementation, this would call LevelPlay's show playable method
                // For this demo, we'll simulate showing with a coroutine
                if (_coroutineRunner != null)
                {
                    _coroutineRunner.StartCoroutine(SimulatePlayableShow());
                }
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to show playable: {ex.Message}";
                AdLogger.LogError(_lastError);
                onComplete?.Invoke(false);
                OnAdLoadFailed?.Invoke(new AdErrorEventArgs(PLAYABLE_PLACEMENT_ID, "Playable", _lastError));
            }
        }
        
        public void Destroy()
        {
            try
            {
                _isInitialized = false;
                _isLoading = false;
                _isAdReady = false;
                _currentShowCallback = null;
                
                AdLogger.Log("Playable Ad Service destroyed");
            }
            catch (Exception ex)
            {
                AdLogger.LogError($"Error destroying playable service: {ex.Message}");
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private void SimulateInitialization()
        {
            // Simulate async initialization
            if (_coroutineRunner != null)
            {
                _coroutineRunner.StartCoroutine(SimulateInitCoroutine());
            }
        }
        
        private IEnumerator SimulateInitCoroutine()
        {
            yield return new WaitForSeconds(1.8f); // Simulate initialization delay
            
            _isInitialized = true;
            AdLogger.Log("Playable Ad Service initialized successfully");
            
            // Auto-load the first ad
            LoadAd();
        }
        
        private IEnumerator SimulatePlayableLoad()
        {
            yield return new WaitForSeconds(3.5f); // Simulate load time
            
            _isLoading = false;
            
            // Simulate success/failure (75% success rate for demo)
            bool success = UnityEngine.Random.value > 0.25f;
            
            if (success)
            {
                _isAdReady = true;
                AdLogger.Log("Playable ad loaded successfully");
                
                var eventArgs = new AdEventArgs(PLAYABLE_PLACEMENT_ID, "Playable");
                OnAdLoaded?.Invoke(eventArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoaded(eventArgs);
            }
            else
            {
                _lastError = "Simulated playable load failure";
                AdLogger.LogError(_lastError);
                var errorArgs = new AdErrorEventArgs(PLAYABLE_PLACEMENT_ID, "Playable", _lastError, 4001);
                OnAdLoadFailed?.Invoke(errorArgs);
                
                // Notify event dispatcher
                AdServiceManager.Instance.EventDispatcher?.DispatchAdLoadFailed(errorArgs);
            }
        }
        
        private IEnumerator SimulatePlayableShow()
        {
            // Mark ad as consumed
            _isAdReady = false;
            
            // Show display event
            var displayEventArgs = new AdEventArgs(PLAYABLE_PLACEMENT_ID, "Playable");
            OnAdDisplayed?.Invoke(displayEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDisplayed(displayEventArgs);
            
            // Create and show playable UI
            var playableUI = CreatePlayableUI();
            
            // Simulate playable interaction time (10-60 seconds)
            float playTime = UnityEngine.Random.Range(10f, 60f);
            float elapsedTime = 0f;
            bool gameCompleted = false;
            
            // Simulate user interaction during playable
            while (elapsedTime < playTime && !gameCompleted)
            {
                elapsedTime += Time.deltaTime;
                
                // Simulate game completion chance (increases over time)
                float completionChance = (elapsedTime / playTime) * 0.3f; // 30% max chance
                if (UnityEngine.Random.value < completionChance * Time.deltaTime)
                {
                    gameCompleted = true;
                }
                
                UpdatePlayableProgress(playableUI, elapsedTime, playTime);
                yield return null;
            }
            
            // Simulate click chance (40% chance during playable)
            bool wasClicked = UnityEngine.Random.value < 0.4f;
            if (wasClicked)
            {
                var clickEventArgs = new AdEventArgs(PLAYABLE_PLACEMENT_ID, "Playable");
                OnAdClicked?.Invoke(clickEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchAdClicked(clickEventArgs);
            }
            
            // Clean up UI
            if (playableUI != null)
            {
                UnityEngine.Object.Destroy(playableUI);
            }
            
            // Show completion event if game was completed
            if (gameCompleted)
            {
                var completedEventArgs = new AdEventArgs(PLAYABLE_PLACEMENT_ID, "Playable");
                OnPlayableCompleted?.Invoke(completedEventArgs);
                AdServiceManager.Instance.EventDispatcher?.DispatchPlayableCompleted(completedEventArgs);
                
                AdLogger.Log("Playable ad completed successfully");
            }
            
            // Show dismiss event
            var dismissEventArgs = new AdEventArgs(PLAYABLE_PLACEMENT_ID, "Playable");
            OnAdDismissed?.Invoke(dismissEventArgs);
            AdServiceManager.Instance.EventDispatcher?.DispatchAdDismissed(dismissEventArgs);
            
            // Notify completion callback
            _currentShowCallback?.Invoke(gameCompleted);
            _currentShowCallback = null;
            
            AdLogger.Log("Playable ad dismissed");
            
            // Auto-load next ad
            LoadAd();
        }
        
        private GameObject CreatePlayableUI()
        {
            // Find or create the ad overlay canvas
            var overlayCanvas = FindAdOverlayCanvas();
            
            // Create full-screen playable container
            var playableContainer = new GameObject("PlayableAdContainer");
            var rectTransform = playableContainer.AddComponent<RectTransform>();
            
            // Set full screen
            rectTransform.SetParent(overlayCanvas.transform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Add background
            var background = playableContainer.AddComponent<UnityEngine.UI.Image>();
            background.color = new Color(0.1f, 0.3f, 0.5f, 1f); // Blue-tinted background
            
            // Add game area
            var gameArea = new GameObject("GameArea");
            gameArea.transform.SetParent(playableContainer.transform, false);
            var gameRect = gameArea.AddComponent<RectTransform>();
            gameRect.anchorMin = new Vector2(0.05f, 0.15f);
            gameRect.anchorMax = new Vector2(0.95f, 0.85f);
            gameRect.offsetMin = Vector2.zero;
            gameRect.offsetMax = Vector2.zero;
            
            var gameImage = gameArea.AddComponent<UnityEngine.UI.Image>();
            gameImage.color = new Color(0.2f, 0.6f, 0.9f, 1f); // Lighter blue for game area
            
            // Add title
            var titleObj = new GameObject("GameTitle");
            titleObj.transform.SetParent(gameArea.transform, false);
            var titleText = titleObj.AddComponent<UnityEngine.UI.Text>();
            titleText.text = "Demo Playable Game";
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.white;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 28;
            titleText.fontStyle = FontStyle.Bold;
            
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            // Add game instructions
            var instructionsObj = new GameObject("Instructions");
            instructionsObj.transform.SetParent(gameArea.transform, false);
            var instructionsText = instructionsObj.AddComponent<UnityEngine.UI.Text>();
            instructionsText.text = "Try this interactive demo!\\nTap anywhere to play\\nComplete the challenge to win!";
            instructionsText.alignment = TextAnchor.MiddleCenter;
            instructionsText.color = Color.white;
            instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructionsText.fontSize = 18;
            
            var instructionsRect = instructionsText.GetComponent<RectTransform>();
            instructionsRect.anchorMin = new Vector2(0, 0.4f);
            instructionsRect.anchorMax = new Vector2(1, 0.7f);
            instructionsRect.offsetMin = Vector2.zero;
            instructionsRect.offsetMax = Vector2.zero;
            
            // Add interactive elements (simple tap targets)
            for (int i = 0; i < 3; i++)
            {
                var tapTarget = new GameObject($"TapTarget{i}");
                tapTarget.transform.SetParent(gameArea.transform, false);
                var targetRect = tapTarget.AddComponent<RectTransform>();
                targetRect.anchorMin = new Vector2(0.1f + i * 0.3f, 0.1f);
                targetRect.anchorMax = new Vector2(0.3f + i * 0.3f, 0.3f);
                targetRect.offsetMin = Vector2.zero;
                targetRect.offsetMax = Vector2.zero;
                
                var targetImage = tapTarget.AddComponent<UnityEngine.UI.Image>();
                targetImage.color = new Color(1f, 0.8f, 0f, 0.8f); // Golden color
                
                var targetButton = tapTarget.AddComponent<UnityEngine.UI.Button>();
                targetButton.onClick.AddListener(() => {
                    // Simulate tap interaction
                    targetImage.color = Color.green;
                });
            }
            
            // Add progress indicator
            var progressObj = new GameObject("ProgressText");
            progressObj.transform.SetParent(playableContainer.transform, false);
            var progressText = progressObj.AddComponent<UnityEngine.UI.Text>();
            progressText.text = "Playing... 0%";
            progressText.alignment = TextAnchor.MiddleCenter;
            progressText.color = Color.yellow;
            progressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            progressText.fontSize = 16;
            
            var progressRect = progressText.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0, 0.05f);
            progressRect.anchorMax = new Vector2(1, 0.1f);
            progressRect.offsetMin = Vector2.zero;
            progressRect.offsetMax = Vector2.zero;
            
            // Add close button
            var closeButtonObj = new GameObject("CloseButton");
            closeButtonObj.transform.SetParent(playableContainer.transform, false);
            var closeButton = closeButtonObj.AddComponent<UnityEngine.UI.Button>();
            var closeButtonImage = closeButtonObj.AddComponent<UnityEngine.UI.Image>();
            closeButtonImage.color = Color.red;
            
            var closeButtonRect = closeButton.GetComponent<RectTransform>();
            closeButtonRect.anchorMin = new Vector2(0.9f, 0.9f);
            closeButtonRect.anchorMax = new Vector2(0.95f, 0.95f);
            closeButtonRect.offsetMin = Vector2.zero;
            closeButtonRect.offsetMax = Vector2.zero;
            
            // Add X text to close button
            var closeTextObj = new GameObject("CloseText");
            closeTextObj.transform.SetParent(closeButtonObj.transform, false);
            var closeText = closeTextObj.AddComponent<UnityEngine.UI.Text>();
            closeText.text = "X";
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.color = Color.white;
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = 20;
            closeText.fontStyle = FontStyle.Bold;
            
            var closeTextRect = closeText.GetComponent<RectTransform>();
            closeTextRect.anchorMin = Vector2.zero;
            closeTextRect.anchorMax = Vector2.one;
            closeTextRect.offsetMin = Vector2.zero;
            closeTextRect.offsetMax = Vector2.zero;
            
            return playableContainer;
        }
        
        private void UpdatePlayableProgress(GameObject playableUI, float elapsed, float total)
        {
            if (playableUI == null) return;
            
            var progressText = playableUI.transform.Find("ProgressText");
            if (progressText != null)
            {
                var text = progressText.GetComponent<UnityEngine.UI.Text>();
                int percentage = Mathf.FloorToInt((elapsed / total) * 100f);
                text.text = $"Playing... {percentage}%";
            }
        }
        
        private Canvas FindAdOverlayCanvas()
        {
            // Try to find existing ad overlay canvas
            var existingCanvas = GameObject.Find("AdOverlayCanvas");
            if (existingCanvas != null)
            {
                return existingCanvas.GetComponent<Canvas>();
            }
            
            // Create ad overlay canvas if it doesn't exist
            var canvasObj = new GameObject("AdOverlayCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000; // Ensure ads appear on top
            
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            return canvas;
        }
        
        #endregion
    }
}