using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableVector3 : DependentVariable<Vector3> {
        public DependentVariableVector3() {
            DataType = SupportedDataType.Vector3;
        }
    }
}