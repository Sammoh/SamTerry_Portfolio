using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Sammoh.TurnBasedStrategy
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Game UI Elements")]
        [SerializeField] private TextMeshProUGUI currentTurnText;
        [SerializeField] private TextMeshProUGUI gameStateText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Transform abilityButtonParent;
        [SerializeField] private Transform targetButtonParent;
        [SerializeField] private Button endTurnButton;

        [Header("Character Display")]
        [SerializeField] private Transform playerTeamParent;
        [SerializeField] private Transform enemyTeamParent;
        [SerializeField] private GameObject characterDisplayPrefab;

        [Header("Control Buttons")]
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button restartGameButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Prefabs")]
        [SerializeField] private GameObject abilityButtonPrefab;
        [SerializeField] private GameObject targetButtonPrefab;

        private TurnBasedGameManager gameManager;
        private CharacterAbility selectedAbility;
        private List<Button> abilityButtons = new List<Button>();
        private List<Button> targetButtons = new List<Button>();

        private void Awake()
        {
            gameManager = FindObjectOfType<TurnBasedGameManager>();
            
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged += HandleGameStateChanged;
                gameManager.OnGameMessage += HandleGameMessage;
                gameManager.OnCharacterDefeated += HandleCharacterDefeated;
            }

            SetupButtons();
            ShowMainMenu();
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged -= HandleGameStateChanged;
                gameManager.OnGameMessage -= HandleGameMessage;
                gameManager.OnCharacterDefeated -= HandleCharacterDefeated;
            }
        }

        private void SetupButtons()
        {
            if (startGameButton != null)
                startGameButton.onClick.AddListener(StartGame);
            
            if (restartGameButton != null)
                restartGameButton.onClick.AddListener(RestartGame);
            
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(ShowMainMenu);
            
            if (endTurnButton != null)
                endTurnButton.onClick.AddListener(EndTurn);
        }

        private void ShowMainMenu()
        {
            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(gamePanel, false);
            SetPanelActive(gameOverPanel, false);
        }

        private void ShowGamePanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(gamePanel, true);
            SetPanelActive(gameOverPanel, false);
        }

        private void ShowGameOverPanel()
        {
            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(gamePanel, false);
            SetPanelActive(gameOverPanel, true);
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
                panel.SetActive(active);
        }

        private void StartGame()
        {
            if (gameManager != null)
            {
                gameManager.StartNewGame();
                ShowGamePanel();
            }
        }

        private void RestartGame()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
                ShowGamePanel();
            }
        }

        private void EndTurn()
        {
            if (gameManager != null)
            {
                gameManager.EndCurrentTurn();
            }
        }

        private void HandleGameStateChanged(GameState newState)
        {
            UpdateGameStateText(newState);
            UpdateTurnDisplay();
            UpdateAbilityButtons();

            switch (newState)
            {
                case GameState.PlayerTurn:
                    ShowGamePanel();
                    break;
                case GameState.EnemyTurn:
                    ClearTargetButtons();
                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    ShowGameOverPanel();
                    break;
            }
        }

        private void HandleGameMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
                Debug.Log($"Game Message: {message}");
            }
        }

        private void HandleCharacterDefeated(Character character)
        {
            // Update character displays or other UI elements as needed
            UpdateCharacterDisplays();
        }

        private void UpdateGameStateText(GameState state)
        {
            if (gameStateText != null)
            {
                gameStateText.text = $"Game State: {state}";
            }
        }

        private void UpdateTurnDisplay()
        {
            if (currentTurnText != null && gameManager != null)
            {
                var currentCharacter = gameManager.CurrentCharacter;
                if (currentCharacter != null)
                {
                    currentTurnText.text = $"Current Turn: {currentCharacter.CharacterName}";
                }
                else
                {
                    currentTurnText.text = "No Active Character";
                }
            }
        }

        private void UpdateAbilityButtons()
        {
            ClearAbilityButtons();
            ClearTargetButtons();

            if (gameManager?.CurrentCharacter?.IsPlayerControlled == true)
            {
                CreateAbilityButtons(gameManager.CurrentCharacter.Abilities);
            }

            UpdateEndTurnButton();
        }

        private void CreateAbilityButtons(CharacterAbility[] abilities)
        {
            if (abilityButtonParent == null || abilityButtonPrefab == null) return;

            foreach (var ability in abilities)
            {
                GameObject buttonObj = Instantiate(abilityButtonPrefab, abilityButtonParent);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    buttonText.text = $"{ability.AbilityName} ({ability.ManaCost} MP)";
                }

                button.onClick.AddListener(() => SelectAbility(ability));
                
                // Disable button if ability can't be used
                button.interactable = gameManager.CanUseAbility(ability);

                abilityButtons.Add(button);
            }
        }

        private void SelectAbility(CharacterAbility ability)
        {
            selectedAbility = ability;
            ClearTargetButtons();

            var validTargets = gameManager.GetValidTargets(ability);
            if (validTargets.Count > 0)
            {
                CreateTargetButtons(validTargets);
            }
            else
            {
                // Use ability without target
                UseSelectedAbility(null);
            }
        }

        private void CreateTargetButtons(List<Character> targets)
        {
            if (targetButtonParent == null || targetButtonPrefab == null) return;

            foreach (var target in targets)
            {
                GameObject buttonObj = Instantiate(targetButtonPrefab, targetButtonParent);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                {
                    string healthInfo = $"HP: {target.Stats.CurrentHealth}/{target.Stats.MaxHealth}";
                    buttonText.text = $"{target.CharacterName}\n{healthInfo}";
                }

                button.onClick.AddListener(() => UseSelectedAbility(target));
                targetButtons.Add(button);
            }
        }

        private void UseSelectedAbility(Character target)
        {
            if (selectedAbility != null && gameManager != null)
            {
                gameManager.UseAbility(selectedAbility, target);
                selectedAbility = null;
                ClearTargetButtons();
            }
        }

        private void ClearAbilityButtons()
        {
            foreach (var button in abilityButtons)
            {
                if (button != null && button.gameObject != null)
                    Destroy(button.gameObject);
            }
            abilityButtons.Clear();
        }

        private void ClearTargetButtons()
        {
            foreach (var button in targetButtons)
            {
                if (button != null && button.gameObject != null)
                    Destroy(button.gameObject);
            }
            targetButtons.Clear();
        }

        private void UpdateEndTurnButton()
        {
            if (endTurnButton != null)
            {
                endTurnButton.interactable = gameManager?.IsPlayerTurn == true;
            }
        }

        private void UpdateCharacterDisplays()
        {
            // This could be expanded to show character health bars, status, etc.
            // For now, we'll keep it simple
        }

        // Create simple UI element creation if prefabs aren't assigned
        private void CreateSimpleButton(Transform parent, string text, System.Action onClick)
        {
            GameObject buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(parent);

            Image image = buttonObj.AddComponent<Image>();
            image.color = Color.white;

            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(() => onClick());

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.color = Color.black;
            textComponent.alignment = TextAlignmentOptions.Center;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
    }
}