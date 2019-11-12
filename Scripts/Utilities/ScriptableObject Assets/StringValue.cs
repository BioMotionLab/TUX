﻿
using UnityEngine;

namespace BML_Utilities.ScriptableObject_Assets {
    [CreateAssetMenu()]
    public class StringValue : ScriptableObject {

        [TextArea]
        public string Value;

        public static implicit operator string(StringValue value) {
            return value.Value;
        }

    }
}