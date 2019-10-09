using System;

namespace BML_Utilities.Extensions {

    public static class IntExtension {
        public static bool IsWithin(this int value, int firstInt, int secondInt) {
            int min = Math.Min(firstInt, secondInt);
            int max = Math.Max(firstInt, secondInt);
            return value >= min && value <= max;
        }
    }
}
