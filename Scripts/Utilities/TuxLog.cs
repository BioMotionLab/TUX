using UnityEditor.Graphs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace bmlTUX {
    public static class TuxLog {
        public const string Prefix = "<b>[bmlTUX]</b>";

        public static string HexFromColor(Color color)
        {
            string hexColor = $"#{ColorUtility.ToHtmlStringRGB(color)}";
            return hexColor;
        }

        static BmlTuxEditorSettings bmlTuxEditorSettings;
        static BmlTuxEditorSettings BmlTuxEditorSettings {
            get
            {
                bmlTuxEditorSettings ??= BmlTuxEditorSettings.GetOrCreateSettings();
                return bmlTuxEditorSettings;
            }
        }
        
        /// <summary>
        /// Formats string as a bmlTUX errror
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void LogError(string message) {
            Debug.LogError(Prefix + $" <color={HexFromColor(BmlTuxEditorSettings.BadColor)}>{message}</color>");
        }

        public static void LogError(string message, Object context) {
            Debug.LogError(Prefix + $" <color={HexFromColor(BmlTuxEditorSettings.BadColor)}>{message}</color>", context);
        }
        
        /// <summary>
        /// Formats string as a bmlTUX success
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Good(string message) {
            return Prefix + $" <color={HexFromColor(BmlTuxEditorSettings.GoodColor)}>{message}</color>";
        }

        /// <summary>
        /// Formats string as a bmlTUX warning
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Warn(string message) {
            return Prefix + $" <color={HexFromColor(BmlTuxEditorSettings.WarnColor)}>{message}</color>";
        }

        public static void Log(string message, Object context) {
            Debug.Log( $"{Prefix} {message}", context);
        }
        
        public static void Log(string message) {
            Debug.Log($"{Prefix} {message}");
        }
        
    }
}
