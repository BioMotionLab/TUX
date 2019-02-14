namespace BML_Utilities {



    public static class IntExtension {
        public static bool IsWithin(this int value, int minimum, int maximum) {
            return value >= minimum && value <= maximum;
        }
    }
}
