using UnityEngine;
using UnityEditor;

namespace Sammoh.Advertisement.Editor
{
    /// <summary>
    /// Custom editor for BannerContainerConfiguration ScriptableObject.
    /// Provides enhanced UI and validation following the pattern established in Global Data & Editor Tools.
    /// </summary>
    [CustomEditor(typeof(BannerContainerConfiguration))]
    public class BannerContainerConfigurationEditor : UnityEditor.Editor
    {
        private SerializedProperty backgroundColor;
        private SerializedProperty textColor;
        private SerializedProperty customFont;
        private SerializedProperty textAlignment;
        private SerializedProperty enableClickInteraction;
        private SerializedProperty enableSmartResize;
        private SerializedProperty minWidth;
        private SerializedProperty maxWidth;
        private SerializedProperty enableFadeAnimation;
        private SerializedProperty animationDuration;
        private SerializedProperty textTemplate;
        private SerializedProperty fontSize;
        private SerializedProperty respectSafeArea;
        private SerializedProperty padding;
        private SerializedProperty bannerSizeConfigs;
        
        // Preview variables
        private bool showPreview = false;
        private BannerSize previewSize = BannerSize.Standard;
        private BannerPosition previewPosition = BannerPosition.Bottom;
        
        void OnEnable()
        {
            // Cache serialized properties
            backgroundColor = serializedObject.FindProperty("backgroundColor");
            textColor = serializedObject.FindProperty("textColor");
            customFont = serializedObject.FindProperty("customFont");
            textAlignment = serializedObject.FindProperty("textAlignment");
            enableClickInteraction = serializedObject.FindProperty("enableClickInteraction");
            enableSmartResize = serializedObject.FindProperty("enableSmartResize");
            minWidth = serializedObject.FindProperty("minWidth");
            maxWidth = serializedObject.FindProperty("maxWidth");
            enableFadeAnimation = serializedObject.FindProperty("enableFadeAnimation");
            animationDuration = serializedObject.FindProperty("animationDuration");
            textTemplate = serializedObject.FindProperty("textTemplate");
            fontSize = serializedObject.FindProperty("fontSize");
            respectSafeArea = serializedObject.FindProperty("respectSafeArea");
            padding = serializedObject.FindProperty("padding");
            bannerSizeConfigs = serializedObject.FindProperty("bannerSizeConfigs");
        }
        
