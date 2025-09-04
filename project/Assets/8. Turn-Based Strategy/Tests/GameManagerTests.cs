using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;
using System.Collections.Generic;

public class GameManagerTests
{
    private GameObject gameManagerObject;
    private TurnBasedGameManager gameManager;
    private GameObject factoryObject;
    private CharacterFactory factory;
    private GameObject turnManagerObject;
    private TurnManager turnManager;

    [SetUp]
    public void Setup()
    {
        // Create game manager
        gameManagerObject = new GameObject("TestGameManager");
        gameManager = gameManagerObject.AddComponent<TurnBasedGameManager>();
        
        // Create factory
        factoryObject = new GameObject("TestFactory");
        factory = factoryObject.AddComponent<CharacterFactory>();
        
        // Create turn manager
        turnManagerObject = new GameObject("TestTurnManager");
        turnManager = turnManagerObject.AddComponent<TurnManager>();
        
        // Set up references using reflection since fields are private
        var factoryField = typeof(TurnBasedGameManager).GetField("characterFactory", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var turnManagerField = typeof(TurnBasedGameManager).GetField("turnManager", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
        factoryField?.SetValue(gameManager, factory);
        turnManagerField?.SetValue(gameManager, turnManager);
    }

    [TearDown]
    public void TearDown()
    {
        if (gameManagerObject != null)
            Object.DestroyImmediate(gameManagerObject);
        if (factoryObject != null)
            Object.DestroyImmediate(factoryObject);
        if (turnManagerObject != null)
            Object.DestroyImmediate(turnManagerObject);
    }

    [Test]
    public void GameManager_InitialState_IsNotStarted()
    {
        Assert.AreEqual(GameState.NotStarted, gameManager.GameState.currentState);
        Assert.AreEqual(0, gameManager.GameState.turnNumber);
    }

    [Test]
    public void GameManager_StartNewGame_CreatesTeams()
    {
        gameManager.StartNewGame();
        
        Assert.GreaterOrEqual(gameManager.PlayerTeam.Count, 1);
        Assert.GreaterOrEqual(gameManager.EnemyTeam.Count, 1);
        Assert.AreNotEqual(GameState.NotStarted, gameManager.GameState.currentState);
    }

    [Test]
    public void GameManager_CleanupGame_ClearsTeams()
    {
        gameManager.StartNewGame();
        int playerCount = gameManager.PlayerTeam.Count;
        int enemyCount = gameManager.EnemyTeam.Count;
        
        Assert.Greater(playerCount, 0);
        Assert.Greater(enemyCount, 0);
        
        gameManager.CleanupGame();
        
        Assert.AreEqual(0, gameManager.PlayerTeam.Count);
        Assert.AreEqual(0, gameManager.EnemyTeam.Count);
        Assert.AreEqual(GameState.NotStarted, gameManager.GameState.currentState);
    }

    [Test]
    public void GameManager_RestartGame_ResetsAndStartsNewGame()
    {
        gameManager.StartNewGame();
        var firstPlayerTeamSize = gameManager.PlayerTeam.Count;
        
        gameManager.RestartGame();
        
        Assert.AreEqual(firstPlayerTeamSize, gameManager.PlayerTeam.Count);
        Assert.AreNotEqual(GameState.NotStarted, gameManager.GameState.currentState);
    }

    [Test]
    public void GameManager_GetValidTargets_Attack_ReturnsEnemies()
    {
        gameManager.StartNewGame();
        
        var attackAbility = new CharacterAbility("Test Attack", AbilityType.Attack, 10, 5, "Test");
        var targets = gameManager.GetValidTargets(attackAbility);
        
        Assert.Greater(targets.Count, 0);
        foreach (var target in targets)
        {
            Assert.IsFalse(target.IsPlayerControlled);
            Assert.IsTrue(target.CanAct());
        }
    }

    [Test]
    public void GameManager_GetValidTargets_Heal_ReturnsPlayers()
    {
        gameManager.StartNewGame();
        
        var healAbility = new CharacterAbility("Test Heal", AbilityType.Heal, 20, 10, "Test");
        var targets = gameManager.GetValidTargets(healAbility);
        
        Assert.Greater(targets.Count, 0);
        foreach (var target in targets)
        {
            Assert.IsTrue(target.IsPlayerControlled);
            Assert.IsTrue(target.CanAct());
        }
    }

    [Test]
    public void GameManager_CanUseAbility_ReturnsFalseWhenNoCurrentCharacter()
    {
        var ability = new CharacterAbility("Test", AbilityType.Attack, 10, 5, "Test");
        
        bool canUse = gameManager.CanUseAbility(ability);
        
        Assert.IsFalse(canUse);
    }
}

public class TurnManagerTests
{
    private GameObject turnManagerObject;
    private TurnManager turnManager;
    private List<Character> testCharacters;

    [SetUp]
    public void Setup()
    {
        turnManagerObject = new GameObject("TestTurnManager");
        turnManager = turnManagerObject.AddComponent<TurnManager>();
        
        testCharacters = new List<Character>();
        
        // Create test characters with different speeds
        for (int i = 0; i < 3; i++)
        {
            GameObject characterObj = new GameObject($"TestCharacter_{i}");
            Character character = characterObj.AddComponent<Character>();
            
            // Different speeds to test turn order
            var stats = new CharacterStats(100, 15, 10, 10 + i * 2, 50);
            var abilities = new CharacterAbility[]
            {
                new CharacterAbility("Test", AbilityType.Attack, 10, 5, "Test")
            };
            
            character.Initialize($"TestChar_{i}", stats, abilities, i == 0); // First is player
            testCharacters.Add(character);
        }
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var character in testCharacters)
        {
            if (character != null && character.gameObject != null)
                Object.DestroyImmediate(character.gameObject);
        }
        
        if (turnManagerObject != null)
            Object.DestroyImmediate(turnManagerObject);
    }

    [Test]
    public void TurnManager_InitializeTurnOrder_SortsBySpeed()
    {
        turnManager.InitializeTurnOrder(testCharacters);
        turnManager.StartNextTurn();
        
        // Highest speed character should go first (TestChar_2 with speed 14)
        Assert.AreEqual("TestChar_2", turnManager.CurrentCharacter.CharacterName);
    }

    [Test]
    public void TurnManager_StartNextTurn_AdvancesToNextCharacter()
    {
        turnManager.InitializeTurnOrder(testCharacters);
        turnManager.StartNextTurn(); // First character
        var firstCharacter = turnManager.CurrentCharacter;
        
        turnManager.StartNextTurn(); // Second character
        var secondCharacter = turnManager.CurrentCharacter;
        
        Assert.AreNotEqual(firstCharacter, secondCharacter);
    }

    [Test]
    public void TurnManager_RemoveCharacter_RemovesFromTurns()
    {
        turnManager.InitializeTurnOrder(testCharacters);
        var characterToRemove = testCharacters[0];
        
        turnManager.RemoveCharacterFromTurns(characterToRemove);
        
        // Start turns and make sure removed character doesn't appear
        for (int i = 0; i < 10; i++)
        {
            turnManager.StartNextTurn();
            if (turnManager.CurrentCharacter != null)
            {
                Assert.AreNotEqual(characterToRemove, turnManager.CurrentCharacter);
            }
        }
    }

    [Test]
    public void TurnManager_ResetTurnManager_ClearsAllData()
    {
        turnManager.InitializeTurnOrder(testCharacters);
        turnManager.StartNextTurn();
        
        turnManager.ResetTurnManager();
        
        Assert.IsNull(turnManager.CurrentCharacter);
    }

    [Test]
    public void TurnManager_IsPlayerTurn_ReflectsCurrentCharacterType()
    {
        turnManager.InitializeTurnOrder(testCharacters);
        
        // Keep advancing until we find a player character
        for (int i = 0; i < 10; i++)
        {
            turnManager.StartNextTurn();
            if (turnManager.CurrentCharacter?.IsPlayerControlled == true)
            {
                Assert.IsTrue(turnManager.IsPlayerTurn);
                return;
            }
        }
        
        // If we get here, no player character was found, which is unexpected
        Assert.Fail("No player character found in turn order");
    }
}