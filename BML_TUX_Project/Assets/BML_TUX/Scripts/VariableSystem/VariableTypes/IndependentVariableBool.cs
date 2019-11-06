using System;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableBool : IndependentVariable<bool> {
        public IndependentVariableBool() {
            DataType = SupportedDataType.Bool;
            Values.Add(true);
            Values.Add(false);
        }

        
    }
}