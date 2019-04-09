using UnityEngine;

namespace BML_Utilities {

    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create Float Value")]
    public class FloatValue : ScriptableObject {
        public float Value;

        public static implicit operator float(FloatValue value) {
            return value.Value;
        }

    }

}
