using System;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableBool : DependentVariable<bool> {
        public DependentVariableBool() {
            DataType = SupportedDataType.Bool;
        }
    }
}