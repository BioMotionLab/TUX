using System;

namespace BML_Utilities {



    public static class StringExtension {
        const string Ellipses = "..";

        public static string Truncate(this string text, int length) {
            if (text == null) {
                return null;
            }


            if (text.Length > length) {
                return length < 0 ? "" : text.Substring(0, Math.Min(text.Length, length - Ellipses.Length)) + Ellipses;
            }

            return text;


        }
    }
}
