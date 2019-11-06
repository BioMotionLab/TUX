using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableString : DependentVariable<string> {
        public DependentVariableString() {
            DataType = SupportedDataType.String;
        }
    }
}