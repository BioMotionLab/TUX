namespace BML_Utilities {
    
    public static class LowestCommon {
        public static int GreatestCommonFactor(int a, int b) {
            while (b != 0) {
                int temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        public static int Multiple(int a, int b) {
            return (a / GreatestCommonFactor(a, b)) * b;
        }
    }
}
