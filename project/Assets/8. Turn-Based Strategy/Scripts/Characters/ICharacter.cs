namespace Sammoh.TurnBasedStrategy
{
    public interface ICharacter
    {
        string CharacterName { get; }
        CharacterStats Stats { get; }
        CharacterAbility[] Abilities { get; }
        bool IsPlayerControlled { get; }
        
        void Initialize(string name, CharacterStats stats, CharacterAbility[] abilities, bool isPlayer = false);
        bool CanAct();
        void ResetForNewTurn();
        void RestoreToFull();
    }
}