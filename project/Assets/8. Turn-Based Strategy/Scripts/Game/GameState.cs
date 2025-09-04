using System;

namespace Sammoh.TurnBasedStrategy
{
    public enum GameState
    {
        NotStarted,
        PlayerTurn,
        EnemyTurn,
        GameOver,
        Victory
    }

    [Serializable]
    public class GameStateData
    {
        public GameState currentState;
        public int turnNumber;
        public int playerTeamAlive;
        public int enemyTeamAlive;
        
        public GameStateData()
        {
            currentState = GameState.NotStarted;
            turnNumber = 0;
            playerTeamAlive = 0;
            enemyTeamAlive = 0;
        }
        
        public void Reset()
        {
            currentState = GameState.NotStarted;
            turnNumber = 0;
            playerTeamAlive = 0;
            enemyTeamAlive = 0;
        }
    }
}