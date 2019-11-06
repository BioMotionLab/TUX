using System;

namespace BML_TUX.Scripts.VariableSystem {
    [Serializable]
    public enum VariableMixingType {
        Balanced,
        Looped,
        EvenProbability,
        CustomProbability
    }
}