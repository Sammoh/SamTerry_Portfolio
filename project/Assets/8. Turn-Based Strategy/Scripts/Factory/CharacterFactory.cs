using UnityEngine;

namespace Sammoh.TurnBasedStrategy
{
    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Healer
    }

    public class CharacterFactory : MonoBehaviour
    {
        [SerializeField] private GameObject characterPrefab;

        public Character CreateCharacter(CharacterClass characterClass, string name, bool isPlayerControlled = false, Transform parent = null)
        {
            // Create the character GameObject
            GameObject characterObj = characterPrefab != null 
                ? Instantiate(characterPrefab, parent) 
                : new GameObject($"Character_{name}");

            if (parent != null)
                characterObj.transform.SetParent(parent);

            // Add Character component if it doesn't exist
            Character character = characterObj.GetComponent<Character>();
            if (character == null)
                character = characterObj.AddComponent<Character>();

            // Configure stats and abilities based on class
            CharacterStats stats = CreateStatsForClass(characterClass);
            CharacterAbility[] abilities = CreateAbilitiesForClass(characterClass);

            // Initialize the character
            character.Initialize(name, stats, abilities, isPlayerControlled, characterClass);
            characterObj.name = name;

            // Equip default equipment based on class
            EquipDefaultEquipment(character, characterClass);

            return character;
        }

        private CharacterStats CreateStatsForClass(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return new CharacterStats(health: 120, attack: 20, defense: 15, speed: 8, mana: 30);

                case CharacterClass.Mage:
                    return new CharacterStats(health: 80, attack: 10, defense: 5, speed: 10, mana: 80);

                case CharacterClass.Rogue:
                    return new CharacterStats(health: 90, attack: 18, defense: 8, speed: 16, mana: 40);

                case CharacterClass.Healer:
                    return new CharacterStats(health: 100, attack: 8, defense: 12, speed: 12, mana: 70);

                default:
                    return new CharacterStats(health: 100, attack: 15, defense: 10, speed: 12, mana: 50);
            }
        }

        private CharacterAbility[] CreateAbilitiesForClass(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return new CharacterAbility[]
                    {
                        new CharacterAbility("Slash", AbilityType.Attack, 15, 5, "A powerful sword attack"),
                        new CharacterAbility("Shield Block", AbilityType.Defend, 10, 8, "Reduces incoming damage"),
                        new CharacterAbility("Berserker Rage", AbilityType.Special, 25, 15, "Devastating attack with high mana cost")
                    };

                case CharacterClass.Mage:
                    return new CharacterAbility[]
                    {
                        new CharacterAbility("Magic Missile", AbilityType.Attack, 12, 10, "A magical projectile"),
                        new CharacterAbility("Healing Light", AbilityType.Heal, 20, 15, "Restores health"),
                        new CharacterAbility("Fireball", AbilityType.Special, 30, 25, "Explosive fire magic")
                    };

                case CharacterClass.Rogue:
                    return new CharacterAbility[]
                    {
                        new CharacterAbility("Backstab", AbilityType.Attack, 18, 8, "Quick strike from behind"),
                        new CharacterAbility("Stealth", AbilityType.Defend, 5, 12, "Become harder to hit"),
                        new CharacterAbility("Poison Blade", AbilityType.Special, 22, 18, "Attack with poison damage")
                    };

                case CharacterClass.Healer:
                    return new CharacterAbility[]
                    {
                        new CharacterAbility("Staff Strike", AbilityType.Attack, 8, 3, "Basic melee attack"),
                        new CharacterAbility("Greater Heal", AbilityType.Heal, 35, 20, "Powerful healing magic"),
                        new CharacterAbility("Divine Protection", AbilityType.Defend, 15, 15, "Magical protection")
                    };

                default:
                    return new CharacterAbility[]
                    {
                        new CharacterAbility("Basic Attack", AbilityType.Attack, 10, 0, "Simple attack"),
                        new CharacterAbility("Rest", AbilityType.Heal, 5, 0, "Recover small amount of health")
                    };
            }
        }

        private void EquipDefaultEquipment(Character character, CharacterClass characterClass)
        {
            Equipment defaultWeapon = CreateDefaultWeapon(characterClass);
            Equipment defaultArmor = CreateDefaultArmor(characterClass);
            
            if (defaultWeapon != null)
                character.EquipItem(defaultWeapon);
                
            if (defaultArmor != null)
                character.EquipItem(defaultArmor);
        }

        private Equipment CreateDefaultWeapon(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return new Equipment(
                        "Iron Sword", 
                        "A sturdy iron sword", 
                        EquipmentSlot.Weapon, 
                        EquipmentRarity.Common,
                        new EquipmentStats(0, 5, 0, 0, 0)
                    );

                case CharacterClass.Mage:
                    return new Equipment(
                        "Oak Staff", 
                        "A magical oak staff", 
                        EquipmentSlot.Weapon, 
                        EquipmentRarity.Common,
                        new EquipmentStats(0, 2, 0, 0, 10)
                    );

                case CharacterClass.Rogue:
                    return new Equipment(
                        "Steel Dagger", 
                        "A sharp steel dagger", 
                        EquipmentSlot.Weapon, 
                        EquipmentRarity.Common,
                        new EquipmentStats(0, 4, 0, 2, 0)
                    );

                case CharacterClass.Healer:
                    return new Equipment(
                        "Healing Rod", 
                        "A rod imbued with healing magic", 
                        EquipmentSlot.Weapon, 
                        EquipmentRarity.Common,
                        new EquipmentStats(5, 1, 2, 0, 8)
                    );

                default:
                    return null;
            }
        }

        private Equipment CreateDefaultArmor(CharacterClass characterClass)
        {
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    return new Equipment(
                        "Leather Armor", 
                        "Basic leather protection", 
                        EquipmentSlot.Armor, 
                        EquipmentRarity.Common,
                        new EquipmentStats(15, 0, 8, -1, 0)
                    );

                case CharacterClass.Mage:
                    return new Equipment(
                        "Mage Robes", 
                        "Robes that enhance magical power", 
                        EquipmentSlot.Armor, 
                        EquipmentRarity.Common,
                        new EquipmentStats(5, 0, 2, 1, 15)
                    );

                case CharacterClass.Rogue:
                    return new Equipment(
                        "Studded Leather", 
                        "Light armor for mobility", 
                        EquipmentSlot.Armor, 
                        EquipmentRarity.Common,
                        new EquipmentStats(8, 0, 4, 3, 0)
                    );

                case CharacterClass.Healer:
                    return new Equipment(
                        "Blessed Garments", 
                        "Garments blessed by divine power", 
                        EquipmentSlot.Armor, 
                        EquipmentRarity.Common,
                        new EquipmentStats(10, 0, 5, 0, 10)
                    );

                default:
                    return null;
            }
        }
    }
}