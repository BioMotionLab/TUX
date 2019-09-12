using UnityEngine;

namespace BML_Utilities.Extensions {
    public static class Vector3Extension {
        public static string ToPreciseString(this Vector3 v3) {
            return $"{v3.x}, {v3.y}, {v3.z}";
        }
    }
}
