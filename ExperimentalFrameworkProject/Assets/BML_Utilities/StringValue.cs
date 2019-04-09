
using UnityEngine;

namespace BML_Utilities {
    [CreateAssetMenu(menuName = MenuNames.BmlAssetMenu + "Create String Value")]
    public class StringValue : ScriptableObject {

        [TextArea]
        public string Value;

        public static implicit operator string(StringValue value) {
            return value.Value;
        }

    }
}
