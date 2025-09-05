using UnityEngine;
using Sammoh.TurnBasedStrategy;

namespace Sammoh.TurnBasedStrategy
{
    /// <summary>
    /// Demonstration script that shows how to use the Turn-Based Strategy Game system.
    /// This script can be attached to a GameObject to start a game programmatically.
    /// </summary>
    public class TurnBasedGameDemo : MonoBehaviour
    {
        [Header("Game Setup")]
        [SerializeField] private bool startGameOnAwake = true;
        [SerializeField] private bool enableDebugLogging = true;
        
        private TurnBasedGameManager gameManager;
        private bool gameStarted = false;

        private void Awake()
        {
            // Find or create game manager
            gameManager = FindObjectOfType<TurnBasedGameManager>();
            if (gameManager == null)
            {
                GameObject gameManagerObj = new GameObject("TurnBasedGameManager");
                gameManager = gameManagerObj.AddComponent<TurnBasedGameManager>();
                gameManagerObj.AddComponent<CharacterFactory>();
                gameManagerObj.AddComponent<TurnManager>();
            }

            // Subscribe to events for demonstration
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged += OnGameStateChanged;
                gameManager.OnGameMessage += OnGameMessage;
                gameManager.OnCharacterDefeated += OnCharacterDefeated;
            }
        }

        private void Start()
        {
            if (startGameOnAwake && !gameStarted)
            {
                StartDemoGame();
            }
        }

        private void OnDestroy()
        {
            if (gameManager != null)
            {
                gameManager.OnGameStateChanged -= OnGameStateChanged;
                gameManager.OnGameMessage -= OnGameMessage;
                gameManager.OnCharacterDefeated -= OnCharacterDefeated;
            }
        }

        public void StartDemoGame()
        {
            if (gameManager != null && !gameStarted)
            {
                Debug.Log("Starting Turn-Based Strategy Game Demo");
                gameManager.StartNewGame();
                gameStarted = true;
                
                Debug.Log($"Game started with {gameManager.PlayerTeam.Count} players vs {gameManager.EnemyTeam.Count} enemies");
                
                // Display team information
                DisplayTeamInfo("Player Team", gameManager.PlayerTeam);
                DisplayTeamInfo("Enemy Team", gameManager.EnemyTeam);
            }
        }

        public void RestartDemoGame()
        {
            if (gameManager != null)
            {
                Debug.Log("Restarting Turn-Based Strategy Game Demo");
                gameManager.RestartGame();
                gameStarted = true;
            }
        }

        public void CleanupGame()
        {
            if (gameManager != null)
            {
                Debug.Log("Cleaning up Turn-Based Strategy Game Demo");
                gameManager.CleanupGame();
                gameStarted = false;
            }
        }

        private void DisplayTeamInfo(string teamName, System.Collections.Generic.List<Character> team)
        {
            var msg = "";
            msg += ($"\n{teamName}:");
            foreach (var character in team)
            {
                msg += ($"\n  - {character.CharacterName}:");
                msg += ($"\n    Health: {character.Stats.CurrentHealth}/{character.Stats.MaxHealth}");
                msg += ($"\n    Attack: {character.Stats.Attack}, Defense: {character.Stats.Defense}");
                msg += ($"\n    Speed: {character.Stats.Speed}, Mana: {character.Stats.CurrentMana}/{character.Stats.Mana}");
                msg += ($"\n    Abilities: {string.Join(", ", System.Array.ConvertAll(character.Abilities, a => a.AbilityName))}");
            }

            Debug.Log(msg);
        }

        private void OnGameStateChanged(GameState newState)
        {
            Debug.Log($"Game State Changed: {newState}");
            
            switch (newState)
            {
                case GameState.PlayerTurn:
                    Debug.Log($"Player's turn: {gameManager.CurrentCharacter?.CharacterName}");
                    DisplayAvailableActions();
                    break;
                    
                case GameState.EnemyTurn:
                    Debug.Log($"Enemy's turn: {gameManager.CurrentCharacter?.CharacterName}");
                    break;
                    
                case GameState.GameOver:
                    Debug.Log("Game Over! All players have been defeated.");
                    break;
                    
                case GameState.Victory:
                    Debug.Log("Victory! All enemies have been defeated.");
                    break;
            }
        }

        private void OnGameMessage(string message)
        {
            Debug.Log($"Game Message: {message}");
        }

        private void OnCharacterDefeated(Character character)
        {
            Debug.Log($"{character.CharacterName} has been defeated!");
        }

        private void DisplayAvailableActions()
        {
            if (gameManager?.CurrentCharacter != null)
            {
                var currentChar = gameManager.CurrentCharacter;
                var msg = $"Available abilities for {currentChar.CharacterName}:";
                
                foreach (var ability in currentChar.Abilities)
                {
                    bool canUse = gameManager.CanUseAbility(ability);
                    string status = canUse ? "Available" : "Not enough mana";
                    msg += ($"\n  - {ability.AbilityName} ({ability.ManaCost} MP): {status}");
                }

                Debug.Log(msg);
            }
        }

        // Public methods for UI or external control
        public void UseAbilityOnRandomTarget(int abilityIndex)
        {
            if (gameManager?.CurrentCharacter != null && gameManager.IsPlayerTurn)
            {
                var abilities = gameManager.CurrentCharacter.Abilities;
                if (abilityIndex >= 0 && abilityIndex < abilities.Length)
                {
                    var ability = abilities[abilityIndex];
                    var validTargets = gameManager.GetValidTargets(ability);
                    
                    if (validTargets.Count > 0)
                    {
                        var randomTarget = validTargets[Random.Range(0, validTargets.Count)];
                        gameManager.UseAbility(ability, randomTarget);
                    }
                    else
                    {
                        gameManager.UseAbility(ability);
                    }
                }
            }
        }

        public void EndPlayerTurn()
        {
            if (gameManager?.IsPlayerTurn == true)
            {
                gameManager.EndCurrentTurn();
            }
        }

        // Demonstration of factory pattern usage
        public void DemonstrateFactoryPattern()
        {
            Debug.Log("Demonstrating Factory Pattern:");
            
            GameObject factoryObj = new GameObject("DemoFactory");
            CharacterFactory factory = factoryObj.AddComponent<CharacterFactory>();
            
            // Create one of each character class
            var characterClasses = System.Enum.GetValues(typeof(CharacterClass));
            foreach (CharacterClass characterClass in characterClasses)
            {
                Character character = factory.CreateCharacter(characterClass, $"Demo_{characterClass}", false);
                Debug.Log($"Created {characterClass}: {character.CharacterName}");
                Debug.Log($"  Stats: HP={character.Stats.MaxHealth}, ATK={character.Stats.Attack}, DEF={character.Stats.Defense}, SPD={character.Stats.Speed}, MP={character.Stats.Mana}");
                
                // Cleanup
                DestroyImmediate(character.gameObject);
            }
            
            DestroyImmediate(factoryObj);
            Debug.Log("Factory Pattern demonstration complete");
        }
    }
}