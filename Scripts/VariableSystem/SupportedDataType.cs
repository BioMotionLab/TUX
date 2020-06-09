using System;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public enum SupportedDataType {
        Int,
        Float,
        String,
        Bool,
        GameObject,
        Vector2,
        Vector3,
        CustomDataTypeNotYetImplemented,
        ChooseType,
        
    }
}