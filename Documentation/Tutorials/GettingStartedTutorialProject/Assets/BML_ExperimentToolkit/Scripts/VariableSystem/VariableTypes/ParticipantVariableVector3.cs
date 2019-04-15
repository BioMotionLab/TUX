using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableVector3 : ParticipantVariable<Vector3> {
        public ParticipantVariableVector3() {
            DataType = SupportedDataTypes.Vector3;
        }
    }
}