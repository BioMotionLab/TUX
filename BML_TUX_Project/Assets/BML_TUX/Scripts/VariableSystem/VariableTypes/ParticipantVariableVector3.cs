using System;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableVector3 : ParticipantVariable<Vector3> {
        public ParticipantVariableVector3() {
            DataType = SupportedDataType.Vector3;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }

        public override void AddValueFieldInEditor() {
            throw new NotImplementedException();
        }
    }
}