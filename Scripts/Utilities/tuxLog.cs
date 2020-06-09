namespace bmlTUX.Scripts.Utilities {
    public static class TuxLog {
        public const string Prefix = "<b>[bmlTUX]</b>";

        public static string Error(string message) {
            return Prefix + $" <color=red>{message}</color>";
        }

        public static string Good(string message) {
            return Prefix + $" <color=green>{message}</color>";
        }

        public static string Warn(string message) {
            return Prefix + $" <color=purple>{message}</color>";
        }
    }
}
