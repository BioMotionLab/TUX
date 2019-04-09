using UnityEngine;

namespace BML_Utilities {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create Bool Value")]
    public class BoolValue : ScriptableObject {
        public bool Value;

        public static implicit operator bool(BoolValue value) {
            return value.Value;
        }

    }
}