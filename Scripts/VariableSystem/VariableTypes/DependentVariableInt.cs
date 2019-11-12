using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableInt : DependentVariable<int> {
        public DependentVariableInt() {
            DataType = SupportedDataType.Int;
        }
    }
}