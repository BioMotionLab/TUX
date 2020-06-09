using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableInt : IndependentVariable<int> {
        public IndependentVariableInt() {
            DataType = SupportedDataType.Int;
        }
    }
}