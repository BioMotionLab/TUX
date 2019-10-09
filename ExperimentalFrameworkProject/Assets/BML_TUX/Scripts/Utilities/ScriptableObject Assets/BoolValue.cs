using UnityEngine;

namespace BML_Utilities.ScriptableObject_Assets {
    [CreateAssetMenu()]
    public class BoolValue : ScriptableObject {
        public bool Value;

        public static implicit operator bool(BoolValue value) {
            return value.Value;
        }

    }
}