using System;
using UnityEngine;

namespace bmlTUX.Scripts.VariableSystem.VariableTypes {
    [Serializable]
    public class DependentVariableGameObject : DependentVariable<GameObject> {
        public DependentVariableGameObject() {
            DataType = SupportedDataType.GameObject;
        }
    }
}