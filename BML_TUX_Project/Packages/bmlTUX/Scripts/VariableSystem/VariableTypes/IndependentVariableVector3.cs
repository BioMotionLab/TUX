using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableVector3 : IndependentVariable<Vector3> {
        public IndependentVariableVector3() {
            DataType = SupportedDataType.Vector3;
        }
    }
}