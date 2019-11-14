using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableVector3 : ParticipantVariable<Vector3> {
        public ParticipantVariableVector3() {
            DataType = SupportedDataType.Vector3;
        }

        public override void SelectValue(string value) {
            throw new NotImplementedException();
        }


        protected override Vector3 DefaultValue => new Vector3(-999,-999,-999);
    }
}