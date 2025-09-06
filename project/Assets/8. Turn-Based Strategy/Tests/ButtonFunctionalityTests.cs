using NUnit.Framework;
using UnityEngine;
using Sammoh.TurnBasedStrategy;
using System.Collections.Generic;
using System.Linq;

public class ButtonFunctionalityTests
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
    public void PlayerAttackAbility_GetValidTargets_ReturnsEnemies()
    {
        gameManager.StartNewGame();
        
        // Force current character to be a player
        var playerCharacter = gameManager.PlayerTeam.First();
        SetCurrentCharacter(playerCharacter);
        
        var attackAbility = new CharacterAbility("Player Attack", AbilityType.Attack, 10, 5, "Test");
        var targets = gameManager.GetValidTargets(attackAbility);
        
        Assert.Greater(targets.Count, 0, "Player should have enemy targets for attack abilities");
        foreach (var target in targets)
        {
            Assert.IsFalse(target.IsPlayerControlled, "Player attack targets should be enemies");
            Assert.IsTrue(target.CanAct(), "Targets should be alive");
        }
    }

    [Test]
    public void PlayerHealAbility_GetValidTargets_ReturnsPlayers()
    {
        gameManager.StartNewGame();
        
        // Force current character to be a player
        var playerCharacter = gameManager.PlayerTeam.First();
        SetCurrentCharacter(playerCharacter);
        
        var healAbility = new CharacterAbility("Player Heal", AbilityType.Heal, 20, 10, "Test");
        var targets = gameManager.GetValidTargets(healAbility);
        
        Assert.Greater(targets.Count, 0, "Player should have player targets for heal abilities");
        foreach (var target in targets)
        {
            Assert.IsTrue(target.IsPlayerControlled, "Heal targets should be players");
            Assert.IsTrue(target.CanAct(), "Targets should be alive");
        }
    }

    [Test]
    public void PlayerAttackAbility_UseAbility_DamagesEnemyTarget()
    {
        gameManager.StartNewGame();
        
        // Force current character to be a player
        var playerCharacter = gameManager.PlayerTeam.First();
        SetCurrentCharacter(playerCharacter);
        
        var enemyTarget = gameManager.EnemyTeam.First();
        var originalHealth = enemyTarget.Stats.CurrentHealth;
        
        var attackAbility = new CharacterAbility("Player Attack", AbilityType.Attack, 10, 5, "Test");
        bool success = gameManager.UseAbility(attackAbility, enemyTarget);
        
        Assert.IsTrue(success, "Ability should execute successfully");
        Assert.Less(enemyTarget.Stats.CurrentHealth, originalHealth, "Enemy should take damage");
    }

    [Test]
    public void PlayerHealAbility_UseAbility_HealsTarget()
    {
        gameManager.StartNewGame();
        
        // Force current character to be a player with damaged health
        var playerCharacter = gameManager.PlayerTeam.First();
        SetCurrentCharacter(playerCharacter);
        
        // Damage the target first
        var healTarget = gameManager.PlayerTeam.Last();
        healTarget.Stats.TakeDamage(20);
        var damagedHealth = healTarget.Stats.CurrentHealth;
        
        var healAbility = new CharacterAbility("Player Heal", AbilityType.Heal, 15, 5, "Test");
        bool success = gameManager.UseAbility(healAbility, healTarget);
        
        Assert.IsTrue(success, "Heal ability should execute successfully");
        Assert.Greater(healTarget.Stats.CurrentHealth, damagedHealth, "Target should be healed");
    }

    // Helper method to set current character using reflection
    private void SetCurrentCharacter(Character character)
    {
        // Get the turn manager and set the current character
        var currentCharacterField = typeof(TurnManager).GetField("currentCharacter", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        currentCharacterField?.SetValue(turnManager, character);
    }
}