        public override void OnInspectorGUI()
        {
            BannerContainerConfiguration config = (BannerContainerConfiguration)target;
            
            serializedObject.Update();
            
            // Header
            EditorGUILayout.Space();
            GUILayout.Label("Banner Container Configuration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure banner ad container appearance, behavior and styling. " +
                                   "This ScriptableObject provides a user-friendly way to customize banners without code changes.", 
                                   MessageType.Info);
            
            EditorGUILayout.Space();
            
            // Visual Appearance Section
            DrawSection("Visual Appearance", () =>
            {
                EditorGUILayout.PropertyField(backgroundColor, new GUIContent("Background Color", 
                    "The background color of the banner container"));
                
                EditorGUILayout.PropertyField(textColor, new GUIContent("Text Color", 
                    "The color of text displayed on the banner"));
                
                EditorGUILayout.PropertyField(customFont, new GUIContent("Custom Font", 
                    "Optional custom font. If null, Unity's default font will be used"));
                
                EditorGUILayout.PropertyField(textAlignment, new GUIContent("Text Alignment", 
                    "How text should be aligned within the banner"));
                
                EditorGUILayout.PropertyField(fontSize, new GUIContent("Font Size", 
                    "The size of the banner text"));
                
                EditorGUILayout.PropertyField(textTemplate, new GUIContent("Text Template", 
                    "Template for banner text. Use {size} and {position} as placeholders"));
            });
            
            // Behavior Section
            DrawSection("Container Behavior", () =>
            {
                EditorGUILayout.PropertyField(enableClickInteraction, new GUIContent("Enable Click Interaction", 
                    "Whether users can click on the banner"));
                
                EditorGUILayout.PropertyField(enableSmartResize, new GUIContent("Enable Smart Resize", 
                    "Automatically resize banner based on screen dimensions"));
                
                if (enableSmartResize.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(minWidth, new GUIContent("Min Width", 
                        "Minimum width for smart resize"));
                    
                    EditorGUILayout.PropertyField(maxWidth, new GUIContent("Max Width", 
                        "Maximum width for smart resize"));
                    
                    // Validation warning
                    if (minWidth.floatValue >= maxWidth.floatValue)
                    {
                        EditorGUILayout.HelpBox("Min Width must be less than Max Width!", MessageType.Warning);
                    }
                    EditorGUI.indentLevel--;
                }
            });
            
            // Animation Section
            DrawSection("Animation Settings", () =>
            {
                EditorGUILayout.PropertyField(enableFadeAnimation, new GUIContent("Enable Fade Animation", 
                    "Enable fade-in/fade-out when showing/hiding banners"));
                
                if (enableFadeAnimation.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(animationDuration, new GUIContent("Animation Duration", 
                        "Duration of fade animations in seconds"));
                    EditorGUI.indentLevel--;
                }
            });
            
            // Safe Area & Padding Section  
            DrawSection("Safe Area & Padding", () =>
            {
                EditorGUILayout.PropertyField(respectSafeArea, new GUIContent("Respect Safe Area", 
                    "Account for device safe areas (notches, home indicators)"));
                
                EditorGUILayout.PropertyField(padding, new GUIContent("Padding", 
                    "Additional padding from screen edges"));
            });
            
            // Banner Size Configurations
            DrawSection("Banner Size Configurations", () =>
            {
                EditorGUILayout.HelpBox("Configure dimensions for each banner size type. " +
                                       "For Smart banners, width of 0 means auto-calculate based on screen size.", 
                                       MessageType.Info);
                
                EditorGUILayout.PropertyField(bannerSizeConfigs, new GUIContent("Size Configurations"), true);
            });
            
            // Preview Section
            DrawPreviewSection(config);
            
            // Action Buttons
            EditorGUILayout.Space();
            DrawActionButtons(config);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawSection(string title, System.Action content)
        {
            EditorGUILayout.Space();
            GUILayout.Label(title, EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginVertical("box");
            content();
            EditorGUILayout.EndVertical();
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
        
        private void DrawPreviewSection(BannerContainerConfiguration config)
        {
            EditorGUILayout.Space();
            
            showPreview = EditorGUILayout.Foldout(showPreview, "Preview", true);
            
            if (showPreview)
            {
                EditorGUILayout.BeginVertical("box");
                
                // Preview controls
                previewSize = (BannerSize)EditorGUILayout.EnumPopup("Preview Size", previewSize);
                previewPosition = (BannerPosition)EditorGUILayout.EnumPopup("Preview Position", previewPosition);
                
                EditorGUILayout.Space();
                
                // Preview information
                Vector2 dimensions = config.GetDimensionsForSize(previewSize);
                string formattedText = config.GetFormattedText(previewSize, previewPosition);
                
                EditorGUILayout.LabelField("Dimensions:", $"{dimensions.x} x {dimensions.y}");
                EditorGUILayout.LabelField("Preview Text:", formattedText);
                
                // Color preview
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Background Color:");
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(50, 20), config.BackgroundColor);
                EditorGUILayout.LabelField("Text Color:");
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(50, 20), config.TextColor);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
        }
        
        private void DrawActionButtons(BannerContainerConfiguration config)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Validate Configuration button
            if (GUILayout.Button("Validate Configuration"))
            {
                bool isValid = config.ValidateConfiguration();
                if (isValid)
                {
                    EditorUtility.DisplayDialog("Validation Result", 
                        "Configuration is valid! ✓", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Validation Result", 
                        "Configuration has issues. Check the Console for details.", "OK");
                }
            }
            
            // Reset to Defaults button
            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset Configuration", 
                    "Are you sure you want to reset all settings to default values? This action cannot be undone.", 
                    "Reset", "Cancel"))
                {
                    config.ResetToDefaults();
                    EditorUtility.SetDirty(config);
                    serializedObject.Update();
                }
            }
            
            // Create Test Banner button (in future this could create a test banner in scene)
            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Test in Scene"))
            {
                EditorUtility.DisplayDialog("Test Banner", 
                    "Test banner functionality will be available when the game is running. " +
                    "This feature requires runtime testing in the Advertisement Demo scene.", "OK");
            }
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}