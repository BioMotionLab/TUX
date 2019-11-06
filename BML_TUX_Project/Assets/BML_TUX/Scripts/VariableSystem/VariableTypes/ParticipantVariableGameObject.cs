using System;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableGameObject : ParticipantVariable<GameObject> {
        public ParticipantVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }

        public override void AddValueFieldInEditor() {
            throw new NotImplementedException();
        }
    }
}