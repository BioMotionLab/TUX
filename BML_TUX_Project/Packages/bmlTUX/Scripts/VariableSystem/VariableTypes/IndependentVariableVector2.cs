using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableVector2 : IndependentVariable<Vector2> {
        public IndependentVariableVector2() {
            DataType = SupportedDataType.Vector2;
        }
    }
}