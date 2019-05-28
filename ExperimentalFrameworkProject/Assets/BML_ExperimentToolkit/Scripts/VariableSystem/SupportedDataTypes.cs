using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public enum SupportedDataTypes {
        Int,
        Float,
        String,
        Bool,
        GameObject,
        Vector2,
        Vector3,
        CustomDataType_NotYetImplemented,
        ChooseType,
        
    }
}