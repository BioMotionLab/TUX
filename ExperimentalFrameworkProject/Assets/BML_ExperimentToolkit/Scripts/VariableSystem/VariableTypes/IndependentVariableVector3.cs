using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class IndependentVariableVector3 : IndependentVariable<Vector3> {
        public IndependentVariableVector3() {
            DataType = SupportedDataTypes.Vector3;
        }
    }
}