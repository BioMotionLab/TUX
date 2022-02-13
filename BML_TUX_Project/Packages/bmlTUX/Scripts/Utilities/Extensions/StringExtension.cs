using System;

namespace bmlTUX {

    public static class StringExtension {
        const string Ellipses = "..";

        public static string Truncate(this string text, int toLength, bool ellipses = false) {
            if (text == null) return null;
            
            if (toLength < 0) throw new ArgumentException("Truncate Length cannot be negative");

            if (text.Length <= toLength) return text;

            if (!ellipses) return text.Substring(0, toLength);

            string truncatedString = text.Substring(0, toLength - Ellipses.Length);
            return truncatedString + Ellipses;

        }
        
    }
}
