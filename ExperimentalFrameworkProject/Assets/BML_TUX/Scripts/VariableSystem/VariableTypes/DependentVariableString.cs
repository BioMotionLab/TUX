using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableString : DependentVariable<string> {
        public DependentVariableString() {
            DataType = SupportedDataType.String;
        }
    }
}