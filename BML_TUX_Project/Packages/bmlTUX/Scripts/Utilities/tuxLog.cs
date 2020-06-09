using System;

namespace bmlTUX.Scripts.Utilities {
    public static class TuxLog {
        public const string Prefix = "<b>[bmlTUX]</b>";

        /// <summary>
        /// Formats string as a bmlTUX errror
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Error(string message) {
            return Prefix + $" <color=red>{message}</color>";
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
        
    }
}
