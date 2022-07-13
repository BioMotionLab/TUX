using UnityEditor;

namespace bmlTUX.UI.EditorUI
{
    /// <summary>
    /// Originally contributed by GitHub user A-Ivan and modified by Adam Bebko
    /// </summary>
    public static class SetProjectCompatibilitySettingsMenuItem
    {
        /// <summary>
        /// Sets the Build Target Group to be the current build target group (required) and the API Compatibility Level to be .NET 4.6
        /// </summary>
        [MenuItem("bmlTUX/Set project compatibility settings (Runtime & API)")]
        static void SetProjectCompatibilitySettings()
        {
            
            PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, ApiCompatibilityLevel.NET_4_6);
            TuxLog.Good("API Compatibility set to .NET 4.x");
            
#if UNITY_2019
            // Only set the Scripting Runtime Version if the Unity version is 2019 (as scripting runtime version was deprecated as of 2020)
            PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
            TuxLog.Good("API Compatibility set to Latest");
#endif            
            
            
        }
    }
    
}
