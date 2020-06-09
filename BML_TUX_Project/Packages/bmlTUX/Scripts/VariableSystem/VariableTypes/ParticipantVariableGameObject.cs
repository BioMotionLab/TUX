using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableGameObject : ParticipantVariable<GameObject> {
        public ParticipantVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }

        protected override GameObject DefaultValue => new GameObject("Empty default GameObject");
    }
}