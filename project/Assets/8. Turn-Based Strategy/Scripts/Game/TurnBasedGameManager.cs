using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sammoh.TurnBasedStrategy
{
    public class TurnBasedGameManager : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private int playerTeamSize = 2;
        [SerializeField] private int enemyTeamSize = 2;
        [SerializeField] private Transform playerTeamParent;
        [SerializeField] private Transform enemyTeamParent;

        [Header("Components")]
        [SerializeField] private CharacterFactory characterFactory;
        [SerializeField] private TurnManager turnManager;

        private GameStateData gameState = new GameStateData();
        private List<Character> playerTeam = new List<Character>();
        private List<Character> enemyTeam = new List<Character>();
        private List<Character> allCharacters = new List<Character>();

        // Events
        public event Action<GameState> OnGameStateChanged;
        public event Action<Character> OnCharacterDefeated;
        public event Action<string> OnGameMessage;

        // Properties
        public GameStateData GameStateData => gameState;
        public List<Character> PlayerTeam => playerTeam;
        public List<Character> EnemyTeam => enemyTeam;
        public Character CurrentCharacter => turnManager.CurrentCharacter;
        public bool IsPlayerTurn => turnManager.IsPlayerTurn;

        private void Awake()
        {
            if (characterFactory == null)
                characterFactory = GetComponent<CharacterFactory>();
            
            if (turnManager == null)
                turnManager = GetComponent<TurnManager>();

            // Subscribe to turn manager events
            turnManager.OnTurnStart += HandleTurnStart;
            turnManager.OnTurnEnd += HandleTurnEnd;
            turnManager.OnRoundComplete += HandleRoundComplete;
        }

        private void OnDestroy()
        {
            if (turnManager != null)
            {
                turnManager.OnTurnStart -= HandleTurnStart;
                turnManager.OnTurnEnd -= HandleTurnEnd;
                turnManager.OnRoundComplete -= HandleRoundComplete;
            }
        }

        public void StartNewGame()
        {
            CleanupGame();
            CreateTeams();
            InitializeGame();
        }

        public void RestartGame()
        {
            CleanupGame();
            CreateTeams();
            InitializeGame();
        }

        public void CleanupGame()
        {
            // Destroy existing characters
            foreach (var character in allCharacters)
            {
                if (character != null && character.gameObject != null)
                    DestroyImmediate(character.gameObject);
            }

            // Clear lists
            playerTeam.Clear();
            enemyTeam.Clear();
            allCharacters.Clear();

            // Reset managers
            turnManager.ResetTurnManager();
            gameState.Reset();

            OnGameMessage?.Invoke("Game cleaned up and ready for restart");
        }

        private void CreateTeams()
        {
            // Create player team
            for (int i = 0; i < playerTeamSize; i++)
            {
                CharacterClass playerClass = (CharacterClass)Random.Range(0, System.Enum.GetValues(typeof(CharacterClass)).Length);
                Character player = characterFactory.CreateCharacter(
                    playerClass, 
                    $"Player_{playerClass}_{i + 1}", 
                    true, 
                    playerTeamParent
                );
                playerTeam.Add(player);
                allCharacters.Add(player);
            }

            // Create enemy team
            for (int i = 0; i < enemyTeamSize; i++)
            {
                CharacterClass enemyClass = (CharacterClass)Random.Range(0, System.Enum.GetValues(typeof(CharacterClass)).Length);
                Character enemy = characterFactory.CreateCharacter(
                    enemyClass, 
                    $"Enemy_{enemyClass}_{i + 1}", 
                    false, 
                    enemyTeamParent
                );
                enemyTeam.Add(enemy);
                allCharacters.Add(enemy);
            }
        }

        private void InitializeGame()
        {
            gameState.playerTeamAlive = playerTeam.Count(c => c.CanAct());
            gameState.enemyTeamAlive = enemyTeam.Count(c => c.CanAct());

            turnManager.InitializeTurnOrder(allCharacters);

            // Start the first turn; this sets CurrentCharacter and triggers OnTurnStart.
            // HandleTurnStart will set the GameState appropriately (PlayerTurn/EnemyTurn).
            turnManager.StartNextTurn();

            OnGameMessage?.Invoke($"Game started! {gameState.playerTeamAlive} players vs {gameState.enemyTeamAlive} enemies");
        }

        public bool UseAbility(CharacterAbility ability, Character target = null)
        {
            if (CurrentCharacter == null || !CurrentCharacter.CanAct())
                return false;

            if (!ability.CanUse(CurrentCharacter.Stats))
            {
                OnGameMessage?.Invoke($"{CurrentCharacter.CharacterName} cannot use {ability.AbilityName} - not enough mana!");
                return false;
            }

            int result = ability.Execute(CurrentCharacter.Stats, target?.Stats);
            
            string message = $"{CurrentCharacter.CharacterName} uses {ability.AbilityName}";
            if (target != null)
            {
                message += $" on {target.CharacterName}";
                if (ability.AbilityType == AbilityType.Attack)
                    message += $" for {result} damage";
            }
            else if (ability.AbilityType == AbilityType.Heal)
            {
                message += $" and heals for {result}";
            }

            OnGameMessage?.Invoke(message);

            // Check if target was defeated
            if (target != null && !target.CanAct())
            {
                HandleCharacterDefeated(target);
            }

            // End turn after using ability
            EndCurrentTurn();
            return true;
        }

        public void EndCurrentTurn()
        {
            CheckWinConditions();
            
            if (gameState.currentState != GameState.GameOver && gameState.currentState != GameState.Victory)
            {
                turnManager.StartNextTurn();
            }
        }

        private void HandleTurnStart(Character character)
        {
            ChangeGameState(character.IsPlayerControlled ? GameState.PlayerTurn : GameState.EnemyTurn);
            
            OnGameMessage?.Invoke($"{character.CharacterName}'s turn begins");

            // AI turn for enemy characters
            if (!character.IsPlayerControlled)
            {
                // Simple AI: attack random player
                StartCoroutine(PerformEnemyTurn(character));
            }
        }

        private System.Collections.IEnumerator PerformEnemyTurn(Character enemy)
        {
            yield return new WaitForSeconds(1f); // Delay for readability

            var alivePlayerTargets = playerTeam.Where(p => p.CanAct()).ToList();
            if (alivePlayerTargets.Count > 0)
            {
                var randomTarget = alivePlayerTargets[Random.Range(0, alivePlayerTargets.Count)];
                var randomAbility = enemy.Abilities[Random.Range(0, enemy.Abilities.Length)];
                
                UseAbility(randomAbility, randomTarget);
            }
            else
            {
                EndCurrentTurn();
            }
        }

        private void HandleTurnEnd(Character character)
        {
            // Turn cleanup logic if needed
        }

        private void HandleRoundComplete()
        {
            gameState.turnNumber++;
            OnGameMessage?.Invoke($"Round {gameState.turnNumber} complete");
        }

        private void HandleCharacterDefeated(Character character)
        {
            OnCharacterDefeated?.Invoke(character);
            turnManager.RemoveCharacterFromTurns(character);
            
            OnGameMessage?.Invoke($"{character.CharacterName} has been defeated!");

            // Update alive counts
            gameState.playerTeamAlive = playerTeam.Count(c => c.CanAct());
            gameState.enemyTeamAlive = enemyTeam.Count(c => c.CanAct());
        }

        private void CheckWinConditions()
        {
            gameState.playerTeamAlive = playerTeam.Count(c => c.CanAct());
            gameState.enemyTeamAlive = enemyTeam.Count(c => c.CanAct());

            if (gameState.playerTeamAlive <= 0)
            {
                ChangeGameState(GameState.GameOver);
                OnGameMessage?.Invoke("Game Over! All players have been defeated.");
            }
            else if (gameState.enemyTeamAlive <= 0)
            {
                ChangeGameState(GameState.Victory);
                OnGameMessage?.Invoke("Victory! All enemies have been defeated.");
            }
        }

        private void ChangeGameState(GameState newState)
        {
            if (gameState.currentState != newState)
            {
                gameState.currentState = newState;
                OnGameStateChanged?.Invoke(newState);
            }
        }

        // Public methods for UI interaction
        public List<Character> GetValidTargets(CharacterAbility ability)
        {
            switch (ability.AbilityType)
            {
                case AbilityType.Attack:
                case AbilityType.Special:
                    return enemyTeam.Where(e => e.CanAct()).ToList();
                
                case AbilityType.Heal:
                    return playerTeam.Where(p => p.CanAct()).ToList();
                
                case AbilityType.Defend:
                    return new List<Character> { CurrentCharacter };
                
                default:
                    return new List<Character>();
            }
        }

        public bool CanUseAbility(CharacterAbility ability)
        {
            return CurrentCharacter?.CanAct() == true && ability.CanUse(CurrentCharacter.Stats);
        }
    }
}