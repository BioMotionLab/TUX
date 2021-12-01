using UnityEngine;
using Object = UnityEngine.Object;

namespace bmlTUX {
    public static class TuxLog {
        public const string Prefix = "<b>[bmlTUX]</b>";

        /// <summary>
        /// Formats string as a bmlTUX errror
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void LogError(string message) {
            Debug.LogError(Prefix + $" <color=red>{message}</color>");
        }

        public static void LogError(string message, Object context) {
            Debug.LogError(Prefix + $" <color=red>{message}</color>", context);
        }
        
        /// <summary>
        /// Formats string as a bmlTUX success
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Good(string message) {
            return Prefix + $" <color=green>{message}</color>";
        }

        /// <summary>
        /// Formats string as a bmlTUX warning
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Warn(string message) {
            return Prefix + $" <color=purple>{message}</color>";
        }

        public static void Log(string message, Object context) {
            Debug.Log( $"{Prefix} {message}", context);
        }
        
        public static void Log(string message) {
            Debug.Log($"{Prefix} {message}");
        }

        public static string FormatGreen(string message) {
            return $"<color=green>{message}</color>";
        }

        public static string FormatOrange(string message) {
            return $"<color=orange>{message}</color>";
        }
    }
}
