using UnityEditor;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.ExperimentParts {
    /// <summary>
    /// Adds a menu item to create a new config file
    /// </summary>
    public class MakeConfigObject {
        [MenuItem("BML/Create Experimental Experiment Config File in Assets Folder")]
        public static void CreateMyAsset() {
            ExperimentConfig asset = ScriptableObject.CreateInstance<ExperimentConfig>();

            AssetDatabase.CreateAsset(asset, "Assets/New Experiment Config File.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}