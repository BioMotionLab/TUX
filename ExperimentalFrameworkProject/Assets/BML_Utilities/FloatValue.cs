using UnityEngine;

namespace BML_Utilities {

    [CreateAssetMenu]
    public class FloatValue : ScriptableObject {
        public float Value;

        public static implicit operator float(FloatValue value) {
            return value.Value;
        }

    }

}
