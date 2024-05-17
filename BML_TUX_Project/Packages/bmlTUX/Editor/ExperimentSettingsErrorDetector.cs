using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace bmlTUX.Editor
{
    public static class ExperimentSettingsErrorDetector
    {

        const string DefaultFileLocationSettingsName = "DefaultFileLocationSettings";

        static FileLocationSettings defaultFileLocationSettingsLoaded;
        
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            List<FileLocationSettings> allFileLocationSettings = ScriptableObjectUtilities
                .GetAllInstancesInProject<FileLocationSettings>();
            FileLocationSettings defaultFileLocationSettings =
                allFileLocationSettings.FirstOrDefault(file => file.name == DefaultFileLocationSettingsName);

            if (defaultFileLocationSettings == null)
            {
                TuxLog.LogError($"No default FileLocationSettings found named {DefaultFileLocationSettingsName}");
                return;
            }
            defaultFileLocationSettingsLoaded = defaultFileLocationSettings;

            CheckForEmptyFileSettings();
        }
        
        static void CheckForEmptyFileSettings()
        {
            List<ExperimentSettings> allExperimentSettings = ScriptableObjectUtilities
                .GetAllInstancesInProject<ExperimentSettings>();
            
            foreach (ExperimentSettings experimentSettingsFile in allExperimentSettings)
            {
                if (experimentSettingsFile.FileLocationSettings != null) continue;
                
                if (EditorUtility.DisplayDialog(
                        $"bmlTUX: Missing {nameof(FileLocationSettings)} in ExperimentSettings file named\n \n{experimentSettingsFile.name}",
                        
                        "BmlTUX has changed how it handles file settings, " +
                        $"and has detected an old ExperimentSettings file with no file settings defined." +
                        $"\n \n Path: {AssetDatabase.GetAssetPath(experimentSettingsFile)}",
                        
                        "Use default settings",
                        "I will update this myself"))
                {
                    experimentSettingsFile.FileLocationSettings = defaultFileLocationSettingsLoaded;

                }
            }
        }
    }
}
