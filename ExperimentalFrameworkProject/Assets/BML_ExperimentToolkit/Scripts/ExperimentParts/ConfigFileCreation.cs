using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    /// <summary>
    /// Adds a menu item to create a new config file
    /// </summary>
    public class ConfigFileCreation {
        [MenuItem("BML/Create Experimental Experiment Config File in Assets Folder")]
        public static void CreateMyAsset() {
            ConfigDesignFile asset = ScriptableObject.CreateInstance<ConfigDesignFile>();

            AssetDatabase.CreateAsset(asset, "Assets/New Experiment Config File.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}