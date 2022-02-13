using JetBrains.Annotations;

namespace bmlTUX {
    
    public static class Delimiter {
        [PublicAPI]
        public const string Comma = ",";
        
        [PublicAPI]
        public const string CommaWithSpace = ", ";
        
        [PublicAPI]
        public const string Tab   = "\t";

        public const string Period = ".";
    }
}