using System;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableFloat : DependentVariable<float> {
        public DependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}