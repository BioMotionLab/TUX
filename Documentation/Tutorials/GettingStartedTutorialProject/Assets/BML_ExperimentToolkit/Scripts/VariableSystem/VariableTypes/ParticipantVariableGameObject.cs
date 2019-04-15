using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class ParticipantVariableGameObject : ParticipantVariable<GameObject> {
        public ParticipantVariableGameObject() {
            DataType = SupportedDataTypes.GameObject;
        }
    }
}