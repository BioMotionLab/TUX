using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableInt : DependentVariable<int> {
        public DependentVariableInt() {
            DataType = SupportedDataType.Int;
        }
    }
}