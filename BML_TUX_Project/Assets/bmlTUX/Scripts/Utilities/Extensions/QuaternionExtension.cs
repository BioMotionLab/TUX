using UnityEngine;

namespace bmlTUX.Scripts.Utilities.Extensions {
    
    public static class QuaternionExtension {
        public static string ToPreciseString(this Quaternion v3) {
            return $"{v3.x}, {v3.y}, {v3.z}, {v3.w}";
        }
    }
    
}
