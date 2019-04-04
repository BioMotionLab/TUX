using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem {
    [Serializable]
    public enum SupportedDataTypes {
        Int,
        Float,
        String,
        Bool,
        GameObject,
        Vector3,
        CustomDataType,
        ChooseType,
    }
}