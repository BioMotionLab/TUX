using System.Collections.Generic;
using UnityEditor;

namespace bmlTUX.Editor
{
    static class BmlTuxEditorSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            SettingsProvider provider = new SettingsProvider("Project/bmlTUX", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "bmlTUX",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    SerializedObject settings = BmlTuxEditorSettings.GetSerializedSettings();
                    EditorGUILayout.LabelField("Debug Log Coloring", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(BmlTuxEditorSettings.GoodColor)));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(BmlTuxEditorSettings.BadColor)));
                    EditorGUILayout.PropertyField(settings.FindProperty(nameof(BmlTuxEditorSettings.WarnColor)));
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] {"Good Color", "Bad Color", "Warn Color" })
            };

            return provider;
        }
        
        
       
    }
}