using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {

    [Serializable]
    public class IndependentVariableGameObject : IndependentVariable<GameObject> {
        public IndependentVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }
    }
}