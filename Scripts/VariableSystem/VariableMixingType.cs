using System;

namespace bmlTUX.Scripts.VariableSystem {
    [Serializable]
    public enum VariableMixingType {
        Balanced,
        Looped,
        EvenProbability,
        CustomProbability
    }
}