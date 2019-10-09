using System;
using UnityEngine;

namespace BML_ExperimentToolkit.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableGameObject : DependentVariable<GameObject> {
        public DependentVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }
    }
}