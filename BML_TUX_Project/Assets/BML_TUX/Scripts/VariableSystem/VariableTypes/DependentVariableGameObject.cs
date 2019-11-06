using System;
using UnityEngine;

namespace BML_TUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableGameObject : DependentVariable<GameObject> {
        public DependentVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }
    }
}