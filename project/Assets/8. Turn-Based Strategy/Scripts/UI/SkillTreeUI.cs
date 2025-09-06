using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sammoh.TurnBasedStrategy
{
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject skillTreePanel;
        [SerializeField] private Button openSkillTreeButton;
        [SerializeField] private Button closeSkillTreeButton;

        [Header("Current Abilities")]
        [SerializeField] private Transform currentAbilitiesParent;
        [SerializeField] private GameObject currentAbilityButtonPrefab;

        [Header("Available Abilities")]
        [SerializeField] private Transform availableAbilitiesParent;
        [SerializeField] private GameObject availableAbilityButtonPrefab;

        [Header("Replacement UI")]
        [SerializeField] private GameObject replacementPanel;
        [SerializeField] private Button replaceButton;
        [SerializeField] private Button cancelReplaceButton;
        [SerializeField] private TextMeshProUGUI firstSelectionText;
        [SerializeField] private TextMeshProUGUI secondSelectionText;
        [SerializeField] private TextMeshProUGUI comparisonText;

        private Character currentCharacter;
        private CharacterAbility selectedCurrentAbility;
        private CharacterAbility selectedReplacementAbility;
        private int selectedAbilitySlot = -1;

        private List<Button> currentAbilityButtons = new List<Button>();
        private List<Button> availableAbilityButtons = new List<Button>();

        private void Awake()
        {
            SetupButtons();
            HideSkillTree();
            HideReplacementPanel();
        }

        private void SetupButtons()
        {
            if (openSkillTreeButton != null)
                openSkillTreeButton.onClick.AddListener(ShowSkillTree);

            if (closeSkillTreeButton != null)
                closeSkillTreeButton.onClick.AddListener(HideSkillTree);

            if (replaceButton != null)
                replaceButton.onClick.AddListener(ConfirmReplacement);

            if (cancelReplaceButton != null)
                cancelReplaceButton.onClick.AddListener(CancelReplacement);
        }

        public void SetCharacter(Character character)
        {
            currentCharacter = character;
            if (skillTreePanel != null && skillTreePanel.activeInHierarchy)
            {
                RefreshSkillTree();
            }
        }

        public void ShowSkillTree()
        {
            if (currentCharacter == null || skillTreePanel == null) return;

            skillTreePanel.SetActive(true);
            RefreshSkillTree();
            HideReplacementPanel();
        }

        public void HideSkillTree()
        {
            if (skillTreePanel != null)
                skillTreePanel.SetActive(false);
            HideReplacementPanel();
        }

        private void RefreshSkillTree()
        {
            RefreshCurrentAbilities();
            ClearAvailableAbilities(); // Clear until an ability is selected
        }

        private void RefreshCurrentAbilities()
        {
            ClearCurrentAbilities();

            if (currentCharacter?.SkillTree?.CurrentAbilities == null || currentAbilitiesParent == null)
                return;

            var currentAbilities = currentCharacter.SkillTree.CurrentAbilities;
            for (int i = 0; i < currentAbilities.Length; i++)
            {
                var ability = currentAbilities[i];
                if (ability != null)
                {
                    CreateCurrentAbilityButton(ability, i);
                }
            }
        }

        private void CreateCurrentAbilityButton(CharacterAbility ability, int slotIndex)
        {
            if (currentAbilityButtonPrefab == null) return;

            GameObject buttonObj = Instantiate(currentAbilityButtonPrefab, currentAbilitiesParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = $"{ability.AbilityName}\nPower: {ability.Power}\nMana: {ability.ManaCost}";
            }

            button.onClick.AddListener(() => SelectCurrentAbility(ability, slotIndex));
            currentAbilityButtons.Add(button);
        }

        private void SelectCurrentAbility(CharacterAbility ability, int slotIndex)
        {
            selectedCurrentAbility = ability;
            selectedAbilitySlot = slotIndex;
            
            // Determine category based on slot index
            AbilityCategory category = (AbilityCategory)slotIndex;
            RefreshAvailableAbilities(category);
            
            UpdateSelectionDisplay();
        }

        private void RefreshAvailableAbilities(AbilityCategory category)
        {
            ClearAvailableAbilities();

            if (currentCharacter?.SkillTree == null || availableAbilitiesParent == null)
                return;

            var availableNodes = currentCharacter.SkillTree.GetAvailableNodesForCategory(category);
            
            foreach (var node in availableNodes)
            {
                // Don't show the currently equipped ability in the available list
                if (node.Ability != selectedCurrentAbility)
                {
                    CreateAvailableAbilityButton(node.Ability);
                }
            }
        }

        private void CreateAvailableAbilityButton(CharacterAbility ability)
        {
            if (availableAbilityButtonPrefab == null) return;

            GameObject buttonObj = Instantiate(availableAbilityButtonPrefab, availableAbilitiesParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = $"{ability.AbilityName}\nPower: {ability.Power}\nMana: {ability.ManaCost}\n{ability.Description}";
            }

            button.onClick.AddListener(() => SelectReplacementAbility(ability));
            availableAbilityButtons.Add(button);
        }

        private void SelectReplacementAbility(CharacterAbility ability)
        {
            selectedReplacementAbility = ability;
            UpdateSelectionDisplay();
            ShowReplacementPanel();
        }

        private void UpdateSelectionDisplay()
        {
            if (firstSelectionText != null && selectedCurrentAbility != null)
            {
                firstSelectionText.text = $"Current: {selectedCurrentAbility.AbilityName}\nPower: {selectedCurrentAbility.Power}\nMana: {selectedCurrentAbility.ManaCost}";
            }

            if (secondSelectionText != null && selectedReplacementAbility != null)
            {
                secondSelectionText.text = $"New: {selectedReplacementAbility.AbilityName}\nPower: {selectedReplacementAbility.Power}\nMana: {selectedReplacementAbility.ManaCost}";
            }

            if (comparisonText != null && selectedCurrentAbility != null && selectedReplacementAbility != null)
            {
                int powerDiff = selectedReplacementAbility.Power - selectedCurrentAbility.Power;
                int manaDiff = selectedReplacementAbility.ManaCost - selectedCurrentAbility.ManaCost;
                
                comparisonText.text = $"Power: {(powerDiff >= 0 ? "+" : "")}{powerDiff}\nMana: {(manaDiff >= 0 ? "+" : "")}{manaDiff}";
            }
        }

        private void ShowReplacementPanel()
        {
            if (replacementPanel != null)
                replacementPanel.SetActive(true);
        }

        private void HideReplacementPanel()
        {
            if (replacementPanel != null)
                replacementPanel.SetActive(false);
            
            selectedReplacementAbility = null;
            if (secondSelectionText != null)
                secondSelectionText.text = "";
            if (comparisonText != null)
                comparisonText.text = "";
        }

        private void ConfirmReplacement()
        {
            if (currentCharacter?.SkillTree != null && selectedReplacementAbility != null && selectedAbilitySlot >= 0)
            {
                currentCharacter.SkillTree.ReplaceAbility(selectedAbilitySlot, selectedReplacementAbility);
                RefreshSkillTree();
                HideReplacementPanel();
                ClearSelection();
            }
        }

        private void CancelReplacement()
        {
            HideReplacementPanel();
            ClearSelection();
        }

        private void ClearSelection()
        {
            selectedCurrentAbility = null;
            selectedReplacementAbility = null;
            selectedAbilitySlot = -1;
            
            if (firstSelectionText != null)
                firstSelectionText.text = "";
            if (secondSelectionText != null)
                secondSelectionText.text = "";
            if (comparisonText != null)
                comparisonText.text = "";
            
            ClearAvailableAbilities();
        }

        private void ClearCurrentAbilities()
        {
            foreach (var button in currentAbilityButtons)
            {
                if (button != null && button.gameObject != null)
                    Destroy(button.gameObject);
            }
            currentAbilityButtons.Clear();
        }

        private void ClearAvailableAbilities()
        {
            foreach (var button in availableAbilityButtons)
            {
                if (button != null && button.gameObject != null)
                    Destroy(button.gameObject);
            }
            availableAbilityButtons.Clear();
        }

        private void OnDestroy()
        {
            ClearCurrentAbilities();
            ClearAvailableAbilities();
        }
    }
}