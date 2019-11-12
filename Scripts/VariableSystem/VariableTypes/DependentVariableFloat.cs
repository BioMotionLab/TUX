using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableFloat : DependentVariable<float> {
        public DependentVariableFloat() {
            DataType = SupportedDataType.Float;
        }
    }
}