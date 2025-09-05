using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public class TurnManager : MonoBehaviour
    {
        private List<Character> allCharacters = new List<Character>();
        private Queue<Character> turnQueue = new Queue<Character>();
        private Character currentCharacter;

        public event Action<Character> OnTurnStart;
        public event Action<Character> OnTurnEnd;
        public event Action OnRoundComplete;

        public Character CurrentCharacter => currentCharacter;
        public bool IsPlayerTurn => currentCharacter?.IsPlayerControlled ?? false;

        public void InitializeTurnOrder(List<Character> characters)
        {
            allCharacters = characters.Where(c => c.CanAct()).ToList();
            
            // Sort by speed (highest first)
            allCharacters.Sort((a, b) => b.Stats.Speed.CompareTo(a.Stats.Speed));
            
            BuildTurnQueue();
        }

        private void BuildTurnQueue()
        {
            turnQueue.Clear();
            foreach (var character in allCharacters.Where(c => c.CanAct()))
            {
                turnQueue.Enqueue(character);
            }
        }

        public void StartNextTurn()
        {
            // End current turn
            if (currentCharacter != null)
            {
                OnTurnEnd?.Invoke(currentCharacter);
                currentCharacter.ResetForNewTurn();
            }

            // Check if round is complete
            if (turnQueue.Count == 0)
            {
                OnRoundComplete?.Invoke();
                BuildTurnQueue(); // Start new round
            }

            // Get next character
            if (turnQueue.Count > 0)
            {
                currentCharacter = turnQueue.Dequeue();
                
                // Skip dead characters
                while (currentCharacter != null && !currentCharacter.CanAct() && turnQueue.Count > 0)
                {
                    currentCharacter = turnQueue.Dequeue();
                }

                if (currentCharacter?.CanAct() == true)
                {
                    OnTurnStart?.Invoke(currentCharacter);
                }
                else
                {
                    currentCharacter = null;
                }
            }
        }

        public void RemoveCharacterFromTurns(Character character)
        {
            allCharacters.Remove(character);
            
            // Rebuild queue without the removed character
            var remainingInQueue = turnQueue.Where(c => c != character).ToList();
            turnQueue.Clear();
            foreach (var c in remainingInQueue)
            {
                turnQueue.Enqueue(c);
            }
        }

        public void ResetTurnManager()
        {
            allCharacters.Clear();
            turnQueue.Clear();
            currentCharacter = null;
        }
    }
}