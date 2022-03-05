using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace bmlTUX
{
  
// Create a new type of Settings Asset.
    public class BmlTuxEditorSettings : ScriptableObject
    {
        public const string MyCustomSettingsPath = "Assets/ﻩbmlTUX_Settings/bmlTUX_EditorSettings.asset";

        [SerializeField] public Color GoodColor = Color.green;
        [SerializeField] public Color BadColor = Color.red;
        [SerializeField] public Color WarnColor = Color.magenta;

        public static BmlTuxEditorSettings GetOrCreateSettings()
        {
            BmlTuxEditorSettings settings;
#if UNITY_EDITOR
            settings = AssetDatabase.LoadAssetAtPath<BmlTuxEditorSettings>(MyCustomSettingsPath);
            if (settings == null)
            {
                settings = CreateInstance<BmlTuxEditorSettings>();
                Directory.CreateDirectory(Path.GetDirectoryName(MyCustomSettingsPath));
                AssetDatabase.CreateAsset(settings, MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }

#else 
            //use default outside editor because it does not matter
            settings = CreateInstance<MyCustomSettings>();
#endif
            return settings;
        }

        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
    
    // Register a SettingsProvider using IMGUI for the drawing framework:
}
