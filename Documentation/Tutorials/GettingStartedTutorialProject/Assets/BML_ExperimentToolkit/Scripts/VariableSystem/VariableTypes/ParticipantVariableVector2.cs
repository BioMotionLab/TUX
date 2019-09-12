using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableVector2 : ParticipantVariable<Vector2> {
        public ParticipantVariableVector2() {
            DataType = SupportedDataType.Vector2;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }
    }
}