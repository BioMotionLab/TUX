using System;

namespace BML_TUX.Scripts.VariableSystem {
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