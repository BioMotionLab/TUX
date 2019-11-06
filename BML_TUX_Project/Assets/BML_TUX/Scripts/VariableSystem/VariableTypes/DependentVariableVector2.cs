using System;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableVector2 : DependentVariable<Vector2> {
        public DependentVariableVector2() {
            DataType = SupportedDataType.Vector2;
        }
    }
}