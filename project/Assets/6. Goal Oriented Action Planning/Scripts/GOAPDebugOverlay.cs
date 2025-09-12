using System.Collections.Generic;
using UnityEngine;

namespace Sammoh.GOAP
{
    /// <summary>
    /// Debug overlay for GOAP system - displays current goal, plan, and agent state
    /// </summary>
    public class GOAPDebugOverlay : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private bool showOverlay = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;
        [SerializeField] private Vector2 overlayPosition = new Vector2(10, 10);
        [SerializeField] private Vector2 overlaySize = new Vector2(400, 600);
        
        private GOAPAgent targetAgent;
        private GUIStyle boxStyle;
        private GUIStyle labelStyle;
        private GUIStyle headerStyle;
        private bool stylesInitialized = false;
        
        public void SetAgent(GOAPAgent agent)
        {
            targetAgent = agent;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showOverlay = !showOverlay;
            }
        }
        
        private void OnGUI()
        {
            if (!showOverlay || targetAgent == null)
                return;
                
            InitializeStyles();
            
            // Main overlay window
            Rect overlayRect = new Rect(overlayPosition.x, overlayPosition.y, overlaySize.x, overlaySize.y);
            GUI.Box(overlayRect, "", boxStyle);
            
            GUILayout.BeginArea(new Rect(overlayRect.x + 10, overlayRect.y + 10, overlayRect.width - 20, overlayRect.height - 20));
            
            DrawHeader();
            DrawCurrentGoalAndPlan();
            DrawAgentState();
            DrawWorldState();
            DrawAvailableGoals();
            DrawControls();
            
            GUILayout.EndArea();
        }
        
        private void InitializeStyles()
        {
            if (stylesInitialized)
                return;
                
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = MakeTexture(1, 1, new Color(0, 0, 0, 0.8f));
            
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontSize = 12;
            
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.normal.textColor = Color.yellow;
            headerStyle.fontSize = 14;
            headerStyle.fontStyle = FontStyle.Bold;
            
            stylesInitialized = true;
        }
        
        private void DrawHeader()
        {
            GUILayout.Label("GOAP Agent Debug", headerStyle);
            GUILayout.Label($"Press {toggleKey} to toggle", labelStyle);
            GUILayout.Space(10);
        }
        
        private void DrawCurrentGoalAndPlan()
        {
            GUILayout.Label("Current Goal & Plan", headerStyle);
            
            var currentGoal = targetAgent.GetCurrentGoal();
            var currentAction = targetAgent.GetCurrentAction();
            var currentPlan = targetAgent.GetCurrentPlan();
            
            GUILayout.Label($"Goal: {currentGoal?.GoalType ?? "None"}", labelStyle);
            GUILayout.Label($"Current Action: {currentAction?.ActionType ?? "None"}", labelStyle);
            
            if (currentAction != null)
            {
                GUILayout.Label($"Action Status: {(currentAction.IsExecuting ? "Executing" : "Idle")}", labelStyle);
            }
            
            if (currentPlan != null)
            {
                GUILayout.Label($"Plan Actions Remaining: {currentPlan.Actions.Count}", labelStyle);
                GUILayout.Label("Plan Steps:", labelStyle);
                for (int i = 0; i < currentPlan.Actions.Count; i++)
                {
                    string prefix = i == 0 ? "→ " : "   ";
                    GUILayout.Label($"{prefix}{currentPlan.Actions[i].ActionType}", labelStyle);
                }
            }
            
            GUILayout.Space(10);
        }
        
        private void DrawAgentState()
        {
            GUILayout.Label("Agent State (Needs)", headerStyle);
            
            var agentState = targetAgent.GetAgentState();
            var needs = agentState.GetAllNeeds();
            
            foreach (var need in needs)
            {
                float percentage = need.Value * 100f;
                Color color = GetNeedColor(need.Value);
                
                // Simple progress bar using GUI elements
                string needDisplay = $"{need.Key}: {percentage:F0}%";
                GUILayout.Label(needDisplay, labelStyle);
                
                Rect barRect = GUILayoutUtility.GetLastRect();
                barRect.x += 100;
                barRect.width = 100;
                barRect.height = 10;
                
                GUI.color = Color.gray;
                GUI.DrawTexture(barRect, Texture2D.whiteTexture);
                
                barRect.width *= need.Value;
                GUI.color = color;
                GUI.DrawTexture(barRect, Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            
            GUILayout.Space(10);
        }
        
        private void DrawWorldState()
        {
            GUILayout.Label("World Facts", headerStyle);
            
            var worldState = targetAgent.GetWorldState();
            var facts = worldState.GetAllFacts();
            
            foreach (var fact in facts)
            {
                Color color = fact.Value ? Color.green : Color.red;
                string status = fact.Value ? "TRUE" : "FALSE";
                
                GUI.color = color;
                GUILayout.Label($"{fact.Key}: {status}", labelStyle);
                GUI.color = Color.white;
            }
            
            GUILayout.Space(10);
        }
        
        private void DrawAvailableGoals()
        {
            GUILayout.Label("Available Goals (Priority)", headerStyle);
            
            var goals = targetAgent.GetAvailableGoals();
            var agentState = targetAgent.GetAgentState();
            var worldState = targetAgent.GetWorldState();
            
            foreach (var goal in goals)
            {
                bool canSatisfy = goal.CanSatisfy(agentState, worldState);
                float priority = canSatisfy ? goal.CalculatePriority(agentState, worldState) : 0f;
                
                Color color = canSatisfy ? Color.white : Color.gray;
                GUI.color = color;
                
                GUILayout.Label($"{goal.GoalType}: {priority:F1}", labelStyle);
                GUI.color = Color.white;
            }
            
            GUILayout.Space(10);
        }
        
        private void DrawControls()
        {
            GUILayout.Label("Controls", headerStyle);
            
            if (GUILayout.Button("Force Replan"))
            {
                targetAgent.ForceReplan();
            }
        }
        
        private Color GetNeedColor(float needValue)
        {
            if (needValue > 0.8f) return Color.red;
            if (needValue > 0.6f) return Color.yellow;
            if (needValue > 0.4f) return Color.green;
            return Color.blue;
        }
        
        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
                
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}