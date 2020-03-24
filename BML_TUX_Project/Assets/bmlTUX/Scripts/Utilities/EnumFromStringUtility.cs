using System;
using System.Data;
using JetBrains.Annotations;
using UnityEngine;

namespace bmlTUX.Scripts.Utilities {
    
    [PublicAPI]
    public static class EnumFromStringUtility {
        
        static T ExtractEnumFromData<T>(DataRow data, string enumVariableName) where T : struct, Enum  {
            string value = (string) data[enumVariableName];
            if (!Enum.TryParse(value, out T enumValue)) {
                Debug.LogError($"Could not convert value {value} of variable {enumVariableName} to Enum. Check Spelling matches enum options.");
            }
            return enumValue;
        }
        
    }
}
