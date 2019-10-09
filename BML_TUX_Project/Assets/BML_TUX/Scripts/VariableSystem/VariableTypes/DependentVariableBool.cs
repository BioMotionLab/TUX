using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableBool : DependentVariable<bool> {
        public DependentVariableBool() {
            DataType = SupportedDataType.Bool;
        }
    }
}